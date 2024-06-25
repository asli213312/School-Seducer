using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class AnimatorSequence : MonoBehaviour
    {
        [Serializable]
        public class Sequence
        {
            public Transform endPosition;
            public float speed;
            public UnityEvent onEndEvent;
            public bool IsSkipped { get; private set; }

            public void Skip() => IsSkipped = true;
        }
        
        [SerializeField] private Transform animator;
        [SerializeField] private float speed;
        [SerializeField] private Sequence[] sequences;

        private float _speed;
        private bool _isWorking;

        private void Awake()
        {
            _speed = speed;
        }

        public void InvokeAnimation()
        {
            _isWorking = true;
            StartCoroutine(Process());
        }

        public void SkipSequenceByIndex(int index)
        {
            for (int i = 0; i < sequences.Length; i++)
            {
                if (i == index)
                {
                    sequences[i].Skip();
                    Debug.Log("Skipped sequence: " + i + " in: sequencer: " + gameObject.name, gameObject);
                    break;
                }
            }
        }

        public void ChangeSequenceByIndex(Sequence newSequence, int index)
        {
            if (index >= sequences.Length) return;
            
            sequences[index] = newSequence;
        }

        public void ClearSequences()
        {
            sequences = Array.Empty<Sequence>();
        }

        public void Continue() => _isWorking = true;
        public void Stop() => _isWorking = false;

        private IEnumerator Process()
        {
            foreach (var sequence in sequences)
            {
                yield return new WaitUntil(() => _isWorking);
                
                if (sequence.IsSkipped) continue;

                if (sequence.speed > 0) _speed = sequence.speed;
                else _speed = speed;
                
                
                float t = 0;
                Vector3 startPosition = animator.transform.position;
                Vector3 targetPosition = sequence.endPosition.position;

                while (t <= 1.0f)
                {
                    t += Time.deltaTime * _speed;
                    animator.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                    yield return null;
                }

                Stop();
                sequence.onEndEvent?.Invoke();
            }
        }
    }
}