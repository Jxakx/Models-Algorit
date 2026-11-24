using UnityEngine;
using UnityEngine.UI;

namespace ArenaSurvivor.UI
{
    public class LevelUpButtonUI : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text titleText;

        public Button Button => button;

        public void SetTitle(string title)
        {
            titleText.text = title;
        }
    }
}
