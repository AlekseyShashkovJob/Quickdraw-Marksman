using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Game
{
    public class LevelProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;

        public void SetProgress(float progress01)
        {
            _fillImage.fillAmount = Mathf.Clamp01(progress01);
        }
    }
}