using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerWeapon", menuName = "ScriptableObjects/Player Weapon", order = 1)]
public class PlayerWeapon : ScriptableObject
{

    public bool enabled = false;

    public string ammoId = "Bullet";

}