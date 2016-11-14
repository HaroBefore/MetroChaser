using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text textStation;
    public Text textSubway;
    public Text textSide;

    GameManager gameManager;

	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.EventSetUserInfo += UpdateUI;
	}

    void UpdateUI()
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
}
