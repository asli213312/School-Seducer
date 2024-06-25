using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Kittens__Kitchen.Editor.Scripts.Utility.Extensions
{
    public static class SliderExtensions
    {
        public static IEnumerator AnimateProgression(this Slider slider, float toValue, float duration, Action onMaxValue = null)
        {
            float elapsed = 0f;
            
            while (Mathf.Approximately(slider.value, toValue))
            {
                elapsed += Time.deltaTime;
                float newValue = Mathf.Lerp(slider.value, toValue, elapsed / duration);
                slider.value = Mathf.Round(newValue);

                if (Mathf.Approximately(slider.value, slider.maxValue)) 
                {
                    onMaxValue?.Invoke();
                    yield break;
                }

                yield return null;
            }
            
            slider.value = toValue;
        }
        
        public static IEnumerator AnimateProgressionBySpeed(this Slider slider, float toValue, float speed, Action onMaxValue = null)
        {
            while (!Mathf.Approximately(slider.value, toValue))
            {
                float direction = Mathf.Sign(toValue - slider.value); // Determines if we are increasing or decreasing the value
                float newValue = slider.value + direction * speed * Time.deltaTime;
            
                // Ensure we don't overshoot the target value
                if ((direction > 0 && newValue > toValue) || (direction < 0 && newValue < toValue))
                {
                    newValue = toValue;
                }
            
                slider.value = newValue;

                if (Mathf.Approximately(slider.value, slider.maxValue))
                {
                    onMaxValue?.Invoke();
                    yield break;
                }

                yield return null;
            }
        
            slider.value = toValue;
        }
    }
}