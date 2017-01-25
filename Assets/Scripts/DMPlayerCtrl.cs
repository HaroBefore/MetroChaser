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

    ePlayerState state = ePlayerState.None;

    // Use this for initialization
    void Start () {
        modelTransform = transform.FindChild("Model");
        attackCtrl = GetComponent<DMAttackCtrl>();

        state = ePlayerState.PlayerInitialization;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
