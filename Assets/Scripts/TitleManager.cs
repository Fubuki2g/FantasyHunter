using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public GameObject[] Button; // 最初にフォーカスする各ボタン
    public GameObject[] Canvas; // 表示を切り替えるキャンバス

    // スコアに関する変数
    public TextMeshProUGUI[] Score;
    public string[] minute;
    public string[] second;

    GameObject currentFocus;    //現在のフォーカス先のボタン
    GameObject previousFocus;   //前フレームデフォーカスしていたボタン

    void Start()
    {
        SoundManager.Instance.PlayBGM(0);
        // マウスカーソルを非表示にする処理
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (GameObject canvas in Canvas)
        {
            canvas.SetActive(false);
        }
        Canvas[0].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[0]);

        // 保存されているハイスコアを表示するための処理
        minute[0] = (DataManager.LoadInt("FirstScore") / 60).ToString();
        if (minute[0].Length == 1)
        {
            minute[0] = "0" + minute[0];
        }
        second[0] = (DataManager.LoadInt("FirstScore") % 60).ToString();
        if (second[0].Length == 1)
        {
            second[0] = "0" + second[0];
        }
        minute[1] = (DataManager.LoadInt("SecondScore") / 60).ToString();
        if (minute[1].Length == 1)
        {
            minute[1] = "0" + minute[1];
        }
        second[1] = (DataManager.LoadInt("SecondScore") % 60).ToString();
        if (second[1].Length == 1)
        {
            second[1] = "0" + second[1];
        }
        minute[2] = (DataManager.LoadInt("ThirdScore") / 60).ToString();
        if (minute[2].Length == 1)
        {
            minute[2] = "0" + minute[2];
        }
        second[2] = (DataManager.LoadInt("ThirdScore") % 60).ToString();
        if (second[2].Length == 1)
        {
            second[2] = "0" + second[2];
        }
    }

    private void Update()
    {
        Score[0].text = minute[0] + "：" + second[0];
        Score[1].text = minute[1] + "：" + second[1];
        Score[2].text = minute[2] + "：" + second[2];
        FocusCheck();
    }

    // ゲームシーンに遷移する処理
    public void Play()
    {
        SoundManager.Instance.PlaySE_Sys(2);
        FadeManager.Instance.LoadSceneIndex(1, 1);
    }

    // スコアキャンバスを表示する処理
    public void HiScore()
    {
        Canvas[0].SetActive(false);
        Canvas[1].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[1]);
        SoundManager.Instance.PlaySE_Sys(0);
    }

    // 操作説明キャンバスを表示する処理
    public void Manual()
    {
        Canvas[0].SetActive(false);
        Canvas[2].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[2]);
        SoundManager.Instance.PlaySE_Sys(0);
    }

    // 終了確認キャンバスを表示する処理
    public void Check()
    {
        Canvas[0].SetActive(false);
        Canvas[3].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[3]);
        SoundManager.Instance.PlaySE_Sys(0);

    }

    // ゲーム終了の処理
    public void Exit()
    {
        Application.Quit();
        SoundManager.Instance.PlaySE_Sys(0);
    }

    // 各キャンバスから戻る処理
    public void CloseCanvas()
    {
        foreach (GameObject canvas in Canvas)
        {
            canvas.SetActive(false);
        }
        Canvas[0].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[0]);
        SoundManager.Instance.PlaySE_Sys(1);
    }

    //フォーカスが外れていないかチェック
    void FocusCheck()
    {
        currentFocus = EventSystem.current.currentSelectedGameObject;
        if (currentFocus == previousFocus) return;

        //もしフォーカスが外れていたら
        //前フレームのフォーカスに戻す
        if (currentFocus == null)
        {
            EventSystem.current.SetSelectedGameObject(previousFocus);
            return;
        }
        previousFocus = EventSystem.current.currentSelectedGameObject;
    }

}
