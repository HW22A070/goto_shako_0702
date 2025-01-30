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

    private bool _isCredit;

    [SerializeField]
    private SpriteRenderer _titleWindow;
    [SerializeField]
    private Sprite _title, _credit;

    /// <summary>
    /// スタートされたか
    /// </summary>
    private bool _isStarted;

    // Start is called before the first frame update
    void Start()
    {
        SceneManagementC.PlayerDaed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isCredit) _titleWindow.sprite = _credit;
        else _titleWindow.sprite = _title;


        if (!_isStarted)
        {
            if (_isCredit)
            {
                if (!_isStarted)
                {
                    if (Input.GetKeyDown("joystick button 0"))
                    {
                        _isCredit = false;
                    }
                }
            }
            else
            {

                if (Input.GetKeyDown("joystick button 2"))
                {
                    _isCredit = true;
                }

                if (Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 3")
                    || Input.GetKeyDown("joystick button 4") || Input.GetKeyDown("joystick button 5") || Input.GetKeyDown("joystick button 6")
                    || Input.GetKeyDown("joystick button 7") || Input.GetKeyDown("joystick button 8") || Input.GetKeyDown("joystick button 9"))
                {
                    StartCoroutine(ChangeScene(_startME, _selectScn));
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    StartCoroutine(ChangeScene(_startME, "Stage1Scene"));
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    StartCoroutine(ChangeScene(_startME, "Stage2Scene"));
                }
            }
        }

    }

    private IEnumerator ChangeScene(AudioClip audio, string loadScene)
    {
        _isStarted = true;
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(loadScene);
    }

    
}
