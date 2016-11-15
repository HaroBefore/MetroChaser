using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;



public class SubwayCtrl : MonoBehaviour {

    public enum eSubwaySide
    {
        TOP,
        DOWN
    }

    public delegate void EventSubwayHandler(SubwayCtrl subwayCtrl);
    public event EventSubwayHandler EventSubwayArriveStation;
    public event EventSubwayHandler EventSubwayLeaveStation;

    public eSubwaySide subwaySide;
    public int subwayID;
    public int stationID;

    DoorCtrl[] arrTopDoor;
    DoorCtrl[] arrDownDoor;

    //-16 -2 subwayA 2 16 subwayB
    Transform passengers;

    public EnterPlaceCtrl[] subwayPlace;
    public GameObject BG;

    public bool isMovingSubway = false;
    public bool isHavePlayer = false;

    //임시
    public GameObject passengerPrefab;

    // Use this for initialization
    void Start () {
        passengers = transform.GetChild(2);
        arrTopDoor = new DoorCtrl[2];
        arrDownDoor = new DoorCtrl[2];
        arrTopDoor[0] = transform.GetChild(0).GetChild(1).GetComponent<DoorCtrl>();
        arrTopDoor[1] = transform.GetChild(1).GetChild(1).GetComponent<DoorCtrl>();
        arrDownDoor[0] = transform.GetChild(0).GetChild(0).GetComponent<DoorCtrl>();
        arrDownDoor[1] = transform.GetChild(1).GetChild(0).GetComponent<DoorCtrl>();
        FillPassengersInSubway();

        for (int i = 0; i < subwayPlace.Length; i++)
        {
            subwayPlace[i].EventEnterPlayer += OnPlayerEnterSubway;
            subwayPlace[i].EventStayPlayer += OnPlayerStaySubway;
            subwayPlace[i].EventExitPlayer += OnPlayerExitSubway;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    void SetHavePlayer(bool isHavePlayer)
    {
        this.isHavePlayer = isHavePlayer;
        FindObjectOfType<GameManager>().SetPlayerInfo(isHavePlayer, subwayID, stationID, subwaySide);
        BG.SetActive(isHavePlayer);
    }

    //이벤트 플레이어가 탑승했을 때
    void OnPlayerEnterSubway()
    {
        SetHavePlayer(true);
        //print("OnPlayerEnterSubway()");
        //print("false" + " " + stationID + " " + subwayID + " " + subwaySide);
    }

    //이벤트 플레이어가 열차안에 있을 때
    void OnPlayerStaySubway()
    {
        if (isHavePlayer)
        {
            return;
        }
        else
        {
            SetHavePlayer(true);
        }
    }

    //이벤트 플레이어가 내릴 때
    void OnPlayerExitSubway()
    {
        SetHavePlayer(false);
    }

    void FillPassengersInSubway()
    {
        Transform pos;
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(passengerPrefab, Vector3.zero, Quaternion.identity, passengers) as GameObject;
            pos = go.transform;
            pos.localPosition = new Vector3(Random.Range(-16f, -2f), 0f, Random.Range(-5f, 5f));
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(passengerPrefab, Vector3.zero, Quaternion.identity, passengers) as GameObject;
            pos = go.transform;
            pos.localPosition = new Vector3(Random.Range(2f, 16f), 0f, Random.Range(-5f, 5f));
        }
    }

    //열차 움직이기 시작
    public void OnStartMoveToNextStation(float t)
    {
        StartCoroutine(MoveToNextStation(t));
    }

    //열차 실재 움직임
    public IEnumerator MoveToNextStation(float t)
    {
        float halfTimeMoveStation = t * 0.5f;
        MoveToPointOfInterest(halfTimeMoveStation);

        yield return new WaitForSeconds(halfTimeMoveStation);
        MoveToStation(halfTimeMoveStation);
        yield return new WaitForSeconds(halfTimeMoveStation);
        isMovingSubway = false;

        OnEndMoveToNextStation();
    }
    
    //열차 움직이기 끝
    public void OnEndMoveToNextStation()
    {

    }

    //열차간 거리 70 / 2 = 35(중간지점)
    void MoveToPointOfInterest(float time)
    {
        isMovingSubway = true;
        int halfStationIdx = SubwayManager.canGenerateMaxSubwayCnt / 2;
        Vector3 pos = Vector3.zero;

        switch (subwaySide)
        {
            case eSubwaySide.TOP:
                {
                    stationID--;
                    if (stationID == -1)
                    {
                        stationID = 0;
                        subwaySide = eSubwaySide.DOWN;
                    }
                    pos = transform.position - new Vector3(35f, 0f, 35f);
                }
                break;
            case eSubwaySide.DOWN:
                {
                    stationID++;
                    if (stationID == halfStationIdx)
                    {
                        stationID = halfStationIdx - 1;
                        subwaySide = eSubwaySide.TOP;
                    }
                    pos = transform.position + new Vector3(35f, 0f, 35f);
                }
                break;
        }
        transform.DOMove(pos, time * 0.5f).SetEase(Ease.InQuad);

        float BGAngle = subwaySide == eSubwaySide.TOP ? 180f : 0f;
        BG.transform.localRotation = Quaternion.Euler(90f, 0f, BGAngle);
    }

    void MoveToStation(float time)
    {
        int halfStationIdx = SubwayManager.canGenerateMaxSubwayCnt / 2;
        Vector3 pos = Vector3.zero;

        switch (subwaySide)
        {
            case eSubwaySide.TOP:
                {
                    if (stationID == halfStationIdx - 1)
                        transform.position = SubwayManager.arrSubwayStopTop[stationID].position + new Vector3(35f, 0f, 35f);
                    pos = SubwayManager.arrSubwayStopTop[stationID].position;
                }
                break;
            case eSubwaySide.DOWN:
                {
                    if (stationID == 0)
                        transform.position = SubwayManager.arrSubwayStopDown[stationID].position - new Vector3(35f, 0f, 35f);
                    pos = SubwayManager.arrSubwayStopDown[stationID].position;
                }
                break;
        }
        transform.DOMove(pos, time * 0.5f);

        if(isHavePlayer)
        {
            SetHavePlayer(true);
        }
    }

    public void OnOpenWaitCloseDoor(float doorTime, float waitTime, float closedWaitTime)
    {
        StartCoroutine(OpenWaitCloseDoor(doorTime, waitTime, closedWaitTime));
    }

    IEnumerator OpenWaitCloseDoor(float doorTime, float waitTime, float closedWaitTime)
    {
        DoorCtrl[] subways = subwaySide == eSubwaySide.TOP ? arrTopDoor : arrDownDoor;
        yield return new WaitForSeconds(closedWaitTime);
        for (int i = 0; i < subways.Length; i++)
        {
            subways[i].OnOpenDoor(closedWaitTime, doorTime);
            EventSubwayArriveStation(this);
        }

        yield return new WaitForSeconds(waitTime);

        for (int i = 0; i < subways.Length; i++)
        {
            subways[i].OnCloseDoor(closedWaitTime, doorTime);
            EventSubwayLeaveStation(this);
        }
        yield return new WaitForSeconds(doorTime);
    }
}
