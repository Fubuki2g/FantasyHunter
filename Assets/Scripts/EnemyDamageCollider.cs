using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageCollider : Singleton<EnemyDamageCollider>
{
    public float damage; // 部位ごとのダメージ
    public GameObject attackEffect; // 攻撃エフェクト

    // 攻撃を受けたらHpを減らし、エフェクトを発生させる処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackObj") && !EnemyController.Instance.takedamage)
        {
            EnemyController.Instance.TakeDamage(damage);
            SoundManager.Instance.PlaySE_Game(5);
            GameObject effectInstance = Instantiate(attackEffect, transform.position, Quaternion.identity);
            Destroy(effectInstance, 3f);
            CharacterController.Instance.AttackCollider.enabled = false;
        }
    }

}
