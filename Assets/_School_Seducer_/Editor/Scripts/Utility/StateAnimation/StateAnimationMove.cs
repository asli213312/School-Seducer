using System.Collections;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class StateAnimationMove : StateAnimationBase, IStateAnimation
    {
        private Vector3 _startPosition;

        protected override void Initialize()
        {
            _startPosition = Position;
        }

        public void Invoke()
        {
            
        }
        
        private IEnumerator DoLocalMove(RectTransform rectTransform, Vector3 position)
        {
            Vector2 startPosition = rectTransform.anchoredPosition;
            float t = Time.deltaTime;
            while (t < 0.1f)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, position, t / 0.1f);
                yield return null;
                t += Time.deltaTime;
            }
        
            rectTransform.anchoredPosition = position;
        }
    }
}