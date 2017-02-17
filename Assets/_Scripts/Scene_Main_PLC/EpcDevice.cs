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
	// Sensor properties
	private readonly IpEndPoint _ipEndPoint;
	public readonly string sensorName;
	// Property added for more flexibility. Refer to ListOfTrackedSensors class.
	public ushort _dataID { get; private set; }
	private static byte[] _receivedBytes;

	private TcpClient _tcpClient;
	private Stream _stream;

//	public int WatchDog { private set; get; }
//	public int WatchDogOld = -1;
	public EpcDevice()
	{
		isConnected = false;
		_receivedBytes = new byte[12];
		epcMessageValue = -1;
		epcMessageValueOld = -1;
	}

	public EpcDevice(string _ipAddress, int _port, string _deviceName, ushort _sensorID, string _sensorName) : this()
	{
		_ipEndPoint = new IpEndPoint(_ipAddress, _port, _deviceName);
		_dataID = _sensorID;
		sensorName = _sensorName;
	}

	public string Name (){
		return _ipEndPoint.Name;
	}
	public string IpPort (){
		return _ipEndPoint.IpPort();
	}
	public bool isConnected { get; private set; }
	public int epcMessageValue { get; private set; }
	public ushort epcMessageID { get; private set; }
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
			while (isConnected){
				Debug.Log ("will try to read stream");
				// Blokuje az do momentu pojawienia sie danych do odczytu
				// Gdy wszystkie wiadomości bedą disabled, wątek zawiesi się
				// po wywołaniu funkcji disconnect aż do timeout.
				// Nie będzie wtedy możliwe ponowne połączenie
				_stream.Read(_receivedBytes,0,12);
				Debug.Log ("stworzy message"); 
				EpcMessage message = new EpcMessage (_receivedBytes);
				epcMessageValue = message.Value;
				epcMessageID = message.ObjectIndex;
				Thread.Sleep (10);
//				Debug.Log ("AAAAAA_OTRZYMANE DANE: " + "ID: " + message.ObjectIndex + " value: " + message.Value);
			}
			Debug.Log ("Za while"); 
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

	public void Send(EpcMessage message)
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
			Debug.Log ("Error sending message to plc");
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