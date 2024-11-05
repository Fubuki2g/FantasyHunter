using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageCollider : Singleton<EnemyDamageCollider>
{
    public float damage; // ���ʂ��Ƃ̃_���[�W
    public GameObject attackEffect; // �U���G�t�F�N�g

    // �U�����󂯂���Hp�����炵�A�G�t�F�N�g�𔭐������鏈��
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
