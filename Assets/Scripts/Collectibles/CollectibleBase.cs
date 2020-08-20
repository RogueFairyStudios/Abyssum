using UnityEngine;

using DEEP.Stage;
using DEEP.Entities;

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

        protected virtual void OnTriggerEnter(Collider col)
        {

            // Checks for a Player component.
            Player player = col.GetComponent<Player>();
            if (player != null)               
                Collect(); // Calls the collection function.

        }

        // Overwrite to do what you want with your collectible and them call base to log and destroy the object.
        protected virtual void Collect() {

            // Logs that this item has been collected.
            if(logText.Length > 0)
                Player.Instance.HUD.Log.Message(logText, logIcon, logColor);

            // Count this item as collected.
            StageManager.Instance.CountCollection();

            // Destroys the object if the collectible is used.
            Destroy(gameObject);

        }
    }
}