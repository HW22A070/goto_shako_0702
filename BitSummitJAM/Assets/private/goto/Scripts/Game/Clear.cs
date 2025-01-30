using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : MonoBehaviour
{
    /// <summary>
    /// �{�X���j�ɂ��N���A�H
    /// </summary>
    private bool _isBossClear=true;

    [SerializeField,Header("�N���AX���W")]
    private float _clearTransformX;

    /// <summary>
    /// �N���A������
    /// </summary>
    public static bool IsClear;

    /// <summary>
    /// �Q�[���I��点�Ă�����
    /// </summary>
    private bool _isAbleEndGame;

    private Coroutine _clearCorotine;

    private GameObject _goPlayer,_goCamera;

    [SerializeField,Tooltip("������")]
    private KamihubukiC _prhbKamihubuki;

    [SerializeField]
    private string _selectScn;

    // Start is called before the first frame update
    void Start()
    {
        SceneManagementC.SaveNowSceneNameToNowScene();

        if (!GameObject.Find("Boss1")) _isBossClear = false;
        _goPlayer = GameObject.Find("PlayerManager");
        _goCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsClear && !_isBossClear && _clearTransformX < _goPlayer.transform.position.x)
        {
            ClearEffect();
        }

        if (IsClear)
        {
            if (Input.GetKeyDown("joystick button 0"))
            {
                Debug.Log("Finish!");
                _isAbleEndGame = true;
            }
        }
        
    }

    public void ClearEffect()
    {
        if (!IsClear)
        {
            PlayerManager.IsPlayerMoveRock = true;
            IsClear = true;
            _clearCorotine = StartCoroutine(ClearMove());
            SceneManagementC.PlayerDaed = false;
        }
        
    }

    private IEnumerator ClearMove()
    {
        //�N���A�|�[�Y
        _goPlayer.GetComponent<GraphicC>().ResetAnimation(14);
        yield return new WaitForSeconds(0.5f);
        FireWork();
        while (!_isAbleEndGame)
        {
            yield return new WaitForSeconds(0.1f);
        }
        _goPlayer.GetComponent<PlayerAnimC>().StartTriumphantReturn();
        _goCamera.GetComponent<PlayerCamera>().Stop();
        _goPlayer.GetComponent<Mover>().Goal(_goPlayer.transform.position + new Vector3(20, 0, 0));
        yield return new WaitForSeconds(3.0f);
        SceneManagementC.LoadScene(_selectScn);
    }

    /// <summary>
    /// �Ԑ���
    /// </summary>
    private void FireWork()
    {
        Instantiate(_prhbKamihubuki, _goCamera.transform.position+(transform.up*3.0f)+transform.forward, transform.rotation).transform.parent = _goCamera.transform;
        //�G�S�E�C
        GameObject[] _enemys = GameObject.FindGameObjectsWithTag("Enemy");
        for(int hoge = 0; hoge < _enemys.Length; hoge++)
        {
            Destroy(_enemys[hoge]);
        }
        
    }
}
