using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerWeapon", menuName = "ScriptableObjects/Player Weapon", order = 1)]
public class PlayerWeapon : ScriptableObject
{

    [Tooltip("If the Player can use the weapon.")]
    public bool enabled = false;

    [Tooltip("The id of the ammo source to be used.")]
    public string ammoId = "Bullet";

    [Tooltip("Prefab to be used as the GameObject of the weapon.")]
    public GameObject prefab;

    [Tooltip("Icon used to represent this weapon on the HUD.")]     
    public Sprite icon;

}