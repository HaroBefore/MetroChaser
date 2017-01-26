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

    public GameObject passengerPrefab;

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
        SpawnPassengers();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnPassengers()
    {
        for (int i = 0; i < 100; i++)
        {
            Transform passenger = Instantiate(passengerPrefab, new Vector3(Random.Range(-55f, 55f), 0f, Random.Range(-55f, 55f)), Quaternion.Euler(new Vector3(0f, Random.Range(0f,360f),0f))).transform;
            passenger.parent = passengersObject.transform;
        }
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
