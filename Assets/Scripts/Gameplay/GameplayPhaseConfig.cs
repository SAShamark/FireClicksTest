using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "GameplayPhase", menuName = "ScriptableObjects/GameplayPhaseConfig")]
    public class GameplayPhaseConfig : ScriptableObject
    {
        [SerializeField] private List<SpawnPhase> _spawnPhases;

        public List<SpawnPhase> SpawnPhases => _spawnPhases;

        public SpawnPhase GetSpawnPhase(float elapsedTime)
        {
            SpawnPhase activePhase = _spawnPhases[0];
            for (int i = 1; i < _spawnPhases.Count; i++)
            {
                if (elapsedTime < _spawnPhases[i].StartTime)
                {
                    break;
                }

                activePhase = _spawnPhases[i];
            }

            return activePhase;
        }

        [Serializable]
        public struct SpawnPhase
        {
            [SerializeField] private float _startTime;
            [SerializeField] private float _spawnsPerSecond;

            public float StartTime => _startTime;

            public float SpawnsPerSecond => _spawnsPerSecond;
        }
    }
}