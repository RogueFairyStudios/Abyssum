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
}
