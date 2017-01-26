using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMPlayerCtrl : MonoBehaviour {
    private string mMacAddress = "";
    public string MacAddress
    {
        get { return mMacAddress; }
        set { mMacAddress = value; }
    }

    new Collider collider;
    new Rigidbody rigidbody;

    MeshRenderer[] arrModelRenderer;

    Transform modelTransform;
    public Transform ModelTransform
    {
        get { return modelTransform; }
    }

    DMAttackCtrl attackCtrl;
    public DMAttackCtrl AttackCtrl
    {
        get { return attackCtrl; }
    }

    public GameObject hitParticle;
    public GameObject respawnParticle;

    public eUnitState state = eUnitState.None;

    public float respawnDelay = 2f;

    // Use this for initialization
    void Start () {
        MacAddress = DMNetworkManager.Instance.MacAddress;
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        modelTransform = transform.FindChild("Model");
        arrModelRenderer = GetComponentsInChildren<MeshRenderer>();
        attackCtrl = GetComponent<DMAttackCtrl>();
        Init();
    }

    // Update is called once per frame
    void Update () {

    }

    public void Init()
    {
        state = eUnitState.UnitInitialization;
        SetModelActive(false);
        //transform.position = new Vector3(Random.Range(-70f, 70f), 0f, Random.Range(-70f, 70f));
        transform.position = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
        //transform.position = Vector3.zero;
        DMGameManager.Instance.ResetKillCnt();
    }

    public void Respawn()
    {
        Debug.Log("Respawning!!");
        if(state != eUnitState.UnitRespawning)
            StartCoroutine(CoRespawn());
    }

    IEnumerator CoRespawn()
    {
        Init();
        state = eUnitState.UnitRespawning;
        rigidbody.isKinematic = true;
        collider.enabled = false;
        yield return new WaitForSeconds(respawnDelay);
        SetModelActive(true);
        rigidbody.isKinematic = false;
        collider.enabled = true;
        state = eUnitState.UnitPlaying;
    }

    public void SetModelActive(bool isActive)
    {
        foreach (var item in arrModelRenderer)
        {
            item.enabled = isActive;
        }
    }
}
