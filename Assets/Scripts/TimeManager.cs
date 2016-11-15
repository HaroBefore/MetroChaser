using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {
    public delegate void EventHandler();
    public event EventHandler EventTickTimer;
    public event EventHandler EventTimeOut;

    static TimeManager instance;
    public static TimeManager Instance
    {
        get { return instance; }
    }

    public int startMin = 5;
    [Range(0,59)]
    public int startSec = 0;

    public bool isTimerCounting = false;

    int min;
    int sec;

    public int Min
    {
        get { return min; }
    }

    public int Sec
    {
        get { return sec; }
    }

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
        EventTimeOut += OnTimeOut;
        GameManager.Instance.EventStartGame += BeginTimer;
        GameManager.Instance.EventRestart += OnRestart;
        min = startMin;
        sec = startSec;
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnRestart()
    {
        instance = null;
    }

    public void BeginTimer()
    {
        isTimerCounting = true;
        min = startMin;
        sec = startSec;
        StartCoroutine(CoTimer());
    }

    public void PauseTimer()
    {
        isTimerCounting = false;
        StopCoroutine(CoTimer());
    }

    public void ResumeTimer()
    {
        isTimerCounting = true;
        StartCoroutine(CoTimer());
    }

    public void EndTimer()
    {
        isTimerCounting = false;
        StopCoroutine(CoTimer());
    }

    IEnumerator CoTimer()
    {
        yield return new WaitForSeconds(1f);
        SetTimer();
        if (EventTickTimer != null)
            EventTickTimer();

        StartCoroutine(CoTimer());
    }

    void OnTimeOut()
    {
        Debug.Log("timerOut");
    }

    public void SetTimer()
    {
        if (sec == 0)
        {
            if (min == 0)
            {
                if (EventTimeOut != null)
                    EventTimeOut();
            }
            else
            {
                min--;
                sec = 59;
            }
        }
        else
        {
            sec--;
        }
    }
}
