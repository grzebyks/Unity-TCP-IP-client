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
	public Int32 port = 4270;
	public String deviceName = "Tripod";
	public ushort dataID = 12;
	public String sensorName = "Pos X";
	private EpcDevice _EpcDevice;
	private Thread connectThread;

	// Create a list of sensors connected to a certain device.
	private List<EpcDevice> connectedSensors;


	// Use this for initialization
	void Start () {
		_EpcDevice = new EpcDevice (host, port, deviceName, dataID, sensorName);
		connectedSensors = new List<EpcDevice>();
		connectedSensors = ListOfTrackedSensors.DeviceList (host,port,deviceName);
	}
	
	// Update is called once per frame
	void Update () {
		foreach (EpcDevice obj in connectedSensors) {
			// TODO: Display message value

//			print (obj.sensorName); 

		}

		if (_EpcDevice.epcMessageValueOld != _EpcDevice.epcMessageValue) {
//			connectThread.Abort ();
			_EpcDevice.epcMessageValueOld = _EpcDevice.epcMessageValue;
		}
		if (_EpcDevice.isConnected) {
			print ("CONNECTED");
//			print ("Wartość odczytu: "+ _EpcDevice.epcMessageValue);
		}
	}
	void OnGUI(){

		if (!_EpcDevice.isConnected) {

			if (GUI.Button (new Rect (10, 10, 150, 100), "Connect")) {
				print ("You clicked the button!");
//				_EpcDevice = new EpcDevice (host, port, deviceName, dataID, sensorName);
				// TODO: Connect list of devices to PLC. Xamarin -> MaintananceViewModel, line 176
				connectThread = new Thread (_connect);
				connectThread.Start ();
			}
		} else {
			if (GUI.Button (new Rect (10, 10, 150, 100), "Disconnect")) {
				_EpcDevice.Disconnect ();
			}
			if (GUI.Button (new Rect (10, 110, 150, 100), "Pos X")) {
				EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, 10, 4, 0);
				_EpcDevice.Send(message);
			}
			if (GUI.Button (new Rect (10, 210, 150, 100), "Pos Y")) {
				EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, 11, 4, 0);
				_EpcDevice.Send(message);
			}
			if (GUI.Button (new Rect (160, 10, 150, 100), "Pos Z")) {
				EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, 12, 4, 0);
				_EpcDevice.Send(message);
			}
			if (GUI.Button (new Rect (160, 110, 150, 100), "Disable all")) {
				foreach (EpcDevice obj in connectedSensors)
				{
					EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Disable, obj._dataID, 4, 0);
					_EpcDevice.Send(message);
				}
			}
			if (GUI.Button (new Rect (160, 210, 150, 100), "Enable all")) {
				foreach (EpcDevice obj in connectedSensors)
				{
					EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, obj._dataID, 4, 0);
					_EpcDevice.Send(message);
				}
			}
		}
	}

	void _connect(){
		print ("Connection thread started");

		_EpcDevice.Connect ();

		foreach (EpcDevice obj in connectedSensors)
		{
			EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, obj._dataID, 4, 0);
			_EpcDevice.Send(message);
		}
	}





}