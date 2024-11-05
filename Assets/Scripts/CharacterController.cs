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

    public float moveSpeed; // �ړ����x
    public float turnTimeRate = 0.5f; // ��]���x
    public bool mov = true; // �ړ������ǂ���

    // �v���C���[�̃X�e�[�^�X
    public float maxHP; // �v���C���[�̍ő�HP
    public float currentHP; // �v���C���[�̌���HP
    public float maxStamina = 50f; // �X�^�~�i�Q�[�W�̍ő�l
    public float currentStamina; // �X�^�~�i�Q�[�W�̌��ݒl
    public Slider HPSlider; // HP�̃X���C�_�[

    public bool invincible; // ���G���Ԃ��ǂ���
    public bool burning; // �R���Ă����Ԃ��̃t���O
    public int burningCount = 0; // �R���Ă��Ԃ���������܂ł̕K�v���[�����O�J�E���g
    public GameObject player; // �G�t�F�N�g�𐶐����邽�߂̃v���C���[
    private GameObject child; // �q�I�u�W�F�N�g�Ƃ��Đ������邽�߂̕ϐ�
    public GameObject fireEffect; // �R���Ă����Ԃ̃G�t�F�N�g

    public Slider StaminaSlider; // �X�^�~�i�̃X���C�_�[
    public Image fillImage; // �X���C�_�[�̐F��ύX���邽�߂�Image
    public float decreaseRate = 10f; // �X�^�~�i�������x
    public float recoveryRate = 5f; // �X�^�~�i�񕜑��x
    public float moveAmount; // �Q�[�W�����炷�傫��

    public bool attack; // �U�����͂��\���ǂ����̃t���O
    public Collider AttackCollider; // �U���̓����蔻��

    public bool run = true; // �v���C���[������邩�ǂ����̃t���O
    public bool rol; // ���[�����O����
    public bool damage; // �_���[�W���󂯂Ă���Œ���
    public bool walkable; // �ړ��ł����Ԃ�
    public bool timeDie; // ���Ԑ؂�ŃQ�[���I�[�o�[�ɂȂ������̃t���O

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

        // �V�t�g�L�[��������Ă��Ȃ��Ԃ̓X�^�~�i���񕜂���
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            // �Q�[�W�����X�ɉ񕜂�����
            currentStamina += recoveryRate * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }

        // �X�^�~�i��10�ȏ�܂ŉ񕜂���ƍĂё����悤�ɂȂ�
        if (currentStamina >= 10f)
        {
            run = true;
        }

        // �e�X���C�_�[�̒l���X�V
        if (GameManager.Instance.mainGame)
        {
            UpdateHP();
            UpdateStamina();
        }

        // �R���Ă����Ԃ̎��͏��X��HP����������
        if (burning)
        {
            currentHP -= recoveryRate * Time.deltaTime;
        }

        // ���ă_���[�W�ł̃Q�[���I�[�o�[����
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

        // ���Ԑ؂�ł̃Q�[���I�[�o�[����
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

    // �ړ�
    public void Move()
    {
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        moveForward = cameraForward * move.z + Camera.main.transform.right * move.x;
        moveForward = moveForward.normalized;

        if (move.magnitude > 0 && !damage && !attack)
        {
            // �V�t�g��������Ă���Ԃ̓_�b�V������
            if (Input.GetKey(KeyCode.LeftShift) && run)
            {
                rb.velocity = moveForward * moveSpeed * move.magnitude * 1.5f + new Vector3(0, rb.velocity.y, 0);
                animator.SetFloat("speed", 2, 0.1f, Time.deltaTime);
                currentStamina -= decreaseRate * Time.deltaTime;

                // �Q�[�W��0�ȉ��ɂȂ�����run�t���O��false�ɂ���
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

    // �J������]
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

    // ���Ԍo�߂ɂ��ő�X�^�~�i�̌���
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

    // �_���[�W���󂯂����̏���
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

    // HP�̕ω�����
    void UpdateHP()
    {
        if (HPSlider != null)
        {
            HPSlider.value = (float)currentHP / maxHP;
        }
    }

    // �X�^�~�i�̕ω�����
    void UpdateStamina()
    {
        StaminaSlider.value = currentStamina / maxStamina;
        fillImage.color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.yellow, StaminaSlider.value);
    }

    // �ȉ��A�j���[�V�����Ŏg�p���郁�\�b�h
    // ���G���ԊJ�n
    public void invinsibleTime()
    {
        invincible = true;
    }

    // ���[�����O�I�����̏���
    public void RollEnd()
    {
        rol = false;
        invincible = false;
        attack = false;
    }

    // �U���̓����蔻��\��
    public void AttackColliderOn()
    {
        AttackCollider.enabled = true;
    }

    // �U���I��
    public void AttackEnd()
    {
        attack = false;
        EnemyController.Instance.takedamage = false;
        AttackCollider.enabled = false;
    }

    // ���G���ԏI��
    public void DamageEnd()
    {
        damage = false;
        attack = false;
        EnemyController.Instance.takedamage = false;
        AttackCollider.enabled = false;
        walkable = true;
    }

    // �ȉ�InputAction�p�̃��\�b�h
    // �ړ�����
    public void OnMove(InputAction.CallbackContext context)
    {
        move = new Vector3(context.ReadValue<Vector2>().x, 0f, context.ReadValue<Vector2>().y);
    }

    public void MoveOn() { mov = true; }

    public void MoveOff() { mov = false; }

    // ���[�����O
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

    // �U���P
    public void Attack1(InputAction.CallbackContext context)
    {
        if (!attack && walkable && !rol && !damage)
        {
            SoundManager.Instance.PlaySE_Game(6);
            animator.SetTrigger("PlayerAttack1");
            attack = true;
        }
    }

    // �U���Q
    public void Attack2(InputAction.CallbackContext context)
    {
        if (!attack && walkable && !rol && !damage)
        {
            SoundManager.Instance.PlaySE_Game(7);
            animator.SetTrigger("PlayerAttack2");
            attack = true;
        }
    }

    // �_���[�W���󂯂����̏���
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
