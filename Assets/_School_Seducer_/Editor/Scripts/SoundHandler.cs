using System.Collections;
using _School_Seducer_.Editor.Scripts.Tests;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
    public class SoundHandler : MonoBehaviour
    {
        [SerializeField, Range(0, 1f)] private float volume = 1f;

        private AudioSource _audioSource;
        private AudioClip _clipInQueue;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Mute() => _audioSource.mute = true;
        public void Unmute() => _audioSource.mute = false;

        public void StartControlStopClip() => StartCoroutine(ControlToStopClip());
        public void StopControlStopClip() => StopCoroutine(ControlToStopClip());

        public bool TrySetQueueClip(AudioClip clipInQueue)
        {
            if (IsClipPlaying()) _clipInQueue = clipInQueue; return true;

            Debug.LogWarning("SoundHandler: Can't set clip in queue because current clip is not playing");
            return false;
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
            if (IsClipNull("Clip is null to Unpause")) return;
            
            _audioSource.UnPause();
        }

        public void PauseClip()
        {
            if (IsClipNull("Clip is null to Pause")) return;
            
            _audioSource.Pause();
        }

        public void StopClip()
        {
            if (IsClipNull("Clip is null to Stop")) return;
            
            _audioSource.Stop();
        }
        
        public void PlayClip()
        {
            if (IsClipNull("Clip is null to Play")) return;

            _audioSource.Play();
        }

        public void InvokeQueueClip(UnityAction onComplete = null, float delay = 0)
        {
            if (_clipInQueue == null) return;
            
            PlayOneShot(_clipInQueue, onComplete, delay);
        }

        public void InvokeOneClip(AudioClip clip, UnityAction onComplete = null, float delay = 0)
        {
            PlayOneShot(clip, onComplete, delay);
        }

        public void InvokeClipAfterPlayback(UnityAction onComplete)
        {
            StartCoroutine(WaitUntilPlaybackToInvoke(onComplete));
        }

        public void InvokeClipAfterExistClip(UnityAction onComplete)
        {
            StartCoroutine(WaitPlayedClipToInvoke(onComplete));
        }

        private void PlayOneShot(AudioClip clip, UnityAction onComplete = null, float delay = 0)
        {
            if (_audioSource == null) return;

            StartCoroutine(InstallClip(clip, onComplete, delay));
        }
        
        private IEnumerator WaitPlayedClipToInvoke(UnityAction onComplete)
        {
            yield return new WaitWhile(IsClipPlaying);
            onComplete?.Invoke();
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

        private IEnumerator ControlToStopClip()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            yield return null;
        }

        private bool IsClipNull(string warningMessage)
        {
            if (_audioSource.clip != null) return false;
            Debug.LogWarning(warningMessage);
            return true;
        }
    }
