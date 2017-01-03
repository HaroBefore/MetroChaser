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
	private WebSocket	mWebSocket;
	private string	mMacAddress;

	public LinkedList< string >		mlistUserMacAddress	= new LinkedList< string >();

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
		mMacAddress	= SystemInfo.deviceUniqueIdentifier;
		mWebSocket	= null;
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

	public void OnUserMsg(string mac, string msg)
	{
	}

	public void OnSendMsg(string msg)
	{
		if (mWebSocket != null) {
			if (mWebSocket.ReadyState == WebSocketState.Open) {
				JsonData	json	= new JsonData ();
				json ["flag"]	= (int)eResponseFlag.MSG;
				json ["msg"] 	= msg;
				mWebSocket.Send (json.ToString ());
			}
		}
	}

	public void ConnectServer()
	{
		mWebSocket	= new WebSocket ("ws://nanoapps.synology.me:7070/MetroChaser/Server");

		mWebSocket.OnOpen += OnOpen;
		mWebSocket.OnClose += OnClose;
		mWebSocket.OnMessage += OnMessage;
		mWebSocket.OnError += OnError;

		mWebSocket.Connect ();
	}

	private void OnOpen(object sender, EventArgs e)
	{
		JsonData	json	= new JsonData ();
		json ["flag"] = (int)eResponseFlag.LOGIN;

		JsonData	user	= new JsonData ();
		user ["mac"]	= mMacAddress;
		json ["user"]	= user;

		JsonData	array	= new JsonData ();

		string[] sArray = new string[mlistUserMacAddress.Count];
		mlistUserMacAddress.CopyTo(sArray, 0);

		for ( int iNum = 0; iNum < mlistUserMacAddress.Count; iNum++ )
		{
			array.Add (sArray [iNum]);
		}
		json ["list"]	= array;
		mWebSocket.Send (json.ToString());
	}

	private void OnMessage(object sender, MessageEventArgs e)
	{
		JsonData	json	= new JsonData (e.Data);
		int			flag	= int.Parse( json ["flag"].ToJson ());

		switch (flag) {
		case (int)eRequestFlag.USER_LOGIN:
			{
				string	mac = json ["mac"].ToString ();
				mlistUserMacAddress.AddLast (mac);
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
			}
			break;
		case (int)eRequestFlag.USER_MSG:
			{
				this.OnUserMsg(json["mac"].ToString(), json["msg"].ToString());
			}
			break;
		}
	}

	private void OnError(object sender, ErrorEventArgs e)
	{
	}

	private void OnClose(object sender, CloseEventArgs e)
	{
	}
}
