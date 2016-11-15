using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;

public class UIManager : MonoBehaviour {
    static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }

    public Text textStation;
    public Text textSubway;
    public Text textSide;
    public Text textTimer;

    public ETCJoystick joystickMove;
    public ETCButton btnAttack;
    public ETCButton btnSpacil;

    public GameObject btnGameStart;
    public GameObject btnRestart;

    GameManager gameManager;
    TimeManager timeManager;

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

    // Use this for initialization
    void Start () {
        gameManager = GameManager.Instance;
        gameManager.EventStartGame += OnStartGame;
        gameManager.EventGameOver += OnGameOver;
        gameManager.EventSetUserInfo += OnUpdateUI;

        timeManager = FindObjectOfType<TimeManager>();
        timeManager.EventTickTimer += OnUpdateTimer;

        btnGameStart.SetActive(true);
        btnRestart.SetActive(false);

        OnUpdateUI();
        OnUpdateTimer();
	}

    void OnStartGame()
    {
        btnGameStart.SetActive(false);   
    }

    void OnGameOver()
    {
        btnRestart.SetActive(true);
    }

    void OnUpdateUI()
    {
        UserInfo playerInfo = gameManager.GetPlayerInfo();
        textStation.text = "Station : " + playerInfo.stationID;
        if (playerInfo.isInSubway)
        {
            textSubway.text = "Subway : " + playerInfo.subwayID;
            textSide.text = "Side : " + playerInfo.subwaySide;
        }
        else
        {
            textSubway.text = "Subway : X";
            textSide.text = "Side : X";
        }
    }

    public void OnUpdateTimer()
    {
        int sec = timeManager.Sec;
        string strSec = sec == 0 ? sec + "0" :
            timeManager.Sec / 10 == 0 ? "0" + sec :
            timeManager.Sec + "";
        textTimer.text = timeManager.Min + " : " + strSec;
    }
}
