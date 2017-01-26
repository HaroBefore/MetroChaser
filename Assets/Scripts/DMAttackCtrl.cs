using UnityEngine;
using System.Collections;

public class DMAttackCtrl : MonoBehaviour {

    public float coolTime;

    public bool isAttackAble = true;
    public GameObject attackParticleObject;
    public ParticleSystem attackParticle;
    [HideInInspector]
    public DMEnemyCtrl lastHitEnemy = null;

    public bool isPlayer = false;

    public void Attack()
    {
        if(gameObject.activeSelf)
        {
            if (isAttackAble)
            {
                Debug.Log("Attack");
                StartCoroutine(CoAttack());
            }
        }
    }

    IEnumerator CoAttack()
    {
        if (isPlayer)
            DMNetworkManager.Instance.OnSendMsg(eNetworkMsg.NetworkAttackPlayer);

        Debug.Log("attack");
        isAttackAble = false;
        attackParticleObject.SetActive(true);
        attackParticleObject.GetComponent<Collider>().enabled = true;
        yield return null;
        attackParticleObject.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(coolTime);
        attackParticleObject.SetActive(false);
        isAttackAble = true;
    }

    void OnTriggerEnter(Collider coll)
    {
        if(isPlayer)
        {
            if (coll.CompareTag("Enemy"))
            {
                DMGameManager.Instance.AddKillCnt();
                DMEnemyCtrl enemy = coll.GetComponent<DMEnemyCtrl>();
                lastHitEnemy = enemy;
                if (lastHitEnemy != null)
                {

                    DMNetworkManager.Instance.OnSendMsg(eNetworkMsg.NetworkHitPlayer);
                }
            }

            if (coll.CompareTag("Passenger"))
            {
                Debug.Log(coll.name);
                coll.GetComponent<Rigidbody>().AddForce(Vector3.up * 300f);
                DMNetworkManager.Instance.OnSendMsg(eNetworkMsg.NetworkHitWrongPlayer);
            }
        }
        else
        {
            if (coll.CompareTag("Passenger"))
            {
                Debug.Log(coll.name);
                coll.GetComponent<Rigidbody>().AddForce(Vector3.up * 300f);
            }
        }
    }
}
