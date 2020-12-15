using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
This should be attached to the camera, not the tank. In fact, tankController should probably be attached to the camera as well? 
     
     
*/

public class UI_Controller : MonoBehaviour {

    public Text currentWeapon;
    public GameObject weapon;
    TankControls tankControls;

	// Use this for initialization
	void Start ()
    {

        CurrentWeapon();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void CurrentWeapon()
    {
        currentWeapon.text = weapon.name;
    }




}
