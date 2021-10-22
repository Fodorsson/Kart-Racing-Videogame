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

	void Start () {
		CutsceneMode = true;

		m_WheelColliders[0].attachedRigidbody.centerOfMass = new Vector3(0,0,0);
		rb = GetComponent<Rigidbody>();

	}

	public void Update ()
	{

		if (!CutsceneMode)
		{

			float h = Input.GetAxis(HorizontalAxisName);
			float v = Input.GetAxis(VerticalAxisName);
			Brake = Input.GetAxis(BrakeAxisName);

			VehicleMove(h, v, v);

		}
		else
		{
			//If the player has entered the finish line, release all buttons
			VehicleMove(0f, 0f, 0f);
		}

	}

	public void LateUpdate()
	{

		//Cap the kart's rotation so that it doesn't fal on its top

		float rotZ = transform.localEulerAngles.z;

		float rotX = transform.localEulerAngles.x;

		Rigidbody rb = GetComponent<Rigidbody>();

		if (rotZ > 89f || rotZ < -89f)
		{

			//rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		}

		if (rotX > 89f || rotX < -89f)
		{
			//rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
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
