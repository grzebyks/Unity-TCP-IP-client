using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatusText : MonoBehaviour {

	public Text connectionStatusText;
	string textToDisplay;

	// Use this for initialization
	void Start () {
		print ("pole tekstowe");
		connectionStatusText.text = "Brak połączenia123";
	}
	
	// Update is called once per frame
	void Update () {
		connectionStatusText.text = textToDisplay;
	}

	public void textUpdate(string text){
		textToDisplay = text;
	}	
}
