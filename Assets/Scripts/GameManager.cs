using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    // ゲームの状態を表すフラグ
    public bool mainGame;
    public bool clearGame;
    public bool overGame;

    // 時間計測に関する変数
    public float time;
    public float limitTime;
    public Slider timeSlider;
    public bool timeStamina;

    // タイムライン用のUI
    public GameObject startCanvas;
    public GameObject mainGameCanvas;
    public GameObject clearCanvas;
    public GameObject overCanvas;

    // 各タイムライン
    public PlayableDirector startPD;
    public PlayableDirector clearPD;
    public PlayableDirector overPD;

    // 各タイムライン時のフォーカス先
    public GameObject[] focusButton;

    // スコア計算に関する変数
    public TextMeshProUGUI timeScore;
    public string minute; 
    public string second;

    void Start()
    {
        // マウスカーソルを非表示にする処理
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        startCanvas.SetActive(true);
        startPD.Play();

        time = limitTime;
        timeSlider.maxValue = limitTime;
        timeSlider.value = limitTime;
        timeStamina = false;
    }

    void Update()
    {
        // mainGameがtrueの時のみタイマーが動く
        if (mainGame)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 0;
            }
            // 残り時間半分でスタミナの最大値が減る
            else if (time <= limitTime/2 && !timeStamina)
            {
                timeStamina = true;
                CharacterController.Instance.StaminaDown();
            }
            timeSlider.value = time;
        }
        
        // ゲームクリア時の処理
        if (clearGame && mainGame)
        {
            SoundManager.Instance.StopBGM();
            mainGame = false;
            ClearDemo();
        }

        // ゲームオーバー時の処理
        if (overGame && mainGame)
        {
            SoundManager.Instance.StopBGM();
            mainGame = false;
            OverDemo();
        }

    }

    // スタートタイムライン再生後の処理
    public void MainGame()
    {
        startCanvas.SetActive(false);
        mainGame = true;
        CharacterController.Instance.walkable = true;
        StartTimer();
        SoundManager.Instance.PlayBGM(0);
    }

    // タイマーを開始するメソッド
    public void StartTimer()
    {
        mainGame = true;
    }

    // クリア時にタイマーを停止するメソッド
    public void StopTimer()
    {
        mainGame = false;
        // タイマー停止時にクリア状態だとクリアタイムを保存する処理
        // すでに保存されている時間より少ない場合更新される
        // ３位まで保存
        if (clearGame)
        {
            int currentScore = (int)limitTime - (int)time;
            if (DataManager.LoadInt("FirstScore") > currentScore)
            {
                DataManager.SaveInt("ThirdScore", DataManager.LoadInt("SecondScore"));
                DataManager.SaveInt("SecondScore", DataManager.LoadInt("FirstScore"));
                DataManager.SaveInt("FirstScore", currentScore);
            }
            else if (DataManager.LoadInt("SecondScore") > currentScore)
            {
                DataManager.SaveInt("ThirdScore", DataManager.LoadInt("SecondScore"));
                DataManager.SaveInt("SecondScore", currentScore);
            }
            else if (DataManager.LoadInt("ThirdScore") > currentScore)
            {
                DataManager.SaveInt("ThirdScore", currentScore);
            }
        }
    }

    // クリア時のタイムラインを再生する処理
    public void ClearDemo()
    {
        clearPD.Play();
        mainGameCanvas.SetActive(false);
        clearCanvas.SetActive(true);
        CharacterController.Instance.walkable = false;
        StopTimer();
        minute = (((int)limitTime - (int)time) / 60).ToString();
        if (minute.Length == 1)
        {
            minute = "0" + minute;
        }
        second = (((int)limitTime - (int)time) % 60).ToString();
        if (second.Length == 1)
        {
            second = "0" + second;
        }
        timeScore.text = minute + "：" + second;
    }

    // ゲームオーバー時のタイムラインを再生する処理
    public void OverDemo()
    {
        overPD.Play();
        mainGameCanvas.SetActive(false);
        overCanvas.SetActive(true);
        CharacterController.Instance.walkable = false;
        StopTimer();
    }

    // タイムライン再生時のボタンフォーカス処理
    public void ButtonFocus()
    {
        if (clearGame)
        {
            EventSystem.current.SetSelectedGameObject(focusButton[0]);
        }
        else if (overGame)
        {
            EventSystem.current.SetSelectedGameObject(focusButton[1]);
        }
    }

    // 以下クリア時とゲームオーバー時のボタンのメソッド
    // シーンをリロードする処理
    public void Retry()
    {
        SoundManager.Instance.PlaySE_Sys(0);
        FadeManager.Instance.LoadSceneIndex(1, 1);
    }

    // タイトルシーンへ遷移する処理
    public void Title()
    {
        SoundManager.Instance.PlaySE_Sys(0);
        FadeManager.Instance.LoadSceneIndex(0, 1);
    }

}
