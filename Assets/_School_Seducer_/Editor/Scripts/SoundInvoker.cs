using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
    public class SoundInvoker : MonoBehaviour
    {
        [SerializeField, Range(0, 1f)] private float volume = 1f;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource ??= GetComponent<AudioSource>();
        }

        public void InvokeClip(AudioClip clip, UnityAction onComplete = null, float delay = 0)
        {
            PlayOneShot(clip, onComplete, delay);
        }

        private void PlayOneShot(AudioClip clip, UnityAction onComplete = null, float delay = 0)
        {
            if (_audioSource == null) return;

            StartCoroutine(InstallClip(clip, onComplete, delay));
        }

        private IEnumerator InstallClip(AudioClip clip, UnityAction onComplete = null, float delay = 0)
        {
            _audioSource.volume = volume;
            _audioSource.clip = clip;
            if (onComplete != null)
            {
                _audioSource.PlayOneShot(clip);
                yield return new WaitForSeconds(delay);
                onComplete.Invoke();
            }
            else if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
                _audioSource.PlayOneShot(clip);
            }
            else
                _audioSource.PlayOneShot(clip);
        }
    }
