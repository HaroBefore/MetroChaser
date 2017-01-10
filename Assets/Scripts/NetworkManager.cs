using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class NetworkManager : MonoBehaviour {
    Network network;
    PlayerCtrl owner;
    List<PlayerCtrl> enemyList;
    public GameObject enemyPrefab;

    Dictionary<string, Queue<JsonData>> msgMap;

	// Use this for initialization
	void Start () {
        msgMap = new Dictionary<string, Queue<JsonData>>();
        network = GetComponent<Network>();
        owner = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        enemyList = new List<PlayerCtrl>();

        network.EventUserMsg += OnUserMsg;
        network.EventUserLogin += OnUserLogin;
        network.EventUserLogout += OnUserLogout;
        StartCoroutine(CoLoginSend());
	}

    IEnumerator CoLoginSend()
    {
        yield return new WaitUntil(() => network.IsLogin == true);
        OnSendMsg(eNetworkMsg.NetworkLogin);
        StartCoroutine(CoSend());
    }

    IEnumerator CoSend()
    {
        yield return new WaitForSeconds(0.2f);
        OnSendMsg(eNetworkMsg.NetworkPlaying);
        Debug.Log("playing");
        StartCoroutine(CoSend());
    }

    // Update is called once per frame
    void Update () {
        ProcessUserMsg();
	}

    public void OnUserLogin(string mac)
    {
        //자신이라면 끝내기
        if (mac == network.MacAddress)
            return;

        if(msgMap.ContainsKey(mac))
        {
            Debug.Log("이미 존재하는 mac입니다");
            return;
        }
        Queue<JsonData> queue = new Queue<JsonData>();
        msgMap.Add(mac, queue);
        Debug.Log("Login Complete");
    }

    public void OnUserLogout(string mac)
    {
        //자신이라면 끝내기
        if (mac == network.MacAddress)
            return;

        if (msgMap.ContainsKey(mac))
        {
            JsonData msg = new JsonData();
            msg["msgType"] = (int)eNetworkMsg.NetworkLogout;
            msgMap[mac].Clear();
            msgMap[mac].Enqueue(msg);
        }
        else
        {
            Debug.Log("존재하지 않는 mac이 로그아웃 했습니다");
            return;
        }
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
            case eNetworkMsg.NetworkLogout:
                {
                    Debug.Log("Send Logout");
                    data["msgType"] = (int)eNetworkMsg.NetworkLogout;
                }
                break;
            case eNetworkMsg.NetworkPlaying:
                {
                    Debug.Log("Send Playing");
                    data["msgType"] = (int)eNetworkMsg.NetworkPlaying;
                    data["x"] = (int)(owner.transform.position.x * 100) * 0.01;
                    data["z"] = (int)(owner.transform.position.z * 100) * 0.01;
                }
                break;
            default:
                break;
        }

        network.OnSendMsg(data);
    }
    
    public void ProcessUserMsg()
    {
        if (msgMap.Keys.Count <= 0)
            return;

        var it = msgMap.Keys.GetEnumerator();
        while(it.MoveNext())
        {
            Queue<JsonData> queue = msgMap[it.Current];
            for (int j = 0; j < queue.Count; j++)
            {
                JsonData msg = queue.Dequeue();

                eNetworkMsg msgType = (eNetworkMsg)int.Parse(msg["msgType"].ToString());

                switch (msgType)
                {
                    case eNetworkMsg.NetworkLogin:
                        {
                            Debug.Log("Receive Login");
                            PlayerCtrl enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerCtrl>();
                            enemyList.Add(enemy);
                            enemy.MacAddress = it.Current;
                        }
                        break;
                    case eNetworkMsg.NetworkLogout:
                        {
                            Debug.Log("Receive Logout");
                            for (int i = 0; i < enemyList.Count; i++)
                            {
                                if(it.Current == enemyList[i].MacAddress)
                                {
                                    PlayerCtrl player = enemyList[i];
                                    enemyList.RemoveAt(i);
                                    Destroy(player.gameObject);
                                    break;
                                }
                            }

                            msgMap[it.Current].Clear();
                            msgMap[it.Current] = null;
                            msgMap.Remove(it.Current);
                            it = msgMap.Keys.GetEnumerator();
                        }
                        break;
                    case eNetworkMsg.NetworkPlaying:
                        {
                            PlayerCtrl enemy = null;
                            Debug.Log("Receive Playing");
                            for (int i = 0; i < enemyList.Count; i++)
                            {
                                if(it.Current == enemyList[i].MacAddress)
                                {
                                    enemy = enemyList[i];
                                    enemy.transform.position = new Vector3(float.Parse(msg["x"].ToString()), 0f, float.Parse(msg["z"].ToString()));

                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void OnUserMsg(string mac, JsonData msg)
    {
        //자신이라면 끝내기
        if (mac == network.MacAddress)
            return;

        if (!msgMap.ContainsKey(mac))
        {
            Queue<JsonData> queue = new Queue<JsonData>();
            msgMap.Add(mac, queue);
            JsonData data = new JsonData();
            data["msgType"] = (int)eNetworkMsg.NetworkLogin;
            msgMap[mac].Enqueue(data);
            return;
        }

        msgMap[mac].Enqueue(msg);
    }
}
