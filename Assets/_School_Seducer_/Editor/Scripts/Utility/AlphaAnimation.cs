using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class AlphaAnimation : MonoBehaviour
    {
        public Graphic graphic;
        public float duration = 0.2f;
        public float targetAlpha = 0.5f;
        public bool startAtEnable;

        private float _startAlpha;

        private void Awake()
        {
            _startAlpha = graphic.color.a;
        }

        private void OnEnable()
        {
        	if (startAtEnable)
            	FadeAlpha();
        }

        private void OnDisable()
        {
            //ResetAlpha();
        }

        public void StartFadeAlpha()
        {
            StartCoroutine(ProcessAlpha(0, targetAlpha));
        }

        public void EndFadeAlpha()
        {
            StartCoroutine(ProcessAlpha(graphic.color.a, 0));
        }

        private void FadeAlpha()
        {
            StartCoroutine(ProcessAlpha(0, targetAlpha));
        }

        private IEnumerator ProcessAlpha(float start, float end)
        {
            float elapsedTime = 0f;
            Color color = graphic.color;
            float startAlpha = start;

            while (elapsedTime < duration)
            {
                float alpha = Mathf.Lerp(startAlpha, end, elapsedTime / duration);
                color.a = alpha;
                graphic.color = color;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            color.a = end;
            graphic.color = color;
        }

        private void ResetAlpha()
        {
            Color currentColor = graphic.color;

            graphic.color = new Color(currentColor.r, currentColor.g, currentColor.b, _startAlpha);
        }
    }
}