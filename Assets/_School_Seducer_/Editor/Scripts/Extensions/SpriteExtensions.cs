using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Kittens__Kitchen.Editor.Scripts.Utility.Extensions
{
    public static class SpriteExtensions
    {
        public static bool IsWideSprite(this Sprite sprite)
        {
            return sprite.rect.width > sprite.rect.height;
        }
        
        public static void SetSortingOrderForAllChildrens(this SpriteRenderer sprite, int order)
        {
            foreach (var child in sprite.GetComponentsInChildren<SpriteRenderer>())
            {
                child.sortingOrder = order;
            }

            sprite.sortingOrder = order;
        }

        public static IEnumerator FadeOut(this SpriteRenderer sprite, float duration)
        {
            Color originalColor = sprite.color;
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                sprite.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                
                if (elapsedTime >= duration)
                    break;

                yield return null;
            }
            
            sprite.color = targetColor;
        }
        
        public static IEnumerator FadeOut(this Image sprite, float duration, UnityAction onComplete = null)
        {
            Color originalColor = sprite.color;
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                sprite.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                
                if (elapsedTime >= duration)
                    break;

                yield return null;
            }
            
            sprite.color = targetColor;
            onComplete?.Invoke();
        }
        
        public static IEnumerator FadeIn(this Image sprite, float duration)
        {
            Color originalColor = sprite.color;
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                sprite.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                
                if (elapsedTime >= duration)
                    break;

                yield return null;
            }
            
            sprite.color = targetColor;
        }
    }
}