using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeColor : MonoBehaviour {

    MeshRenderer bodyRenderer;
    MeshRenderer headRenderer;

	// Use this for initialization
	void Start () {
        bodyRenderer = transform.FindChild("Model").GetComponent<MeshRenderer>();
        headRenderer = transform.FindChild("Model").FindChild("Head").GetComponent<MeshRenderer>();
	}
    
    IEnumerator test()
    {
        yield return new WaitForSeconds(2f);
        TurnColor(DataColor.PassengerColor, 5f);
    }

    public void TurnColor(Color color, float time)
    {
        TurnHeadColor(color, time);
        TurnBodyColor(color, time);
    }

    public void TurnHeadColor(Color color, float time)
    {
        headRenderer.material.DOColor(color, time);
    }

    public void TurnBodyColor(Color color, float time)
    {
        bodyRenderer.material.DOColor(color, time);
    }
}
