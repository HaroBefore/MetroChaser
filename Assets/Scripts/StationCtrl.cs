using UnityEngine;
using System.Collections;

public class StationCtrl : MonoBehaviour {

    public GameObject leftCollider;
    public GameObject rightCollider;

    public void SetColliderActive(SubwayCtrl.eSubwaySide side, bool isActive)
    {
        switch (side)
        {
            case SubwayCtrl.eSubwaySide.TOP:
                leftCollider.SetActive(isActive);
                break;
            case SubwayCtrl.eSubwaySide.DOWN:
                rightCollider.SetActive(isActive);
                break;
            default:
                break;
        }
    }
}
