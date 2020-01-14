using System.Collections.Generic;

using UnityEngine;

namespace DEEP.UI {

    public class DisableUIOnWeb : MonoBehaviour {

        [Tooltip("UI elements to be disabled when running a Web build.")]
        [SerializeField] List<GameObject> disableOnWeb = new List<GameObject>();

        void Start() {

#if UNITY_WEBGL

            foreach (GameObject element in disableOnWeb)
                element.SetActive(false);

#endif

        }

    }

}