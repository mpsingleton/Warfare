using UnityEngine;
using System.Collections;

public class Weapon_Gun : MonoBehaviour {

    //Weapon Type Variables
    public string weaponType = "152mm D11 K Howitzer";
    public Transform muzzle;

    //Audio variables
    public AudioClip gunShot;
    new AudioSource audio;
    float lowPitchRange = .8f;
    float highPitchRange = 1.2f;
    //float lowVolRange = .6f;
    //float highVolRange = .8f;

    public GameObject shell;
    GameObject[] shellPool = new GameObject[6];
    int shellCount = 0;

    //Ammo variables
    public GameObject[] ammoTypes = new GameObject[1]; //Store types of shells here.
    public float caliber = 152f; //caliber in mm. use this for overmatch, damage multipliers, spalling.
    public float shellSize = 10.5f; //use this one to calculate how many rounds can be held in a magazine.
    public float shellForce = 2000f; //modified by shell type.
    public float dispersion = 0.0f;

    //Firing Mode Variables
    public float breechLoadTime = 5.5f; //base load time, modified by turret;

    public bool rippleFire = false; //move these to a mount script?
    public bool salvoFire = false;

    public bool burstFire = false;
    public int burstTotal = 3;
    public int burstCount = 0;

    public bool autoFire = false;

    public int magCapacity = 1; //if this isn't greater than 1, set burstFire and autoFire to false;
    public float magLoadTime = 1.0f;
    public float magRefillTime = 5.5f;
    public int magCount = 0;

    //Fire Control Variables
    public bool loaded = false;
    public bool fire = false;
    bool firing = false;

    int count = 0;


    // Use this for initialization
    void Start () {
        audio = GetComponentInChildren<AudioSource>();

        if(burstFire && magCapacity < 2) {
            burstFire = false;
            Debug.Log("burst/auto invalid: no magazine");
        }
        if (autoFire && magCapacity < 2)
        {
            autoFire = false;
            Debug.Log("burst/auto invalid: no magazine");
        }

        ShellPool();
        if (magCapacity > 1) {
            
        }
        StartCoroutine(Reload());
    }
	
	// Update is called once per frame
	void Update () {

        if (fire) {
            fire = false;
            if (loaded && firing == false)
            {
                firing = true;
                Fire();
            }
            else {
                Debug.Log("Gun Cycle Not Complete");
            }
        }
	}


    void ShellPool() {
        count = 0;
        while (count < shellPool.Length)
        {
            float varX = this.transform.position.x;
            float varY = this.transform.position.y;
            float varZ = this.transform.position.z;
            shellPool[count] = Instantiate(shell, new Vector3(varX, varY, varZ), Quaternion.identity) as GameObject;
            shellPool[count].transform.parent = this.transform;
            count++;
        }
    }


    void Fire()
    {
        if (shellCount >= shellPool.Length)
        {
            shellCount = 0;
        }

        Rigidbody rb = shellPool[shellCount].GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
        shellPool[shellCount].transform.position = muzzle.position;
        shellPool[shellCount].transform.rotation = muzzle.rotation;
        shellPool[shellCount].SetActive(true);
        rb.AddRelativeForce(Vector3.forward * shellForce);

        audio.pitch = Random.Range(lowPitchRange, highPitchRange);
        audio.PlayOneShot(gunShot);

        shellCount++;
        loaded = false;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        magCount--;
        if (burstFire == true)
        {
            if (magCount < 0)
            {
                loaded = false;
                firing = false;
                yield return new WaitForSeconds(magRefillTime);
                yield return new WaitForSeconds(breechLoadTime);
                magCount = magCapacity - 1;
                burstCount = 0;
                loaded = true;
            }
            else
            {
                yield return new WaitForSeconds(magLoadTime);
                if (burstCount + 1 >= burstTotal)
                {
                    burstCount = 0;
                    firing = false;
                    loaded = true;
                    //Debug.Log("burst ended");
                }
                else
                {
                    burstCount++;
                    firing = true;
                    Fire();
                    //Debug.Log("bang!");
                }
            }
        }
        else if (autoFire == true)
        {
            if (magCount < 0)
            {
                yield return new WaitForSeconds(magRefillTime);
                magCount = magCapacity - 1;
                yield return new WaitForSeconds(breechLoadTime);
                loaded = true;
                firing = false;
            }
            else
            {
                yield return new WaitForSeconds(magLoadTime);
                loaded = true;
                firing = false;
            }
        }
        else {
            yield return new WaitForSeconds(breechLoadTime);
            loaded = true;
            firing = false;
        }
    }
}
