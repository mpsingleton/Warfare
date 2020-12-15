using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*




This script should handle weapon selection between all the weapons on a vehicle, one weapon at a time.
it should check whether a gun is ready.
it should check whether the gun elevation is correct.
it should check if the turret rotation is correct.
if elevation, rotation, and the gun are all ready, it should accept firing input.

Rather than linking all the other scripts to this one, let's have each one handle its own input.

it should have a gunIndex built up of all the weapons in the various turrets.
it should be able to select between different guns, but all guns should be tracking the cursor at all times.
it should be passing and receiving data both from the various guns and also from the various turrets.

none of this means anything without the UI to control it, so let's start with that.
a simple readout that displays the currently selected weapon, and some buttons to change which weapon is selected.

movement control is handled through the movementcontrols_tank script, on the controller object.


*/


public class TankControls : MonoBehaviour {
    
	GameObject hull;
	Rigidbody hullRigidBody;
	public Transform turnCenter;

	static int turretCount = 1;
    public TurretControls[] turretControls = new TurretControls[turretCount];
    List<GunIndex> gunIndex = new List<GunIndex>();
    Transform gun;

    public string currentWeaponName;
    int gunCount = 0;//used for switching between weapons. 
    Weapon_Gun weapon_Gun;



    enum FireMode {FullSalvo, TurretSalvo, SingleFire};
	FireMode fireMode = FireMode.SingleFire;

	int count = 0;

    //public GameObject shell;



    // Use this for initialization
    void Start () {
		hull = this.gameObject;
		hullRigidBody = hull.GetComponent<Rigidbody>();
		ComponentAssignment ();
        currentWeaponName = gunIndex[0].weapon_Gun.name;
        Debug.Log(gunIndex.Count);
	}

    //gunIndex[gunCount].weapon_Gun.fire = true;
    // Update is called once per frame
    void FixedUpdate () {

		if(Input.GetButtonDown("Fire1")){

		}
        if (Input.GetButtonDown("SwitchWeapons"))
        {
            SwitchWeapons();
        }
    }

    //salvo and ripple-fire controls here, when we need them again.
    /*
void FullSalvo() //these firing modes either need to be removed entirely or moved to the turret level. not sure they're super useful.
{
    count = 0;
    while (count < turretCount){
        if(turretControlsTank[count].gunsReady == turretControlsTank[count].gunsTotal 
           && turretControlsTank[count].elevReady && turretControlsTank[count].rotReady){
            turretControlsTank[count].fireSalvo = true;
        }
        else{
            //Turret not ready!
        }
        count++;
    }		
}


void TurretSalvo(){
    count = 0;
    while(count < turretCount){
        if(turretControlsTank[count].gunsReady == turretControlsTank[count].gunsTotal 
           && turretControlsTank[count].rotReady && turretControlsTank[count].elevReady){
            turretControlsTank[count].fireSalvo = true;
            break;
        }
        else{
            count++;
        }
    }
}

//shooting is broken here until we seperate the code out better.
void SingleFire(){
    Debug.Log("SingleFire fire activated");
    count = 0;
    while(count < turretCount){

        if(turretControlsTank[count].gunsReady > 0 && turretControlsTank[count].elevReady 
           && turretControlsTank[count].rotReady){
            turretControlsTank[count].fireSingle = true;
            Debug.Log("sending fire order to turretControlsTank script");
            break;
        }
        else{
            count++;
        }
    }
}
*/
    void SwitchWeapons()
    {
        if (gunCount >= gunIndex.Count - 1)
        {
            gunCount = 0;
            currentWeaponName = gunIndex[gunCount].weapon_Gun.name;
        }
        else
        {
            gunCount++;
            currentWeaponName = gunIndex[gunCount].weapon_Gun.name;
        }
    }


    void ComponentAssignment()
    {
        count = 0;
        Transform[] allChildren = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name == "Turret_Main")
            {
                child.gameObject.name = "Turret_Main_0" + (count + 1);
                turretControls[count] = child.GetComponent<TurretControls>();
                count++;
            }
            if (child.gameObject.tag == "Weapon_Gun")
            {
                gun = child;
                weapon_Gun = gun.GetComponent<Weapon_Gun>();
                if (gun != null)
                {
                    Debug.Log(gun.name);
                    gunIndex.Add(new GunIndex(gun, weapon_Gun));
                }
                else
                {
                    Debug.Log("INDEXING ERROR! TankControls>ComponentAssignment gunIndex failed!");
                }
            }
        }
    }



}





