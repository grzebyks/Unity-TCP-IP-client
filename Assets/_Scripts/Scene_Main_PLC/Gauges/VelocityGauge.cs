using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityGauge : MonoBehaviour {

	static float _minAngle = 130.0f;
	static float _maxAngle = -130.0f;
	static VelocityGauge _vGauge;

	// Use this for initialization
	void Start () {
		_vGauge = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void ShowSpeed(float speed, float min, float max){
		float ang = Mathf.Lerp (_minAngle, _maxAngle, Mathf.InverseLerp (min, max, speed));
		_vGauge.transform.eulerAngles = new Vector3 (0, 0, ang);
	}
}
