//using UnityEngine;
//using System.Collections;
//using System;
//using System.IO;
//using System.Net.Sockets;
//using System.Threading;
//
//public class PLCSocket : MonoBehaviour
//{
//	public String host = "192.168.1.61";
//	public Int32 port = 43700;
//
//	internal Boolean socket_ready = false;
//	internal String input_buffer = "";
//	TcpClient tcp_socket;
////	NetworkStream net_stream;
//
////	StreamReader socket_reader;
//	ConnectionStatusText connectionStatusText;
//	ReceivedDataText receivedDataText;
//
//	//My modifications
//	public String _description = "";
//	private readonly byte[] _receivedBytes = new byte[12];
//	private Stream _stream;
//	//TODO: zrobic uniwersalnie zeby tworzyc osobne dla różnych markerów - podpiac pod markery i odpowiednio oznaczyc w unity editor. Zabepieczenie przed pusta nazwa
//
//	void Start() {
//		print ("network script start called");
//	}
//
//	void Update()
//	{
//
//		readSocket ();
////		string received_data = readSocket();
//
//		// TODO: Sending a message
//
//	}
//
//
//	void Awake()
//	{
//		connectionStatusText = GetComponent<ConnectionStatusText> ();
//		receivedDataText = GetComponent<ReceivedDataText> ();
//		Debug.Log("Awake");
//		setupSocket();
//	}
//
//	void OnApplicationQuit()
//	{
//		closeSocket();
//	}
//
//	//TODO: Zrobic non blocking
//	public void setupSocket()
//	{
//		try
//		{
//			tcp_socket = new TcpClient(host, port);
//			_stream = tcp_socket.GetStream();
//			tcp_socket.Connect(host,port);
//
////			net_stream = tcp_socket.GetStream();
////			socket_reader = new StreamReader(net_stream);
//
//
//			socket_ready = true;
//			Debug.Log("Połączono");
//			connectionStatusText.textUpdate("Połączono");
//		}
//		catch (Exception e)
//		{
//			// Something went wrong
//			Debug.Log("Socket error: " + e);
//			connectionStatusText.textUpdate("Error");
//		}
//	}
//		
//	public void readSocket()
//	{
//		if (!socket_ready) {
//			Debug.Log ("Socket not ready");
//		} else {
//			_stream.Read (_receivedBytes, 0, 12);
//			beginParsing ();
//		}
//
//		if (net_stream.DataAvailable)
//			return socket_reader.ReadLine();
//
//		return "";
//	}
//
//	public void closeSocket()
//	{
//		if (!socket_ready)
//			return;
//
//		socket_reader.Close();
//		tcp_socket.Close();
//		socket_ready = false;
//	}
//
//	private void beginParsing()
//	{
//		var noOfBytes = obj.Result;
//		if (noOfBytes == 0)
//		{
//			Debug.WriteLine("noOfBytes == 0");
//			_cts.Cancel();
//		}
//		else
//		{
//
//			WatchDog++;
//			var message = new EpcMessage(_receivedBytes);
//			DataReceivedCallback?.Invoke(message);
//
//			Task.Run(() => ReceivingThread()).TimeoutAfter(TimeSpan.FromSeconds(2));
//		}
//	}
//
//}