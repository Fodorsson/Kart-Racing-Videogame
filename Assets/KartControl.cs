using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartControl : MonoBehaviour {

	[SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
	[SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
	private Rigidbody rb;

	[SerializeField] private float topSpeed = 80;
	[SerializeField] private float maxSteerAngle = 10;
	[SerializeField] private float maxTorque = 50;
	[SerializeField] private float maxReverseTorque = 50;
	[SerializeField] private float maxBrakeTorque = 60;

	public float currentSpeed;
	public float SteerSensitivity=0.75f;
	public float AccelSensitivity=1f;

	public float Brake;

	public string HorizontalAxisName;
	public string VerticalAxisName;
	public string BrakeAxisName;
	public string ActivateAxisName;

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

	private void Awake()
	{
		//Get the script as the component of the ListGO GameObject
		FindGO = ListGO.GetComponent<FindGO>();
	}

	void Start () {

		m_WheelColliders[0].ConfigureVehicleSubsteps(1,71,71);

		CutsceneMode = true;

		m_WheelColliders[0].attachedRigidbody.centerOfMass = new Vector3(0,0,0);
		rb = GetComponent<Rigidbody>();

		//We need to place the center of mass a bit lower, so that the kart would always land on its feet
		rb.centerOfMass = new Vector3(0f, -0.1f, 0f);

		CheckpointsCleared = 0;

		currentLap = 1;

		//The powerup icon is invisible by default
		playerCanvas.transform.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 0);

	}

	public void Update ()
	{

		if (!CutsceneMode && !stunned)
		{

			float h = Input.GetAxis(HorizontalAxisName);
			float v = Input.GetAxis(VerticalAxisName);
			Brake = Input.GetAxis(BrakeAxisName);

			VehicleMove(h, v, v);

			//If the player presses the fire button and has a powerup equipped
			if (Input.GetAxis(ActivateAxisName) > 0 && powerupPossessed != 0)
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

				playerCanvas.transform.GetChild(4).GetComponent<Image>().color = new Color(1,1,1,0);

				


			}

		}
		else
		{
			//If the player has entered the finish line, release all buttons
			VehicleMove(0f, 0f, 0f);
		}

	}

	public void DropTrap(Vector3 pos, Quaternion rot)
	{
		GameObject TrapPrefab = Resources.Load("TrapPrefab", typeof(GameObject)) as GameObject;

		GameObject TrapGO = Instantiate(TrapPrefab, pos + Vector3.up * 1f + transform.forward * -8f, rot);

	}

	public void FireBullet(Vector3 pos, Quaternion rot)
	{
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

			GameObject ShieldGO = Instantiate(ShieldPrefab, pos + Vector3.up * 0.5f, Quaternion.identity);
			ShieldGO.transform.parent = this.transform;

			ShieldGO.GetComponent<ShieldScript>().lifespan = 5f;

			invincible = true;
		}
		else
		{
			//If the player already has a shield, then just fetch it
			GameObject ShieldGO = transform.GetChild(4).gameObject;

			ShieldGO.GetComponent<ShieldScript>().lifespan = 5f;
		}
		

	}

	public void VehicleMove (float Turn, float Accel, float footbrake)
	{
		//Rotate wheel meshes
		for (int i = 0; i < 4; i++) {
			Quaternion quat;
			Vector3 position;
//			m_WheelColliders [i].GetWorldPose (out position, out quat);
//			m_WheelMeshes [i].transform.rotation = quat;
		}

		//Clamp input values
		Turn = Mathf.Clamp (Turn, -1, 1);
		Accel = Mathf.Clamp (Accel, 0, 1);
		footbrake = -1 * Mathf.Clamp (footbrake, -1, 0);

		//Set the steer on the front wheels.
		//Assuming that wheels 0 and 1 are the front wheels.
		m_WheelColliders [0].steerAngle = Turn * maxSteerAngle * SteerSensitivity;
		m_WheelColliders [1].steerAngle = Turn * maxSteerAngle * SteerSensitivity;

		//Apply Drive
		for (int i = 0; i < 4; i++) {
			m_WheelColliders [i].motorTorque = Accel * maxTorque * AccelSensitivity;
			if (footbrake > 0) {
				m_WheelColliders [i].brakeTorque = 0f;
				m_WheelColliders [i].motorTorque = -maxReverseTorque * footbrake;
			}
			else m_WheelColliders [i].brakeTorque = maxBrakeTorque *footbrake;
		}

		//Cap Speed
		if (rb.velocity.magnitude > topSpeed)
			rb.velocity = topSpeed * rb.velocity.normalized;

		for (int i = 0; i < 4; i++) {
			m_WheelColliders [i].brakeTorque = maxBrakeTorque * Brake;
		}

	}

}
