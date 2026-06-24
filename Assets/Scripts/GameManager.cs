using System;
using Gameplay.Entities.Enemies;
using Gameplay.Entities.Tower;
using Gameplay;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private TowerControl _towerControl;
    [SerializeField] private EnemySpawnManager _enemySpawnManager;

    private void Start()
    {
        _enemySpawnManager.Initialize(_config.EnemyConfigs, _config.GameplayPhaseConfig, _towerControl.transform);
        _towerControl.Initialize(_config.Tower, _enemySpawnManager.EnemyRegistry);
        _uiManager.GameplayScreen.Initialize(_config.Tower.MaxHealth);
        _uiManager.ShowGameplayScreen();
        Subscribes();
    }

    private void OnDestroy()
    {
        Unsubscribes();
    }

    private void Update()
    {
        _enemySpawnManager.Update(Time.deltaTime);
    }

    private void Subscribes()
    {
        _towerControl.OnDied += Died;
        _towerControl.OnHealthChanged += _uiManager.GameplayScreen.UpdateHealth;
        _uiManager.GameOverScreen.OnRestart += RestartGame;
    }

    private void Unsubscribes()
    {
        _towerControl.OnDied -= Died;
        _towerControl.OnHealthChanged -= _uiManager.GameplayScreen.UpdateHealth;
        _uiManager.GameOverScreen.OnRestart -= RestartGame;
    }

    private void Died()
    {
        _uiManager.ShowGameOverScreen();
        Time.timeScale = 0;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}