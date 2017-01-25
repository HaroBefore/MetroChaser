using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMCameraCtrl : MonoBehaviour {

    public Transform target;
    public PhysicsMoveCtrl moveCtrl;
    Vector3 offset;

    float followSpeed = 3f;
    public float minFollowSpeed = 3f;
    public float maxFollowSpeed = 6f;

    // Use this for initialization
    void Start () {
        followSpeed = minFollowSpeed;
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if(moveCtrl.moveVector != Vector3.zero)
        {
            followSpeed += Mathf.Clamp01(Time.smoothDeltaTime);
        }
        else
        {
            followSpeed -= Mathf.Clamp01(Time.smoothDeltaTime * 3f);
        }
        followSpeed = Mathf.Clamp(followSpeed, minFollowSpeed, maxFollowSpeed);

        Vector3 movePos = Vector3.MoveTowards(transform.position, target.position + offset, followSpeed * Time.deltaTime);
        transform.position = movePos;
    }

}
