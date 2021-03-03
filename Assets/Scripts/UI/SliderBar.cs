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
                UpdateInfoText();
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
                UpdateInfoText();
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
        /// Returns the current gradient color according with <see cref="CurrentValue"/>.
        /// </summary>
        /// <returns>Always a <see cref="Color"/>.</returns>
        public Color GetCurrentGradientColor() => gradient.Evaluate(slider.normalizedValue);

        private void UpdateInfoText() => info.text = $"{CurrentValue:F0} / {MaxValue:F0}";
    }
}
