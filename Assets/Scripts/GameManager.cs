using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    // �Q�[���̏�Ԃ�\���t���O
    public bool mainGame;
    public bool clearGame;
    public bool overGame;

    // ���Ԍv���Ɋւ���ϐ�
    public float time;
    public float limitTime;
    public Slider timeSlider;
    public bool timeStamina;

    // �^�C�����C���p��UI
    public GameObject startCanvas;
    public GameObject mainGameCanvas;
    public GameObject clearCanvas;
    public GameObject overCanvas;

    // �e�^�C�����C��
    public PlayableDirector startPD;
    public PlayableDirector clearPD;
    public PlayableDirector overPD;

    // �e�^�C�����C�����̃t�H�[�J�X��
    public GameObject[] focusButton;

    // �X�R�A�v�Z�Ɋւ���ϐ�
    public TextMeshProUGUI timeScore;
    public string minute; 
    public string second;

    void Start()
    {
        // �}�E�X�J�[�\�����\���ɂ��鏈��
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
        // mainGame��true�̎��̂݃^�C�}�[������
        if (mainGame)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 0;
            }
            // �c�莞�Ԕ����ŃX�^�~�i�̍ő�l������
            else if (time <= limitTime/2 && !timeStamina)
            {
                timeStamina = true;
                CharacterController.Instance.StaminaDown();
            }
            timeSlider.value = time;
        }
        
        // �Q�[���N���A���̏���
        if (clearGame && mainGame)
        {
            SoundManager.Instance.StopBGM();
            mainGame = false;
            ClearDemo();
        }

        // �Q�[���I�[�o�[���̏���
        if (overGame && mainGame)
        {
            SoundManager.Instance.StopBGM();
            mainGame = false;
            OverDemo();
        }

    }

    // �X�^�[�g�^�C�����C���Đ���̏���
    public void MainGame()
    {
        startCanvas.SetActive(false);
        mainGame = true;
        CharacterController.Instance.walkable = true;
        StartTimer();
        SoundManager.Instance.PlayBGM(0);
    }

    // �^�C�}�[���J�n���郁�\�b�h
    public void StartTimer()
    {
        mainGame = true;
    }

    // �N���A���Ƀ^�C�}�[���~���郁�\�b�h
    public void StopTimer()
    {
        mainGame = false;
        // �^�C�}�[��~���ɃN���A��Ԃ��ƃN���A�^�C����ۑ����鏈��
        // ���łɕۑ�����Ă��鎞�Ԃ�菭�Ȃ��ꍇ�X�V�����
        // �R�ʂ܂ŕۑ�
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

    // �N���A���̃^�C�����C�����Đ����鏈��
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
        timeScore.text = minute + "�F" + second;
    }

    // �Q�[���I�[�o�[���̃^�C�����C�����Đ����鏈��
    public void OverDemo()
    {
        overPD.Play();
        mainGameCanvas.SetActive(false);
        overCanvas.SetActive(true);
        CharacterController.Instance.walkable = false;
        StopTimer();
    }

    // �^�C�����C���Đ����̃{�^���t�H�[�J�X����
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

    // �ȉ��N���A���ƃQ�[���I�[�o�[���̃{�^���̃��\�b�h
    // �V�[���������[�h���鏈��
    public void Retry()
    {
        SoundManager.Instance.PlaySE_Sys(0);
        FadeManager.Instance.LoadSceneIndex(1, 1);
    }

    // �^�C�g���V�[���֑J�ڂ��鏈��
    public void Title()
    {
        SoundManager.Instance.PlaySE_Sys(0);
        FadeManager.Instance.LoadSceneIndex(0, 1);
    }

}
