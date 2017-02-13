using System;
using System.Collections.Generic;
using UnityEngine;

// Here are all devices to be connected to.
// Logic will have to be changed in order to apply for multiple targets (tracked objects)

// Simple business object. A PartId is used to identify the type of part 
// but the part name can change. 
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Sockets;



public class EpcDeviceProperties : IEquatable<EpcDeviceProperties>
{
	public String hostIP { get; set; }
	public Int32 port { get; set; }
	public String deviceName { get; set; }
	public ushort dataID { get; set; }
	public String descriptionOfData { get; set; }
	public EpcDevice thisDevice { get; set; }
	public String sensorValue { get; set; }
	public TcpClient tcpClient { get; set; }

	public void readSensorValue(){
		try{
			thisDevice.readSensorManager (tcpClient);
		} catch (Exception e){
			Debug.Log (e);
		}
	}


	public override bool Equals(object obj)
	{
		if (obj == null) return false;
		EpcDeviceProperties objAsPart = obj as EpcDeviceProperties;
		if (objAsPart == null) return false;
		else return Equals(objAsPart);
	}
	public override int GetHashCode()
	{
		return dataID;
	}
	public bool Equals(EpcDeviceProperties other)
	{
		if (other == null) return false;
		return (this.dataID.Equals(other.dataID));
	}
//	public Thread connectAsync(){
//		Thread _thread = new Thread (connect);
//		return _thread;
//	}
	// Should also override == and != operators.

}

//public class Example
//{
//	public static void Main()
//	{
//		// Create a list of connectedDevices.
//		List<EpcDeviceList> connectedDevices = new List<EpcDeviceList>();
//
//		// Add connectedDevices to the list.
//
//		connectedDevices.Add(new Part() {PartName="crank arm", PartId=1234});
//		connectedDevices.Add(new Part() { PartName = "chain ring", PartId = 1334 });
//		connectedDevices.Add(new Part() { PartName = "regular seat", PartId = 1434 });
//		connectedDevices.Add(new Part() { PartName = "banana seat", PartId = 1444 });
//		connectedDevices.Add(new Part() { PartName = "cassette", PartId = 1534 });
//		connectedDevices.Add(new Part() { PartName = "shift lever", PartId = 1634 });
//
//		// Write out the connectedDevices in the list. This will call the overridden ToString method
//		// in the Part class.
//		Console.WriteLine();
//		foreach (Part aPart in connectedDevices)
//		{
//			Console.WriteLine(aPart);
//		}
//
//
//		// Check the list for part #1734. This calls the IEquitable.Equals method
//		// of the Part class, which checks the PartId for equality.
//		Console.WriteLine("\nContains(\"1734\"): {0}",
//			connectedDevices.Contains(new Part {PartId=1734, PartName="" }));
//
//		// Insert a new item at position 2.
//		Console.WriteLine("\nInsert(2, \"1834\")");
//		connectedDevices.Insert(2, new Part() { PartName = "brake lever", PartId = 1834 });
//
//
//		//Console.WriteLine();
//		foreach (Part aPart in connectedDevices)
//		{
//			Console.WriteLine(aPart);
//		}
//
//		Console.WriteLine("\nconnectedDevices[3]: {0}", connectedDevices[3]);
//
//		Console.WriteLine("\nRemove(\"1534\")");
//
//		// This will remove part 1534 even though the PartName is different,
//		// because the Equals method only checks PartId for equality.
//		connectedDevices.Remove(new Part(){PartId=1534, PartName="cogs"});
//
//		Console.WriteLine();
//		foreach (Part aPart in connectedDevices)
//		{
//			Console.WriteLine(aPart);
//		}
//		Console.WriteLine("\nRemoveAt(3)");
//		// This will remove the part at index 3.
//		connectedDevices.RemoveAt(3);
//
//		Console.WriteLine();
//		foreach (Part aPart in connectedDevices)
//		{
//			Console.WriteLine(aPart);
//		}
//
//		/*
//
//             ID: 1234   Name: crank arm
//             ID: 1334   Name: chain ring
//             ID: 1434   Name: regular seat
//             ID: 1444   Name: banana seat
//             ID: 1534   Name: cassette
//             ID: 1634   Name: shift lever
//
//             Contains("1734"): False
//
//             Insert(2, "1834")
//             ID: 1234   Name: crank arm
//             ID: 1334   Name: chain ring
//             ID: 1834   Name: brake lever
//             ID: 1434   Name: regular seat
//             ID: 1444   Name: banana seat
//             ID: 1534   Name: cassette
//             ID: 1634   Name: shift lever
//
//             connectedDevices[3]: ID: 1434   Name: regular seat
//
//             Remove("1534")
//
//             ID: 1234   Name: crank arm
//             ID: 1334   Name: chain ring
//             ID: 1834   Name: brake lever
//             ID: 1434   Name: regular seat
//             ID: 1444   Name: banana seat
//             ID: 1634   Name: shift lever
//
//             RemoveAt(3)
//
//             ID: 1234   Name: crank arm
//             ID: 1334   Name: chain ring
//             ID: 1834   Name: brake lever
//             ID: 1444   Name: banana seat
//             ID: 1634   Name: shift lever
//
//
//         */
//
//	}
//}
