using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class DMNetworkManager : MonoBehaviour {
    static DMNetworkManager instance;
    public static DMNetworkManager Instance
    {
        get { return instance; }
    }

    Network network;
    DMPlayerCtrl owner;
    List<DMEnemyCtrl> enemyList;
    public GameObject enemyPrefab;
    string macAddress = "";
    public string MacAddress
    {
        get { return macAddress; }
    }

    Dictionary<string, Queue<JsonData>> msgMap;
    bool isReceiveInitData = false;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        msgMap = new Dictionary<string, Queue<JsonData>>();
        network = GetComponent<Network>();
        owner = GameObject.Find("Player").GetComponent<DMPlayerCtrl>();
        macAddress = network.MacAddress;
        owner.MacAddress = MacAddress;
        enemyList = new List<DMEnemyCtrl>();

        network.EventUserMsg += OnUserMsg;
        network.EventUserLogin += OnUserLogin;
        network.EventUserLogout += OnUserLogout;

        network.ConnectServer();

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
#if UNITY_EDITOR
        Debug.Log("Login Complete");
#endif
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
#if UNITY_EDITOR
        Debug.Log("Logout Complete");
#endif
    }

    public void OnSendMsg(eNetworkMsg networkEvent)
    {
        JsonData data = new JsonData();
#if UNITY_EDITOR
        Debug.Log("NetworkManager OnSendMsg");
#endif
        switch (networkEvent)
        {
            case eNetworkMsg.NetworkLogin:
                {
#if UNITY_EDITOR
                    Debug.Log("Send Login");
#endif
                    data["msgType"] = (int)eNetworkMsg.NetworkLogin;
                    owner.Respawn();
                }
                break;
            case eNetworkMsg.NetworkLogout:
                {
#if UNITY_EDITOR
                    Debug.Log("Send Logout");
#endif
                    data["msgType"] = (int)eNetworkMsg.NetworkLogout;
                }
                break;
            case eNetworkMsg.NetworkPlaying:
                {
#if UNITY_EDITOR
                    Debug.Log("Send Playing");
#endif
                    data["msgType"] = (int)eNetworkMsg.NetworkPlaying;
                    data["x"] = (int)(owner.transform.position.x * 100) * 0.01;
                    data["z"] = (int)(owner.transform.position.z * 100) * 0.01;
                    data["yAngle"] = owner.ModelTransform.eulerAngles.y;
                }
                break;
            case eNetworkMsg.NetworkInitInfoReq:
                {
#if UNITY_EDITOR
                    Debug.Log("Send InitInfoReq");
#endif
                    data["msgType"] = (int)eNetworkMsg.NetworkInitInfoReq;
                }
                break;
            case eNetworkMsg.NetworkInitInfoRes:
                {
#if UNITY_EDITOR
                    Debug.Log("Send InitInfoRes");
#endif
                    data["msgType"] = (int)eNetworkMsg.NetworkInitInfoRes;
                    //보내야될 데이터
                    
                }
                break;
            case eNetworkMsg.NetworkAttackPlayer:
                {
#if UNITY_EDITOR
                    Debug.Log("Send AttackPlayer");
#endif
                    data["msgType"] = (int)eNetworkMsg.NetworkAttackPlayer;
                    string macAdd = owner.MacAddress;
                    Debug.Log(macAdd);
                    data["attackUser"] = owner.MacAddress;
                    string str = data["attackUser"].ToString();
                    Debug.Log(str);
                }
                break;
            case eNetworkMsg.NetworkHitPlayer:
                {
#if UNITY_EDITOR
                    Debug.Log("Send HitPlayer");
#endif
                    data["msgType"] = (int)eNetworkMsg.NetworkHitPlayer;
                    data["hitUser"] = owner.AttackCtrl.lastHitEnemy.MacAddress;

                    owner.AttackCtrl.lastHitEnemy.Respawn();
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
#if UNITY_EDITOR
                            Debug.Log("Receive Login");
#endif
                            DMEnemyCtrl enemy = Instantiate(enemyPrefab, new Vector3(-200f, 0f, 0f), Quaternion.identity).GetComponent<DMEnemyCtrl>();
                            enemyList.Add(enemy);
                            enemy.MacAddress = it.Current;

                            if(network.mlistUserMacAddress.Count == 1)
                            {
                                break;
                            }
                            OnSendMsg(eNetworkMsg.NetworkInitInfoReq);
                        }
                        break;
                    case eNetworkMsg.NetworkLogout:
                        {
#if UNITY_EDITOR
                            Debug.Log("Receive Logout");
#endif
                            for (int i = 0; i < enemyList.Count; i++)
                            {
                                if(it.Current == enemyList[i].MacAddress)
                                {
                                    DMEnemyCtrl enemy = enemyList[i];
                                    enemyList.RemoveAt(i);
                                    Destroy(enemy.gameObject);
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
                            DMEnemyCtrl enemy = null;
#if UNITY_EDITOR
                            Debug.Log("Receive Playing");
#endif
                            for (int i = 0; i < enemyList.Count; i++)
                            {
                                if(it.Current == enemyList[i].MacAddress)
                                {
                                    enemy = enemyList[i];
                                    //enemy.transform.position = new Vector3(float.Parse(msg["x"].ToString()), 0f, float.Parse(msg["z"].ToString()));
                                    float x = float.Parse(msg["x"].ToString());
                                    float z = float.Parse(msg["z"].ToString());
                                    float yAngle = float.Parse(msg["yAngle"].ToString());
                                    enemy.EnqueuePosAndRot(x, z, yAngle);

                                    break;
                                }
                            }
                        }
                        break;
                    case eNetworkMsg.NetworkInitInfoReq:
                        {
#if UNITY_EDITOR
                            Debug.Log("Receive InitInfoReq");
#endif
                            OnSendMsg(eNetworkMsg.NetworkInitInfoRes);

                        }
                        break;
                    case eNetworkMsg.NetworkInitInfoRes:
                        {
                            if (isReceiveInitData == true)
                                break;
                            isReceiveInitData = true;

#if UNITY_EDITOR
                            Debug.Log("Receive InitInfoRes");
#endif

                        }
                        break;
                    case eNetworkMsg.NetworkAttackPlayer:
                        {
                            for (int i = 0; i < enemyList.Count; i++)
                            {
                                string strMac = msg["attackUser"].ToString();
                                if (enemyList[i].MacAddress == strMac)
                                {
                                    enemyList[i].attackCtrl.Attack();
                                    break;
                                }
                            }
                        }
                        break;
                    case eNetworkMsg.NetworkHitPlayer:
                        {
                            foreach (var enemy in enemyList)
                            {
                                if(enemy.MacAddress == msg["hitUser"].ToString())
                                {
                                    enemy.Respawn();
                                    break;
                                }
                            }
                            if(owner.MacAddress == msg["hitUser"].ToString())
                            {
                                owner.Respawn();
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
