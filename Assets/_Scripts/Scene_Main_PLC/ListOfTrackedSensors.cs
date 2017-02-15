using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ListOfTrackedSensors {


	public static List <EpcDevice> DeviceList (string host, Int32 port, string deviceName){

		List<String> SensorNameList = new List<String>();
		SensorNameList = NamesOfTrackedObjects ();
		List<ushort> DeviceIDList = new List<ushort> ();
		DeviceIDList = IdOfTrackedObjects ();
		Debug.Log ("Ilosc sensorow: " + SensorNameList.Count);
		List <EpcDevice> _DeviceList = new List<EpcDevice>();
		for (int i = 0; i < 9; i++) {
			_DeviceList.Add (new EpcDevice (
				host, port, deviceName,DeviceIDList.ElementAt (i), SensorNameList.ElementAt (i)));
		}
		return _DeviceList;
	}
		

	private static List<String> NamesOfTrackedObjects(){
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
	private static List<ushort> IdOfTrackedObjects(){
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
