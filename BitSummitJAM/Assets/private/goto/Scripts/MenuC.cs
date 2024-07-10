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

    private int _pouse=1;

    /// <summary>
    /// 0=‚à‚Á‚©‚¢
    /// 1=‚¨‚í‚é
    /// </summary>
    private short _titleMode = 0;

    // Start is called before the first frame update
    void Start()
    {

        _firstPos = transform.localPosition;
        //transform.position += transform.up * 1000;
        //transform.localPosition = new Vector3(transform.localPosition.x, _firstPos.y + (_posDelta * _pouse), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Pouse) _pouse = 0;
        else _pouse = -1;        

        if (Pouse)
        {
            //transform.position = _firstPos;
            if (Input.GetKeyDown("joystick button 0"))
            {
                Time.timeScale = 1.0f;
                Debug.Log("start!");
                if (_titleMode == 0) SceneManagementC.LoadScene("Stage1Scene");
                else if (_titleMode == 1) SceneManagementC.LoadScene("TitleScene");
                //audioSource.PlayOneShot(startS);
            }
            if (Input.GetKeyDown("joystick button 4"))
            {
                _titleMode--;
                if (_titleMode < 0) _titleMode = 1;
                _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
                Debug.Log("ã");
            }
            if (Input.GetKeyDown("joystick button 5"))
            {
                _titleMode++;
                if (_titleMode > 1) _titleMode = 0;
                _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
                Debug.Log("‰º");
            }
            if (Input.GetKeyDown("joystick button 6") || Input.GetKeyDown("joystick button 3"))
            {
                Time.timeScale = 1.0f;
                transform.localPosition = new Vector3(transform.localPosition.x, _firstPos.y + (_posDelta * _pouse), 0);
                Pouse = false;
            }
        }
        else
        {
            //transform.position = _deletePos;
            if (Input.GetKeyDown("joystick button 6")|| Input.GetKeyDown("joystick button 3"))
            {
                transform.localPosition = new Vector3(transform.localPosition.x, _firstPos.y + (_posDelta * _pouse), 0);
                Pouse = true;
                Time.timeScale = 0.0f;
            }
        }
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1.0f);
        if (_titleMode == 0) SceneManagementC.LoadScene("Stage1Scene");
        else if (_titleMode == 1) SceneManagementC.LoadScene("TitleScene");
    }

    private void Reset()
    {
        
    }
}
