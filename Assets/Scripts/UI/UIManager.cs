using UI.Screens;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameplayScreen _gameplayScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;

        private BaseScreen _activeScreen;
        public GameplayScreen GameplayScreen => _gameplayScreen;
        public GameOverScreen GameOverScreen => _gameOverScreen;
        

        public void ShowGameplayScreen() => ShowScreen(_gameplayScreen);

        public void ShowGameOverScreen() => ShowScreen(_gameOverScreen);

        private void ShowScreen(BaseScreen screen)
        {
            if (_activeScreen != null)
            {
                _activeScreen.Hide();
            }

            _activeScreen = screen;
            _activeScreen.Show();
        }
    }
}