using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuC : MonoBehaviour
{
    private Vector3 _firstPos, _deletePos;

    [SerializeField]
    private GameObject _arrowUI;

    private float _posDelta=100.0f;

    public static bool Pouse = false;

    private int _pouse;

    /// <summary>
    /// 0=Ç‡Ç¡Ç©Ç¢
    /// 1=Ç®ÇÌÇÈ
    /// </summary>
    private short _titleMode = 0;

    private bool _isStarted;

    // Start is called before the first frame update
    void Start()
    {
        Clear.IsClear = false;
        Pouse = false;
        _firstPos = transform.localPosition;
        //transform.position += transform.up * 1000;
        //transform.localPosition = new Vector3(transform.localPosition.x, _firstPos.y + (_posDelta * _pouse), 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Clear.IsClear)
        {
            if (Pouse) _pouse = 0;
            else _pouse = -1;

            if (!_isStarted)
            {
                if (Pouse)
                {
                    //transform.position = _firstPos;
                    if (Input.GetKeyDown("joystick button 0"))
                    {
                        Debug.Log("start!");
                        _isStarted = true;
                        Time.timeScale = 1.0f;
                        PlayerManager.IsPlayerMoveRock = false;

                        if (_titleMode == 0)
                        {
                            SceneManagementC.PlayerDaed = false;
                            SceneManagementC.LoadScene(SceneManagementC.NowScene);
                        }
                        else if (_titleMode == 1) SceneManagementC.LoadScene("TitleScene");
                        //audioSource.PlayOneShot(startS);
                    }
                    if (Input.GetAxis("Horizontal") > 0.5f)
                    {
                        /*_titleMode--;
                        if (_titleMode < 0) */
                        _titleMode = 1;
                        _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
                        Debug.Log("è„");
                    }
                    if (Input.GetAxis("Horizontal") < -0.5f)
                    {
                        /*_titleMode++;
                        if (_titleMode > 1) */
                        _titleMode = 0;
                        _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
                        Debug.Log("â∫");
                    }
                    if (Input.GetKeyDown("joystick button 6") || Input.GetKeyDown("joystick button 7"))
                    {
                        Time.timeScale = 1.0f;
                        transform.localPosition = new Vector3(transform.localPosition.x, _firstPos.y + (_posDelta * _pouse), 0);
                        PlayerManager.IsPlayerMoveRock = false;
                        Pouse = false;
                    }
                }
                else
                {
                    //transform.position = _deletePos;
                    if (Input.GetKeyDown("joystick button 6") || Input.GetKeyDown("joystick button 7"))
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, _firstPos.y + (_posDelta * _pouse), 0);
                        Pouse = true;
                        PlayerManager.IsPlayerMoveRock = true;
                        Time.timeScale = 0.0f;
                    }
                }
            }
        }
    }

    private IEnumerator Load()
    {
        SceneManagementC.SaveNowSceneNameToNowScene();
        yield return new WaitForSeconds(0.9f);

        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1.0f;
        if (_titleMode == 0)
        {
            SceneManagementC.PlayerDaed = false;
            SceneManagementC.LoadScene(SceneManagementC.NowScene);
        }
        else if (_titleMode == 1) SceneManagementC.LoadScene("TitleScene");
    }

    private void Reset()
    {
        
    }
}
