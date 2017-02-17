using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// Class created in order to give more flexibility in the future usage with
// multiple robots. Uses new EpcDevice property - _dataID. Thanks to it every
// sensor is treated as a single EpcDevice, but connection is established only
// once with a particular ip.
// TODO: consider creating EpcSensor - a subclass of EpcDevice; consider adding a value multiplier.
public class ListOfTrackedSensors {

	private String _host;
	private Int32 _port;
	private String _deviceName;
	public List <SensorAndValue> SensorAndValueList { get; private set; } 


	public ListOfTrackedSensors (string host, Int32 port, string deviceName){
		_host = host;
		_port = port;
		_deviceName = deviceName;
		SensorAndValueList = new List<SensorAndValue> ();
		SensorAndValueList = _createDeviceList ();
	}

	private List<SensorAndValue> _createDeviceList (){

		List<String> SensorNameList = new List<String>();
		SensorNameList = NamesOfTrackedObjects ();
		List<ushort> DeviceIDList = new List<ushort> ();
		DeviceIDList = IdOfTrackedObjects ();
		Debug.Log ("Ilosc sensorow: " + SensorNameList.Count);
		List <EpcDevice> _DeviceList = new List<EpcDevice>();
		for (int i = 0; i < 9; i++) {
			_DeviceList.Add (new EpcDevice (
				_host, _port, _deviceName, DeviceIDList.ElementAt (i), SensorNameList.ElementAt (i)));
		}

		List <SensorAndValue> _SensorAndValueList = new List<SensorAndValue>();
		foreach (EpcDevice _sensor in _DeviceList) {
			SensorAndValue _singleSAV = new SensorAndValue (_sensor, 0);
			_SensorAndValueList.Add (_singleSAV);
		}

		return _SensorAndValueList;
	}
		

	private List<String> NamesOfTrackedObjects(){
		// TODO: create list of names
		List <String> SensorNameList = new List<String>();

		// Hardcoded. Could be from a cloud file
		SensorNameList.Add ("Active Program Number");
		SensorNameList.Add ("Tripod State");
		SensorNameList.Add ("Camera State");
		SensorNameList.Add ("Pos X");
		SensorNameList.Add ("Pos Y");
		SensorNameList.Add ("Pos Z");
		SensorNameList.Add ("Velocity");
		SensorNameList.Add ("Acceleration");
		SensorNameList.Add ("Air Supply Pressure");

		return SensorNameList;
	}
	private List<ushort> IdOfTrackedObjects(){
		// TODO: create list of names
		List <ushort> DeviceIDList = new List<ushort>();

		// Hardcoded. Could be from a cloud file
		DeviceIDList.Add (1);
		DeviceIDList.Add (2);
		DeviceIDList.Add (3);
		DeviceIDList.Add (10);
		DeviceIDList.Add (11);
		DeviceIDList.Add (12);
		DeviceIDList.Add (20);
		DeviceIDList.Add (21);
		DeviceIDList.Add (30);

		return DeviceIDList;
	}
}
