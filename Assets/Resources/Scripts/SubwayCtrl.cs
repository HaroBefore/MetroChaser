using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;



public class SubwayCtrl : MonoBehaviour {

    public delegate void EventSubwayHandler(SubwayCtrl subwayCtrl);
    public event EventSubwayHandler EventSubwayArriveStation;
    public event EventSubwayHandler EventSubwayLeaveStation;

    public eSubwaySide subwaySide;
    public int subwayID;
    public int stationID;

    DoorCtrl[] arrTopDoor;
    DoorCtrl[] arrDownDoor;

    //-16 -2 subwayA 2 16 subwayB
    Transform[] arrPassengers;
    
    [HideInInspector]
    public EnterPlaceCtrl[] arrSubwayPlace;
    MeshRenderer[] arrInModelsRenderer;
    GameObject[] arrModelsObject;
    GameObject[] arrPassengersObject;

    public GameObject BG;

    public bool isMovingSubway = false;
    public bool isHavePlayer = false;

    //임시
    public GameObject passengerPrefab;

    // Use this for initialization
    void Start () {
        arrTopDoor = new DoorCtrl[2];
        arrDownDoor = new DoorCtrl[2];
        arrPassengers = new Transform[2];
        arrSubwayPlace = new EnterPlaceCtrl[2];
        arrModelsObject = new GameObject[2];
        arrInModelsRenderer = new MeshRenderer[2];
        arrPassengersObject = new GameObject[2];

        for (int i = 0; i < 2; i++)
        {
            arrTopDoor[i] = transform.GetChild(i).GetChild(0).GetComponent<DoorCtrl>();
            arrDownDoor[i] = transform.GetChild(i).GetChild(1).GetComponent<DoorCtrl>();
            arrPassengers[i] = transform.GetChild(i).FindChild("Passengers");
            arrSubwayPlace[i] = transform.GetChild(i).GetComponent<EnterPlaceCtrl>();
            arrModelsObject[i] = arrSubwayPlace[i].transform.FindChild("Models").gameObject;
            arrInModelsRenderer[i] = arrSubwayPlace[i].transform.FindChild("InModel").GetComponent<MeshRenderer>();
            arrPassengersObject[i] = arrSubwayPlace[i].transform.FindChild("Passengers").gameObject;
        }

        FillPassengersInSubway();

        if (subwaySide == eSubwaySide.DOWN)
            SetSubwayRendererActive(false);

        for (int i = 0; i < arrSubwayPlace.Length; i++)
        {
            arrSubwayPlace[i].subwayNum = (eSubwayNum)(i + 1);
            arrSubwayPlace[i].EventEnterPlayer += OnPlayerEnterSubway;
            arrSubwayPlace[i].EventStayPlayer += OnPlayerStaySubway;
            arrSubwayPlace[i].EventExitPlayer += OnPlayerExitSubway;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    void SetHavePlayer(bool isHavePlayer, eSubwayNum subwayNum)
    {
        this.isHavePlayer = isHavePlayer;
        FindObjectOfType<GameManager>().SetPlayerInfo(isHavePlayer, subwayID, stationID, subwaySide, subwayNum);
        BG.SetActive(isHavePlayer);
    }

    //이벤트 플레이어가 탑승했을 때
    void OnPlayerEnterSubway(EnterPlaceCtrl sender)
    {
        SetSubwayRendererActive(false);
        //if (!isHavePlayer)
        {
            SetSubwayNumRendererActive(true, sender.subwayNum);
        }
        SetHavePlayer(true, sender.subwayNum);
        //print("OnPlayerEnterSubway()");
        //print("false" + " " + stationID + " " + subwayID + " " + subwaySide);
    }

    //이벤트 플레이어가 열차안에 있을 때
    void OnPlayerStaySubway(EnterPlaceCtrl sender)
    {
        /*
        if (isHavePlayer)
        {
            if(sender.isHavePlayer)
                return;
            else
            {
                SetHavePlayer(this, sender.subwayNum);
            }
        }
        else
        {
            SetHavePlayer(true, sender.subwayNum);
        }
        */
    }

    //이벤트 플레이어가 내릴 때
    void OnPlayerExitSubway(EnterPlaceCtrl sender)
    {
        if (isHavePlayer)
        {
            if (subwaySide == eSubwaySide.TOP)
                SetSubwayRendererActive(true);
            else
                SetSubwayRendererActive(false);
        }

        SetHavePlayer(false, eSubwayNum.None);
    }

    void FillPassengersInSubway()
    {
        Transform pos;
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(passengerPrefab, Vector3.zero, Quaternion.identity, arrPassengers[0]) as GameObject;
            pos = go.transform;
            pos.localPosition = new Vector3(Random.Range(-16f, -2f), 0f, Random.Range(-5f, 5f));
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(passengerPrefab, Vector3.zero, Quaternion.identity, arrPassengers[1]) as GameObject;
            pos = go.transform;
            pos.localPosition = new Vector3(Random.Range(2f, 16f), 0f, Random.Range(-5f, 5f));
        }
    }

    void SetSubwayRendererActive(bool isActive)
    {
        for (int i = 0; i < 2; i++)
        {
            arrInModelsRenderer[i].enabled = isActive;
            List<MeshRenderer> rendererList = new List<MeshRenderer>();
            arrModelsObject[i].GetComponentsInChildren<MeshRenderer>(rendererList);
            for (int j = 0; j < rendererList.Count; j++)
            {
                rendererList[j].enabled = isActive;
            }

            rendererList.Clear();
            arrPassengersObject[i].GetComponentsInChildren<MeshRenderer>(rendererList);
            for (int j = 0; j < rendererList.Count; j++)
            {
                rendererList[j].enabled = isActive;
            }
        }
    }

    void SetSubwayNumRendererActive(bool isActive, eSubwayNum num)
    {
        int n = (int)num - 1;
        Debug.Log(n);
        arrInModelsRenderer[n].enabled = isActive;
        List<MeshRenderer> rendererList = new List<MeshRenderer>();
        arrPassengersObject[n].GetComponentsInChildren<MeshRenderer>(rendererList);
        for (int j = 0; j < rendererList.Count; j++)
        {
            rendererList[j].enabled = isActive;
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
        SetSubwaySide();
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
        Vector3 pos = Vector3.zero;

        switch (subwaySide)
        {
            case eSubwaySide.TOP:
                {
                    pos = transform.position - new Vector3(35f, 0f, 35f);
                }
                break;
            case eSubwaySide.DOWN:
                {
                    pos = transform.position + new Vector3(35f, 0f, 35f);
                }
                break;
        }

        transform.DOMove(pos, time * 0.5f).SetEase(Ease.InQuad);

        float BGAngle = subwaySide == eSubwaySide.TOP ? 180f : 0f;
        BG.transform.localRotation = Quaternion.Euler(90f, 0f, BGAngle);
    }

    void SetSubwaySide()
    {
        int halfStationIdx = SubwayManager.canGenerateMaxSubwayCnt / 2;
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
                }
                break;
        }

        if (!isHavePlayer)
        {
            if (subwaySide == eSubwaySide.TOP)
            {
                SetSubwayRendererActive(true);
            }
            else
            {
                SetSubwayRendererActive(false);
            }
        }
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
            SetHavePlayer(true, GameManager.Instance.GetPlayerInfo().subwayNum);
        }
    }

    public void OnOpenWaitCloseDoor(float doorTime, float waitTime, float closedWaitTime)
    {
        StartCoroutine(OpenWaitCloseDoor(doorTime, waitTime, closedWaitTime));
    }

    IEnumerator OpenWaitCloseDoor(float doorTime, float waitTime, float closedWaitTime)
    {
        DoorCtrl[] subways = subwaySide == eSubwaySide.TOP ? arrDownDoor : arrTopDoor;
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
