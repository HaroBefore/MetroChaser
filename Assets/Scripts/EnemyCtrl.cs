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

    public new Rigidbody rigidbody;
    Queue<Vector3> posQueue;
    public Queue<Vector3> PosQueue
    {
        get { return posQueue; }
    }
    Queue<float> yAngleQueue;
    Vector3 desirePos;
    public Vector3 DesirePos
    {
        get { return desirePos; }
    }
    Vector3 desireRot;

    [HideInInspector]
    public ChangeColor changeColor;
    public float turnColorDelay = 3f;

    State<EnemyCtrl> currentState;
    [HideInInspector]
    public bool isMoving;

    private void Awake()
    {
        posQueue = new Queue<Vector3>();
        yAngleQueue = new Queue<float>();
    }

    // Use this for initialization
    void Start () {
        modelTransform = transform.FindChild("Model");
        rigidbody = GetComponent<Rigidbody>();
        changeColor = GetComponent<ChangeColor>();
        desirePos = transform.position;
        desirePos.y = 0f;
        rigidbody.position = new Vector3(-200f, 0f, 0f);
        beforePos = transform.position;

        StartCoroutine(CoIsMovingCheck());
        ChangeState(StateEnemyIdle.Instance);
	}

    Vector3 beforePos;
    IEnumerator CoIsMovingCheck()
    {
        yield return new WaitForSeconds(0.1f);
        if (beforePos == transform.position)
            isMoving = false;
        else
            isMoving = true;
        beforePos = transform.position;
        StartCoroutine(CoIsMovingCheck());
    }

	// Update is called once per frame
	void Update () {
        if (currentState != null)
            currentState.Execute(this);
    }

    public void ChangeState(State<EnemyCtrl> state)
    {
        if (currentState != null)
            currentState.Exit(this);
        currentState = state;
        currentState.Enter(this);
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
