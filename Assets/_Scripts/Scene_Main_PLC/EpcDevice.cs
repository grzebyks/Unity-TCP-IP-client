using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using UnityEngine.Networking;
using UnityEditor;  

public class EpcDevice
{
	// ManualResetEvent instances signal completion.  
//	private static ManualResetEvent connectDone =   
//		new ManualResetEvent(false);  
//	private static ManualResetEvent receiveDone =   
//		new ManualResetEvent(false);  

	private readonly IpEndPoint _ipEndPoint;
	public readonly string sensorName;
	public ushort _dataID { get; private set; }
	private static byte[] _receivedBytes;

	//		private readonly Socket _socket;
	private TcpClient _tcpClient;
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

	}

	public EpcDevice(string _ipAddress, int _port, string _deviceName, ushort _sensorID, string _sensorName) : this()
	{
		_ipEndPoint = new IpEndPoint(_ipAddress, _port, _deviceName);
		_dataID = _sensorID;
		sensorName = _sensorName;
//		Debug.Log ("Will connect to: " + Name () + ", "+ IpPort ()+" "+sensorName);
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

	public void Connect()
	{
		_tcpClient = new TcpClient(AddressFamily.InterNetwork);
		Debug.Log("Will connect");
		try
		{
			_tcpClient.BeginConnect (
				_ipEndPoint.IpAddress,_ipEndPoint.Port,
				new AsyncCallback(_connectCallback), _tcpClient);
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}
	}

	private void _connectCallback(IAsyncResult ar) {  
		try {  
			// Retrieve the socket from the state object.  
			TcpClient client = (TcpClient) ar.AsyncState;  
			// Complete the connection.  
			client.EndConnect(ar);  
			Debug.Log("Socket connected to: " + client.Client.RemoteEndPoint.ToString ());
			// Signal that the connection has been made.  
//			connectDone.Set();
			isConnected = true;
			Debug.Log ("will assign Stream variable");
			_stream=_tcpClient.GetStream();

			Debug.Log("Running receiving thread");
			_receive ();



		} catch (Exception e) {  
			Debug.Log(e.ToString());  
		}  
	}  
	private void _receive() {  
		Debug.Log ("in receive method"); 
		try {  
			// Create the state object.  
			//				StateObject state = new StateObject();  
			//				state.workSocket = client;  

			// Begin receiving the data from the remote device.  
			//				client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
			//					new AsyncCallback(ReceiveCallback), state);  

			//				_tcpClient=_socket.
//			Debug.Log ("will assign Stream variable");
//			_stream=_tcpClient.GetStream();
//			Debug.Log ("will send initial frame");
			//Send initial frame to begin listening for data updates
//			EpcMessage initialFrame = new EpcMessage(EpcMessage.MessgaeType.Enable, _dataID, 4, 0);
//			Send(initialFrame);
			while (isConnected){
				Debug.Log ("will try to read stream");
				// Blokuje az do momentu pojawienia sie danych do odczytu
				// Gdy wszystkie wiadomości bedą disabled, wątek zawiesi się
				// po wywołaniu funkcji disconnect aż do timeout.
				// Nie będzie wtedy możliwe ponowne połączenie
				_stream.Read(_receivedBytes,0,12);
				Debug.Log ("stworzy message"); 
				EpcMessage message = new EpcMessage (_receivedBytes);
				if (DataReceivedCallback != null) {
					DataReceivedCallback.Invoke (message);
				}
				epcMessageValue = message.Value;
				Debug.Log ("AAAAAA_OTRZYMANE DANE: " + "ID: " + message.ObjectIndex + " value: " + message.Value);
			}
			Debug.Log ("Za while"); 
//			receiveDone.Set ();
			_cleanDisconnect();
		} catch (Exception e) {  
			Debug.Log(e.ToString());  
		}  
	}  

	public void Disconnect(){
		Debug.Log ("will disconnect");
		// Will break the value reading loop
		isConnected = false;
	}
	private void _cleanDisconnect(){
		Debug.Log ("in clean disconnect");
		if (_stream != null) {
			try {
				Debug.Log ("will dispose stream");
				_stream.Dispose ();
			} catch (Exception e){
				Debug.Log (e);
			}
		}
		if (_tcpClient != null) {
			try {
				Debug.Log ("will close tcpclient");
				_tcpClient.GetStream ().Close ();
				_tcpClient.Close ();
			} catch (Exception e){
				Debug.Log (e);
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
	public void Send(EpcMessage message)
	{
		//while (_stream?.CanWrite != true)
		//{
		//    //wait
		//}
		try
		{
			Debug.Log ("Trying to send initial frame"); 
			if (_stream != null && _stream.CanWrite == true)
			{
				Debug.Log ("sending initial frame"); 
				byte[] buffer = message.ToBytes();
				_stream.Write(buffer, 0, buffer.Length);
			}
		}
		catch (Exception)
		{
			Debug.Log ("Error sending message to plc");
			//ignore
		}
	}


	public class IpEndPoint
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

