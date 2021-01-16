using UnityEngine;

using DEEP.Stage;
using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public abstract class CollectibleBase : MonoBehaviour
    {

        [Tooltip("If this collectible is used in multiplayer mode, if it isn't add the default collection functions to OnCollect.")]
        [SerializeField] protected bool onlineCollectible = false;

        [Tooltip("Audio that can be played when a item is collected.")]
        [SerializeField] protected AudioClip collectionSound = null;

        [Tooltip("Text to be logged when collected.")]
        [SerializeField] protected string logText = "Collected";

        [Tooltip("Icon to be presented in the log message when collected.")]
        [SerializeField] protected Sprite logIcon = null;

        [Tooltip("Color to be used in the log message when collected.")]
        [SerializeField] protected Color logColor = new Color( 0.6f, 0.6f, 0.6f, 0.6f);

        // An spawned weapon doesn't count as an item at the stage statistics.
        private bool spawned = false;
        [SerializeField] protected bool IsSpawned {

            get {
                return spawned;
            }

        }

        protected virtual void OnTriggerEnter(Collider other)
        {

            // If it's not a multiplayer item, checks for the Player.
            if (!onlineCollectible && other.tag == "Player") {

                // Tries to collect the item.
                if(Collect(other.gameObject)) {

                    // Logs that this item has been collected.
                    if(logText.Length > 0)
                        other.GetComponent<PlayerController>().HUD.Log.Message(logText, logIcon, logColor);

                     // Count this item as collected.
                    if(!IsSpawned && StageManager.Instance != null)
                        StageManager.Instance.CountCollection();

                    // Destroys the object if the collectible is used.
                    Destroy(gameObject);

                }

            }

        }

        // Overwrite to do what you want with your collectible.
        public abstract bool Collect(GameObject collector);

    }
}