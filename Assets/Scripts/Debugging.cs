using UnityEngine;
using System.Collections;

public class Debugging : MonoBehaviour {

    public GameObject gun;

    Weapon_Gun weapon_Gun;

	// Use this for initialization
	void Start () {

        weapon_Gun = gun.GetComponent<Weapon_Gun>();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1"))
        {
            weapon_Gun.fire = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            weapon_Gun.fire = false;
        }
	}
}
