using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public int[] ItemCount; // アイテムごとの所持数
    public bool[] itemFlag;  // 使用するアイテムのフラグ
    public bool ItemUse; // アイテムを使用している状態
    Animator PlayerAnimator; // プレイヤーのアニメーター
    public TextMeshProUGUI[] ItemText; // アイテムの残り数を表示するテキスト
    public GameObject[] Icon; // 選択時の見た目用画像
    public GameObject[] Cover; // 数値が0の時の見た目用画像
    public int itemNumber; // アイテムを切り替えるときに使う変数

    public GameObject player; // エフェクトを生成するためのプレイヤー
    private GameObject child; // 子オブジェクトとして生成するための変数
    public GameObject[] effect; // アイテムのエフェクト

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
        // アイテム数の表示処理
        for (int t = 0; t < ItemText.Length; t++)
        {
            ItemText[t].text = ItemCount[t].ToString();
        }

        // アイテムがなくなったらアイコンを暗くする処理
        for (int c = 0; c < ItemCount.Length; c++)
        {
            if (ItemCount[c] <= 0)
            {
                Cover[c].SetActive(true);
            }
        }
    }

    // InputActionでアイテムを使用する処理
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

    // InputActionでアイテムの切り替え(次のアイテム)処理
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

    // InputActionでアイテムの入れ替え(前のアイテム)処理
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

    // 各アイテムを使用した時の処理
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

    // アイテムの使用後再使用可能にする処理
    public void ItemUsed()
    {
        ItemUse = false;
        CharacterController.Instance.walkable = true;
        Destroy(child);
    }

}
