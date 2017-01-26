using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DMGameManager : MonoBehaviour {

    static DMGameManager instance;
    public static DMGameManager Instance
    {
        get { return instance; }
    }

    [HideInInspector]
    public Network network;
    [HideInInspector]
    public DMNetworkManager networkManager;

    public GameObject passengersObject;

    public int killCnt;
    public Text textKillCnt;

    public int seed;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        network = GameObject.FindObjectOfType<Network>();
        networkManager = DMNetworkManager.Instance;		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnPassengers()
    {

    }

    public void AddKillCnt()
    {
        killCnt++;
        textKillCnt.text = killCnt.ToString();
    }

    public void ResetKillCnt()
    {
        killCnt = 0;
        textKillCnt.text = killCnt.ToString();
    }
}
