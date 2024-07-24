using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance;

    public Transform trackTarget;

    void Start()
    {
        trackTarget = PlayerControl.instance.gameObject.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //타겟 따라가기
        Vector3 trackPos = new Vector3 (trackTarget.position.x, trackTarget.position.y, trackTarget.position.z - 10f);
        transform.position = trackPos;
    }
}
