using UnityEngine;
using System.Collections;

/*
TurretIndex stores turret enum Types, and Vector3 Positions. This seems like it 
should be loaded from a seperate data file. for now, we'll assign it from 
ShipControls, and then use its values to load the GunIndex. TurretClass is drawn
from the variables in ShipStats.

*/

public enum TurretClass{Main, DualPurpose, AntiAir, Torpedo}; //probably more classes needed later. 

public class TurretIndex{

	public Vector3 turretPosition;
	public GameObject turretType;

	public TurretClass Class;
	
	public TurretIndex(TurretClass currentClass, Vector3 currentPosition)
	{
		Class = currentClass;
		turretPosition = currentPosition;
	}

}