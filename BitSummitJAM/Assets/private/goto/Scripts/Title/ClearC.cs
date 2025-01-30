using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearC : MonoBehaviour
{
    /// <summary>
    /// スタートされたか
    /// </summary>
    private bool _isStarted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isStarted)
        {
            if (Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3")
                || Input.GetKeyDown("joystick button 4") || Input.GetKeyDown("joystick button 5") || Input.GetKeyDown("joystick button 6")
                || Input.GetKeyDown("joystick button 7") || Input.GetKeyDown("joystick button 8") || Input.GetKeyDown("joystick button 9"))
            {
                StartCoroutine(ChangeScene());
            }
        }

    }

    private IEnumerator ChangeScene()
    {
        _isStarted = true;
        yield return new WaitForSeconds(1.0f);
        SceneManagementC.LoadScene("TitleScene");
    }
}
