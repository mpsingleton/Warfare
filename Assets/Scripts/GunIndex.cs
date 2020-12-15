using UnityEngine;
using System.Collections;

//GunIndex maintains a simple and efficient index of all guns in a turret.
//Each turret gets a copy.


public enum GunState {Ready, Loading, Locked};
public enum ShellType {HighExplosive, ArmorPiercing};

public class GunIndex
{
	public Transform gun;
    public Weapon_Gun weapon_Gun;
    


	public GunIndex(Transform gunBarrel, Weapon_Gun gunScript)
	{
		gun = gunBarrel;
        weapon_Gun = gunScript;
	}

}