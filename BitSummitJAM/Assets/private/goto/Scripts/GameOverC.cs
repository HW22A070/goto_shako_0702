using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverC : MonoBehaviour
{
    [SerializeField]
    private GameObject _arrowUI;

    private bool start = false;

    [SerializeField]
    private SpriteRenderer _playerdead;

    [SerializeField]
    private Sprite _playerFine;

    /// <summary>
    /// 0=Ç‡Ç¡Ç©Ç¢
    /// 1=Ç®ÇÌÇÈ
    /// </summary>
    private short _titleMode = 0;

    // Start is called before the first frame update
    void Start()
    {
        SceneManagementC.PlayerDaed = true;
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
        if (Input.GetAxis("Horizontal") > 0.5f && !start)
        {
            _titleMode = 1;
            _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
            Debug.Log("è„");
        }
        if (Input.GetAxis("Horizontal") < -0.5f && !start)
        {
            _titleMode = 0;
            _arrowUI.GetComponent<Arrow>().MoveArrow(_titleMode);
            Debug.Log("â∫");
        }
    }

    private IEnumerator Load()
    {
        if (_titleMode == 0)
        {
            _playerdead.sprite = _playerFine;
        }
        yield return new WaitForSeconds(1.0f);
        if (_titleMode == 0)
        {
            SceneManagementC.LoadScene(SceneManagementC.NowScene);
        }
        else if (_titleMode == 1)
        {
            SceneManagementC.LoadScene("TitleScene");
        }
    }

}
