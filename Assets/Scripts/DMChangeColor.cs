using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DMChangeColor : MonoBehaviour {

    MeshRenderer[] arrRenderer;
    Tweener[] arrTweener;

	// Use this for initialization
	void Awake () {
        arrRenderer = GetComponentsInChildren<MeshRenderer>();

        arrTweener = new Tweener[arrRenderer.Length];
	}
    
    public void TurnColor(Color color, float time)
    {
        for (int i = 0; i < arrTweener.Length; i++)
        {
            if (arrTweener[i] != null)
                arrTweener[i].Kill();
            arrTweener[i] = arrRenderer[i].material.DOColor(color, time);
        }
    }
}
