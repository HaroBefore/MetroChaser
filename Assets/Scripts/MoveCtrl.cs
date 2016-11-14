using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

public class MoveCtrl : MonoBehaviour {
    public float moveSpeed = 6f;
    public float rotSpeed = 120f;

    public GameObject model;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (v == 0f && h == 0f)
        {
            v = ETCInput.GetAxis("Vertical");
            h = ETCInput.GetAxis("Horizontal");
        }

        Vector3 dir = new Vector3(h, 0f, v);
        if(dir != Vector3.zero)
        {
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.LookRotation(dir), 0.1f);
            transform.Translate(dir * Time.deltaTime * moveSpeed, Space.World);
        }
    }
}
