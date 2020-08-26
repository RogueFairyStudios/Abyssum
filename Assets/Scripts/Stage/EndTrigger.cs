using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Stage
{

    public class EndTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {

            if(other.tag != "Player")
                return;

            Debug.Log("Level ended!");
            PlayerController.Instance.EndLevel();
            Destroy(this.gameObject);

        }
    }
}
