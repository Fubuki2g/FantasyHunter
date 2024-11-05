using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public GameObject[] Button; // �ŏ��Ƀt�H�[�J�X����e�{�^��
    public GameObject[] Canvas; // �\����؂�ւ���L�����o�X

    // �X�R�A�Ɋւ���ϐ�
    public TextMeshProUGUI[] Score;
    public string[] minute;
    public string[] second;

    GameObject currentFocus;    //���݂̃t�H�[�J�X��̃{�^��
    GameObject previousFocus;   //�O�t���[���f�t�H�[�J�X���Ă����{�^��

    void Start()
    {
        SoundManager.Instance.PlayBGM(0);
        // �}�E�X�J�[�\�����\���ɂ��鏈��
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (GameObject canvas in Canvas)
        {
            canvas.SetActive(false);
        }
        Canvas[0].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[0]);

        // �ۑ�����Ă���n�C�X�R�A��\�����邽�߂̏���
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
        Score[0].text = minute[0] + "�F" + second[0];
        Score[1].text = minute[1] + "�F" + second[1];
        Score[2].text = minute[2] + "�F" + second[2];
        FocusCheck();
    }

    // �Q�[���V�[���ɑJ�ڂ��鏈��
    public void Play()
    {
        SoundManager.Instance.PlaySE_Sys(2);
        FadeManager.Instance.LoadSceneIndex(1, 1);
    }

    // �X�R�A�L�����o�X��\�����鏈��
    public void HiScore()
    {
        Canvas[0].SetActive(false);
        Canvas[1].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[1]);
        SoundManager.Instance.PlaySE_Sys(0);
    }

    // ��������L�����o�X��\�����鏈��
    public void Manual()
    {
        Canvas[0].SetActive(false);
        Canvas[2].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[2]);
        SoundManager.Instance.PlaySE_Sys(0);
    }

    // �I���m�F�L�����o�X��\�����鏈��
    public void Check()
    {
        Canvas[0].SetActive(false);
        Canvas[3].SetActive(true);
        EventSystem.current.SetSelectedGameObject(Button[3]);
        SoundManager.Instance.PlaySE_Sys(0);

    }

    // �Q�[���I���̏���
    public void Exit()
    {
        Application.Quit();
        SoundManager.Instance.PlaySE_Sys(0);
    }

    // �e�L�����o�X����߂鏈��
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

    //�t�H�[�J�X���O��Ă��Ȃ����`�F�b�N
    void FocusCheck()
    {
        currentFocus = EventSystem.current.currentSelectedGameObject;
        if (currentFocus == previousFocus) return;

        //�����t�H�[�J�X���O��Ă�����
        //�O�t���[���̃t�H�[�J�X�ɖ߂�
        if (currentFocus == null)
        {
            EventSystem.current.SetSelectedGameObject(previousFocus);
            return;
        }
        previousFocus = EventSystem.current.currentSelectedGameObject;
    }

}
