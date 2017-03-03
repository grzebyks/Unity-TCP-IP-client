using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShader : MonoBehaviour {

	public static Renderer rend;
	private static float n;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("MyCustomShader");
//		rend.material.SetColor("_Color", new Color (1.0f,0.0f,0.0f,0.5f));
		n = 0;
	}
	
	// Update is called once per frame
	void Update () {
//		this.rend.material.SetColor("_Color", new Color (1.0f,0.0f,0.0f,0.5f));
	}

	public static void ChangeShade(){
		rend.material.SetColor ("_Color", new Color (1.0f, 0.0f+n, 0.0f, 0.5f));
		n += 0.2f;
	}


//	void OnGUI(){
//
//		if (GUI.Button (new Rect (10, 10, 150, 100), "Change")) {
//			print ("You clicked the button!");
//			//				_EpcDevice = new EpcDevice (host, port, deviceName, dataID, sensorName);
//			this.rend.material.SetColor ("_Color", new Color (1.0f, 0.0f+n, 0.0f, 0.5f));
//			n += 0.2f;
//		}
//	}
}
