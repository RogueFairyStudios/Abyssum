using UnityEngine;

namespace DEEP.HUD
{

    /* 
        This class works as a hub for getting a reference to the different parts of the HUD.
    */
    public class HUDController : MonoBehaviour {

        [SerializeField] protected PlayerInfoHUD playerInfo = null;

        // Shortcuts to the PlayerInfo classes.
        public PlayerInfoHUD.HealthHUD Health { get { return playerInfo.health; } }
        public PlayerInfoHUD.ArmorHUD Armor { get { return playerInfo.armor; } }
        public PlayerInfoHUD.AmmoAndWeaponHUD AmmoAndWeapons { get { return playerInfo.ammoAndWeapons; } }
        public PlayerInfoHUD.KeyHUD Keycards { get { return playerInfo.keycards; } }
        public PlayerInfoHUD.LogHUD Log { get { return playerInfo.Log; } }

        [SerializeField] protected SpeedrunHUD speedrun = null;
        public SpeedrunHUD Speedrun {
            get { return speedrun; }
        }

        [SerializeField] protected StatisticsHUD statistics = null;
        public StatisticsHUD Statistics {
            get { return statistics; }
        }

        [SerializeField] protected FeedbackHUD feedback = null;
        public FeedbackHUD Feedback {
            get { return feedback; }
        }

        // ====================================================================================================================

        public void Start() {

            Debug.Log("Initializing HUDController...");

            // Checks for the player info HUD.
            if(playerInfo == null) {
                Debug.LogError("HUDController.Start: PlayerInfoHUD was not found!");
            } else {
                if(Health == null)          Debug.LogError("HUDController.Start: PlayerInfoHUD.health was not found!");
                if(Armor == null)           Debug.LogError("HUDController.Start: PlayerInfoHUD.armor was not found!");
                if(AmmoAndWeapons == null)  Debug.LogError("HUDController.Start: PlayerInfoHUD.ammoAndWeapons was not found!");
                if(Keycards == null)        Debug.LogError("HUDController.Start: PlayerInfoHUD.keycards was not found!");
                if(Log == null)             Debug.LogError("HUDController.Start: PlayerInfoHUD.Log was not found!");
            }

            // Checks for the statistics HUD.
            if(Statistics == null) {
                Debug.LogError("HUDController.Start: StatisticsHUD was not found!");
            } else {
                // Gets initial statistics HUD value.
                if(!PlayerPrefs.HasKey("StatisticsHUD"))
                    PlayerPrefs.SetInt("StatisticsHUD", 0);
                Statistics.SetEnabled(PlayerPrefs.GetInt("StatisticsHUD") != 0);
            }

            // Checks for the speedrun HUD.
            if(Speedrun == null) {
                Debug.LogError("HUDController.Start: SpeedrunHUD was not found!");
            } else {        
                // Gets initial speedrun HUD value.
                if(!PlayerPrefs.HasKey("SpeedrunHUD"))
                    PlayerPrefs.SetInt("SpeedrunHUD", 0);
                Speedrun.SetEnabled(PlayerPrefs.GetInt("SpeedrunHUD") != 0);    
            }

            // Checks for the feedback HUD.
            if(Feedback == null)        Debug.LogError("HUDController.Start: FeedbackHUD was not found!");

        }     

    }

}
