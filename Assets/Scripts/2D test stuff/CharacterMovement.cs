using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

    public GameObject character;
    public float speed = .1f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
 
        CharacterMove();

	}

    void CharacterMove() {

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            //Debug.Log("move left!");
            character.transform.Translate(Vector3.right * (speed));

        }
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            //Debug.Log("move right!");
            character.transform.Translate(Vector3.right * (speed * -1));

        }
    }



}
