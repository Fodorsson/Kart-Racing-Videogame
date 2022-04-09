using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxScript : MonoBehaviour
{

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, -0.01f));
    }
}
