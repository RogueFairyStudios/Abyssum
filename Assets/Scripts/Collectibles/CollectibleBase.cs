using UnityEngine;

using DEEP.Stage;
using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public abstract class CollectibleBase : MonoBehaviour
    {

        [Tooltip("Audio that can be played when a item is collected.")]
        [SerializeField] protected AudioClip collectionSound = null;

        [Tooltip("Text to be logged when collected.")]
        [SerializeField] protected string logText = "Collected";

        [Tooltip("Icon to be presented in the log message when collected.")]
        [SerializeField] protected Sprite logIcon = null;

        [Tooltip("Color to be used in the log message when collected.")]
        [SerializeField] protected Color logColor = new Color( 0.6f, 0.6f, 0.6f, 0.6f);

        protected virtual void OnTriggerEnter(Collider other)
        {

            // Checks for the Player.
            if (other.tag == "Player") {       
                Collect(other.gameObject); // Calls the collection function.
            }

        }

        // Overwrite to do what you want with your collectible and them call base to log and destroy the object.
        protected virtual void Collect(GameObject player) {

            // Logs that this item has been collected.
            if(logText.Length > 0)
                player.GetComponent<PlayerController>().HUD.Log.Message(logText, logIcon, logColor);

            // Count this item as collected.
            StageManager.Instance.CountCollection();

            // Destroys the object if the collectible is used.
            Destroy(gameObject);

        }
    }
}