using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleC : MonoBehaviour
{

    [SerializeField]
    private AudioClip _startME;

    [SerializeField]
    private AudioSource _audioSou;

    [SerializeField]
    private string _selectScn;

    /// <summary>
    /// スタートされたか
    /// </summary>
    private bool _isStarted;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //selectScene
        if (Input.GetKeyDown("joystick button 0")&&!_isStarted) StartCoroutine(ChangeScene(_startME,_selectScn));
    }

    private IEnumerator ChangeScene(AudioClip audio,string loadScene)
    {
        _isStarted = true;
        _audioSou.PlayOneShot(audio);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(loadScene);
    }
}
