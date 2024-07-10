using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectC : MonoBehaviour
{
    [SerializeField]
    private AudioClip _startME, _backME,_selectME;

    [SerializeField]
    private AudioSource _audioSou;

    [SerializeField]
    private string _titleScn;

    [SerializeField,Tooltip("ステージ")]
    private string[] _gameScn;

    [SerializeField]
    private GameObject _arrowUI;

    /// <summary>
    /// 選ばれているステージ
    /// </summary>
    public int _titleMode;

    /// <summary>
    /// スタートされたか
    /// </summary>
    private bool start;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("joystick button 0") && !start)
        {
            start = true;
            Debug.Log("start!");
            StartCoroutine(Load());
            _audioSou.PlayOneShot(_startME);
        }
        if (Input.GetKeyDown("joystick button 4") && !start)
        {
            _titleMode--;
            if (_titleMode < 0) _titleMode = 2;
            _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
            _audioSou.PlayOneShot(_selectME);
        }
        if (Input.GetKeyDown("joystick button 5") && !start)
        {
            _titleMode++;
            if (_titleMode > 2) _titleMode = 0;
            _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
            _audioSou.PlayOneShot(_selectME);
        }

    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManagementC.LoadScene(_gameScn[_titleMode]);
    }
}
