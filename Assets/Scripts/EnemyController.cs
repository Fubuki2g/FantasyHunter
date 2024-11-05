using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Singleton<EnemyController>
{
    Animator animator;
    NavMeshAgent nav; // NavMeshAget�R���|�[�l���g�p

    // �v���C���[�̒ǐՂɊւ���ϐ�
    public GameObject target; // �ǐՂ������Ώ�
    public float distance; // �v���C���[�Ƃ̋���
    public float targetDistance; // �ǐՎ��̒ǐՑΏۂƂ̊Ԋu
    public bool walkable; // �ړ��ł��邩�ǂ����̃t���O
    public float staytime; // ��~���Ă�������
    public bool stay; // ��~���Ă���Ƃ��̃t���O
    public float baseSpeed; // ��{�̃X�s�[�h
    public float leaveSpeed; // �v���C���[���痣���Ƃ��̃X�s�[�h
    public Transform[] root; // �v���C���[���痣���Ƃ��ɍs���ꏊ
    private int rootNumber; // �ꏊ�̔ԍ�

    // �G�̍U���Ɋւ���ϐ�
    public bool action; // �U�����ł�����
    public bool inrange; // ���o�͈͓��ɓ����Ă��邩
    public bool detection; // ���o��Ԃ��ǂ���
    public bool nextAttack; // ���̍U���Ɉڂ�t���O
    public bool closerangeAttack; // �ߋ����U���̃t���O
    public bool longrangeAttack; // �������U���̃t���O
    float lapseTimeC; // �ߋ����U���̃N�[���^�C�����v�����邽�߂̕ϐ�
    float lapseTimeL; // �������U���̃N�[���^�C�����v�����邽�߂̕ϐ�
    public float coolC; // �ߋ����U���̃N�[���^�C��
    public float coolL; // �������U���̃N�[���^�C��
    public Collider[] attackCollider; // �U������
    public GameObject firePos; // ���G�t�F�N�g�����ꏊ
    public GameObject fireeEfect; // ���G�t�F�N�g
    public GameObject flyfirePos; // ��s���̉��G�t�F�N�g�����ꏊ
    public GameObject flyefirEffect; // ��s���̉��G�t�F�N�g
    private GameObject child; // �q�I�u�W�F�N�g�Ƃ��Đ������邽�߂̕ϐ�

    // �G�̃X�e�[�^�X��HP�����ɂ��s���ω��Ɋւ���ϐ�
    public float enemyMaxHP; // �G�̍ő�HP
    public float enemyNowHP; // �G�̌���HP
    public bool takedamage; // �A���Ń_���[�W���󂯂Ȃ��悤�ɂ���t���O
    public bool halfHP; // �̗͂������ɂȂ������̃t���O
    public bool quarterHP; // �̗͂�4����1�ɂȂ������̃t���O
    public bool fly; // ��ԃt���O
    public bool playerLook; // ��s���Ƀv���C���[�̂ق��������t���O
    public bool die; // �G���|�ꂽ���ǂ����̃t���O

    public int SE; // �e�s���ōĐ�����SE

    private void Start()
    {
        TryGetComponent(out animator);
        lapseTimeC = 0.0f;
        lapseTimeL = 0.0f;
        enemyNowHP = enemyMaxHP;

        foreach (Collider collider in attackCollider)
        {
            collider.enabled = false;
        }

        //�R���|�[�l���g�擾
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        distance = Vector3.Distance(this.transform.position, target.transform.position);

        EnemyMove();
        EnemyAction();
        EnemyAttack();

    }

    // �G�̈ړ��Ɋւ��郁�\�b�h
    public void EnemyMove()
    {
        if (walkable && distance > 7 && !stay && !fly)
        {
            Vector3 destination = target.transform.position - transform.forward * targetDistance;
            nav.SetDestination(destination);
            animator.SetBool("walk", true);
        }
        // ��~��ԂŃv���C���[���߂��ɋ�������Ɨ����
        else if (walkable && distance <= 6 && !stay && !fly)
        {
            animator.SetBool("walk", false);
            staytime += Time.deltaTime;
            if (staytime >= 3)
            {
                StartCoroutine(Leave());
            }
        }
    }

    // �G�̍U���ȊO�̍s���Ɋւ��郁�\�b�h
    public void EnemyAction()
    {
        // �����o
        if (!inrange)
        {
            animator.SetBool("walk", false);
            animator.SetBool("stay", true);
        }

        // ���o
        if (distance <= 20 && !detection)
        {
            detection = true;
            inrange = true;
            SE = 0;
            animator.SetTrigger("scream");
        }

        // ��s���Ƀv���C���[���������鏈��
        if (playerLook)
        {
            transform.LookAt(target.transform);
        }

    }

    // �G�̍U���Ɋւ��郁�\�b�h
    public void EnemyAttack()
    {
        if (action && !die && !stay && !fly)
        {
            int randomInt = Random.Range(1, 3);
            if (randomInt == 1)
            {
                if (distance <= 15 && distance >= 7 && longrangeAttack && nextAttack)
                {
                    walkable = false;
                    SE = 1;
                    animator.SetTrigger("clawAttack");
                    longrangeAttack = false;
                    nextAttack = false;
                    action = false;
                }
                else if (distance < 7 && closerangeAttack && nextAttack)
                {
                    SE = 2;
                    walkable = false;
                    animator.SetTrigger("mouseAttack");
                    closerangeAttack = false;
                    nextAttack = false;
                    action = false;
                }
            }
            if (randomInt == 2)
            {
                if (distance <= 15 && distance >= 7 && longrangeAttack && nextAttack)
                {
                    SE = 3;
                    walkable = false;
                    animator.SetTrigger("fireAttack");
                    longrangeAttack = false;
                    nextAttack = false;
                    action = false;
                }
                else if (distance < 7 && closerangeAttack && nextAttack)
                {
                    SE = 4;
                    walkable = false;
                    animator.SetTrigger("wingAttack");
                    closerangeAttack = false;
                    nextAttack = false;
                    action = false;
                }
            }
        }

        // �U����̃N�[���^�C������
        if (!closerangeAttack)
        {
            lapseTimeC += Time.deltaTime;
            if (lapseTimeC >= coolC)
            {
                closerangeAttack = true;
                lapseTimeC = 0.0f;
            }
        }
        if (!longrangeAttack)
        {
            lapseTimeL += Time.deltaTime;
            if (lapseTimeL >= coolL)
            {
                longrangeAttack = true;
                lapseTimeL = 0.0f;
            }
        }

        // HP�����l�ɂȂ����ꍇ�ɍs���s������
        if (enemyNowHP <= enemyMaxHP / 2 && !halfHP)
        {
            halfHP = true;
            fly = true;
            StartCoroutine(Fly());
        }
        if (enemyNowHP <= enemyMaxHP / 4 && !quarterHP)
        {
            quarterHP = true;
            fly = true;
            StartCoroutine(Fly());
        }
    }

    // �����ԃv���C���[���߂��ɂ������ɗ���鏈��
    IEnumerator Leave()
    {
        stay = true;
        animator.SetBool("run", true);
        nav.destination = root[rootNumber].position;
        nav.speed = leaveSpeed;
        rootNumber += 1;
        if (rootNumber == 4)
        {
            rootNumber = 0;
        }

        yield return new WaitForSeconds(3.0f);

        animator.SetBool("run", false);
        nav.speed = baseSpeed;
        staytime = 0;
        stay = false;
    }

    // ���ł��璅�n�܂ł̈�A�̏���
    IEnumerator Fly()
    {
        if (fly)
        {
            animator.SetTrigger("takeOff");
            animator.SetBool("walk", false);
            animator.SetBool("fly", true);

            yield return new WaitForSeconds(1.0f);

            playerLook = true;

            yield return new WaitForSeconds(4.0f);

            animator.SetTrigger("flyfire");

            yield return new WaitForSeconds(5.0f);

            animator.SetBool("fly", false);
            animator.SetTrigger("land");

            yield return new WaitForSeconds(3.0f);

            fly = false;
            playerLook = false;
        }
    }

    // �_���[�W���󂯂鏈��
    public void TakeDamage(float damage)
    {
        if (!takedamage)
        {
            takedamage = true;
            enemyNowHP -= damage;

            if (enemyNowHP <= 0 && !die)
            {
                SoundManager.Instance.PlaySE_Game(12);
                die = true;
                walkable = false;
                animator.SetTrigger("die");
                GameManager.Instance.clearGame = true;
            }
        }
    }

    // �ȉ��A�j���[�V�����Ŏg�p���郁�\�b�h
    public void AnimationEnd()
    {
        nextAttack = true;
        action = true;
        foreach (Collider collider in attackCollider)
        {
            collider.enabled = false;
        }
        walkable = true;
        Destroy(child,3);

    }

    public void ScreamEnd()
    {
        nextAttack = true;
        closerangeAttack = true;
        longrangeAttack = true;
        action = true;
        walkable = true;
        animator.SetBool("stay", false);
    }

    public void MouseColliderOn()
    {
        attackCollider[0].enabled = true;
    }

    public void ClawColliderOn()
    {
        attackCollider[1].enabled = true;
        attackCollider[2].enabled = true;
    }

    public void FireColliderOn()
    {
        attackCollider[3].enabled = true;
    }

    public void FlyFireColliderOn()
    {
        attackCollider[5].enabled = true;
    }

    public void WingColliderOn()
    {
        attackCollider[4].enabled = true;
    }

    public void PlaySE()
    {
        SoundManager.Instance.PlaySE_Game(SE);
    }

    public void FireEffect()
    {
        child = Instantiate(fireeEfect, firePos.transform.position, transform.rotation * Quaternion.Euler(10, 0, 0));
        child.transform.parent = firePos.transform;
    }

    public void FlyFireEffect()
    {
        child = Instantiate(flyefirEffect, flyfirePos.transform.position, transform.rotation * Quaternion.Euler(40, 0, 0));
        child.transform.parent = flyfirePos.transform;
    }

}
