using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Busta.LoopRacers
{
    public class TitleScreenController : MonoBehaviour
    {
        [SerializeField] private PlayerUiTitle playerUiTemplate;
        [SerializeField] private Button startButton;
        [SerializeField] private CanvasGroup startCanvasGroup;
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private Canvas creditsCanvas;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button backButton;
        
        private readonly List<KeyCode> _players = new();
        private readonly PlayerUiTitle[] _playerUi = new PlayerUiTitle[4];

        private Array _keys;

        private void Awake()
        {
            _keys = Enum.GetValues(typeof(KeyCode));

            for (var i = 0; i < Constants.MaxPlayers; i++)
            {
                _playerUi[i] = Instantiate(playerUiTemplate, playerUiTemplate.transform.parent);
                _playerUi[i].gameObject.name = $"Player {i + 1}";
                _playerUi[i].SetPlayer($"P{i + 1}", Constants.PlayerColors[i]);
                _playerUi[i].gameObject.SetActive(false);
            }

            playerUiTemplate.gameObject.SetActive(false);
            startCanvasGroup.interactable = false;
            startCanvasGroup.alpha = 0.5f;

            startButton.onClick.AddListener(StartGame);
            creditsButton.onClick.AddListener(() => ToggleScreens(false));
            backButton.onClick.AddListener(() => ToggleScreens(true));
            ToggleScreens(true);
        }

        private void Update()
        {
            foreach (KeyCode keyCode in _keys)
            {
                if (!Input.GetKeyDown(keyCode)) continue;

                if (keyCode is >= KeyCode.WheelUp and <= KeyCode.Mouse6)
                    // ignore mouse inputs
                    continue;

                if (_players.Contains(keyCode))
                {
                    _players.Remove(keyCode);
                    RefreshPlayers();
                    continue;
                }

                if (_players.Count < Constants.MaxPlayers)
                {
                    _players.Add(keyCode);
                    RefreshPlayers();
                }
            }
        }

        private void ToggleScreens(bool mainScreen)
        {
            uiCanvas.gameObject.SetActive(mainScreen);
            creditsCanvas.gameObject.SetActive(!mainScreen);
        }

        private void StartGame()
        {
            for (var i = 0; i < Constants.MaxPlayers; i++)
                if (i < _players.Count)
                    PlayerPrefs.SetInt("player" + i, (int)_players[i]);
                else
                    PlayerPrefs.DeleteKey("player" + i);

            SceneManager.LoadScene("Gameplay");
        }

        private void RefreshPlayers()
        {
            for (var index = 0; index < Constants.MaxPlayers; index++)
                if (index < _players.Count)
                {
                    _playerUi[index].gameObject.SetActive(true);
                    _playerUi[index].SetPlayerButton(_players[index].ToString());
                }
                else
                {
                    _playerUi[index].gameObject.SetActive(false);
                }

            startCanvasGroup.interactable = _players.Count > 0;
            startCanvasGroup.alpha = _players.Count > 0 ? 1.0f : 0.5f;
        }
    }
}