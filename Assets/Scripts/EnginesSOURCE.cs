using UnityEngine;
using System.Collections;

public class enginesSOURCE : MonoBehaviour {

/*
looping environment - the environment loops cleanly on both X and Y axis. 
Not sure how to do this, as the camera needs to bridge the loop as well. Possibly
manually loop any mobile object near the transition as well, with some buffer?

the Ship has a spherical volume around it; this is the playerSpace. Matching 
spherical volumes sit oputside the playspace, one for the X axis, one for the Y
axis. if something enters those volumes, the warp to the opposite end of 
the world triggers early. Likewise, if they're inside the player's sphere, they
don't warp at all. 

The cage around the player is to delay warp for projectiles within visible range of 
the player, and to trigger warp for all projectiles within the cage when the player 
warps.

the other cages enter the environment as the player's cage leaves it. They effectively 
deform the loop boundrey, removing space from one side of the map to compensate for the 
extra space added by the player. Things leaving the edge of the player's cage should 
appear at the corresponding edge of the cage, not the edge of the map; that part of the
map doesn't exist, for looping purposes, so there's no way to "lose space".







*/
	
	public Transform ship;

	public float engineThrust = 3f;
	
	public bool rotationAxisActive = false;
	public float turnCurrent = 0f;
	public float turnAccel = .02f;
	public float turnDecay = .8f;
	public float turnMax = 8f;

	Vector3 xyPosition;	
	float xLimit = 5f;
	float yLimit = 5f;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		/*//Use this to try to make the looping work.
		if(mathf.Abs(position.x) > xlimit){
			if(position.x > ){

			}
			else{

			}
		}
		*/





	}

	void FixedUpdate () {
		LoopSpaceCheck();


		if(Input.GetButton("thrust")){//build to max acceleration rather than ramping speed infinitely.
			ship.GetComponent<Rigidbody>().AddRelativeForce(0, engineThrust, 0);
		}

		if(Input.GetAxisRaw("turn") != 0){
			if(rotationAxisActive == false){
				rotationAxisActive = true;
			}
		}
		if(Input.GetAxisRaw("turn") == 0 && rotationAxisActive == true){
			turnCurrentDecay();
		}

		if(rotationAxisActive == true){
			turnCurrentIncrease();
		}





	}





	void Thrust(){
		//Move thrust mechanics to here once it's developed more.
	}


	void turnCurrentIncrease(){
		if(Input.GetAxisRaw("turn") == 1){
			if(turnCurrent < turnMax){
				turnCurrent += turnAccel;
			}
			else if(turnCurrent > turnMax){
				turnCurrent = turnMax;
			}
		}
		else if(Input.GetAxis("turn") == -1) {
			if(Mathf.Abs(turnCurrent) < turnMax){
				turnCurrent -= turnAccel;
			}
			else if(Mathf.Abs(turnCurrent) > turnMax){
				turnCurrent = turnMax * -1;
			}
			
		}
		ship.Rotate (Vector3.forward * turnCurrent);
	}


	void turnCurrentDecay(){
		if(turnCurrent > 0 && turnCurrent > .001){
			turnCurrent = turnCurrent * turnDecay;
		}
		else if(turnCurrent < 0 && turnCurrent < -.001){
			turnCurrent = (Mathf.Abs(turnCurrent) * turnDecay) * -1;
		}
		else{
			turnCurrent = 0;
			rotationAxisActive = false; //wrong, decay turnCurrent before turning off.
		}
	}

	/*
		probably to be more accurate, we should take the axis value, subtract 
		the diameter of the playfield, and add that to the opposite limit. 
	*/
	void LoopSpaceCheck(){
		if(Mathf.Abs(ship.position.x) > xLimit){
			if(ship.position.x > 0){
				xyPosition = new Vector3(xLimit * -1, 0, 0);
				ship.position = xyPosition;
			}
			else{
				xyPosition = new Vector3(xLimit, 0, 0);
				ship.position = xyPosition;
			}
		}
		if(Mathf.Abs(ship.position.y) > yLimit){
			if(ship.position.y > yLimit){
				xyPosition = new Vector3(0, yLimit * -1, 0);
				ship.position = xyPosition;
			}
			else{
				xyPosition = new Vector3(0, yLimit, 0);
				ship.position = xyPosition;
			}

		}
	}



	
}
