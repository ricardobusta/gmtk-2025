using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class TitleScreenController : MonoBehaviour
    {
        [SerializeField] private PlayerUiTitle playerUiTemplate;
        [SerializeField] private Button startButton;
        [SerializeField] private CanvasGroup startCanvasGroup;

        private const int MaxPlayers = 4;
        private Array _keys;
        private readonly List<KeyCode> _players = new();
        private readonly PlayerUiTitle[] _playerUi = new PlayerUiTitle[4];

        private readonly Color[] _playerColors =
        {
            new(200f / 255f, 55f / 255f, 55f / 255f),
            new(0f / 255f, 136f / 255f, 170f / 255f),
            new(113f / 255f, 200f / 255f, 55f / 255f),
            new(255f / 255f, 212f / 255f, 42f / 255f),
        };

        private void Awake()
        {
            _keys = Enum.GetValues(typeof(KeyCode));

            for (var i = 0; i < MaxPlayers; i++)
            {
                _playerUi[i] = Instantiate(playerUiTemplate, playerUiTemplate.transform.parent);
                _playerUi[i].gameObject.name = $"Player {i + 1}";
                _playerUi[i].SetPlayer($"P{i + 1}", _playerColors[i]);
                _playerUi[i].gameObject.SetActive(false);
            }

            playerUiTemplate.gameObject.SetActive(false);
            startCanvasGroup.interactable = false;
            startCanvasGroup.alpha = 0.5f;

            startButton.onClick.AddListener(() => { SceneManager.LoadScene("Gameplay"); });
        }

        private void Update()
        {
            foreach (KeyCode keyCode in _keys)
            {
                if (!Input.GetKeyDown(keyCode))
                {
                    continue;
                }

                if (keyCode is >= KeyCode.WheelUp and <= KeyCode.Mouse6)
                {
                    // ignore mouse inputs
                    continue;
                }

                if (_players.Contains(keyCode))
                {
                    _players.Remove(keyCode);
                    RefreshPlayers();
                    continue;
                }

                if (_players.Count < MaxPlayers)
                {
                    _players.Add(keyCode);
                    RefreshPlayers();
                }
            }
        }

        private void RefreshPlayers()
        {
            for (var index = 0; index < MaxPlayers; index++)
            {
                if (index < _players.Count)
                {
                    _playerUi[index].gameObject.SetActive(true);
                    _playerUi[index].SetPlayerButton(_players[index].ToString());
                }
                else
                {
                    _playerUi[index].gameObject.SetActive(false);
                }
            }

            startCanvasGroup.interactable = _players.Count > 0;
            startCanvasGroup.alpha = _players.Count > 0 ? 1.0f : 0.5f;
        }
    }
}