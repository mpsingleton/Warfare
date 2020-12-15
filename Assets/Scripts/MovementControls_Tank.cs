using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; //is this wheel attached to motor?
    public bool steering; //does this wheel apply steering angle?
}

public class MovementControls_Tank : MonoBehaviour {

    public GameObject hull;
    public Rigidbody hullRigidBody;
    public List<AxleInfo> axleInfos;
    public Transform turnCenter;

    public int throttle = 0;
    public float torque = 2;



    // Use this for initialization
    void Start () {
        hullRigidBody = hull.GetComponent<Rigidbody>();
        turnCenter = hull.transform;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Throttle();
        Engine();
        Turn();
	}

    void Throttle()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
        {

            throttle = 2;

        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {

            throttle = -1;

        }
        else if (Input.GetAxisRaw("Vertical") == 0)
        {   //add actual throttle decay later

            throttle = 0;

        }
    }

    void Engine()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = torque * throttle;
                axleInfo.rightWheel.motorTorque = torque * throttle;
            }
        }
    }

    void Turn()
    {
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            throttle = 2;
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = -1 * (torque * throttle);
                    axleInfo.rightWheel.motorTorque = torque * throttle;
                }
            }
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            throttle = 2;
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = torque * throttle;
                    axleInfo.rightWheel.motorTorque = -1 * (torque * throttle);
                    Debug.Log(throttle * torque);
                }
            }
        }
        if (Input.GetAxis("Horizontal") == 0)
        {
            throttle = 0;
        }
    }





}
