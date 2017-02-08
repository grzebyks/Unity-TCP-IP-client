//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Text;
//using System.Threading;
//using Sockets.Plugin;
//using Sockets.Plugin.Abstractions;
//
//
//// Klasa opisujaca dostep do wiadomosci publikowanych przez sterownik
//public class EpcMessageManager : MonoBehaviour {
//
//	// Use this for initialization
//	void Start () {
//		
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		
//	}
//
//	public class EpcService
//	{
//		private static List<EpcDevice> _reachableDevices;
//
//		public static async Task<List<EpcDevice>> SearchDevices()
//		{
//			_reachableDevices = new List<EpcDevice>();
//			var allInterfaces = await CommsInterface.GetAllInterfacesAsync();
//			if (allInterfaces.Count == 0) return _reachableDevices;
//			try
//			{
//				var udpClient = new UdpSocketClient();
//				var udpListener = new UdpSocketReceiver();
//				udpListener.MessageReceived += MessageReceived;
//
//				var port = 42700;
//				var address = "255.255.255.255";
//
//				var msg = "DISCOVER_REQUEST";
//				var msgBytes = Encoding.UTF8.GetBytes(msg);
//
//				await udpClient.SendToAsync(msgBytes, address, port);
//				await udpListener.StartListeningAsync(port + 1);
//				await Task.Delay(TimeSpan.FromSeconds(2));
//				await udpListener.StopListeningAsync();
//			}
//			catch (Exception)
//			{
//				//ignore
//			}
//
//			return _reachableDevices;
//		}
//
//		private static void MessageReceived(object sender, UdpSocketMessageReceivedEventArgs e)
//		{
//			var from = $"{e.RemoteAddress} : {e.RemotePort}";
//			var data = Encoding.UTF8.GetString(e.ByteData, 0, e.ByteData.Length);
//
//			var splitted = data.Split('|');
//			if (splitted.Length == 3)
//				_reachableDevices.Add(new EpcDevice(e.RemoteAddress, Convert.ToInt32(splitted[2]),
//					splitted[0], splitted[1]));
//		}
//	}
//
//	public class EpcDevice
//	{
//		private readonly IpEndPoint _ipEndPoint;
//		private readonly string _description;
//		private readonly byte[] _receivedBytes;
//
//		private readonly TcpSocketClient _tcpClient;
//		private CancellationTokenSource _cts;
//		private Stream _stream;
//
//		public Action<bool> ConnectedCallback;
//		public Action<EpcMessage> DataReceivedCallback;
//		public Action DisconnectedCallback;
//
//		public int WatchDog { private set; get; }
//		public int WatchDogOld = -1;
//		public EpcDevice()
//		{
//			Connected = false;
//			_receivedBytes = new byte[12];
//			_tcpClient = new TcpSocketClient();
//		}
//
//		public EpcDevice(string ipAddress, int port, string name, string description) : this()
//		{
//			_ipEndPoint = new IpEndPoint(ipAddress, port, name);
//			_description = description;
//		}
//
//		public string Name => _ipEndPoint.Name;
//		public string IpPort => _ipEndPoint.IpPort;
//
//		public bool Connected { get; private set; }
//
//
//		public void Connect()
//		{
//			_cts = new CancellationTokenSource();
//			try
//			{
//				_tcpClient.ConnectAsync(_ipEndPoint.IpAddress, _ipEndPoint.Port)
//					.TimeoutAfter(TimeSpan.FromSeconds(3), _cts)
//					.ContinueWith(_connectingFinished);
//			}
//			catch (Exception e)
//			{
//				Debug.WriteLine(e);
//			}
//		}
//
//		private void _connectingFinished(Task obj)
//		{
//			Debug.WriteLine("Cancelation requested? - " + _cts.IsCancellationRequested);
//			if (_cts.IsCancellationRequested) return;
//			_cts = null;
//
//			_stream = _tcpClient.GetStream();
//
//			_cts = new CancellationTokenSource();
//
//			Task.Run(() => ReceivingThread());
//
//
//
//			Task.Delay(200);
//			Connected = true;
//			ConnectedCallback(!_cts.IsCancellationRequested);
//
//
//
//		}
//
//		private void ReceivingThread()
//		{
//			if (_cts == null || _cts?.IsCancellationRequested == true)
//			{
//				Debug.WriteLine("_cts == null || _cts?.IsCancellationRequested == true");
//				_disconnected();
//				return;
//			}
//			try
//			{ 
//				_stream.ReadAsync(_receivedBytes, 0, 12).ContinueWith(ReceivingThread2);
//			}
//			catch (Exception e)
//			{
//				_cts?.Cancel();
//				Debug.WriteLine("Receiving Thread: " + e);
//			}
//		}
//
//		private void ReceivingThread2(Task<int> obj)
//		{
//			var noOfBytes = obj.Result;
//			if (noOfBytes == 0)
//			{
//				Debug.WriteLine("noOfBytes == 0");
//				_cts.Cancel();
//			}
//			else
//			{
//
//				WatchDog++;
//				var message = new EpcMessage(_receivedBytes);
//				DataReceivedCallback?.Invoke(message);
//
//				Task.Run(() => ReceivingThread()).TimeoutAfter(TimeSpan.FromSeconds(2));
//			}
//		}
//
//		public void Disconnect()
//		{
//			Debug.WriteLine("Manually");
//			_cts?.Cancel();
//			_stream?.Dispose();
//			_tcpClient?.DisconnectAsync().ContinueWith(task => _disconnected());
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
//				_tcpClient.DisconnectAsync();
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
//		public void Send(EpcMessage message)
//		{
//			//while (_stream?.CanWrite != true)
//			//{
//			//    //wait
//			//}
//			try
//			{
//				if (_stream?.CanWrite == true)
//				{
//					var buffer = message.ToBytes();
//					_stream.Write(buffer, 0, buffer.Length);
//				}
//			}
//			catch (Exception)
//			{
//				Debug.WriteLine("Error sending message to plc");
//				//ignore
//			}
//		}
//	}
//
//
//	public class EpcMessage
//	{
//		public enum MessgaeType
//		{
//			Read = 0x72,
//			Write = 0x77,
//			Confirm = 0x63,
//			Error = 0x21,
//			Enable = 0x65,
//			Disable = 0x64
//		}
//
//		private readonly short _mDataLength;
//		private readonly short _mId;
//		//private readonly ushort _mOIndex;
//
//		private readonly MessgaeType _mType;
//
//
//		public EpcMessage(byte[] message)
//		{
//			_mType = (MessgaeType) message[0];
//			_mId = (short) BitConverter.ToUInt16(new byte[2] {message[2], message[3]}, 0);
//			ObjectIndex = BitConverter.ToUInt16(new byte[2] {message[4], message[5]}, 0);
//			_mDataLength = BitConverter.ToInt16(new byte[2] {message[6], message[7]}, 0);
//			Value = BitConverter.ToInt32(new byte[4] {message[8], message[9], message[10], message[11]}, 0);
//		}
//
//		public EpcMessage(MessgaeType mType, ushort mOIndex, byte mDataLength, int value)
//		{
//			_mType = mType;
//			ObjectIndex = mOIndex;
//			_mDataLength = mDataLength;
//			Value = value;
//		}
//
//		public int Value { get; }
//		public ushort ObjectIndex { get; }
//
//		public byte[] ToBytes()
//		{
//			var message = new byte[12];
//			message[0] = (byte) _mType;
//			message[1] = 0;
//
//			message[2] = BitConverter.GetBytes(_mId)[0];
//			message[3] = BitConverter.GetBytes(_mId)[1];
//
//			message[4] = BitConverter.GetBytes(ObjectIndex)[0];
//			message[5] = BitConverter.GetBytes(ObjectIndex)[1];
//
//			message[6] = BitConverter.GetBytes(_mDataLength)[0];
//			message[7] = BitConverter.GetBytes(_mDataLength)[1];
//
//			message[8] = BitConverter.GetBytes(Value)[0];
//			message[9] = BitConverter.GetBytes(Value)[1];
//			message[10] = BitConverter.GetBytes(Value)[2];
//			message[11] = BitConverter.GetBytes(Value)[3];
//			return message;
//		}
//	}
//
//	internal class IpEndPoint
//	{
//		public IpEndPoint(string ipAddress, int port, string name)
//		{
//			IpAddress = ipAddress;
//			Port = port;
//			Name = name;
//		}
//
//		public string IpAddress { get; set; }
//		public int Port { get; set; }
//		public string Name { get; set; }
//
//		public string IpPort => IpAddress + ":" + Port;
//	}
//
//}
