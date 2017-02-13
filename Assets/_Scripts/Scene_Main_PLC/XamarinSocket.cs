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

	public String _host = "192.168.1.62";
	public Int32 _port = 4270;
	public String _deviceName = "Tripod";
//	public ushort _dataID = 12;
//	public String _descriptionOfData = "Pos X";
	// Classic approach - one device to connect
//	private EpcDevice epcDevice;

	private TcpClient _tcpClient;

	private bool isAppConnected;

	// Create a list of connectedDevices.
	private List<EpcDeviceProperties> connectedDevices;
	// And threads to handle connections and sensor reading
	private Thread connectionThread;
	private List<Thread> sensorReadingThreads;
	// ManualResetEvent instances signal completion.  
	private static ManualResetEvent connectDone =   
		new ManualResetEvent(false);

	// Use this for initialization
	void Start () {
		//List approach can be done from a file or qr code reading (more flexibility)
		isAppConnected = false;
		connectedDevices = new List<EpcDeviceProperties>();
		sensorReadingThreads = new List<Thread> ();
		addDevicesToList ();
		_tcpClient = new TcpClient(AddressFamily.InterNetwork);
	}
	
	// Update is called once per frame
	void Update () {
//		if (epcDevice != null) {
//			if (epcDevice.epcMessageValueOld != epcDevice.epcMessageValue) {
//				epcDevice.epcMessageValueOld = epcDevice.epcMessageValue;
//			}
//			if (epcDevice.isConnected) {
//				print ("CONNECTED");
//				print ("Wartość odczytu: " + epcDevice.epcMessageValue);
//			}
//		}
//		if (isAppConnected) {
//			foreach (EpcDeviceProperties aDevice in connectedDevices) {
//				print ("Device: " + aDevice.descriptionOfData + ", Value: " + aDevice.sensorValue);
//
//			}
//		}
	}
	void OnGUI(){

		if (!isAppConnected) {

			if (GUI.Button (new Rect (10, 10, 150, 100), "Connect")) {
				print ("You clicked the button!");
				foreach (EpcDeviceProperties aDevice in connectedDevices) {
					aDevice.thisDevice = new EpcDevice (aDevice.hostIP, aDevice.port, aDevice.deviceName, aDevice.dataID, aDevice.descriptionOfData);
					print ("Device " + aDevice.descriptionOfData);
					// Adding thread to the list
					Thread _thread = new Thread(new ThreadStart(aDevice.readSensorValue));
					sensorReadingThreads.Add (_thread);
				}
				// Creating thread to handle connection and sensor reading
				connectionThread = new Thread (connectAndRead);
				connectionThread.Start ();
			}
		} else {
			if (GUI.Button (new Rect (10, 10, 150, 100), "Disconnect")) {
//				epcDevice.disconnect ();
			}
		}
	}

	private void connectAndRead(){
		// Connect to remote PLC
		connectWithRemote();
		// Will wait until connection with remote PLC is established
		connectDone.WaitOne ();
		// Executing threads to read values
//		foreach (Thread aThread in sensorReadingThreads) {
//			aThread.Start ();
//		}

		sensorReadingThreads [0].Start ();

	}
		
	// Hardcoded adding devices to the list of accessible devices
	private void addDevicesToList (){
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Active Program No", dataID = 1
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Tripod State", dataID = 2
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Camera State", dataID = 3
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Tripod Pos X", dataID = 10
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Tripod Pos Y", dataID = 11
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Tripod Pos Z", dataID = 12
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Tripod Velocity", dataID = 20
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "Tripod Acceleration", dataID = 21
		});
		connectedDevices.Add (new EpcDeviceProperties () {
			hostIP = _host, port = _port, deviceName = _deviceName,
			descriptionOfData = "AirSupply pressure", dataID = 30
		});
	}

	private void connectWithRemote(){
		print("Will connect");
		_tcpClient.BeginConnect (_host,_port, new AsyncCallback(ConnectCallback), _tcpClient);
		print("Connected, will wait");
		// Pass established tcp client to each object reading sensors
		foreach (EpcDeviceProperties aDevice in connectedDevices) {
			aDevice.tcpClient = _tcpClient;
		}
		connectDone.WaitOne();
	}

	private void ConnectCallback(IAsyncResult ar) {  
		try {  
			// Retrieve the socket from the state object.  
			TcpClient client = (TcpClient) ar.AsyncState;  

			// Complete the connection.  
			client.EndConnect(ar);  

			print("Socket connected to {0}" + client.Client.RemoteEndPoint.ToString ());
			isAppConnected = true;
			// Signal that the connection has been made.  
			connectDone.Set();
		} catch (Exception e) {  
			print(e.ToString());  
		}  
	} 
}