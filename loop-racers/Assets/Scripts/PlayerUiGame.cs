using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Busta.LoopRacers
{
    public class PlayerUiGame : MonoBehaviour
    {
        [SerializeField] private GameConfigs gameConfigs;
        [SerializeField] private TMP_Text playerLabel;
        [SerializeField] private TMP_Text positionLabel;
        [SerializeField] private TMP_Text keyLabelUp;
        [SerializeField] private TMP_Text keyLabelDown;
        [SerializeField] private Image buttonUp;
        [SerializeField] private Image buttonDown;
        [SerializeField] private Image pointer;
        [SerializeField] private Image dangerMarker;
        [SerializeField] private TMP_Text lapLabel;

        public void Init(int index, float dangerSpeed, string key, Color color)
        {
            gameObject.SetActive(true);
            playerLabel.text = $"P{index}";
            keyLabelUp.text = key;
            keyLabelDown.text = key;
            buttonUp.color = color;
            buttonDown.color = color;
            dangerMarker.fillAmount = 1 - dangerSpeed;

            UpdateButton(false);
        }

        public void UpdateButton(bool down)
        {
            buttonUp.gameObject.SetActive(!down);
            buttonDown.gameObject.SetActive(down);
        }

        public void UpdateSpeed(float speed)
        {
            pointer.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(90f, -90f, speed));
        }

        public void UpdateLaps(int current, int total)
        {
            if (current == total)
            {
                lapLabel.text = "Finished!";
                return;
            }

            lapLabel.text = $"Lap {current}/{total}";
        }

        public void UpdatePosition(int index)
        {
            positionLabel.color = gameConfigs.PositionColors[index];
            switch (index)
            {
                case 0:
                    positionLabel.SetText("1st");
                    break;
                case 1:
                    positionLabel.SetText("2nd");
                    break;
                case 2:
                    positionLabel.SetText("3rd");
                    break;
                default:
                    positionLabel.SetText($"{index + 1}th");
                    break;
            }
        }
    }
}