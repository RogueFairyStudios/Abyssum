using UnityEngine;
using UnityEngine.Events;

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

        [System.Serializable]
        public class OnCollectEvent : UnityEvent<GameObject>{}

        // Events to be called when the item is collected.
        public OnCollectEvent OnCollect;

        protected virtual void Awake()
        {

            // Adds the default functions to be used when the object is collected in
            // singleplayer mode.
            if(!onlineCollectible) {
                OnCollect.AddListener(LogCollection);
                OnCollect.AddListener(CountCollection);
                OnCollect.AddListener(DeleteCollectible);
            }

        }

        protected virtual void OnTriggerEnter(Collider other)
        {

            // If it's not a multiplayer collection, checks for the Player and calls for OnCollect function.
            if (!onlineCollectible && other.tag == "Player" && OnCollect != null) {
                if(Collect(other.gameObject)) // Tries to collect the item.
                    OnCollect.Invoke(other.gameObject); // Calls the collection events if collected.
            }

        }

        // Overwrite to do what you want with your collectible.
        public abstract bool Collect(GameObject collector);

        // Logs the collected object information to the log of the player who collected it. (Singleplayer only)
        protected void LogCollection(GameObject collector) {

            // Logs that this item has been collected.
            if(logText.Length > 0)
                collector.GetComponent<PlayerController>().HUD.Log.Message(logText, logIcon, logColor);

        }

        // Counts the colelction of this item to the colelction percentage on the StageManager. (Singleplayer only)
        protected void CountCollection(GameObject collector) {

            // Count this item as collected.
            if(StageManager.Instance != null)
                StageManager.Instance.CountCollection();

        }

        // Deletes the collectible. (Singleplayer only)
        protected void DeleteCollectible(GameObject collector) {

            // Destroys the object if the collectible is used.
            Destroy(gameObject);

        }

    }
}