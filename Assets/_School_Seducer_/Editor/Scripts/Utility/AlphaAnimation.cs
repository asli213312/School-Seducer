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
        
        public void InvokeEndChangeAlphaRecursively(GameObject parent)
                {
                    StartCoroutine(ChangeAlphaRecursive(parent.transform, duration, 0));
                }
        
        public void InvokeStartChangeAlphaRecursively(GameObject parent)
        {
            StartCoroutine(ChangeAlphaRecursive(parent.transform, duration, targetAlpha));
        }

        private IEnumerator ChangeAlphaRecursive(Transform parentTransform, float fadeDuration, float endAlpha)
        {
            Graphic graphic = parentTransform.GetComponent<Graphic>();
            if (graphic != null)
            {
                float startAlpha = graphic.color.a;
                float rate = 1.0f / fadeDuration;
                float progress = 0.0f;

                while (progress < 1.0)
                {
                    Color color = graphic.color;
                    color.a = Mathf.Lerp(startAlpha, endAlpha, progress);
                    graphic.color = color;

                    progress += rate * Time.deltaTime;
                    yield return null;
                }

                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, endAlpha);
            }
            
            foreach (Transform child in parentTransform)
            {
                StartCoroutine(ChangeAlphaRecursive(child, fadeDuration, endAlpha));
            }
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