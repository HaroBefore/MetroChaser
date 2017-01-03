using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TransparencyAnimCtrl : MonoBehaviour {

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            GetComponent<Animator>().SetTrigger("OnTransparency");
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            GetComponent<Animator>().SetTrigger("OffTransparency");
        }
    }
}
