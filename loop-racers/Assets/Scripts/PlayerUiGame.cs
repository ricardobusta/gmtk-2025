using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Busta.LoopRacers
{
    public class PlayerUiGame : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerLabel;
        [SerializeField] private TMP_Text keyLabelUp;
        [SerializeField] private TMP_Text keyLabelDown;
        [SerializeField] private Image buttonUp;
        [SerializeField] private Image buttonDown;
    }
}