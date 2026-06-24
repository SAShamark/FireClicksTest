using System.Collections.Generic;
using Gameplay.Entities.Tower.Spells;
using UnityEngine;

namespace Gameplay.Entities.Tower
{
    [CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObjects/TowerConfig")]
    public class TowerConfig : ScriptableObject
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private List<SpellConfig> _spells = new();

        public float MaxHealth => _maxHealth;
        public List<SpellConfig> Spells => _spells;
    }
}