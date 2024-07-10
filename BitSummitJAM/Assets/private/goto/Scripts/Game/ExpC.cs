using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpC : MonoBehaviour
{
    Vector3 velocity, pos;
    float sspeed, kkaso, aang;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void EShot1(float angle, float speed,float delete)
    {
        var direction = GetDirection(angle);
        velocity = direction * speed;
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;
        Destroy(gameObject,delete);
    }

    public Vector3 GetDirection(float angle)
    {
        Vector3 direction = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0);
        return direction;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = transform.position;
        transform.localPosition += velocity;
    }

}
