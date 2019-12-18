using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Entities;

namespace DEEP.Stage
{

    public class EndTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {

            Debug.Log("Secret found!");

            Player player = FindObjectOfType<Player>();
            player.EndLevel();

            Destroy(this.gameObject);
        }
    }
}
