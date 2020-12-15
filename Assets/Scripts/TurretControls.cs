using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretControls : MonoBehaviour {

    /*
    This is a seperate script because the vehicle can have multiple turrets, which each need to do different things. What this script shouldn't be doing is passing information elsewhere, because it gets really confusing.

    It would be good to have an array of all weapons attached to the turret, which is then passed to TankControls on start. then tankControls can take data from the turret and the guns and pass data directly to them rather than
    through all of them?

    should we be offloading more variables to the weapon rather than recalculating them here every time we switch guns?

    */


	//variables for the new turret rotation script
	public float maxDegreesPerSecond = 45.0f;
	Quaternion targetQuaternion;
	Vector3 targetVector;
	float rotCompare;

	public Transform turret;
	public Transform rotKey;
	public Transform rotTarget;
	public Transform elevTarget;
	public Transform gun; //determined by weapon prefab
	public Transform muzzle;  //determined by weapon prefab

    public string currentWeaponName;
    int gunCount = 0;//used for switching between weapons. 
    public Weapon_Gun weapon_Gun;

    public Transform aimPoint;

    //All of these values should probably be per mount.
	float turnSpeed = .2f;
	float leftLimit = 30f;
	float rightLimit = 330f;

	float elevSpeed = .1f;
    float elevLimit = 45f;
    float depressLimit = -5f;  
    float elevCheck = 0f;
	float elevCurrent = 0f;

	List<GunIndex> gunIndex = new List<GunIndex>();

    float shellForce = 0;
    float shellVelocity;

    //Fire Control Variables - Don't confuse with gun mechanics!
    public bool elevReady = false;
	public bool rotReady = false;
	public int gunsReady = 0; //ripple fire is go if this doesn't equal 0, salvo fire if it equals gunsTotal
	public int gunsTotal; //equals gunIndex.Count


    public bool fireSalvo = false;  //determined by weapon prefab
    public bool fireSingle = false;  //determined by weapon prefab

    int count = 0;

	//the following variables are for the trajectory calculations I found.
	bool haveFiringSolution = false;
	bool directFire = true; //ah, we use flight time to decide whether to set it high or low. nice!
	public float maxRange;
	float firingSolution;	



	// Use this for initialization
	void Start () {
		ComponentAssignment();
        gunCount = 0;
        SelectWeapon();
		maxRange = CalculateMaximumRange();
		shellVelocity = shellForce * Time.fixedDeltaTime;
		targetQuaternion = aimPoint.transform.localRotation;
	}

	
	// Update is called once per frame
	void LateUpdate () {
		TurretRotation();
		count = 0;
		while(count < gunsTotal){
			GunElevation(gunIndex[count].gun);
			count++;
		}

		if(fireSalvo == true){
			fireSalvo = false;
			////FireSalvo();
		}
		else if(fireSingle == true){
            Debug.Log("TurretControls detects fire order!");
			fireSingle = false;
			FireSingle();
		}


	}
		
	void TurretRotation(){ //this doesn't account for off-center gun mounts. we'll need a different system eventually.
		targetVector = aimPoint.transform.position - turret.transform.position;
		targetVector.y = turret.transform.position.y;
		targetQuaternion = Quaternion.LookRotation(targetVector, Vector3.up);
		Quaternion tempRotation = turret.transform.localRotation; //stores the current localRotation so we can cancel out everything but the y-axis.

		rotCompare = Mathf.Abs(turret.transform.eulerAngles.y - targetQuaternion.eulerAngles.y); 

		if(rotCompare < .1){
			turret.transform.eulerAngles = new Vector3(0, targetQuaternion.eulerAngles.y, 0);
			rotReady = true;
		}
		else{
			rotReady = false;
			turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, targetQuaternion, maxDegreesPerSecond * Time.deltaTime);
			turret.localRotation = Quaternion.Euler( 0, turret.localRotation.eulerAngles.y, 0);
		}


	}


	void GunElevation(Transform gun){
		firingSolution = CalculateProjectileFiringSolution() * Mathf.Rad2Deg;

		if(firingSolution == 0){
			firingSolution = elevLimit * -1;
			elevReady = false;
		}
		else if(firingSolution > 5){
			firingSolution = depressLimit * -1;
			elevReady = false;
		}

		elevTarget.eulerAngles = new Vector3(firingSolution, turret.eulerAngles.y, 0);

		elevCheck = FixEulerAngles(elevTarget.localEulerAngles.x);
		elevCurrent = FixEulerAngles(gun.localEulerAngles.x);

		if(elevCurrent < elevCheck){
			gun.Rotate (Vector3.right * (elevSpeed * -1));//elevate
		}
		else{
			gun.Rotate (Vector3.right * elevSpeed);//depress
		}
		if(Mathf.Abs (elevCheck - elevCurrent) < 3){
			if(Mathf.Abs(elevCheck - elevCurrent) < elevSpeed){
				gun.localEulerAngles = new Vector3(elevTarget.localEulerAngles.x, 0, 0);
				elevReady = true;
			}		
		}
		else{
			elevReady = false;
		}
	}

	float FixEulerAngles(float rotSource){
		if(rotSource > 180){
			return (rotSource - 360) * -1;
		}
		else{
			return rotSource * -1;
		}
	}



/*
	void FireSalvo(){
		gunCount = 0;
		while(gunCount < gunsTotal){
			FireShell(gunIndex[gunCount].muzzle);
			gunIndex[gunCount].State = GunState.Loading;
			//StartCoroutine(ReloadTimer(gunCount));
			gunCount++;
		}
	}*/


	void FireSingle(){
        gunIndex[gunCount].weapon_Gun.fire = true;

        /*
		gunCount = 0;
		while(gunCount < gunsTotal){
			if(gunIndex[gunCount].State == GunState.Ready){
				FireShell (gunIndex[gunCount].muzzle);
				StartCoroutine(ReloadTimer(gunCount));
				break;
			}
			else{
				gunCount++;
			}
		}
        */

	}



    void SelectWeapon() {
        weapon_Gun = gunIndex[gunCount].weapon_Gun;
        currentWeaponName = weapon_Gun.name;
        shellForce = weapon_Gun.shellForce;
    }



	void ComponentAssignment(){ //assignment is fixed, but the ready variables aren't switching to "ready".
		Transform[] allChildren = GetComponentsInChildren<Transform>();
		foreach(Transform child in allChildren){
			if(child.gameObject.name == "Turret"){
				turret = child;
			}
			if(child.gameObject.name == "RotationKey"){
				rotKey = child;
			}
			if(child.gameObject.name == "RotationTarget"){
				rotTarget = child;
			}
			if(child.gameObject.name == "ElevationTarget"){
				elevTarget = child;
			}
			
			if(child.gameObject.tag == "Weapon_Gun"){
				gun = child;
                weapon_Gun = gun.GetComponent<Weapon_Gun>(); 
				foreach (Transform childTransform in gun)
				{
					if(childTransform.gameObject.name == "Muzzle"){
						muzzle = childTransform;
					}

				}
				if(gun != null && muzzle != null){
					gunIndex.Add(new GunIndex(gun, weapon_Gun));
				}
				else{
					Debug.Log ("INDEXING ERROR! GUN OR MUZZLE INDEXING FAILED!");
				}
			}
		}
		gunsTotal = gunIndex.Count;
		count = 0;
    }



    float CalculateMaximumRange() { //borrowed the following from a Unity forums answer.

        float g = Physics.gravity.y; 
		float y = gunIndex[0].gun.position.y; //origin //centerpoint of the gun's transform?
		float v = shellVelocity; //projectileSpeed //possibly the value we multiply addforce with?
		float a = 45 * Mathf.Deg2Rad;
		
		float vSin = v * Mathf.Cos(a);
		float vCos = v * Mathf.Sin(a);
		
		float sqrt = Mathf.Sqrt(vSin * vSin + 2 * g * y);
		
		return Mathf.Abs((vSin / g) * (vCos + sqrt));
	}

	//borrowed the following from a Unity forums answer. http://answers.unity3d.com/questions/49195/trajectory-of-a-projectile-formula-does-anyone-kno.html
	float CalculateProjectileFiringSolution() {
		Vector3 targetTransform = aimPoint.position; //target is of course the aimPoint
		Vector3 barrelTransform = gunIndex[0].gun.position;
		
		float y = (barrelTransform.y - targetTransform.y);
		float xx = targetTransform.x - barrelTransform.x;
		float xz = targetTransform.z - barrelTransform.z;
		float x = Mathf.Sqrt(xx * xx + xz * xz);
		float v = shellVelocity;
		float g = Physics.gravity.y;
		
		float sqrt = (v*v*v*v) - (g * (g * (x*x) + 2 * y * (v*v)));
		
		// Not enough range
		
		if (sqrt < 0) {
			haveFiringSolution = false;
			return 0.0f;
		}
		
		haveFiringSolution = true;
		
		sqrt = Mathf.Sqrt(sqrt);
		
		// DirectFire chooses the low trajectory, otherwise high trajectory.
		
		if (directFire) {   
			return Mathf.Atan(((v*v) - sqrt) / (g*x));
			
		} else {
			return Mathf.Atan(((v*v) + sqrt) / (g*x));
			
		}
	}

	float CalculateFlightTime(float angle) { 
		Vector3 targetTransform = aimPoint.position;
		Vector3 barrelTransform = gunIndex[0].gun.position;
		
		float x = (targetTransform - barrelTransform).magnitude;
		float v = shellVelocity;
		
		angle = angle == 0 ? 45 : angle;
		
		float time = x / (v * Mathf.Cos(angle * Mathf.Deg2Rad));
		
		return time * .7f;

	}

    





	
}
