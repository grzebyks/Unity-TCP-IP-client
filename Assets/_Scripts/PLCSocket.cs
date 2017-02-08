using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;


public class PLCSocket : MonoBehaviour
{
	public String host = "localhost";
	public Int32 port = 42700;

	internal Boolean socket_ready = false;
	internal String input_buffer = "";
	TcpClient tcp_socket;
	NetworkStream net_stream;

	StreamReader socket_reader;
	ConnectionStatusText connectionStatusText;
	ReceivedDataText receivedDataText;


	void Start() {
	}

	void Update()
	{
		string received_data = readSocket();
	
		if (received_data != "")
		{
			// Do something with the received data,
			// print it in the log for now
			Debug.Log(received_data);
			receivedDataText.textUpdate (received_data);
		}
	}


	void Awake()
	{
		connectionStatusText = GetComponent<ConnectionStatusText> ();
		receivedDataText = GetComponent<ReceivedDataText> ();
		Debug.Log("Awake");
		setupSocket();
	}

	void OnApplicationQuit()
	{
		closeSocket();
	}

	public void setupSocket()
	{
		try
		{
			tcp_socket = new TcpClient(host, port);

			net_stream = tcp_socket.GetStream();
			socket_reader = new StreamReader(net_stream);

			socket_ready = true;
			Debug.Log("Połączono");
			connectionStatusText.textUpdate("Połączono");
		}
		catch (Exception e)
		{
			// Something went wrong
			Debug.Log("Socket error: " + e);
			connectionStatusText.textUpdate("Error");
		}
	}

	public String readSocket()
	{
		if (!socket_ready)
			return "";

		if (net_stream.DataAvailable)
			return socket_reader.ReadLine();

		return "";
	}

	public void closeSocket()
	{
		if (!socket_ready)
			return;

//		socket_writer.Close();
		socket_reader.Close();
		tcp_socket.Close();
		socket_ready = false;
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