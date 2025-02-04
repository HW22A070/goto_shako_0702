using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustC : MonoBehaviour
{
    Vector3 pos;
    public ExpC ExpPrefab;

    [SerializeField]
    [Tooltip("回転するか")]
    private bool _isKaitenLock;

    [SerializeField, Tooltip("えふぇくとでらい")]
    private float _deray = 0.03f;

    public float sizex, sizey, speed, delete;

    float angle;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EffectEX());
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
    }

    private IEnumerator EffectEX()
    {
        pos.x += Random.Range(-sizex, sizex + 1);
        pos.y += Random.Range(-sizey, sizey + 1);
        if (_isKaitenLock) angle = -GetAngle(transform.right - Vector3.zero);
        else angle = Random.Range(0, 360);
        Quaternion rot = transform.localRotation;
        Instantiate(ExpPrefab, pos, rot).EShot1(angle, speed, delete);
        yield return new WaitForSeconds(_deray);
        StartCoroutine(EffectEX());
    }

    /// <summary>
    /// Aから見たBの向きを特定
    /// </summary>
    /// <param name="direction">Vector3 B-A</param>
    /// <returns>Aから見たBの向き</returns>
    private float GetAngle(Vector3 direction)
    {
        float rad = Mathf.Atan2(direction.x, direction.y);
        return rad * Mathf.Rad2Deg;
    }
}
