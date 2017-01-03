using UnityEngine;
using System.Collections;

public class StationCtrl : MonoBehaviour {

    public GameObject leftCollider;
    public GameObject rightCollider;

    public void SetColliderActive(eSubwaySide side, bool isActive)
    {
        switch (side)
        {
            case eSubwaySide.TOP:
                leftCollider.SetActive(isActive);
                break;
            case eSubwaySide.DOWN:
                rightCollider.SetActive(isActive);
                break;
            default:
                break;
        }
    }
}
