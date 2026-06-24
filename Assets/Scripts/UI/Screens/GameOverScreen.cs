using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class GameOverScreen : BaseScreen
    {
        [SerializeField] private Button _restartButton;

        public event Action OnRestart;

        private void Awake()
        {
            _restartButton.onClick.AddListener(RestartClicked);
        }

        private void RestartClicked()
        {
            OnRestart?.Invoke();
        }
    }
}