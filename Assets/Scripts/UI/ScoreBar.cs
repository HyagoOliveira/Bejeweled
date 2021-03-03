using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled.UI
{
    /// <summary>
    /// UI Score Bar component.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public class ScoreBar : MonoBehaviour
    {
        [SerializeField, Tooltip("The local Slider component.")]
        private Slider slider;
        [SerializeField, Tooltip("The slider fill image component.")]
        private Graphic fillImage;
        [SerializeField, Tooltip("The info text component.")]
        private Text info;
        [SerializeField, Tooltip("The bar gradient color.")]
        private Gradient gradient;

        /// <summary>
        /// The maximum score available.
        /// </summary>
        public float MaxScore
        {
            get => slider.maxValue;
            set
            {
                slider.maxValue = value;
                UpdateInfoText();
            }
        }

        /// <summary>
        /// The current score.
        /// </summary>
        public float CurrentScore
        {
            get => slider.value;
            set
            {
                slider.value = value;
                fillImage.color = GetCurrentGradientColor();
                UpdateInfoText();
            }
        }

        private void Reset()
        {
            slider = GetComponent<Slider>();
        }

        /// <summary>
        /// Initializes the bar using the given score as current and maximum score values.
        /// </summary>
        /// <param name="score">The score to initialize the bar.</param>
        public void Initialize(float score) => Initialize(score, score);

        /// <summary>
        /// Initializes the bar using the given current and maximum score values.
        /// </summary>
        /// <param name="current">The current score to initialize the bar.</param>
        /// <param name="max">The maximum score to initialize the bar.</param>
        public void Initialize(float current, float max)
        {
            MaxScore = max;
            CurrentScore = current;
        }

        /// <summary>
        /// Returns the current gradient color according with <see cref="CurrentScore"/>.
        /// </summary>
        /// <returns>Always a <see cref="Color"/>.</returns>
        public Color GetCurrentGradientColor() => gradient.Evaluate(slider.normalizedValue);

        private void UpdateInfoText() => info.text = $"{CurrentScore:F0} / {MaxScore:F0}";
    }
}
