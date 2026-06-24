using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class GameplayScreen : BaseScreen
    {
        [SerializeField] private Image _healthFill;
        [SerializeField] private TMP_Text _healthText;

        private float _maxHealth = 1f;

        public void Initialize(float maxHealth)
        {
            _maxHealth = Mathf.Max(1f, maxHealth);
            UpdateHealth(_maxHealth);
        }

        public void UpdateHealth(float health)
        {
            float normalizedHealth = Mathf.Clamp01(health / _maxHealth);
            _healthFill.fillAmount = normalizedHealth;
            _healthText.text = $"{Mathf.CeilToInt(health)} / {Mathf.CeilToInt(_maxHealth)}";
        }
    }
}