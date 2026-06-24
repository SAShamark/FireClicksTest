using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Entities.Enemies
{
    public class EnemyRegistry
    {
        private readonly List<Enemy> _enemies = new();

        public IReadOnlyList<Enemy> Enemies => _enemies;
        public int Count => _enemies.Count;

        public void Register(Enemy enemy)
        {
            if (enemy != null && !_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
            }
        }

        public void Unregister(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        public Enemy GetRandomEnemy()
        {
            RemoveInvalidEnemies();
            if (_enemies.Count == 0)
            {
                return null;
            }

            return _enemies[Random.Range(0, _enemies.Count)];
        }

        public List<Enemy> GetVisibleEnemies(Camera camera)
        {
            RemoveInvalidEnemies();

            List<Enemy> result = new List<Enemy>();
            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
                if (camera == null || IsVisible(camera, enemy.AimPosition))
                {
                    result.Add(enemy);
                }
            }

            return result;
        }

        private void RemoveInvalidEnemies()
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                if (_enemies[i] == null || !_enemies[i].IsAlive)
                {
                    _enemies.RemoveAt(i);
                }
            }
        }

        private static bool IsVisible(Camera camera, Vector3 position)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(position);
            return viewportPoint is { z: > 0f, x: >= 0f } and { x: <= 1f, y: >= 0f and <= 1f };
        }
    }
}
