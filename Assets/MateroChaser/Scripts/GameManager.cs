using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;

public struct UserInfo
{
    public bool isInSubway;
    public int subwayID;
    public int stationID;
    public eSubwaySide subwaySide;
    public eSubwayNum subwayNum;

    public void SetUserInfo(bool isInSubway, int subwayID, int stationID, eSubwaySide subwaySide, eSubwayNum subwayNum)
    {
        this.isInSubway = isInSubway;
        this.subwayID = subwayID;
        this.stationID = stationID;
        this.subwaySide = subwaySide;
        this.subwayNum = subwayNum;
    }
}

public class GameManager : MonoBehaviour {
    public delegate void EventHandler();
    public event EventHandler EventSetUserInfo;

    public event EventHandler EventStartGame;
    public event EventHandler EventGameOver;
    public event EventHandler EventRestart;

    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }
    Network network;

    UserInfo playerInfo;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    void Start()
    {
        EventStartGame += OnStartGame;
        EventGameOver += OnGameOver;
        EventRestart += OnRestart;
    }

    public void SetPlayerInfo(bool isInSubway, int subwayID, int stationID, eSubwaySide subwaySide, eSubwayNum subwayNum)
    {
        playerInfo.SetUserInfo(isInSubway, subwayID, stationID, subwaySide, subwayNum);
        EventSetUserInfo();
    }

    public UserInfo GetPlayerInfo()
    {
        return playerInfo;
    }

    public void StartGame()
    {
        EventStartGame();
    }

    void OnStartGame()
    {
        TimeManager.Instance.EventTimeOut += OnTimeOut;
        Debug.Log("GameStart");
    }

    public void GameOver()
    {
        EventGameOver();
    }

    void OnGameOver()
    {
        TimeManager.Instance.EventTimeOut -= OnTimeOut;
        Debug.Log("GameOver");
    }

    public void Restart()
    {
        EventRestart();
    }

    void OnRestart()
    {
        StartCoroutine(CoRestart());
    }

    IEnumerator CoRestart()
    {
        yield return new WaitForSeconds(0.1f);
        instance = null;
        SceneManager.LoadScene(0);
    }

    void OnTimeOut()
    {
        EventGameOver();
    }

}
