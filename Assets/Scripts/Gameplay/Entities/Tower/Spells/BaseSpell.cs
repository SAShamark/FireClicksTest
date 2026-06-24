using Gameplay.Entities.Enemies;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Tower.Spells
{
    public abstract class BaseSpell : BasePoolDestroyable
    {
        protected float Speed { get; private set; }

        public abstract bool TryCast(SpellConfig config, ObjectPool pool, Transform spawnPoint,
            EnemyRegistry enemyRegistry, Camera camera);

        protected void SetSpeed(float speed)
        {
            Speed = speed;
        }
    }
}
