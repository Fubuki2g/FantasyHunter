using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // ��: �f�t�H���g�l
    private const int DefaultIntValue = 0;
    private const float DefaultFloatValue = 0.0f;
    private const string DefaultStringValue = "";
    private const bool DefaultBoolValue = false;

    private void Start()
    {
        // �����������Ă����X�R�A
        if (!LoadBool("DefaultScore"))
        {
            SaveInt("FirstScore", 300);
            SaveInt("SecondScore", 300);
            SaveInt("ThirdScore", 300);
            SaveBool("DefaultScore", true);
        }
    }

    // int�^�̃f�[�^��ۑ�
    public static void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    // int�^�̃f�[�^��ǂݍ���
    public static int LoadInt(string key)
    {
        return PlayerPrefs.GetInt(key, DefaultIntValue);
    }

    // float�^�̃f�[�^��ۑ�
    public static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    // float�^�̃f�[�^��ǂݍ���
    public static float LoadFloat(string key)
    {
        return PlayerPrefs.GetFloat(key, DefaultFloatValue);
    }

    // string�^�̃f�[�^��ۑ�
    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    // string�^�̃f�[�^��ǂݍ���
    public static string LoadString(string key)
    {
        return PlayerPrefs.GetString(key, DefaultStringValue);
    }

    // bool�^�̃f�[�^��ۑ�
    public static void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    // bool�^�̃f�[�^��ǂݍ���
    public static bool LoadBool(string key)
    {
        return PlayerPrefs.GetInt(key, DefaultBoolValue ? 1 : 0) == 1;
    }
}
