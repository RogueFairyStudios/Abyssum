using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Entities;

namespace DEEP.Stage
{

    public class SecretTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {

            Debug.Log("Secret found!");

            Player player = FindObjectOfType<Player>();
            player.FoundSecret();

            Destroy(this.gameObject);
        }
    }
}
