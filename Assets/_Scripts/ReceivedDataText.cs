using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedDataText : MonoBehaviour {

	public Text receivedDataText;
	string textToDisplay;

	// Use this for initialization
	void Start () {
		print ("pole tekstowe");
		receivedDataText.text = "Brak połączenia123";
	}

	// Update is called once per frame
	void Update () {
		receivedDataText.text = "Otrzymane dane: " + textToDisplay;
	}

	public void textUpdate(string text){
		textToDisplay = text;
	}	
}
