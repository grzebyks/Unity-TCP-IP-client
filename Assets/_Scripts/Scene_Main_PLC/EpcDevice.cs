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


public class EpcDevice
{
	// ManualResetEvent instances signal completion.  
//	private static ManualResetEvent connectDone =   
//		new ManualResetEvent(false);  
	private static ManualResetEvent receiveDone =   
		new ManualResetEvent(false);  

	private readonly IpEndPoint _ipEndPoint;
	private readonly string _description;
	private readonly ushort _dataID;
	private static byte[] _receivedBytes;

	//		private readonly Socket _socket;
//	private readonly TcpClient _tcpClient;
	private Stream _stream;

//	public Action<bool> ConnectedCallback;
//	public Action<EpcMessage> DataReceivedCallback;
//	public Action DisconnectedCallback;

	public bool isConnected { get; private set; }

	public int epcMessageValue { get; private set; }
	public int epcMessageValueOld { get; set; }

	public int WatchDog { private set; get; }
	public int WatchDogOld = -1;
	public EpcDevice()
	{
		isConnected = false;
		_receivedBytes = new byte[12];
		//			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		epcMessageValue = -1;
		epcMessageValueOld = -1;
//		_tcpClient = new TcpClient(AddressFamily.InterNetwork);
	}

	public EpcDevice(string ipAddress, int port, string name, ushort dataID, string description) : this()
	{
		_ipEndPoint = new IpEndPoint(ipAddress, port, name);
		_dataID = dataID;
		_description = description;
		Debug.Log ("Will connect to: " + Name () + ", "+ IpPort () + ", "+_dataID);
	}

	public string Name (){
		return _ipEndPoint.Name + " nazwa: "+_description;
	}
	public string IpPort (){
		return _ipEndPoint.IpPort();
	}
		

	public void readSensorManager(TcpClient _tcpClient)
	{
		try
		{
//			Debug.Log("Will connect");
//			_tcpClient.BeginConnect (_ipEndPoint.IpAddress,_ipEndPoint.Port, new AsyncCallback(ConnectCallback), _tcpClient);
//			Debug.Log("Connected, will wait");
//			connectDone.WaitOne();
			isConnected = true;
			//Receive the messaage
			Debug.Log("Will receive");
			receive(_tcpClient);
			Debug.Log("Received, will wait");
			receiveDone.WaitOne();
			Debug.Log("Finished reading messages");
			cleanDisconnect(_tcpClient);

		}
		catch (Exception e)
		{
			Debug.Log(e);
		}
	}

//	private void ConnectCallback(IAsyncResult ar) {  
//		try {  
//			// Retrieve the socket from the state object.  
//			TcpClient client = (TcpClient) ar.AsyncState;  
//
//			// Complete the connection.  
//			client.EndConnect(ar);  
//
//			Debug.Log("Socket connected to {0}" + client.Client.RemoteEndPoint.ToString ());
//
//			// Signal that the connection has been made.  
//			connectDone.Set();
//		} catch (Exception e) {  
//			Debug.Log(e.ToString());  
//		}  
//	} 
	private void receive(TcpClient _tcpClient) {  
		Debug.Log ("in receive method"); 
		try {  
			// Create the state object.  
			//				StateObject state = new StateObject();  
			//				state.workSocket = client;  

			// Begin receiving the data from the remote device.  
			//				client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
			//					new AsyncCallback(ReceiveCallback), state);  

			//				_tcpClient=_socket.
			Debug.Log ("will assign Stream variable");
			_stream=_tcpClient.GetStream();
			Debug.Log ("will send initial frame");
			//Send initial frame to begin listening for data updates
			EpcMessage initialFrame = new EpcMessage(EpcMessage.MessgaeType.Enable, _dataID, 4, 0);
			Debug.Log ("Initial frame: " + _description + " " + _dataID);
			send(initialFrame);
			while (isConnected){
				Debug.Log ("will try to read stream");
				//Blokuje az do momentu pojawienia sie danych do odczytu
				_stream.Read(_receivedBytes,0,12);
				Debug.Log ("stworzy message"); 
				EpcMessage message = new EpcMessage (_receivedBytes);
//				if (DataReceivedCallback != null) {
//					DataReceivedCallback.Invoke (message);
//				}
				epcMessageValue = message.Value;
				Debug.Log ("AAAAAA_OTRZYMANE DANE: " + _description +" dataID: "+_dataID+ " value: "+ message.Value);
			}
			receiveDone.Set ();
		} catch (Exception e) {  
			Debug.Log(e.ToString());  
		}  
	}  

	public void disconnect(){
		Debug.Log ("will disconnect");
		// Will break the value reading loop
		isConnected = false;
	}
	private void cleanDisconnect(TcpClient _tcpClient){
		if (_stream != null) {
			try {
				_stream.Dispose ();
			} catch (Exception e){
				Debug.Log (e);
			}
		}
		if (_tcpClient != null) {
			try {
				_tcpClient.Close ();
			} catch (Exception e){
				Debug.Log (e);
			}
		}

	}
	public void send(EpcMessage message)
	{
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
			Debug.Log ("Error sending initial frame to plc");
			//ignore
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
