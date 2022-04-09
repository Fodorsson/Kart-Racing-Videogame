using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartControl : MonoBehaviour
{

    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
    private Rigidbody rb;

    [SerializeField] private float topSpeed = 80;
    [SerializeField] private float maxSteerAngle = 10;
    [SerializeField] private float maxTorque = 50;
    [SerializeField] private float maxBrakeTorque = 60;

    public float currentSpeed;
    public float SteerSensitivity = 0.75f;
    public float AccelSensitivity = 1f;

    public float Brake;

    public string HorizontalAxisName;
    public string VerticalAxisName;

    private KeyCode key_activate;
    private KeyCode key_brake;
    private KeyCode key_lookBehind;
    private KeyCode key_pause;

    public bool CutsceneMode;
    public bool finished = false;

    public int CheckpointsCleared;
    public int currentLap;
    public int LatestCPindex;

    //What kind of powerup does the player currently possess? 0: none, 1: attack, 2: defend, 3: trap
    public int powerupPossessed = 0;

    public bool stunned = false;

    public bool invincible = false;

    public GameObject playerCanvas;

    //Declare the ListGO GameObject, which has the FindGO script attached to it
    public GameObject ListGO;
    public static FindGO FindGO;

    public bool OnGround;

    public AudioSource ASwheel;

    private bool turnSoundAllowed;

    private void Awake()
    {
        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();
    }

    void Start()
    {
        AudioClip clipEngine = Resources.Load("sfx/03 engine", typeof(AudioClip)) as AudioClip;
        AudioSource ASengine = ListGO.transform.GetChild(1).GetComponent<AudioSource>();
        ASengine.clip = clipEngine;
        ASengine.loop = true;
        ASengine.Play();

        AudioClip clipWheel = Resources.Load("sfx/04 wheel squeal", typeof(AudioClip)) as AudioClip;

        if (transform.tag == "P1")
            ASwheel = ListGO.transform.GetChild(2).GetComponent<AudioSource>();
        else
            ASwheel = ListGO.transform.GetChild(4).GetComponent<AudioSource>();

        ASwheel.clip = clipWheel;
        ASwheel.loop = true;
        ASwheel.Play();

        OnGround = true;


        if (transform.name == "P1 Avatar")
        {
            key_activate = Save.p1ActivateKey;
            key_brake = Save.p1BrakeKey;
            key_lookBehind = Save.p1LookBehindKey;
            key_pause = Save.p1PauseKey;
        }
        else if (transform.name == "P2 Avatar")
        {
            key_activate = Save.p2ActivateKey;
            key_brake = Save.p2BrakeKey;
            key_lookBehind = Save.p2LookBehindKey;
            key_pause = Save.p2PauseKey;
        }


        m_WheelColliders[0].ConfigureVehicleSubsteps(1, 71, 71);

        CutsceneMode = true;

        m_WheelColliders[0].attachedRigidbody.centerOfMass = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();

        //We need to place the center of mass a bit lower, so that the kart would always land on its feet
        rb.centerOfMass = new Vector3(0f, -0f, 0f);

        CheckpointsCleared = 0;

        currentLap = 1;

        //The powerup icon is invisible by default
        playerCanvas.transform.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 0);

    }

    public void Update()
    {

        if (!CutsceneMode && !stunned)
        {

            float h = Input.GetAxis(HorizontalAxisName);
            float v = Input.GetAxis(VerticalAxisName);

            



            Brake = Convert.ToInt32(Input.GetKey(key_brake));

            VehicleMove(h, v);

            //If the vehicle is in the air then rotate it with the input from the controller
            if (!OnGround)
                VehicleRotate(h, v);

            //If the player presses the fire button and has a powerup equipped
            if (Input.GetKeyDown(key_activate) && powerupPossessed != 0)
            {
                //If the powerup is a trap
                if (powerupPossessed == 3)
                    DropTrap(transform.position, transform.rotation);
                //If the powerup is a bullet
                else if (powerupPossessed == 1)
                    FireBullet(transform.position, transform.rotation);
                //If the powerup is a shield
                else if (powerupPossessed == 2)
                    ActivateShield(transform.position);

                //Spend the powerup
                powerupPossessed = 0;

                playerCanvas.transform.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 0);

            }

            //If the look behind button is held down, the camera is rotated by 180 degrees around the kart
            if (Input.GetKeyDown(key_lookBehind))
            {
                if (transform.name == "P1 Avatar")
                {
                    FindGO.P1Avatar.GetComponent<CamScript>().lookBackRot = 180f;
                    FindGO.P1Avatar.GetComponent<CamScript>().lerpSpeed = 100f;
                }
                if (transform.name == "P2 Avatar")
                {
                    FindGO.P2Avatar.GetComponent<CamScript>().lookBackRot = 180f;
                    FindGO.P2Avatar.GetComponent<CamScript>().lerpSpeed = 100f;
                }

            }

            if (Input.GetKeyUp(key_lookBehind))
            {
                if (transform.name == "P1 Avatar")
                {
                    FindGO.P1Avatar.GetComponent<CamScript>().lookBackRot = 0f;
                    //If we change the lerp value back instantly, then the camera will lerp slowly back with the old value instead of snapping
                    //If we employ a coroutine to wait for a split second, thenthe camera will snap back upon button release, and only then restore the slow lerp speed
                    StartCoroutine(WaitAndChangeLerp(FindGO.P1Avatar));
                }
                if (transform.name == "P2 Avatar")
                {
                    FindGO.P2Avatar.GetComponent<CamScript>().lookBackRot = 0f;
                    //If we change the lerp value back instantly, then the camera will lerp slowly back with the old value instead of snapping
                    //If we employ a coroutine to wait for a split second, thenthe camera will snap back upon button release, and only then restore the slow lerp speed
                    StartCoroutine(WaitAndChangeLerp(FindGO.P2Avatar));
                }
            }

        }
        else
        {
            //If the player has entered the finish line, release all buttons
            VehicleMove(0f, 0f);
        }

        if (Input.GetKeyDown(key_brake))
        {
            FindGO.PlaySound("sfx/07 brake", 0.2f);
        }
        //if (Input.GetKeyDown(key_))
        //{

        //}

    }

    private IEnumerator WaitAndChangeLerp(GameObject avatar)
    {
        yield return new WaitForSeconds(0.1f);
        avatar.GetComponent<CamScript>().lerpSpeed = 6f;

    }

    public void DropTrap(Vector3 pos, Quaternion rot)
    {
        FindGO.PlaySound("sfx/17 drop", 0.3f);
        GameObject TrapPrefab = Resources.Load("TrapPrefab", typeof(GameObject)) as GameObject;

        GameObject TrapGO = Instantiate(TrapPrefab, pos + Vector3.up * 1f + transform.forward * -8f, rot);

    }

    public void FireBullet(Vector3 pos, Quaternion rot)
    {
        FindGO.PlaySound("sfx/11 stake shoot", 0.2f);
        GameObject BulletPrefab = Resources.Load("BulletPrefab", typeof(GameObject)) as GameObject;

        Vector3 oldRot = rot.eulerAngles;
        Vector3 newRot = new Vector3(0f, oldRot.y, 0f);
        rot = Quaternion.Euler(newRot);

        //Where does the bullet originate from?
        Vector3 muzzle = pos + Vector3.up * 1f + transform.forward * 8f;
        GameObject BulletGO = Instantiate(BulletPrefab, muzzle, rot);

        //Now find the target
        Vector3 target = muzzle;

        //If this player is P1 then the target is going to be P2
        if (transform.tag == "P1")
            target = FindGO.P2Avatar.transform.position + Vector3.up * 2f;
        else if (transform.tag == "P2")
            target = FindGO.P1Avatar.transform.position + Vector3.up * 2f;

        Vector3 fireVector = target - muzzle;
        fireVector = fireVector.normalized;

        float bulletAngle = Vector3.Angle(transform.forward, fireVector);

        //If the angle between the firing player's forward vector and the directional vector between the two players is more than 10 degrees, then autoaim is disabled
        if (bulletAngle > 10f)
        {
            fireVector = transform.forward * 100f + Vector3.up * 2f;
            fireVector = fireVector.normalized;
        }

        BulletGO.GetComponent<Rigidbody>().velocity = fireVector * 200;

    }

    public void ActivateShield(Vector3 pos)
    {
        //If the player does not have a shield yet
        if (!invincible)
        {
            GameObject ShieldPrefab = Resources.Load("ShieldPrefab", typeof(GameObject)) as GameObject;

            GameObject ShieldGO = Instantiate(ShieldPrefab, pos + Vector3.up * 3f, Quaternion.identity);
            ShieldGO.transform.parent = this.transform;

            ShieldGO.GetComponent<ShieldScript>().lifespan = 5f;

            invincible = true;

            StartCoroutine(ShieldGO.GetComponent<ShieldScript>().ShieldFade(true, 0.3f));
        }
        else
        {
            //If the player already has a shield, then just fetch it
            GameObject ShieldGO = transform.GetChild(4).gameObject;

            ShieldGO.GetComponent<ShieldScript>().lifespan = 5f;

            StartCoroutine(ShieldGO.GetComponent<ShieldScript>().ShieldFade(false, 0.3f));
        }

    }

    public void VehicleMove(float Turn, float Accel)
    {
        //Rotate wheel meshes
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;
            m_WheelColliders[i].GetWorldPose(out position, out quat);

            //Identifying the gameobjects for the two pairs of wheels
            GameObject frontWheels = transform.GetChild(1).GetChild(0).GetChild(1).transform.gameObject;
            GameObject rearWheels = transform.GetChild(1).GetChild(0).GetChild(4).transform.gameObject;

            //Rotating the gameobjects by the amount of the rotation belonging to the wheel collider
            frontWheels.transform.rotation = quat;
            rearWheels.transform.rotation = quat;
        }

        
        if (Mathf.Abs(Turn) == 1f)
        {
            if (turnSoundAllowed)
            {
                turnSoundAllowed = false;
                FindGO.PlaySound("sfx/08 kart turn", 0.3f);
            }
            
        }
        else
            turnSoundAllowed = true;



        //Clamp input values
        Turn = Mathf.Clamp(Turn, -1, 1);
        Accel = Mathf.Clamp(Accel, -1, 1);

        //Set the steer on the front wheels.
        //Assuming that wheels 0 and 1 are the front wheels.
        m_WheelColliders[0].steerAngle = Turn * maxSteerAngle * SteerSensitivity;
        m_WheelColliders[1].steerAngle = Turn * maxSteerAngle * SteerSensitivity;

        //Apply Drive
        for (int i = 0; i < 4; i++)
        {
            m_WheelColliders[i].motorTorque = Accel * maxTorque * AccelSensitivity;
        }

        //Cap Speed
        if (rb.velocity.magnitude > topSpeed)
            rb.velocity = topSpeed * rb.velocity.normalized;
        
        if (rb.velocity.magnitude < 0.1f)
        {
            if (ASwheel.isPlaying)
                ASwheel.Stop();
        }
        else
        {
            if (!ASwheel.isPlaying)
                ASwheel.Play();
        }


        for (int i = 0; i < 4; i++)
        {
            m_WheelColliders[i].brakeTorque = maxBrakeTorque * Brake;
        }

    }

    public void VehicleRotate(float hor, float vert)
    {
        hor = Mathf.Clamp(hor, -1, 1);
        vert = Mathf.Clamp(vert, -1, 1);

        // Set the rotation
        //Vector3 rot = Vector3.right * vert + Vector3.forward * -hor;
        Vector3 rot = Vector3.right * vert + Vector3.up * hor;
        // Apply the rotation to the rigid body
        transform.GetComponent<Rigidbody>().AddRelativeTorque(10f * rot, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ground")
        {
            FindGO.PlaySound("sfx/09 impact", 0.3f);
        }
        else if (other.gameObject.tag != "ground" && other.gameObject.tag != "pickup" && other.gameObject.tag != "bullet" && other.gameObject.tag != "shield" && other.gameObject.tag != "trap" && other.gameObject.tag != "checkpoint")
        {
            FindGO.PlaySound("sfx/06 collision", 0.3f);

        }

    }

}
