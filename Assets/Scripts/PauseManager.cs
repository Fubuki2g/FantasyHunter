using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool pauseFLG; // ポーズ中のフラグ

    public GameObject Pausecanvas; // ポーズ画面のキャンバス
    public GameObject focusPauseMenu; // ポーズ画面表示時のフォーカス先

    GameObject currentFocus;    //現在のフォーカス先のボタン
    GameObject previousFocus;   //前フレームデフォーカスしていたボタン

    void Update()
    {
        //フォーカスが外れていないかチェック
        FocusCheck();
    }

    // InputAction用のポーズ処理
    public void Pause(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.mainGame)
        {
            SoundManager.Instance.PlaySE_Sys(2);
            if (!pauseFLG)
            {
                ChangePause(true);
            }
            else
            {
                ChangePause(false);
            }
        }
    }

    //ポーズ処理
    public void ChangePause(bool flg)
    {
        pauseFLG = flg;
        EventSystem.current.SetSelectedGameObject(focusPauseMenu);

        //ポーズ中だったら時間を停止
        if (flg)
        {
            Time.timeScale = 0;
            Pausecanvas.SetActive(flg);
        }
        else
        {
            Time.timeScale = 1;
            Pausecanvas.SetActive(flg);
        }
    }

    // ポーズ画面からリスタートする処理
    public void ResetGame()
    {
        Time.timeScale = 1;
        GameManager.Instance.StopTimer();
        FadeManager.Instance.LoadSceneIndex(1, 1);
    }

    // ポーズ画面からタイトルに戻る処理
    public void PauseTitle()
    {
        Time.timeScale = 1;
        GameManager.Instance.StopTimer();
        FadeManager.Instance.LoadSceneIndex(0, 1);
    }

    //フォーカスが外れていないかチェックする処理
    void FocusCheck()
    {
        currentFocus = EventSystem.current.currentSelectedGameObject;
        //もし前回までのフォーカスと同じなら即終了
        if (currentFocus == previousFocus) return;

        //もしフォーカスが外れていたら
        //前フレームのフォーカスに戻す
        if (currentFocus == null)
        {
            EventSystem.current.SetSelectedGameObject(previousFocus);
            return;
        }
        //前フレームのフォーカスを更新
        previousFocus = EventSystem.current.currentSelectedGameObject;
    }
}
