using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementC : MonoBehaviour
{
    /// <summary>
    /// プレイ中のゲームシーン
    /// </summary>
    public static string NowScene;

    /// <summary>
    /// プレイヤーが死んだ場所
    /// </summary>
    public static Vector3 PositionPlayerDead;

    /// <summary>
    /// クリアまでにゲームオーバーを経験しているか
    /// </summary>
    public static bool PlayerDaed;

    /// <summary>
    /// プレイ中のゲームシーンの名前をstring型でNowSceneに保存
    /// </summary>
    public static void SaveNowSceneNameToNowScene()
    {
        NowScene=SceneManager.GetActiveScene().name;
        
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void ReloadScene()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
