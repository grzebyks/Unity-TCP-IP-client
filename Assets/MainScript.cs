using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour {

//	private ChangeShader shader;

	private GameObject tripod1;
	private GameObject tripod2;

	// Use this for initialization
	void Start () {
//		shader = GetComponent<ChangeShader>();	
		tripod1 = GameObject.Find ("Tripod1");
		tripod2 = GameObject.Find ("Tripod2");

		tripod1.SetActive (true);
		tripod1.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){

		if (GUI.Button (new Rect (10, 10, 150, 100), "Change")) {
			print ("You clicked the button!");
			//				_EpcDevice = new EpcDevice (host, port, deviceName, dataID, sensorName);
//			ChangeShader.ChangeShade ();
			tripod1.SetActive (!tripod1.activeSelf);
			tripod2.SetActive (!tripod2.activeSelf);
		}
	}

}
