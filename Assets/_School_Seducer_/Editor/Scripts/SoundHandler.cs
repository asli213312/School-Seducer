using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
    public class SoundHandler : MonoBehaviour
    {
        [SerializeField, Range(0, 1f)] private float volume = 1f;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource ??= GetComponent<AudioSource>();
        }

        public bool IsClipPlaying()
        {
            if (_audioSource.clip == null) return false;
            
            return _audioSource.isPlaying;
        }
        
        public void AutoManagePlayback(AudioClip audioClip)
        {
            if (IsClipPlaying())
            {
                _audioSource.Pause();
            }
            else
            {
                if (IsClipPlaying())
                {
                    _audioSource.Pause();
                }
                else
                {
                    _audioSource.clip = audioClip;
                    _audioSource.Play();
                }
            }
        }
        
        public void UnpauseClip()
        {
            if (_audioSource.clip == null)
            {
                Debug.LogWarning("Clip is null to stop!");
                return;
            }
            
            _audioSource.UnPause();
        }

        public void PauseClip()
        {
            if (_audioSource.clip == null)
            {
                Debug.LogWarning("Clip is null to stop!");
                return;
            }
            
            _audioSource.Pause();
        }
        
        public void PlayClip()
        {
            if (_audioSource.clip == null)
            {
                Debug.LogWarning("Clip is null to stop!");
                return;
            }
            
            _audioSource.Play();
        }

        public void InvokeOneClip(AudioClip clip, UnityAction onComplete = null, float delay = 0)
        {
            PlayOneShot(clip, onComplete, delay);
        }

        public void InvokeClipAfterPlayback(UnityAction onComplete)
        {
            StartCoroutine(WaitUntilPlaybackToInvoke(onComplete));
        }

        private void PlayOneShot(AudioClip clip, UnityAction onComplete = null, float delay = 0)
        {
            if (_audioSource == null) return;

            StartCoroutine(InstallClip(clip, onComplete, delay));
        }

        private IEnumerator WaitUntilPlaybackToInvoke(UnityAction onComplete)
        {
            yield return new WaitUntil(() => _audioSource.isPlaying == false);
            onComplete?.Invoke();
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
