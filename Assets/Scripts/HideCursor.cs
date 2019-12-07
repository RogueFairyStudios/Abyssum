using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCursor : MonoBehaviour
{

    public bool hide = true;

    // Start is called before the first frame update
    void Start()
    {
        
        if(hide) Cursor.visible = false;
        else Cursor.visible = true;

    }

    /// <summary>
    ///  If on windows player, lock cursor (It bugs out on linux players)
    /// </summary>
    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && Application.platform == RuntimePlatform.WindowsEditor)
            Cursor.lockState = CursorLockMode.Locked;
    }
}
