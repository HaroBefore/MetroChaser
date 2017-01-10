using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net;
using LitJson;

public class Network : MonoBehaviour {
	private int mCount;
	private bool mConnect;
    public bool IsConnect
    {
        get { return mConnect; }
    }
    private bool mLogin;
    public bool IsLogin
    {
        get { return mLogin; }
    }
	private WebSocket	mWebSocket;
	private string	mMacAddress;
    public string MacAddress
    {
        get { return mMacAddress; }
    }

	public LinkedList< string >		mlistUserMacAddress	= new LinkedList< string >();

    public delegate void UserMsgDelegate(string mac, JsonData msg);
    public event UserMsgDelegate EventUserMsg;
    public event Action<string> EventUserLogin;
    public event Action<string> EventUserLogout;

	enum eRequestFlag
	{
		LOGIN_SUCCESS	= 201,
		LOGIN_FAILD		= 202,
		LOGOUT_SUCCESS	= 301,
		LOGOUT_FAILD	= 302,
		MSG_SUCCESS		= 401,
		MSG_FAILD		= 402,
		USER_LOGIN		= 501,
		USER_LOGOUT		= 502,
		USER_MSG		= 503,
	};

	enum eResponseFlag
	{
		LOGIN	= 101,
		LOGOUT	= 102,
		MSG		= 103,
	};

	// Use this for initialization
	void Start () {
		mCount	= 0;
		mConnect = false;
        mLogin = false;
        //mMacAddress	= SystemInfo.deviceUniqueIdentifier;
        mMacAddress = System.Guid.NewGuid().ToString();
		mWebSocket	= null;
		this.ConnectServer ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (mWebSocket != null) {
			if (!mConnect && mWebSocket.ReadyState == WebSocketState.Open) {
				mConnect	= true;
			} else if ( mConnect && mWebSocket.ReadyState != WebSocketState.Open ) {
				if (mCount < 5) {
					mCount++;
					mConnect	= false;
					mWebSocket.Connect ();
				}
			}
		}
	}

	public void OnUserMsg(string mac, JsonData msg)
	{
        if(EventUserMsg != null)
        {
            EventUserMsg(mac, msg);
        }
	}

	public void OnSendMsg(JsonData msg)
	{
		if (mWebSocket != null) {
			if (mWebSocket.ReadyState == WebSocketState.Open) {
				JsonData	json	= new JsonData ();
				json ["flag"]	= (int)eResponseFlag.MSG;
				json ["msg"] 	= msg;
				mWebSocket.Send (json.ToJson ());
                Debug.Log("json : " + json.ToJson());
            }
        }
	}

	public void ConnectServer()
	{
        Debug.Log("ConnectServer");
        mWebSocket	= new WebSocket ("ws://nanoapps.synology.me:7070/MetroChaser/Server");

        //mWebSocket = new WebSocket("ws://172.30.1.15:8080/MetroChaser/Server");

        mWebSocket.OnOpen += OnOpen;
		mWebSocket.OnClose += OnClose; ;
		mWebSocket.OnMessage += OnMessage;
		mWebSocket.OnError += OnError;

		mWebSocket.Connect ();
	}

	private void OnOpen(object sender, EventArgs e)
	{
        Debug.Log("OnOpen");
        StartCoroutine(CoLogin());
	}

    IEnumerator CoLogin()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("CoLogin");
        Login();
    }

	private void OnMessage(object sender, MessageEventArgs e)
	{

        JsonData json	= JsonMapper.ToObject(e.Data.ToString());
		int			flag	= int.Parse( json ["flag"].ToString ());

		switch (flag) {
		case (int)eRequestFlag.USER_LOGIN:
			{
				string	mac = json ["mac"].ToString ();
				mlistUserMacAddress.AddLast (mac);

                    mLogin = true;
                    if(EventUserLogin != null)
                        EventUserLogin(mac);
                }
                break;
		case (int)eRequestFlag.USER_LOGOUT:
			{
				string	mac = json ["mac"].ToString ();
				
				string[] sArray = new string[mlistUserMacAddress.Count];
				mlistUserMacAddress.CopyTo(sArray, 0);

				for ( int iNum = 0; iNum < mlistUserMacAddress.Count; iNum++ )
				{
					string	str	= sArray [iNum];

					if (str.Equals (mac)) {
						mlistUserMacAddress.Remove (str);
					}
				}

                    mLogin = false;
                    if (EventUserLogout != null)
                        EventUserLogout(mac);
			}
			break;
		case (int)eRequestFlag.USER_MSG:
			{
				this.OnUserMsg(json["mac"].ToString(), json["msg"]);
			}
			break;
		}
	}

	private void OnError(object sender, ErrorEventArgs e)
	{
        Debug.Log("OnError");
	}

	private void OnClose(object sender, CloseEventArgs e)
	{
        mLogin = false;
        mConnect = false;
        JsonData data = new JsonData();
        data["msgType"] = (int)eNetworkMsg.NetworkLogout;
        OnSendMsg(data);

        Debug.Log("OnClose");
	}

    public void Login()
    {
        Debug.Log("Login");
        JsonData json = new JsonData();
        json["flag"] = (int)eResponseFlag.LOGIN;

        JsonData user = new JsonData();
        user["mac"] = mMacAddress;
        json["user"] = user;

        JsonData array = new JsonData();

        if (mlistUserMacAddress.Count > 0)
        {

            string[] sArray = new string[mlistUserMacAddress.Count];
            mlistUserMacAddress.CopyTo(sArray, 0);

            for (int iNum = 0; iNum < mlistUserMacAddress.Count; iNum++)
            {
                array.Add(sArray[iNum]);
            }
        }
        //json ["list"]	= array;
        mWebSocket.Send(json.ToJson());
    }
}
