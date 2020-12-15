using UnityEngine;
using System.Collections;
using System.Collections.Generic; //need this for lists to work.

public class ShipControls : MonoBehaviour {

/*
The goal here is to reproduce the functionality of World of Warships

1. Toggled gunnery view, with its own camera controls. Zoom in and you switch to this.
2. shells parented to world space when fired. doesn't seem to be a problem any more
3. terminal velocities and acceleration curves for all throttle settings.

*Gunnery View*

when the sailing view hits its inner limit, we trigger a new view. This one is rigidly 
centered on the ship's conning tower, has a narrower FOV and hence greater magnification 
(2x?), and rolling the mouse wheel riases the camera vertically rather than moving it in 
and out. Rolling down to the bottom of the range switches back to the sailing camera.

reload bar - simple graphical bar that lerps scale between 0 and 1 according to the reload
timer.

reload counter - numerical readout that counts down seconds until the reload is complete.
tracks two digits.

gun status array - set of reload bars laid out; grey out when guns aren't aimed, turn green
when ready and aimed.

rudder gauge - arrow that slides left and right according to the amount of force being added
by a turn. Also has a set-block that marks the region the arrow is sliding to. Has two 
"notches" per side.

throttle gauge - bar marking the throttle positions. set-block snaps to new rudder positions 
as you toggle through them. arrow slides along the bar showing the actual current output of 
the engine. four notches forward, two reverse, one for stopping.

muzzle blast - particles and sound

shell explosion - particles and sound.





*/
	GameObject ship;
	Rigidbody shipRigidBody;
	public Transform rudder;

	//public GameObject MainTurret; //save this for drydock code;

	static int turretCount = 4;
	public TurretControls[] turretControls = new TurretControls[turretCount];
	public GameObject shell;

	enum FireMode {FullSalvo, TurretSalvo, SingleFire};
	FireMode fireMode = FireMode.SingleFire;

	public int throttle = 0;//this goes from -2 to 4. full reverse to ahead full.
	float accelMax = 1f;
	float accelCurrent = 0f;
	float speedAccel = .1f;
	float speedDecay = 1f;
	float speedMax = 20f;
	float idealDrag;
	float lateralSpeed;


	bool rotationAxisActive = false;
	float turnMax = 1f;
	float turnCurrent = 0f;
	float turnAccel = .02f;
	float turnDecay = .8f;

	int count = 0;




	


	// Use this for initialization
	void Start () {
		ship = this.gameObject;
		shipRigidBody = ship.GetComponent<Rigidbody>();

		idealDrag = accelMax / speedMax;
		shipRigidBody.drag = idealDrag / (idealDrag * Time.fixedDeltaTime + 1);

		ComponentAssignment ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Engine();
		Rudder();

		if(Input.GetButtonDown("Fire1")){
			if(fireMode == FireMode.FullSalvo){
				FullSalvo ();
			}
			else if(fireMode == FireMode.TurretSalvo){
				TurretSalvo();
			}
			else if(fireMode == FireMode.SingleFire){
				SingleFire();
			}


			Debug.Log(turretControls[0].gunsReady);
		}



	}

	void ComponentAssignment(){
		count = 0;
		Transform[] allChildren = GetComponentsInChildren<Transform>();
		foreach(Transform child in allChildren){
			if(child.gameObject.name == "Turret_Main"){
				child.gameObject.name = "Turret_Main_0" + (count + 1);
				turretControls[count] = child.GetComponent<TurretControls>();
				count++;
			}

		}
	}


	void Engine(){
		if(Input.GetButtonDown("EngineUp")){
			throttle = Mathf.Clamp((throttle + 1), -2, 4);
		}
		else if(Input.GetButtonDown("EngineDown")){
			Debug.Log("reducing power");
			throttle = Mathf.Clamp((throttle -1), -2, 4);
		}

		accelCurrent = Mathf.Clamp((accelCurrent + (speedAccel * throttle)), (accelMax * -.8f), accelMax);

		shipRigidBody.AddRelativeForce(accelCurrent, 0, 0);

		float dragScaleValue = 1f;
		Vector3 relVelocity = transform.InverseTransformDirection(shipRigidBody.velocity);
		shipRigidBody.AddRelativeForce(-relVelocity.z * dragScaleValue * Vector3.forward);

	}


	void Rudder(){
		if(Input.GetAxisRaw("Horizontal") != 0){
			if(rotationAxisActive == false){
				rotationAxisActive = true;
			}
		}
		if(Input.GetAxisRaw("Horizontal") == 0 && rotationAxisActive == true){
			turnCurrentDecay();
		}
		
		if(rotationAxisActive == true){
			turnCurrentIncrease();
		}
	}


	void turnCurrentIncrease(){
		if(Input.GetAxis("Horizontal") > 0){
			if(turnCurrent < turnMax){
				turnCurrent += turnAccel;
			}
			else if(turnCurrent > turnMax){
				turnCurrent = turnMax;
			}
		}
		else if(Input.GetAxis("Horizontal") < 0) {
			if(Mathf.Abs(turnCurrent) < turnMax){
				turnCurrent -= turnAccel;
			}
			else if(Mathf.Abs(turnCurrent) > turnMax){
				turnCurrent = turnMax * -1;
			}
			
		}
		if(accelCurrent == 0){
			shipRigidBody.AddTorque (transform.up * turnCurrent);
			shipRigidBody.AddRelativeForce(speedAccel, 0, 0);
		}
		else{
			shipRigidBody.AddTorque (transform.up * (turnCurrent / accelCurrent));
		}


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
			rotationAxisActive = false;
		}
	}


	void FullSalvo(){
		count = 0;
		while (count < turretCount){
			if(turretControls[count].gunsReady == turretControls[count].gunsTotal 
			   && turretControls[count].elevReady && turretControls[count].rotReady){
					turretControls[count].fireSalvo = true;
				
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
			if(turretControls[count].gunsReady == turretControls[count].gunsTotal 
			   && turretControls[count].rotReady && turretControls[count].elevReady){
				turretControls[count].fireSalvo = true;
				break;
			}
			else{
				count++;
			}
		}
	}

	void SingleFire(){
		count = 0;
		while(count < turretCount){
			if(turretControls[count].gunsReady > 0 && turretControls[count].elevReady 
			   && turretControls[count].rotReady){
				turretControls[count].fireSingle = true;
				break;
			}
			else{
				count++;
			}
		}





	}



}
