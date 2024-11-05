using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // 例: デフォルト値
    private const int DefaultIntValue = 0;
    private const float DefaultFloatValue = 0.0f;
    private const string DefaultStringValue = "";
    private const bool DefaultBoolValue = false;

    private void Start()
    {
        // 初期から入れておくスコア
        if (!LoadBool("DefaultScore"))
        {
            SaveInt("FirstScore", 300);
            SaveInt("SecondScore", 300);
            SaveInt("ThirdScore", 300);
            SaveBool("DefaultScore", true);
        }
    }

    // int型のデータを保存
    public static void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    // int型のデータを読み込み
    public static int LoadInt(string key)
    {
        return PlayerPrefs.GetInt(key, DefaultIntValue);
    }

    // float型のデータを保存
    public static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    // float型のデータを読み込み
    public static float LoadFloat(string key)
    {
        return PlayerPrefs.GetFloat(key, DefaultFloatValue);
    }

    // string型のデータを保存
    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    // string型のデータを読み込み
    public static string LoadString(string key)
    {
        return PlayerPrefs.GetString(key, DefaultStringValue);
    }

    // bool型のデータを保存
    public static void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    // bool型のデータを読み込み
    public static bool LoadBool(string key)
    {
        return PlayerPrefs.GetInt(key, DefaultBoolValue ? 1 : 0) == 1;
    }
}
