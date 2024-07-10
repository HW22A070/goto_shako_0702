using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverC : MonoBehaviour
{
    [SerializeField]
    private GameObject _arrowUI;

    private bool start = false;

    /// <summary>
    /// 0=‚à‚Á‚©‚¢
    /// 1=‚¨‚í‚é
    /// </summary>
    private short _titleMode = 0;

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
            //audioSource.PlayOneShot(startS);
        }
        if (Input.GetKeyDown("joystick button 4") && !start)
        {
            _titleMode--;
            if (_titleMode < 0) _titleMode = 1;
            _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
            Debug.Log("ã");
        }
        if (Input.GetKeyDown("joystick button 5") && !start)
        {
            _titleMode++;
            if (_titleMode > 1) _titleMode = 0;
            _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
            Debug.Log("‰º");
        }
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1.0f);
        if (_titleMode == 0) SceneManagementC.LoadScene("Stage1Scene");
        else if (_titleMode == 1)SceneManagementC.LoadScene("TitleScene");
    }

}
