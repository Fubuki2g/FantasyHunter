using System.Collections;
using UnityEngine;
using UnityEngine.Audio;    //AudioMixerを使用するのに必要
using SaveData_Settings;    //自前で作ったSaveData_Settingsの使用に必要

public class SoundManager : Singleton<SoundManager>
{
    //--------------------------------------
    //ゲームに必要なサウンドを管理する
    //--------------------------------------

    //使用方法
    //このスクリプトでゲームに使用する全てのAudioを管理する
    //BGM、SystemSE、GameSEの3種類にAudioClipを分けているので、
    //追加したい場合は、エディターで配列の数を増やして追加する

    //ゲーム起動時、AudioClipごとにAudioSorceを自動的に追加している
    //AudioMixerでグループごとにまとめて音量を変更できる

    //カテゴリごとに音を再生するメソッドを分けているので、
    //音を鳴らしたいタイミングで各メソッドを呼び出してあげれば良い
    //例　PlaySE_Sys

    //注意
    //自前で用意したNameSpace「SaveData_Settings」を使い、
    //音量の保存、ロードを行っているので、そのスクリプトがないとエラーになる
    //SaveData_Settingsのスクリプトはプロジェクトに存在しているだけで良い


    [Header("音量コントロールのMixer")]
    [SerializeField] public AudioMixer mixer;   //音量をコントロールする
    [SerializeField, Label("Masterグループ")] public AudioMixerGroup MasterGroup;
    [SerializeField, Label("BGMグループ")] public AudioMixerGroup BGMGroup;
    [SerializeField, Label("SEグループ")] public AudioMixerGroup SEGroup;
    [Space(10)]
    //各カテゴリごとにAudioClipを入れる変数を配列で用意
    public AudioClip[] bgmClip;
    public AudioClip[] se_SysClip;
    public AudioClip[] se_GameClip;

    //各音声用のAudioSourceを用意する
    [System.NonSerialized] public AudioSource BGMSource;
    [System.NonSerialized] public AudioSource SE_SysSource;
    [System.NonSerialized] public AudioSource[] SE_GameSource;

    //音量の段階（SliderのValueで設定）
    float[] vol_BGM = {-80f,-35f,-27,-24f,-21f,-18f,-15f,-12.5f,-10f,-7.5f,-5f };
    float[] vol_SE  = {-80f, -20f, -12f, -9f, -7f, -5f, -3f, -1f, 1f, 3f, 5f };

    //フェード用に音量を保持
    float bgmVol;


    //シーン開始直後（Startメソッドより早く）に処理
    void Awake()
    {
        Load.Audio(); //保存された音量をロード

        //AddComponentでAudioSourceを追加、ループ設定、優先度、MixerGroupの設定
        //BGM
        BGMSource = gameObject.AddComponent<AudioSource>();
        BGMSource.loop = true;
        BGMSource.priority = 0;
        BGMSource.outputAudioMixerGroup = BGMGroup;

        //基本的に複数のSystem用SEが同時になることはないため
        //System用のSEを鳴らす処理ではAudioSourceは１つだけ
        SE_SysSource = gameObject.AddComponent<AudioSource>();
        SE_SysSource.loop = false;
        SE_SysSource.priority = 1;
        SE_SysSource.outputAudioMixerGroup = SEGroup;

        //メインのゲーム中に使用するSEは複数の音が同時に鳴ることが多いため
        //SEのクリップ数と同じだけAudioSourceを用意する
        SE_GameSource = new AudioSource[se_GameClip.Length];

        for (int i = 0; i < se_GameClip.Length; i++)
        {
            if (se_GameClip[i] != null)
            {
                SE_GameSource[i] = gameObject.AddComponent<AudioSource>();
                SE_GameSource[i].loop = false;
                SE_GameSource[i].priority = 1;
                SE_GameSource[i].clip = se_GameClip[i];
                SE_GameSource[i].outputAudioMixerGroup = SEGroup;
            }
        }
    }

    //BGMを外部から呼び出す時
    public void PlayBGM(int i)
    {
        BGMSource.clip = bgmClip[i];
        BGMSource.Play();
    }
    public void StopBGM()
    {
        BGMSource.Stop();
    }

    //SytemSEを外部から呼び出す時
    public void PlaySE_Sys(int i)
    {
        SE_SysSource.clip = se_SysClip[i];
        SE_SysSource.Play();
    }

    //GameSEを外部から呼び出す時
    public void PlaySE_Game(int i)
    {
        SE_GameSource[i].Play();
    }

    //Silderによる音量の調整
    //（第1引数でBGM、第2引数でSEのボリューム）
    public void VolumeChange(int vol1,int vol2)
    {
        mixer.SetFloat("BGVol", vol_BGM[vol1]);
        mixer.SetFloat("SEVol", vol_SE[vol2]);

        bgmVol = vol_BGM[vol1];
    }

    //画面がフェードアウトする時
    //音量も一緒にフェードアウト フェードするのはBGMのみ
    public IEnumerator FadeOut(float interval)
    {
        float time = 0;
        while (time <= interval)
        {
            float bgm = Mathf.Lerp(bgmVol, -80f, time / interval);
            mixer.SetFloat("BGVol", bgm);
            time += Time.deltaTime;
            yield return null;
        }
    }

    //画面がフェードアウトする時
    //音量も一緒にフェードイン
    public IEnumerator FadeIn(float interval)
    {
        float time = 0;
        while (time <= interval)
        {
            float bgm = Mathf.Lerp(-80f,bgmVol, time / interval);
            mixer.SetFloat("BGVol", bgm);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
