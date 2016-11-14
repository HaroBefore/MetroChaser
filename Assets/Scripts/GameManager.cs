using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UserInfo
{
    public bool isInSubway;
    public int subwayID;
    public int stationID;
    public SubwayCtrl.eSubwaySide subwaySide;

    public void SetUserInfo(bool isInSubway, int subwayID, int stationID, SubwayCtrl.eSubwaySide subwaySide)
    {
        this.isInSubway = isInSubway;
        this.subwayID = subwayID;
        this.stationID = stationID;
        this.subwaySide = subwaySide;
    }
}

public class GameManager : MonoBehaviour {
    public delegate void EventHandler();
    public event EventHandler EventSetUserInfo;

    UserInfo playerInfo;

    public void SetPlayerInfo(bool isInSubway, int subwayID, int stationID, SubwayCtrl.eSubwaySide subwaySide)
    {
        playerInfo.SetUserInfo(isInSubway, subwayID, stationID, subwaySide);
        EventSetUserInfo();
    }

    public UserInfo GetPlayerInfo()
    {
        return playerInfo;
    }

}
