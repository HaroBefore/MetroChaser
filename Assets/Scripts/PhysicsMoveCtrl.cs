using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsMoveCtrl : MonoBehaviour
{
    public GameObject model;

    public float maxVelocity = 20f;
    public float moveSpeed = 5f;
    public float rotAngle = 360f;

    new Rigidbody rigidbody;
    Animator animator;

    public Vector3 moveVector;

    public bool isCanMove = true;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (isCanMove)
        {
            if (moveVector != Vector3.zero)
            {
                model.transform.rotation = Quaternion.RotateTowards(model.transform.rotation,
                    Quaternion.LookRotation(moveVector),
                    rotAngle * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        if (isCanMove)
        {
            if (moveVector != Vector3.zero)
            {
                Vector3 velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxVelocity);
                rigidbody.MovePosition(rigidbody.position + moveVector * moveSpeed * Time.fixedDeltaTime);
                rigidbody.velocity = velocity;
            }
        }
    }

    void GetInput()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.z = Input.GetAxis("Vertical");

        if (moveVector.sqrMagnitude <= 0f)
        {
            moveVector.x = ETCInput.GetAxis("Horizontal");
            moveVector.z = ETCInput.GetAxis("Vertical");
        }

        //animator.SetFloat("MoveAxis", moveVector.sqrMagnitude);
    }
}
