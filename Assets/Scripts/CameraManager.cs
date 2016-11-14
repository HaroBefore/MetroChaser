using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour {

    Camera mainCam;
    SubwayCtrl[] subways = new SubwayCtrl[SubwayManager.totalSubwayCnt];

	// Use this for initialization
	void Start () {
        mainCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMoveCamera(Vector3 pos, float size, float time)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(pos, size, time));
    }

    IEnumerator MoveCamera(Vector3 pos, float size, float time)
    {
        transform.DOKill();
        transform.DOLocalMove(pos, time);
        mainCam.DOOrthoSize(size, time);
        yield return new WaitForSeconds(time);
    }
}
