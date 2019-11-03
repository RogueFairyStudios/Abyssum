using UnityEngine;
using DEEP.Weapons;

namespace DEEP.Entities{

    [RequireComponent(typeof(EnemyAISystem))]
    public class Enemy : EntityBase
    {
        
        private EnemyAISystem AI;

        void Start()
        {
            base.Start();
            AI = GetComponent<EnemyAISystem>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void Damage(int amount, DamageType type){
            Debug.Log("enemy hitted");

            if(!this.AI.search){
                AI.search = true;
                AI.hitted();
            }

            base.Damage(amount,type);
        }

        protected override void Die(){

            //Destroys the object on collision.
                Destroy(gameObject);
        }
    }
}
