using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMinimapRotation : MonoBehaviour
{
    void Update()
    {
        //We don't want the kart's blip on the radar to tumble when the kart tumbles so we only allow it to rotate along the Y axis
        transform.rotation = Quaternion.Euler(90f, transform.parent.transform.rotation.eulerAngles.y, 0f);
    }
}
