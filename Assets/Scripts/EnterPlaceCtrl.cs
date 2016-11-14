using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterPlaceCtrl : MonoBehaviour
{
    public delegate void EventHandler();
    public event EventHandler EventEnterPlayer;
    public event EventHandler EventStayPlayer;
    public event EventHandler EventExitPlayer;

    public float camSize = 10f;
    public float moveCamTime = 0.5f;
    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            CameraManager cameraManager = GameObject.FindObjectOfType<CameraManager>();

            coll.transform.parent = this.transform;
            cameraManager.transform.parent = this.transform;

            Vector3 pos = new Vector3(-30f, 30f, -30f);
            cameraManager.OnMoveCamera(pos, camSize, moveCamTime);

            if(EventEnterPlayer != null)
            {
                EventEnterPlayer();
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            if (EventStayPlayer != null)
            {
                EventStayPlayer();
            }
        }
    }


    void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            if (EventExitPlayer != null)
            {
                EventExitPlayer();
            }
        }
    }
}
