using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.Utility {

    public class CursorLock : MonoBehaviour
    {

        public bool locked = true;


        void Start() {

            SetLock(locked);

        }

        void Update() {

            if(Input.GetKeyDown(KeyCode.F7))
                SetLock(!locked);

        }

        void SetLock(bool locked)
        {

            this.locked = locked;

            if(locked) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            } else {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

        }
    }

}
