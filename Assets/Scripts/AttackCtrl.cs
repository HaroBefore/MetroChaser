using UnityEngine;
using System.Collections;

public class AttackCtrl : MonoBehaviour {

    public float coolTime;

    public bool isAttackAble = true;
    public GameObject attackParticle;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Attack()
    {
        if(isAttackAble)
        {
            Debug.Log("Attack");
            StartCoroutine(CoAttack());
        }
    }

    IEnumerator CoAttack()
    {
        isAttackAble = false;
        attackParticle.SetActive(true);
        attackParticle.GetComponent<Collider>().enabled = true;
        yield return null;
        attackParticle.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(coolTime);
        attackParticle.SetActive(false);
        isAttackAble = true;
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Passenger"))
        {
            Debug.Log(coll.name);
            coll.GetComponent<Rigidbody>().AddForce(Vector3.up * 300f);
        }
    }
}
