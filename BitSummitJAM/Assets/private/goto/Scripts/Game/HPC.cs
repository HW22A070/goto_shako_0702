using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPC : MonoBehaviour
{
    public int hpvalue;

    [SerializeField]
    private SpriteRenderer _spHP;

    [SerializeField]
    private Sprite _red, _void;

    [SerializeField]
    [Tooltip("ÉvÉåÉCÉÑÅ[èÓïÒ")]
    private GameObject _gameobjectPlayer;

    private void Start()
    {
        _gameobjectPlayer = GameObject.Find("PlayerManager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_gameobjectPlayer.GetComponent<PlayerManager>().HPSender() < hpvalue) _spHP.sprite = _void;
        else _spHP.sprite = _red;
    }
}
