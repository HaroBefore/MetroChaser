using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubwayManager : MonoBehaviour {
    public delegate void EventHandler();

    public event EventHandler EventSubwayDoorOpen;
    public event EventHandler EventSubwayDoorClose;

    static SubwayManager instance;
    public static SubwayManager Instance
    {
        get { return instance; }
    }

    public StationCtrl[] arrStationCtrl;

    public static Transform[] arrSubwayStopTop;
    public static Transform[] arrSubwayStopDown;
    GameObject[] arrGoSubway;
    public SubwayCtrl[] arrSubwayCtrl;

    public GameObject subwayPrefab;
    GameObject goSubwayList;

    public const int canGenerateMaxSubwayCnt = 8;
    public const int totalSubwayCnt = 6;

    [Range(2f, 16f)]
    public float timeToMoveStaion = 8f;
    public float timeToWaitInStation = 5f;
    public float timeToDoor = 1f;
    public float timeToWaitDoor = 1f;

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

        arrSubwayStopTop = new Transform[canGenerateMaxSubwayCnt / 2];
        arrSubwayStopDown = new Transform[canGenerateMaxSubwayCnt / 2];
    }

    // Use this for initialization
    void Start () {
        GameManager.Instance.EventStartGame += OnStartGame;
        GameManager.Instance.EventRestart += OnRestart;

        for (int i = 0; i < canGenerateMaxSubwayCnt / 2; i++)
        {
            arrSubwayStopTop[i] = GameObject.Find(string.Format("SubwayStopTop_{0}", i)).transform;
            arrSubwayStopDown[i] = GameObject.Find(string.Format("SubwayStopDown_{0}", i)).transform;
        }

        goSubwayList = GameObject.Find("SubwayList");
        MakeSubways();
    }

    void OnStartGame()
    {
        StartCoroutine(MoveSubway());
    }

    void OnRestart()
    {
        instance = null;
    }

    IEnumerator MoveSubway()
    {
        SubwayCtrl subwayCtrl = null;
        foreach (var subway in arrSubwayCtrl)
        {
            subway.OnStartMoveToNextStation(timeToMoveStaion);
            if (subway.isHavePlayer)
                subwayCtrl = subway;
        }

        if (subwayCtrl)
        {
            float halfTimeMoveStation = timeToMoveStaion * 0.5f;

            DOTween.To(() => OffsetScrollCtrl.speed, x => OffsetScrollCtrl.speed = x, 4f - (timeToMoveStaion * 0.2f), halfTimeMoveStation);
            yield return new WaitForSeconds(halfTimeMoveStation);
            DOTween.To(() => OffsetScrollCtrl.speed, x => OffsetScrollCtrl.speed = x, 0f, halfTimeMoveStation);
            yield return new WaitForSeconds(halfTimeMoveStation);
        }
        else
        {
            yield return new WaitForSeconds(timeToMoveStaion);
        }

        foreach (var subway in arrSubwayCtrl)
        {
            subway.OnOpenWaitCloseDoor(timeToDoor, timeToWaitInStation, timeToWaitDoor);
        }
        yield return new WaitForSeconds(timeToDoor + timeToWaitInStation + (timeToWaitDoor * 2));
        StartCoroutine(MoveSubway());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void MakeSubways()
    {
        arrGoSubway = new GameObject[totalSubwayCnt];
        arrSubwayCtrl = new SubwayCtrl[totalSubwayCnt];

        //제외할 열차의 수
        int exceptCnt = canGenerateMaxSubwayCnt - totalSubwayCnt;
        int[] n = new int[exceptCnt];
        for (int i = 0; i < exceptCnt; i++)
        {
            n[i] = Random.Range(0, canGenerateMaxSubwayCnt);

            for (int j = 0; j < i; j++)
            {
                if (i == j)
                    continue;
                if(n[j] == n[i])
                {
                    i--;
                    break;
                }
            }
        }

        Transform[] arrSubwayStop;
        int idx = 0;
        bool isExcept = false;
        for (int i = 0; i < canGenerateMaxSubwayCnt; i++)
        {
            isExcept = false;
            arrSubwayStop = i < canGenerateMaxSubwayCnt / 2 ? arrSubwayStopTop : arrSubwayStopDown;
            for (int j = 0; j < exceptCnt; j++)
            {
                if(i == n[j])
                {
                    isExcept = true;
                    break;
                }
            }
            //제외된 곳이 아니면
            if (!isExcept)
            {
                arrGoSubway[idx] = Instantiate(subwayPrefab, arrSubwayStop[i % 4].position, Quaternion.Euler(new Vector3(0f, -45f, 0f))) as GameObject;
                arrSubwayCtrl[idx] = arrGoSubway[idx].GetComponent<SubwayCtrl>();

                arrSubwayCtrl[idx].subwaySide = i < canGenerateMaxSubwayCnt / 2 ? SubwayCtrl.eSubwaySide.TOP : SubwayCtrl.eSubwaySide.DOWN;
                arrSubwayCtrl[idx].stationID = i % (canGenerateMaxSubwayCnt / 2);
                arrSubwayCtrl[idx].subwayID = idx;

                idx++;
            }
        }

        for (int i = 0; i < arrGoSubway.Length; i++)
        {
            arrGoSubway[i].transform.parent = goSubwayList.transform;
            arrSubwayCtrl[i].EventSubwayArriveStation += OnSubwayArriveStation;
            arrSubwayCtrl[i].EventSubwayLeaveStation += OnSubwayLeaveStation;
        }
    }

    void OnSubwayArriveStation(SubwayCtrl subwayCtrl)
    {
        StationCtrl station = arrStationCtrl[subwayCtrl.stationID];
        station.SetColliderActive(subwayCtrl.subwaySide, false);
    }

    void OnSubwayLeaveStation(SubwayCtrl subwayCtrl)
    {
        StationCtrl station = arrStationCtrl[subwayCtrl.stationID];
        station.SetColliderActive(subwayCtrl.subwaySide, true);
    }
}
