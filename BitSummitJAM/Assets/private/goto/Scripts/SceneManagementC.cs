using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementC : MonoBehaviour
{
    /// <summary>
    /// �v���C���̃Q�[���V�[��
    /// </summary>
    public static string NowScene;

    /// <summary>
    /// �v���C���[�����񂾏ꏊ
    /// </summary>
    public static Vector3 PositionPlayerDead;

    /// <summary>
    /// �N���A�܂łɃQ�[���I�[�o�[���o�����Ă��邩
    /// </summary>
    public static bool PlayerDaed;

    /// <summary>
    /// �v���C���̃Q�[���V�[���̖��O��string�^��NowScene�ɕۑ�
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
