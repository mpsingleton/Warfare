using UnityEngine;
using System.Collections;

public class ShellFuse : MonoBehaviour {

	public Rigidbody rb;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other){
		Debug.Log (other.name);
		this.gameObject.SetActive (false);




	}




}
