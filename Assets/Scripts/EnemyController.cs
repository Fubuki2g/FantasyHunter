using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Singleton<EnemyController>
{
    Animator animator;
    NavMeshAgent nav; // NavMeshAgetコンポーネント用

    // プレイヤーの追跡に関する変数
    public GameObject target; // 追跡したい対象
    public float distance; // プレイヤーとの距離
    public float targetDistance; // 追跡時の追跡対象との間隔
    public bool walkable; // 移動できるかどうかのフラグ
    public float staytime; // 停止していた時間
    public bool stay; // 停止しているときのフラグ
    public float baseSpeed; // 基本のスピード
    public float leaveSpeed; // プレイヤーから離れるときのスピード
    public Transform[] root; // プレイヤーから離れるときに行く場所
    private int rootNumber; // 場所の番号

    // 敵の攻撃に関する変数
    public bool action; // 攻撃ができる状態
    public bool inrange; // 発覚範囲内に入っているか
    public bool detection; // 発覚状態かどうか
    public bool nextAttack; // 次の攻撃に移るフラグ
    public bool closerangeAttack; // 近距離攻撃のフラグ
    public bool longrangeAttack; // 遠距離攻撃のフラグ
    float lapseTimeC; // 近距離攻撃のクールタイムを計測するための変数
    float lapseTimeL; // 遠距離攻撃のクールタイムを計測するための変数
    public float coolC; // 近距離攻撃のクールタイム
    public float coolL; // 遠距離攻撃のクールタイム
    public Collider[] attackCollider; // 攻撃判定
    public GameObject firePos; // 炎エフェクト発生場所
    public GameObject fireeEfect; // 炎エフェクト
    public GameObject flyfirePos; // 飛行時の炎エフェクト発生場所
    public GameObject flyefirEffect; // 飛行時の炎エフェクト
    private GameObject child; // 子オブジェクトとして生成するための変数

    // 敵のステータスとHP減少による行動変化に関する変数
    public float enemyMaxHP; // 敵の最大HP
    public float enemyNowHP; // 敵の現在HP
    public bool takedamage; // 連続でダメージを受けないようにするフラグ
    public bool halfHP; // 体力が半分になった時のフラグ
    public bool quarterHP; // 体力が4分の1になった時のフラグ
    public bool fly; // 飛ぶフラグ
    public bool playerLook; // 飛行時にプレイヤーのほうを向くフラグ
    public bool die; // 敵が倒れたかどうかのフラグ

    public int SE; // 各行動で再生するSE

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

        //コンポーネント取得
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        distance = Vector3.Distance(this.transform.position, target.transform.position);

        EnemyMove();
        EnemyAction();
        EnemyAttack();

    }

    // 敵の移動に関するメソッド
    public void EnemyMove()
    {
        if (walkable && distance > 7 && !stay && !fly)
        {
            Vector3 destination = target.transform.position - transform.forward * targetDistance;
            nav.SetDestination(destination);
            animator.SetBool("walk", true);
        }
        // 停止状態でプレイヤーが近くに居続けると離れる
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

    // 敵の攻撃以外の行動に関するメソッド
    public void EnemyAction()
    {
        // 未発覚
        if (!inrange)
        {
            animator.SetBool("walk", false);
            animator.SetBool("stay", true);
        }

        // 発覚
        if (distance <= 20 && !detection)
        {
            detection = true;
            inrange = true;
            SE = 0;
            animator.SetTrigger("scream");
        }

        // 飛行時にプレイヤーを見続ける処理
        if (playerLook)
        {
            transform.LookAt(target.transform);
        }

    }

    // 敵の攻撃に関するメソッド
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

        // 攻撃後のクールタイム処理
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

        // HPが一定値になった場合に行う行動処理
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

    // 一定期間プレイヤーが近くにいた時に離れる処理
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

    // 飛んでから着地までの一連の処理
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

    // ダメージを受ける処理
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

    // 以下アニメーションで使用するメソッド
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
