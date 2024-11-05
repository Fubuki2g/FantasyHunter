using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FadeManager : Singleton<FadeManager>
{
	//--------------------------------------------------
	//シーン遷移時にフェードイン・アウトを行うための処理
	//--------------------------------------------------

	//使用方法
	//シーンの「名前」を使って遷移する場合はLoadSceneメソッド、
	//シーンの「番号」を使って遷移する場合はLoadSceneIndexメソッドを使う

	//呼び出したいタイミングで以下のようにメソッドを呼び出す
	//FadeManager.Instance.LoadScene(シーン名,フェードに掛ける秒数);
	//FadeManager.Instance.LoadSceneIndex(シーン番号,フェードに掛ける秒数);

	//注意
	//フェードの際に、BGMの音量もフェードするようSoundManagerの処理を
	//呼び出しているので、そちらのスクリプト（ゲームオブジェクト）が
	//シーン上に存在していないとエラーになる


	private float fadeAlpha = 0;					//フェード中の透明度
	[NonSerialized] public bool isFading = false;	//フェード中かどうか
	public Color fadeColor = Color.black;           //フェード色

	public void Awake ()
	{
		//もし他のオブジェクトの子であれば、親子関係を解除
		if (gameObject.transform.parent != null) gameObject.transform.parent = null;

		//もし他にFadeManagerが存在していたら、このオブジェクトをDestroy
		if (this != Instance)
        {
			Destroy (gameObject);
			return;
		}

		DontDestroyOnLoad (gameObject);	//FadeManagerはシーン遷移しても削除しない
	}

	public void OnGUI ()
	{
		if (isFading == true)
		{
			//色と透明度を更新して白テクスチャを描画 .
			fadeColor.a = fadeAlpha;
			GUI.color = fadeColor;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
		}
	}

	//シーン名で画面遷移
	public void LoadScene (string scene, float interval)
	{
		StartCoroutine (TransScene (scene, interval));
	}

	//IndexNumberで画面遷移
	public void LoadSceneIndex(int sceneIndex, float interval)
	{
		StartCoroutine(TransSceneIndex(sceneIndex, interval));
	}


	//シーン遷移用コルーチン
	private IEnumerator TransScene (string scene, float interval)
	{
		//音量もフェードアウト
		StartCoroutine(SoundManager.Instance.FadeOut(interval*0.9f));

		//だんだん暗く
		isFading = true;
		float time = 0;
		while (time <= interval)
        {
			fadeAlpha = Mathf.Lerp (0f, 1f, time / interval);
            time += Time.deltaTime;
			yield return null;
		}

		Debug.Log("シーンを遷移…" + scene);
		SceneManager.LoadScene (scene); //シーン切替

		//音量もフェードイン
		StartCoroutine(SoundManager.Instance.FadeIn(interval * 0.9f));

		//だんだん明るく
		time = 0;
		while (time <= interval)
        {
			fadeAlpha = Mathf.Lerp (1f, 0f, time / interval);
            time += Time.deltaTime;
			yield return null;
		}

		isFading = false;
	}

	//IndexNumberのシーン遷移用コルーチン
	private IEnumerator TransSceneIndex(int sceneIndex, float interval)
	{
		//音量もフェードアウト
		StartCoroutine(SoundManager.Instance.FadeOut(interval * 0.9f));

		//だんだん暗く
		isFading = true;
		float time = 0;
		while (time <= interval)
		{
			fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return null;
		}

		Debug.Log("シーンを遷移…" + sceneIndex);
		SceneManager.LoadScene(sceneIndex); //シーン切替

		//音量もフェードイン
		StartCoroutine(SoundManager.Instance.FadeIn(interval * 0.9f));

		//だんだん明るく
		time = 0;
		while (time <= interval)
		{
			fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
			time += Time.deltaTime;
			yield return null;
		}

		isFading = false;
	}
}
