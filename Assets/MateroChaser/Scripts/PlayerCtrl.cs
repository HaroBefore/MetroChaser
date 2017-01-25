using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour {
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

    // Use this for initialization
    void Start () {
        modelTransform = transform.FindChild("Model");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
