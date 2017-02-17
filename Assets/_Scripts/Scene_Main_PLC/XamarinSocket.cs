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
using UnityEngine.UI;

public class XamarinSocket : MonoBehaviour {

	public String host = "192.168.1.62";
	public Int32 port = 4270;
	public String deviceName = "Tripod";
	public ushort dataID = 12;
	public String sensorName = "Pos X";
	// Created as an example. To be replaced later when structure of the list is corrected.
	private EpcDevice _EpcDevice;
	private Thread connectThread;
	private bool isAppConnected;

	// Create a list of sensors connected to a certain device.
	public ListOfTrackedSensors tripodSensors { get; private set; }

	// To display text
	private ConnectionStatusText statusText;


	// Use this for initialization
	void Start () {
		isAppConnected = false;
		statusText = GetComponent<ConnectionStatusText>();

		_EpcDevice = new EpcDevice (host, port, deviceName, dataID, sensorName);
		tripodSensors = new ListOfTrackedSensors(host, port, deviceName);
		// Connect automatically
		connectThread = new Thread (_connect);
		connectThread.Start ();
	}

	// Update is called once per frame
	void Update () {
		int msgValue = _EpcDevice.epcMessageValue;
		ushort msgID = _EpcDevice.epcMessageID;
		foreach (SensorAndValue obj in tripodSensors.SensorAndValueList) {
			if (msgID == obj.sensor._dataID) {
				obj.value = msgValue;
			}
			_displayValuesOnGUI ();
		}

		if (_EpcDevice.epcMessageValueOld != _EpcDevice.epcMessageValue) {
			//			connectThread.Abort ();
			_EpcDevice.epcMessageValueOld = _EpcDevice.epcMessageValue;
		}
		if (_EpcDevice.isConnected && !isAppConnected) {
			print ("CONNECTED");
			// Enable messaging
			foreach (SensorAndValue obj in tripodSensors.SensorAndValueList)
			{
				EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, obj.sensor._dataID, 4, 0);
				_EpcDevice.Send(message);
				// Disable
//				EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Disable, obj.sensor._dataID, 4, 0);
//				_EpcDevice.Send(message);
				isAppConnected = true;
			}
			//			print ("Wartość odczytu: "+ _EpcDevice.epcMessageValue);
		}
	}
	private void _connect(){
		print ("Connection thread started");
		_EpcDevice.Connect ();
	}

	private void _displayValuesOnGUI(){
		foreach (SensorAndValue obj in tripodSensors.SensorAndValueList) {
			//TODO: Switch statement for displaying text of each sensor (its value)
			switch (obj.sensor._dataID) {
			case 1:
				statusText.ProgramUpdate ("Odczytano: " + obj.sensor.sensorName + " ID: " + obj.sensor._dataID + " value: " + obj.value);
				break;
			case 2:
				statusText.TripodStateUpdate ("Odczytano: " + obj.sensor.sensorName + " ID: " + obj.sensor._dataID + " value: " + obj.value);
				break;
			case 3:
				statusText.CameraUpdate ("Odczytano: " + obj.sensor.sensorName + " ID: " + obj.sensor._dataID + " value: " + obj.value);
				break;
			case 10:
				statusText.PosXUpdate ("Odczytano: " + obj.sensor.sensorName + " ID: " + obj.sensor._dataID + " value: " + obj.value);
				break;
			case 11:
				statusText.PosYUpdate ("Odczytano: " + obj.sensor.sensorName + " ID: " + obj.sensor._dataID + " value: " + obj.value);
				break;
			case 12:
				statusText.PosZUpdate ("Odczytano: " + obj.sensor.sensorName + " ID: " + obj.sensor._dataID + " value: " + obj.value);
				break;
			case 20:
				statusText.VelocityUpdate ((float)(obj.value)/10000);
				VelocityGauge.ShowSpeed (obj.value, 0, 1000000);
				break;
			case 21:
				statusText.AccelerationUpdate ((float)(obj.value)/10000000);
				AccelerationGauge.ShowSpeed (obj.value, 0, 90000000);
				break;
			case 30:
				statusText.AirSupplyUpdate ((float)(obj.value)/10);
				AirSupplyGauge.ShowSpeed (obj.value, 0, 70);
				break;
			}
		}
	}

	void OnGUI(){

		if (!_EpcDevice.isConnected) {

			if (GUI.Button (new Rect (10, 10, 150, 100), "Connect")) {
				print ("You clicked the button!");
				//				_EpcDevice = new EpcDevice (host, port, deviceName, dataID, sensorName);
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
			if (GUI.Button (new Rect (160, 10, 150, 100), "Pos Y")) {
				EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, 11, 4, 0);
				_EpcDevice.Send(message);
			}
			if (GUI.Button (new Rect (160, 110, 150, 100), "Pos Z")) {
				EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, 12, 4, 0);
				_EpcDevice.Send(message);
			}
			if (GUI.Button (new Rect (310, 110, 150, 100), "Disable all")) {
				foreach (SensorAndValue obj in tripodSensors.SensorAndValueList)
				{
					EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Disable, obj.sensor._dataID, 4, 0);
					_EpcDevice.Send(message);
					// TODO: Make sure all the values are set to 0 after disabling the stream
//					obj.value = 0;
				}
			}
			if (GUI.Button (new Rect (310, 10, 150, 100), "Enable all")) {
				foreach (SensorAndValue obj in tripodSensors.SensorAndValueList)
				{
					EpcMessage message = new EpcMessage(EpcMessage.MessgaeType.Enable, obj.sensor._dataID, 4, 0);
					_EpcDevice.Send(message);
				}
			}
		}
	}



}