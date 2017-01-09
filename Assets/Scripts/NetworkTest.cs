using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;


public class NetworkTest : MonoBehaviour {

    Network network;

	// Use this for initialization
	void Start () {
        network = GetComponent<Network>();
        network.ConnectServer();
        StartCoroutine(SendSomething());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SendSomething()
    {
        yield return new WaitForSeconds(3f);
        JsonData data;
        data = new JsonData();
        data["Test"] = "tset";
        data["aaa"] = 1;
        Debug.Log(data.ToJson());
        network.OnSendMsg(data.ToJson());
    }
}
