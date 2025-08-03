using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Busta.LoopRacers
{
    public class PlayerUiTitle : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerLabel;
        [SerializeField] private TMP_Text keyLabel;
        [SerializeField] private Image playerImage;

        public void SetPlayer(string label, Color color)
        {
            playerImage.color = color;
            playerLabel.text = label;
        }

        public void SetPlayerButton(string player)
        {
            keyLabel.text = player;
        }
    }
}