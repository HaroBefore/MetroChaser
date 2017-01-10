using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public enum eNetworkMsg
{
    NetworkLogin,
    NetworkPlaying,
}

public class NetworkManager : MonoBehaviour {
    Network network;
    PlayerCtrl owner;
    List<PlayerCtrl> playerList;
    public GameObject enemyPrefab;

	// Use this for initialization
	void Start () {
        network = GetComponent<Network>();
        owner = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        playerList = new List<PlayerCtrl>();

        network.EventUserMsg += OnUserMsg;
        network.EventUserLogin += OnUserLogin;
        network.EventUserLogout += OnUserLogout;
        StartCoroutine(CoLoginSend());
	}

    IEnumerator CoLoginSend()
    {
        yield return new WaitUntil(() => network.IsLogin == true);
        OnSendMsg(eNetworkMsg.NetworkLogin);
        //StartCoroutine(CoSend());
    }

    IEnumerator CoSend()
    {
        yield return new WaitForSeconds(0.2f);
        OnSendMsg(eNetworkMsg.NetworkPlaying);
        StartCoroutine(CoSend());
    }

    // Update is called once per frame
    void Update () {
        ProcessUserMsg();
	}

    public void OnUserLogin(string mac)
    {
        Debug.Log("Login Complete");
    }

    public void OnUserLogout(string mac)
    {
        Debug.Log("Logout Complete");
    }

    public void OnSendMsg(eNetworkMsg networkEvent)
    {
        JsonData data = new JsonData();
        Debug.Log("NetworkManager OnSendMsg");

        switch (networkEvent)
        {
            case eNetworkMsg.NetworkLogin:
                {
                    Debug.Log("Send Login");
                    data["msgType"] = (int)eNetworkMsg.NetworkLogin;
                }
                break;
            case eNetworkMsg.NetworkPlaying:
                {
                    Debug.Log("Send Playing");
                    data["msgType"] = (int)eNetworkMsg.NetworkPlaying;
                    data["x"] = (int)(owner.transform.position.x * 100) * 0.01;
                    data["y"] = (int)(owner.transform.position.y * 100) * 0.01;
                }
                break;
            default:
                break;
        }

        network.OnSendMsg(data);
    }

    public void ProcessUserMsg()
    {

    }

    public void OnUserMsg(string mac, JsonData msg)
    {  
        if(mac == network.MacAddress)
        {
            return;
        }
                

        eNetworkMsg msgType = (eNetworkMsg)int.Parse(msg["msgType"].ToString());

        switch (msgType)
        {
            case eNetworkMsg.NetworkLogin:
                {
                    Debug.Log("Receive Login");
                    GameObject go = GameObject.Find("Player");
                    //GameObject go = Instantiate(enemyPrefab); //Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
                    owner.transform.position = new Vector3(5f, 0f, 0f);
                    //Debug.Log(go.name);
                }
                break;
            case eNetworkMsg.NetworkPlaying:
                {
                    Debug.Log("Receive Playing");
                }
                break;
            default:
                break;
        }


    }
}
