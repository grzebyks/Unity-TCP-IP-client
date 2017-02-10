using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using UnityEngine.Networking;  

public class XamarinSocket : MonoBehaviour {

	public String host = "192.168.1.62";
	public Int32 port = 42700;
	public String deviceName = "Tripod";
	public ushort dataID = 12;
	public String descriptionOfData = "Pos X";
	private EpcDevice epcDevice;
	private Thread connectThread;

	// Use this for initialization
	void Start () {
		epcDevice = new EpcDevice (host, port, deviceName, dataID, descriptionOfData);
	}
	
	// Update is called once per frame
	void Update () {
		if (epcDevice.epcMessageValueOld != epcDevice.epcMessageValue) {
//			connectThread.Abort ();
			epcDevice.epcMessageValueOld = epcDevice.epcMessageValue;
		}
		if (epcDevice.isConnected) {
			print ("CONNECTED");
			print ("Wartość odczytu: "+ epcDevice.epcMessageValue);
		}
	}
	void OnGUI(){

		if (!epcDevice.isConnected) {

			if (GUI.Button (new Rect (10, 10, 150, 100), "Connect")) {
				print ("You clicked the button!");
				epcDevice = new EpcDevice (host, port, deviceName, dataID, descriptionOfData);
				connectThread = new Thread (_connect);
				connectThread.Start ();
			}
		} else {
			if (GUI.Button (new Rect (10, 10, 150, 100), "Disconnect")) {
				epcDevice.disconnect ();
			}
		}
	}

	void _connect(){
		print ("Connection thread started"); 
		epcDevice.connectionManager ();

	}


	public class EpcDevice
	{
		// ManualResetEvent instances signal completion.  
		private static ManualResetEvent connectDone =   
			new ManualResetEvent(false);  
		private static ManualResetEvent receiveDone =   
			new ManualResetEvent(false);  
		
		private readonly IpEndPoint _ipEndPoint;
		private readonly string _description;
		private readonly ushort _dataID;
		private static byte[] _receivedBytes;

//		private readonly Socket _socket;
		private readonly TcpClient _tcpClient;
		private Stream _stream;

		public Action<bool> ConnectedCallback;
		public Action<EpcMessage> DataReceivedCallback;
		public Action DisconnectedCallback;

		public int WatchDog { private set; get; }
		public int WatchDogOld = -1;
		public EpcDevice()
		{
			isConnected = false;
			_receivedBytes = new byte[12];
//			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			epcMessageValue = -1;
			epcMessageValueOld = -1;
			_tcpClient = new TcpClient(AddressFamily.InterNetwork);
		}

		public EpcDevice(string ipAddress, int port, string name, ushort dataID, string description) : this()
		{
			_ipEndPoint = new IpEndPoint(ipAddress, port, name);
			_dataID = dataID;
			_description = description;
			print ("Will connect to: " + Name () + ", "+ IpPort ());
		}

		public string Name (){
			return _ipEndPoint.Name;
		}
		public string IpPort (){
			return _ipEndPoint.IpPort();
		}

		public bool isConnected { get; private set; }

		public int epcMessageValue { get; private set; }
		public int epcMessageValueOld { get; set; }


		public void connectionManager()
		{
//			_cts = new CancellationTokenSource();
			try
			{
//				_socket.ConnectAsync(_ipEndPoint.IpAddress, _ipEndPoint.Port)
//					.TimeoutAfter(TimeSpan.FromSeconds(3), _cts)
//					.ContinueWith(_connectingFinished);
				print("Will connect");
//				_socket.BeginConnect(_ipEndPoint.IpAddress,_ipEndPoint.Port, new AsyncCallback(ConnectCallback), _socket);
				_tcpClient.BeginConnect (_ipEndPoint.IpAddress,_ipEndPoint.Port, new AsyncCallback(ConnectCallback), _tcpClient);
				print("Connected, will wait");
				connectDone.WaitOne();
				isConnected = true;
				//Receive the messaage
				print("Will receive");
				receive();
				print("Received, will wait");
				receiveDone.WaitOne();
				print("Finished reading messages");
				cleanDisconnect();

			}
			catch (Exception e)
			{
				print(e);
			}
		}

		private void ConnectCallback(IAsyncResult ar) {  
			try {  
				// Retrieve the socket from the state object.  
				TcpClient client = (TcpClient) ar.AsyncState;  

				// Complete the connection.  
				client.EndConnect(ar);  

				print("Socket connected to {0}" + client.Client.RemoteEndPoint.ToString ());

				// Signal that the connection has been made.  
				connectDone.Set();
			} catch (Exception e) {  
				print(e.ToString());  
			}  
		}  
		private void receive() {  
			print ("in receive method"); 
			try {  
				// Create the state object.  
//				StateObject state = new StateObject();  
//				state.workSocket = client;  

				// Begin receiving the data from the remote device.  
//				client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
//					new AsyncCallback(ReceiveCallback), state);  

//				_tcpClient=_socket.
				print ("will assign Stream variable");
				_stream=_tcpClient.GetStream();
				print ("will send initial frame");
				//Send initial frame to begin listening for data updates
				EpcMessage initialFrame = new EpcMessage(EpcMessage.MessgaeType.Enable, _dataID, 4, 0);
				send(initialFrame);
				while (isConnected){
					print ("will try to read stream");
					//Blokuje az do momentu pojawienia sie danych do odczytu
					_stream.Read(_receivedBytes,0,12);
					print("stream read");
					print ("stworzy message"); 
					EpcMessage message = new EpcMessage (_receivedBytes);
					if (DataReceivedCallback != null) {
						DataReceivedCallback.Invoke (message);
					}
					epcMessageValue = message.Value;
					print ("AAAAAA_OTRZYMANE DANE: " + message.Value);
				}
				receiveDone.Set ();
			} catch (Exception e) {  
				print(e.ToString());  
			}  
		}  
			
		public void disconnect(){
			print ("will disconnect");
			// Will break the value reading loop
			isConnected = false;
		}
		private void cleanDisconnect(){
			if (_stream != null) {
				try {
					_stream.Dispose ();
				} catch (Exception e){
					print (e);
				}
			}
			if (_tcpClient != null) {
				try {
					_tcpClient.Close ();
				} catch (Exception e){
					print (e);
				}
			}

		}
//
//		public void Disconnect()
//		{
//			Debug.WriteLine("Manually");
//			_cts?.Cancel();
//			_stream?.Dispose();
//			_socket?.DisconnectAsync().ContinueWith(task => _disconnected());
//		}
//
//		private void _disconnected()
//		{
//			Connected = false;
//			DisconnectedCallback?.Invoke();
//			_cts = null;
//			try
//			{
//				_stream.Dispose();
//				_socket.DisconnectAsync();
//			}
//			catch (Exception)
//			{
//				//ignore
//			}
//		}
//
//
//		public override string ToString()
//		{
//			return _ipEndPoint.Name + " - " + _ipEndPoint.IpPort;
//		}
//
//		public string Title => _ipEndPoint.Name;
//		public string Description => _description;
//
		public void send(EpcMessage message)
		{
			//while (_stream?.CanWrite != true)
			//{
			//    //wait
			//}
			try
			{
				print ("Trying to send initial frame"); 
				if (_stream != null && _stream.CanWrite == true)
				{
					print ("sending initial frame"); 
					byte[] buffer = message.ToBytes();
					_stream.Write(buffer, 0, buffer.Length);
				}
			}
			catch (Exception)
			{
				print ("Error sending message to plc");
				//ignore
			}
		}
	}




	public class EpcMessage
	{
		public enum MessgaeType
		{
			Read = 0x72,
			Write = 0x77,
			Confirm = 0x63,
			Error = 0x21,
			Enable = 0x65,
			Disable = 0x64
		}

		private readonly short _mDataLength;
		private readonly short _mId;
		//private readonly ushort _mOIndex;

		private readonly MessgaeType _mType;


		public EpcMessage(byte[] message)
		{
			_mType = (MessgaeType) message[0];
			_mId = (short) BitConverter.ToUInt16(new byte[2] {message[2], message[3]}, 0);
			ObjectIndex = BitConverter.ToUInt16(new byte[2] {message[4], message[5]}, 0);
			_mDataLength = BitConverter.ToInt16(new byte[2] {message[6], message[7]}, 0);
			Value = BitConverter.ToInt32(new byte[4] {message[8], message[9], message[10], message[11]}, 0);
		}

		public EpcMessage(MessgaeType mType, ushort mOIndex, byte mDataLength, int value)
		{
			_mType = mType;
			ObjectIndex = mOIndex;
			_mDataLength = mDataLength;
			Value = value;
		}

		public int Value { get; }
		public ushort ObjectIndex { get; }

		public byte[] ToBytes()
		{
			var message = new byte[12];
			message[0] = (byte) _mType;
			message[1] = 0;

			message[2] = BitConverter.GetBytes(_mId)[0];
			message[3] = BitConverter.GetBytes(_mId)[1];

			message[4] = BitConverter.GetBytes(ObjectIndex)[0];
			message[5] = BitConverter.GetBytes(ObjectIndex)[1];

			message[6] = BitConverter.GetBytes(_mDataLength)[0];
			message[7] = BitConverter.GetBytes(_mDataLength)[1];

			message[8] = BitConverter.GetBytes(Value)[0];
			message[9] = BitConverter.GetBytes(Value)[1];
			message[10] = BitConverter.GetBytes(Value)[2];
			message[11] = BitConverter.GetBytes(Value)[3];
			return message;
		}
	}

	internal class IpEndPoint
	{
		public IpEndPoint(string ipAddress, int port, string name)
		{
			IpAddress = ipAddress;
			Port = port;
			Name = name;
		}

		public string IpAddress { get; set; }
		public int Port { get; set; }
		public string Name { get; set; }

		public string IpPort (){
			return IpAddress + ":" + Port;
		}
	}
}