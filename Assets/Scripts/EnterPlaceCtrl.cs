using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterPlaceCtrl : MonoBehaviour
{
    public delegate void EventHandler(EnterPlaceCtrl sender);
    public event EventHandler EventEnterPlayer;
    public event EventHandler EventStayPlayer;
    public event EventHandler EventExitPlayer;

    public ePlaceType placeType;
    public GameObject placeObject = null;
    GameObject subwayModels = null;
    public bool isHavePlayer = false;
    public eSubwayNum subwayNum;
    
    public float camSize = 10f;
    public float moveCamTime = 0.5f;

    private void Start()
    {
        switch (placeType)
        {
            case ePlaceType.None:
#if UNITY_EDITOR
                Debug.Log(gameObject.name + " : 장소 타입을 지정해 주세요");
#endif
                break;
            case ePlaceType.Station:
                placeObject = gameObject;
                break;
            case ePlaceType.Subway:
                placeObject = transform.FindChild("InModel").gameObject;
                subwayModels = transform.FindChild("Models").gameObject;
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            CameraManager cameraManager = GameObject.FindObjectOfType<CameraManager>();

            coll.transform.parent = this.transform;
            cameraManager.transform.parent = this.transform;

            Vector3 pos = new Vector3(-30f, 30f, -30f);
            cameraManager.OnMoveCamera(pos, camSize, moveCamTime);

            isHavePlayer = true;
            if (EventEnterPlayer != null)
            {
                EventEnterPlayer(this);
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        /*
        if (coll.CompareTag("Player"))
        {
            if(isHavePlayer == false)
            {
                isHavePlayer = true;
                if (EventStayPlayer != null)
                {
                    EventStayPlayer(this);
                }
            }
        }
        */
    }


    void OnTriggerExit(Collider coll)
    {
        isHavePlayer = false;
        if (coll.CompareTag("Player"))
        {
            if (EventExitPlayer != null)
            {
                EventExitPlayer(this);
            }
        }
    }
}
