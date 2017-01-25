using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMPassengerCtrl : MonoBehaviour {
    DMChangeColor changeColor;

    private void Start()
    {
        changeColor = GetComponent<DMChangeColor>();
        changeColor.TurnColor(DataColor.PassengerColor, 0f);
    }
}
