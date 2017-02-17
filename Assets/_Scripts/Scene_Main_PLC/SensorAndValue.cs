using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO: Add min,max values of the sensor and multiplier
public class SensorAndValue {
	public EpcDevice sensor { get; private set; }
	public int value;

	public SensorAndValue(EpcDevice _sensor, int _value){
		sensor = _sensor;
		value = 0;
	}

	public String GenerateDisplayString(){
		String text = sensor.sensorName + ": " + value;
		return text;
	}
}