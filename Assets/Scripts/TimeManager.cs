using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator CoTimer()
    {
        yield return new WaitForSeconds(1f);
    }
}
