using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public class healingBase : MonoBehaviour
    {
        [SerializeField]private HealType hType;
        [SerializeField]private int heal;

        protected virtual void OnCollisionEnter(Collision col)
        {
            GameObject hitted = col.gameObject;
            if (hitted.GetComponent(typeof(Player)) != null)
            {
                Player entity = hitted.GetComponent<Player>();
                
               entity.Heal(heal, hType);
            }

            //Destroys the object on collision.
            Destroy(gameObject);

        }
    }
}