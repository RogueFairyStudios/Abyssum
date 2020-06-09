namespace DEEP.Entities
{

    public class DestructibleObject : EntityBase
    {

        private bool destroyed;

        protected override void Start() {

            destroyed = false;
            base.Start();

        }

        protected override void Die() {

            if(destroyed)
                return;

            destroyed = true;
            base.Die();

        }

    }
}
