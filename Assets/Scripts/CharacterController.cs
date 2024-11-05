using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class CharacterController : Singleton<CharacterController>
{
    Animator animator;

    private Rigidbody rb;
    private Vector3 move;
    private Vector3 moveForward;

    public float moveSpeed; // 移動速度
    public float turnTimeRate = 0.5f; // 回転速度
    public bool mov = true; // 移動中かどうか

    // プレイヤーのステータス
    public float maxHP; // プレイヤーの最大HP
    public float currentHP; // プレイヤーの現在HP
    public float maxStamina = 50f; // スタミナゲージの最大値
    public float currentStamina; // スタミナゲージの現在値
    public Slider HPSlider; // HPのスライダー

    public bool invincible; // 無敵時間かどうか
    public bool burning; // 燃えている常態かのフラグ
    public int burningCount = 0; // 燃えてる状態を解除するまでの必要ローリングカウント
    public GameObject player; // エフェクトを生成するためのプレイヤー
    private GameObject child; // 子オブジェクトとして生成するための変数
    public GameObject fireEffect; // 燃えている状態のエフェクト

    public Slider StaminaSlider; // スタミナのスライダー
    public Image fillImage; // スライダーの色を変更するためのImage
    public float decreaseRate = 10f; // スタミナ減少速度
    public float recoveryRate = 5f; // スタミナ回復速度
    public float moveAmount; // ゲージをずらす大きさ

    public bool attack; // 攻撃入力が可能かどうかのフラグ
    public Collider AttackCollider; // 攻撃の当たり判定

    public bool run = true; // プレイヤーが走れるかどうかのフラグ
    public bool rol; // ローリング中か
    public bool damage; // ダメージを受けている最中か
    public bool walkable; // 移動できる状態か
    public bool timeDie; // 時間切れでゲームオーバーになった時のフラグ

    private void Start()
    {
        TryGetComponent(out animator);
        rb = this.gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        if (mov && walkable)
        {
            Move();
        }

        if (!damage && !attack && walkable)
        {
            Rotation();
        }

        // シフトキーが押されていない間はスタミナが回復する
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            // ゲージを徐々に回復させる
            currentStamina += recoveryRate * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }

        // スタミナが10以上まで回復すると再び走れるようになる
        if (currentStamina >= 10f)
        {
            run = true;
        }

        // 各スライダーの値を更新
        if (GameManager.Instance.mainGame)
        {
            UpdateHP();
            UpdateStamina();
        }

        // 燃えている状態の時は徐々にHPが減少する
        if (burning)
        {
            currentHP -= recoveryRate * Time.deltaTime;
        }

        // 炎焼ダメージでのゲームオーバー処理
        if (currentHP <= 0 && burning)
        {
            SoundManager.Instance.PlaySE_Game(10);
            burning = false;
            GameManager.Instance.overGame = true;
            animator.SetTrigger("Die");
            walkable = false;
            invincible = true;
            Destroy(child);
        }

        // 時間切れでのゲームオーバー処理
        if (!timeDie && GameManager.Instance.time <= 0)
        {
            SoundManager.Instance.PlaySE_Game(10);
            timeDie = true;
            GameManager.Instance.overGame = true;
            animator.SetTrigger("Die");
            walkable = false;
            invincible = true;

        }

    }

    // 移動
    public void Move()
    {
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        moveForward = cameraForward * move.z + Camera.main.transform.right * move.x;
        moveForward = moveForward.normalized;

        if (move.magnitude > 0 && !damage && !attack)
        {
            // シフトが押されている間はダッシュする
            if (Input.GetKey(KeyCode.LeftShift) && run)
            {
                rb.velocity = moveForward * moveSpeed * move.magnitude * 1.5f + new Vector3(0, rb.velocity.y, 0);
                animator.SetFloat("speed", 2, 0.1f, Time.deltaTime);
                currentStamina -= decreaseRate * Time.deltaTime;

                // ゲージが0以下になったらrunフラグをfalseにする
                if (currentStamina <= 0.5f)
                {
                    currentStamina = 0f;
                    run = false;
                }
            }
            else
            {
                rb.velocity = moveForward * moveSpeed * move.magnitude + new Vector3(0, rb.velocity.y, 0);
                animator.SetFloat("speed", 1, 0.1f, Time.deltaTime);
            }
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("speed", 0, 0.1f, Time.deltaTime);
        }

    }

    // カメラ回転
    public void Rotation()
    {
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        moveForward = cameraForward * move.z + Camera.main.transform.right * move.x;
        moveForward = moveForward.normalized;

        if (move.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnTimeRate);
        }
        else
        {
            Quaternion targetRotation = transform.rotation;
            transform.rotation = targetRotation;
        }
    }

    // 時間経過による最大スタミナの減少
    public void StaminaDown()
    {
        maxStamina = maxStamina / 2;
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        StaminaSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(StaminaSlider.GetComponent<RectTransform>().sizeDelta.x / 2f, StaminaSlider.GetComponent<RectTransform>().sizeDelta.y);
        StaminaSlider.GetComponent<RectTransform>().anchoredPosition += new Vector2(moveAmount, 0f);
    }

    // ダメージを受けた時の処理
    IEnumerator TakeDamage(int damage)
    {
        invincible = true;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHP();

        if (currentHP <= 0)
        {
            SoundManager.Instance.PlaySE_Game(10);
            GameManager.Instance.overGame = true;
            animator.SetTrigger("Die");
            walkable = false;
            invincible = true;
            yield break;
        }

        yield return new WaitForSeconds(1.5f);

        invincible = false;
    }

    // HPの変化処理
    void UpdateHP()
    {
        if (HPSlider != null)
        {
            HPSlider.value = (float)currentHP / maxHP;
        }
    }

    // スタミナの変化処理
    void UpdateStamina()
    {
        StaminaSlider.value = currentStamina / maxStamina;
        fillImage.color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.yellow, StaminaSlider.value);
    }

    // 以下アニメーションで使用するメソッド
    // 無敵時間開始
    public void invinsibleTime()
    {
        invincible = true;
    }

    // ローリング終了時の処理
    public void RollEnd()
    {
        rol = false;
        invincible = false;
        attack = false;
    }

    // 攻撃の当たり判定表示
    public void AttackColliderOn()
    {
        AttackCollider.enabled = true;
    }

    // 攻撃終了
    public void AttackEnd()
    {
        attack = false;
        EnemyController.Instance.takedamage = false;
        AttackCollider.enabled = false;
    }

    // 無敵時間終了
    public void DamageEnd()
    {
        damage = false;
        attack = false;
        EnemyController.Instance.takedamage = false;
        AttackCollider.enabled = false;
        walkable = true;
    }

    // 以下InputAction用のメソッド
    // 移動入力
    public void OnMove(InputAction.CallbackContext context)
    {
        move = new Vector3(context.ReadValue<Vector2>().x, 0f, context.ReadValue<Vector2>().y);
    }

    public void MoveOn() { mov = true; }

    public void MoveOff() { mov = false; }

    // ローリング
    public void Rolling(InputAction.CallbackContext context)
    {
        if (currentStamina >= 10 && !rol && !attack && walkable)
        {
            SoundManager.Instance.PlaySE_Game(11);
            currentStamina -= 10;
            animator.SetTrigger("Roll");
            rol = true;
            burningCount -= 1;
            if (burningCount == 0)
            {
                burning = false;
                Destroy(child);
            }
        }
    }

    // 攻撃１
    public void Attack1(InputAction.CallbackContext context)
    {
        if (!attack && walkable && !rol && !damage)
        {
            SoundManager.Instance.PlaySE_Game(6);
            animator.SetTrigger("PlayerAttack1");
            attack = true;
        }
    }

    // 攻撃２
    public void Attack2(InputAction.CallbackContext context)
    {
        if (!attack && walkable && !rol && !damage)
        {
            SoundManager.Instance.PlaySE_Game(7);
            animator.SetTrigger("PlayerAttack2");
            attack = true;
        }
    }

    // ダメージを受けた時の処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack1") && !invincible)
        {
            SoundManager.Instance.PlaySE_Game(8);
            StartCoroutine(TakeDamage(30));
            animator.SetTrigger("LightDamage");
            damage = true;
        }
        else if (other.CompareTag("Attack2") && !invincible)
        {
            SoundManager.Instance.PlaySE_Game(9);
            StartCoroutine(TakeDamage(40));
            animator.SetTrigger("HeavyDamage");
            damage = true;
        }
        else if (other.CompareTag("Attack3") && !invincible)
        {
            SoundManager.Instance.PlaySE_Game(8);
            StartCoroutine(TakeDamage(20));
            animator.SetTrigger("LightDamage");
            damage = true;
            burning = true;
            burningCount = 3;
            Destroy(child);
            child = Instantiate(fireEffect, this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            child.transform.parent = player.transform;
        }
        else if (other.CompareTag("Attack4") && !invincible)
        {
            SoundManager.Instance.PlaySE_Game(8);
            StartCoroutine(TakeDamage(30));
            animator.SetTrigger("LightDamage");
            damage = true;
        }
    }
}
