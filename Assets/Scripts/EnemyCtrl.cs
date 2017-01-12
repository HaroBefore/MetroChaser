using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour {
    private string mMacAddress = null;
    public string MacAddress
    {
        get { return mMacAddress; }
        set { mMacAddress = value; }
    }

    Transform modelTransform;
    public Transform ModelTransform
    {
        get { return modelTransform; }
    }

    new Rigidbody rigidbody;
    Queue<Vector3> posQueue;
    Queue<float> yAngleQueue;
    Vector3 desirePos;
    Vector3 desireRot;

    private void Awake()
    {
        posQueue = new Queue<Vector3>();
        yAngleQueue = new Queue<float>();
    }

    // Use this for initialization
    void Start () {
        modelTransform = transform.FindChild("Model");
        rigidbody = GetComponent<Rigidbody>();
        desirePos = transform.position;
        desirePos.y = 0f;
        rigidbody.position = new Vector3(-200f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        Vector3 movePos = rigidbody.position;
        Vector3 eulerAngle = transform.eulerAngles;
        if(posQueue.Count > 0)
        {
            desirePos = posQueue.Dequeue();
            desireRot = new Vector3(0f, yAngleQueue.Dequeue(), 0f);
        }

        //너무 멀면
        if ((rigidbody.position - desirePos).sqrMagnitude > 5f * 5f)
            rigidbody.position = desirePos;

        rigidbody.position = Vector3.MoveTowards(rigidbody.position, desirePos, 5f * Time.deltaTime);
        modelTransform.eulerAngles = Quaternion.Slerp(Quaternion.Euler(modelTransform.eulerAngles), Quaternion.Euler(desireRot), 0.1f).eulerAngles;

    }

    public void EnqueuePosAndRot(float x, float z, float yAngle)
    {
        posQueue.Enqueue(new Vector3(x, 0f, z));
        yAngleQueue.Enqueue(yAngle);
    }
}
