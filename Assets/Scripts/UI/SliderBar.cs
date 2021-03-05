using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled.UI
{
    /// <summary>
    /// UI Slider Bar component.
    /// <para>It requires a local <see cref="Slider"/> component.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public class SliderBar : MonoBehaviour
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
        /// The maximum bar value available.
        /// </summary>
        public float MaxValue
        {
            get => slider.maxValue;
            set
            {
                slider.maxValue = value;
                UpdateScoreText();
            }
        }

        /// <summary>
        /// The current bar value.
        /// </summary>
        public float CurrentValue
        {
            get => slider.value;
            set
            {
                slider.value = value;
                fillImage.color = GetCurrentGradientColor();
                UpdateScoreText();
            }
        }

        private void Reset()
        {
            slider = GetComponent<Slider>();
        }

        /// <summary>
        /// Initializes the bar using the given value as the current and maximum values.
        /// </summary>
        /// <param name="value">The value to initialize the bar.</param>
        public void Initialize(float value) => Initialize(value, value);

        /// <summary>
        /// Initializes the bar using the given current and maximum values.
        /// </summary>
        /// <param name="current">The current value to initialize the bar.</param>
        /// <param name="max">The maximum value to initialize the bar.</param>
        public void Initialize(float current, float max)
        {
            MaxValue = max;
            CurrentValue = current;
        }

        /// <summary>
        /// Updates the <see cref="info"/> text using the given text.
        /// </summary>
        /// <param name="text"></param>
        public void UpdateInfoText(string text) => info.text = text;

        /// <summary>
        /// Returns the current gradient color according with <see cref="CurrentValue"/>.
        /// </summary>
        /// <returns>Always a <see cref="Color"/>.</returns>
        public Color GetCurrentGradientColor() => gradient.Evaluate(slider.normalizedValue);

        /// <summary>
        /// Checks if this bar is fully empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() => CurrentValue < 0F || IsCurrentValue(0F);

        /// <summary>
        /// Checks if this bar is fully completed.
        /// </summary>
        /// <returns></returns>
        public bool IsComplete() => CurrentValue > MaxValue || IsCurrentValue(MaxValue);

        /// <summary>
        /// Checks if the given value is <see cref="CurrentValue"/>.
        /// </summary>
        /// <param name="value">A float value to check.</param>
        /// <returns></returns>
        public bool IsCurrentValue(float value) => Mathf.Approximately(CurrentValue, value);

        private void UpdateScoreText() => UpdateInfoText($"{CurrentValue:F0} / {MaxValue:F0}");
    }
}
