using UnityEngine;

[CreateAssetMenu(fileName = "newPrefabWarmup", menuName = "ScriptableObjects/Prefab Warmups", order = 1)]
public class PrefabWarmup : ScriptableObject
{

    [Tooltip("Prefab to be used.")]
    public GameObject prefab = null;

    [Tooltip("Amount of instances the pooling system should make avaliable at start.")]
    public int amount = 64;

}