using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public int[] ItemCount; // �A�C�e�����Ƃ̏�����
    public bool[] itemFlag;  // �g�p����A�C�e���̃t���O
    public bool ItemUse; // �A�C�e�����g�p���Ă�����
    Animator PlayerAnimator; // �v���C���[�̃A�j���[�^�[
    public TextMeshProUGUI[] ItemText; // �A�C�e���̎c�萔��\������e�L�X�g
    public GameObject[] Icon; // �I�����̌����ڗp�摜
    public GameObject[] Cover; // ���l��0�̎��̌����ڗp�摜
    public int itemNumber; // �A�C�e����؂�ւ���Ƃ��Ɏg���ϐ�

    public GameObject player; // �G�t�F�N�g�𐶐����邽�߂̃v���C���[
    private GameObject child; // �q�I�u�W�F�N�g�Ƃ��Đ������邽�߂̕ϐ�
    public GameObject[] effect; // �A�C�e���̃G�t�F�N�g

    private void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        ItemUse = false;
        foreach (GameObject icon in Icon)
        {
            icon.SetActive(false);
        }
        Icon[0].SetActive(true);

        for (int f = 0; f < itemFlag.Length; f++)
        {
            itemFlag[f] = false;
        }
        itemFlag[0] = true;
        itemNumber = 0;
    }

    private void Update()
    {
        // �A�C�e�����̕\������
        for (int t = 0; t < ItemText.Length; t++)
        {
            ItemText[t].text = ItemCount[t].ToString();
        }

        // �A�C�e�����Ȃ��Ȃ�����A�C�R�����Â����鏈��
        for (int c = 0; c < ItemCount.Length; c++)
        {
            if (ItemCount[c] <= 0)
            {
                Cover[c].SetActive(true);
            }
        }
    }

    // InputAction�ŃA�C�e�����g�p���鏈��
    public void Item(InputAction.CallbackContext context)
    {
        if (ItemCount[0] > 0 && !ItemUse && itemFlag[0] && Time.timeScale == 1 && GameManager.Instance.mainGame)
        {
            ItemUse = true;
            CharacterController.Instance.walkable = false;
            PlayerAnimator.SetTrigger("Item");

        }
        else if (ItemCount[1] > 0 && !ItemUse && itemFlag[1] && Time.timeScale == 1 && GameManager.Instance.mainGame)
        {
            ItemUse = true;
            CharacterController.Instance.walkable = false;
            PlayerAnimator.SetTrigger("Item");

        }
    }

    // InputAction�ŃA�C�e���̐؂�ւ�(���̃A�C�e��)����
    public void NextItem(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 1 && GameManager.Instance.mainGame)
        {
            itemFlag[itemNumber] = false;
            Icon[itemNumber].SetActive(false);
            itemNumber += 1;
            if (itemNumber > itemFlag.Length - 1)
            {
                itemNumber = 0;
            }
            itemFlag[itemNumber] = true;
            Icon[itemNumber].SetActive(true);
        }
    }

    // InputAction�ŃA�C�e���̓���ւ�(�O�̃A�C�e��)����
    public void BackItem(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 1 && GameManager.Instance.mainGame)
        {
            itemFlag[itemNumber] = false;
            Icon[itemNumber].SetActive(false);
            itemNumber -= 1;
            if (itemNumber < 0)
            {
                itemNumber = itemFlag.Length - 1;
            }
            itemFlag[itemNumber] = true;
            Icon[itemNumber].SetActive(true);
        }
    }

    // �e�A�C�e�����g�p�������̏���
    public void ItemUsing()
    {
        if (itemFlag[0])
        {
            SoundManager.Instance.PlaySE_Game(13);
            ItemCount[0] -= 1;
            CharacterController.Instance.currentHP += 30;
            if (CharacterController.Instance.maxHP < CharacterController.Instance.currentHP)
            {
                CharacterController.Instance.currentHP = CharacterController.Instance.maxHP;
            }
            child = Instantiate(effect[0], this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            child.transform.parent = player.transform;
        }
        else if (itemFlag[1])
        {
            ItemCount[1] -= 1;
            if (CharacterController.Instance.maxStamina < 50)
            {
                SoundManager.Instance.PlaySE_Game(14);
                CharacterController.Instance.maxStamina += 25;
                if (CharacterController.Instance.maxStamina < CharacterController.Instance.currentStamina)
                {
                    CharacterController.Instance.currentStamina = CharacterController.Instance.maxStamina;
                }
                CharacterController.Instance.StaminaSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(CharacterController.Instance.StaminaSlider.GetComponent<RectTransform>().sizeDelta.x * 2f, CharacterController.Instance.StaminaSlider.GetComponent<RectTransform>().sizeDelta.y);
                CharacterController.Instance.StaminaSlider.GetComponent<RectTransform>().anchoredPosition -= new Vector2(CharacterController.Instance.moveAmount, 0f);
            }
            child = Instantiate(effect[1], this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            child.transform.parent = player.transform;
        }
    }

    // �A�C�e���̎g�p��Ďg�p�\�ɂ��鏈��
    public void ItemUsed()
    {
        ItemUse = false;
        CharacterController.Instance.walkable = true;
        Destroy(child);
    }

}
