using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeColor : MonoBehaviour {

    MeshRenderer bodyRenderer;
    MeshRenderer headRenderer;
    Tweener headTweener;
    Tweener bodyTweener;

	// Use this for initialization
	void Start () {
        bodyRenderer = transform.FindChild("Model").GetComponent<MeshRenderer>();
        headRenderer = transform.FindChild("Model").FindChild("Head").GetComponent<MeshRenderer>();
	}
    
    public void TurnColor(Color color, float time)
    {
           
        TurnHeadColor(color, time);
        TurnBodyColor(color, time);
    }

    public void TurnHeadColor(Color color, float time)
    {
        if (headTweener != null)
            headTweener.Kill();
        headTweener = headRenderer.material.DOColor(color, time);
    }

    public void TurnBodyColor(Color color, float time)
    {
        if (bodyTweener != null)
            bodyTweener.Kill();
        bodyTweener = bodyRenderer.material.DOColor(color, time);
    }
}
