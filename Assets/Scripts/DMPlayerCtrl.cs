using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMPlayerCtrl : MonoBehaviour {
    private string mMacAddress = null;
    public string MacAddress
    {
        get { return mMacAddress; }
        set { mMacAddress = value; }
    }

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
        modelTransform = transform.FindChild("Model");
        arrModelRenderer = GetComponentsInChildren<MeshRenderer>();
        attackCtrl = GetComponent<DMAttackCtrl>();

    }

    // Update is called once per frame
    void Update () {

    }

    public void Init()
    {
        state = eUnitState.UnitInitialization;
        foreach (var item in arrModelRenderer)
        {
            item.enabled = false;
        }
    }

    public void Respawn()
    {
        StartCoroutine(CoRespawn());
    }

    IEnumerator CoRespawn()
    {

        yield return new WaitForSeconds(respawnDelay);

    }
}
