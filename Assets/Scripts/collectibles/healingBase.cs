using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public class healingBase : MonoBehaviour
    {
        [SerializeField]private HealType hType;
        [SerializeField]private int heal;

        protected virtual void OnTriggerEnter(Collider col)
        {

            if (col.GetComponent(typeof(Player)) != null)
            {
                Player entity = col.GetComponent<Player>();
                
               entity.Heal(heal, hType);
            }

            //Destroys the object on collision.
            Destroy(gameObject);

        }
    }
}