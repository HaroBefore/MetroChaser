using UnityEngine;
using System.Collections;

public class DMAttackCtrl : MonoBehaviour {

    public float coolTime;

    public bool isAttackAble = true;
    public GameObject attackParticle;
    [HideInInspector]
    public DMEnemyCtrl lastHitEnemy = null;
    DMPlayerCtrl playerCtrl;

	// Use this for initialization
	void Start ()
    {
        playerCtrl = GetComponent<DMPlayerCtrl>();
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
        if(coll.CompareTag("Enemy"))
        {
            DMEnemyCtrl enemy = coll.GetComponent<DMEnemyCtrl>();
            lastHitEnemy = enemy;
            if(lastHitEnemy != null)
            {
                
                DMNetworkManager.Instance.OnSendMsg(eNetworkMsg.NetworkHitPlayer);
            }
        }

        if(coll.CompareTag("Passenger"))
        {
            Debug.Log(coll.name);
            coll.GetComponent<Rigidbody>().AddForce(Vector3.up * 300f);
        }
    }
}
