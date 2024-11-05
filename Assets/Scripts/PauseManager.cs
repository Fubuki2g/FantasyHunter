using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool pauseFLG; // �|�[�Y���̃t���O

    public GameObject Pausecanvas; // �|�[�Y��ʂ̃L�����o�X
    public GameObject focusPauseMenu; // �|�[�Y��ʕ\�����̃t�H�[�J�X��

    GameObject currentFocus;    //���݂̃t�H�[�J�X��̃{�^��
    GameObject previousFocus;   //�O�t���[���f�t�H�[�J�X���Ă����{�^��

    void Update()
    {
        //�t�H�[�J�X���O��Ă��Ȃ����`�F�b�N
        FocusCheck();
    }

    // InputAction�p�̃|�[�Y����
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

    //�|�[�Y����
    public void ChangePause(bool flg)
    {
        pauseFLG = flg;
        EventSystem.current.SetSelectedGameObject(focusPauseMenu);

        //�|�[�Y���������玞�Ԃ��~
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

    // �|�[�Y��ʂ��烊�X�^�[�g���鏈��
    public void ResetGame()
    {
        Time.timeScale = 1;
        GameManager.Instance.StopTimer();
        FadeManager.Instance.LoadSceneIndex(1, 1);
    }

    // �|�[�Y��ʂ���^�C�g���ɖ߂鏈��
    public void PauseTitle()
    {
        Time.timeScale = 1;
        GameManager.Instance.StopTimer();
        FadeManager.Instance.LoadSceneIndex(0, 1);
    }

    //�t�H�[�J�X���O��Ă��Ȃ����`�F�b�N���鏈��
    void FocusCheck()
    {
        currentFocus = EventSystem.current.currentSelectedGameObject;
        //�����O��܂ł̃t�H�[�J�X�Ɠ����Ȃ瑦�I��
        if (currentFocus == previousFocus) return;

        //�����t�H�[�J�X���O��Ă�����
        //�O�t���[���̃t�H�[�J�X�ɖ߂�
        if (currentFocus == null)
        {
            EventSystem.current.SetSelectedGameObject(previousFocus);
            return;
        }
        //�O�t���[���̃t�H�[�J�X���X�V
        previousFocus = EventSystem.current.currentSelectedGameObject;
    }
}
