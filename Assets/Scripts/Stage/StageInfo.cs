using UnityEngine;

namespace DEEP.Stage
{

    //
    [CreateAssetMenu(fileName = "newStageInfo", menuName = "ScriptableObjects/Stage Info", order = 1)]
    public class StageInfo : ScriptableObject
    {
        
        [Tooltip("Name of this stage to be used in the UI.")]
        public string stageName = "no name";

        [Tooltip("Name of this stage's scene.")]
        public string stageScene = "no name";

        [Tooltip("PAR time of this stage in seconds.")]
        public float stagePAR = 60.0f;

        [Tooltip("Banner used for this scene on the UI.")]     
        public Sprite banner;

        [Tooltip("This level is the final stage of a Beta build (next stage is the BetaEnd scene).")]
        public bool isBetaEnd = false;

        [Tooltip("This level is the final stage of a Beta build (next stage is the WebEnd scene).")]
        public bool isWebEnd = false;

        [Tooltip("Next stage scene name (will be ignored if isBetaEnd or isWebEnd applies)")]
        public StageInfo nextStage = null;

    }
}