using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatusText : MonoBehaviour {

	public Text ProgramNoText;
	public Text TripodStateText;
	public Text CameraText;
	public Text PosXText;
	public Text PosYText;
	public Text PosZText;
	public Text VelocityText;
	public Text AccelerationText;
	public Text AirSupplyText;
	private string textToDisplay;
//	private string posXText;
//	private string posYText;

	// Use this for initialization
	void Start () {
		print ("pole tekstowe");
		ProgramNoText.text = "Brak połączenia123";
	}
	
	// Update is called once per frame
	void Update () {
//		ProgramNoText.text = textToDisplay;
//		PosXText.text = textToDisplay;
//		PosYText.text = textToDisplay;
	}

	public void ProgramUpdate(object _text){
		//		textToDisplay = text;
		string text = _text.ToString ();
		ProgramNoText.text = text;
	}	
	public void TripodStateUpdate(object _text){
		string text = _text.ToString ();
		TripodStateText.text = text;
	}	
	public void CameraUpdate(object _text){
		string text = _text.ToString ();
		CameraText.text = text;
	}	
	public void PosXUpdate(object _text){
		string text = _text.ToString ();
		PosXText.text = text;
	}	
	public void PosYUpdate(object _text){
		string text = _text.ToString ();
		PosYText.text = text;
	}	
	public void PosZUpdate(object _text){
		string text = _text.ToString ();
		PosZText.text = text;
	}	
	public void VelocityUpdate(object _text){
		string text = _text.ToString ();
		VelocityText.text = text;
	}	
	public void AccelerationUpdate(object _text){
		string text = _text.ToString ();
		AccelerationText.text = text;
	}	
	public void AirSupplyUpdate(object _text){
		string text = _text.ToString ();
		AirSupplyText.text = text;
	}	
}
