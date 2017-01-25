using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorCtrl : MonoBehaviour {

    public GameObject leftDoor;
    public GameObject rightDoor;
    
    public void OnOpenDoor(float doorTime, float closedWaitTime)
    {
        StartCoroutine(OpenDoor(doorTime, closedWaitTime));
    }

    IEnumerator OpenDoor(float doorTime, float closedWaitTime)
    {
        //yield return new WaitForSeconds(closedWaitTime);
        leftDoor.transform.DOLocalMoveX(-2.9f, doorTime);
        rightDoor.transform.DOLocalMoveX(2.9f, doorTime);
        yield return new WaitForSeconds(doorTime);
    }

    public void OnCloseDoor(float doorTime, float closedWaitTime)
    {
        StartCoroutine(CloseDoor(doorTime, closedWaitTime));
    }

    IEnumerator CloseDoor(float doorTime, float closedWaitTime)
    {
        
        leftDoor.transform.DOLocalMoveX(-1.2f, doorTime);
        rightDoor.transform.DOLocalMoveX(1.2f, doorTime);
        yield return new WaitForSeconds(doorTime);
        yield return new WaitForSeconds(closedWaitTime);
    }
}
