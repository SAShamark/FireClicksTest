using System.Collections.Generic;
using Gameplay.Entities.Enemies;
using Gameplay.Entities.Tower;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "Game", menuName = "ScriptableObjects/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private TowerConfig _tower;
        [SerializeField] private List<EnemyConfig> _enemyConfigs = new();
        [SerializeField] private GameplayPhaseConfig _gameplayPhaseConfig;
        
        public TowerConfig Tower => _tower;
        public List<EnemyConfig> EnemyConfigs => _enemyConfigs;
        public GameplayPhaseConfig GameplayPhaseConfig => _gameplayPhaseConfig;
       
    }
}