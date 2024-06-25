using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace _School_Seducer_.Editor.Scripts
{
    public abstract class InfoMiniGameBase : MonoBehaviour
    {
        [Header("Base")] 
        [SerializeField] private Vector3 offsetPosition;
        [SerializeField] protected Button closeButton;

        [Header("Additional")] 
        [SerializeField, HideInInspector] private bool blank;
        
        public event Action StartGameAction;
        public event Action CloseGameAction;
        public event Action CompleteGameAction;

        protected Transform SpawnPoint;
        
        protected virtual void Awake()
        {
            closeButton.AddListener(CloseGame);
        }

        protected virtual void OnDestroy()
        {
            closeButton.RemoveListener(CloseGame);
        }

        private void Start()
        {
            transform.SetAsLastSibling();
        }
        
        public void Initialize(Transform spawnPoint)
        {
            SpawnPoint = spawnPoint;
            transform.localScale = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.position = Vector3.zero + offsetPosition;
            
            OnInitialize();
        }

        public void StartGame()
        {
            StartCoroutine(
                transform.DoLocalScaleAndUnscale
                    (this, Vector3.one, false, 0, () =>
                    {
                        OnStartGame();
                        StartGameAction?.Invoke();
                    }));
        }

        private void CloseGame()
        {
            StartCoroutine(
                transform.DoLocalScaleAndUnscale
                    (this, Vector3.zero, false, 0, () =>
                    {
                        OnCloseGame();
                        CloseGameAction?.Invoke();
                    }));
        }

        protected virtual void OnComplete()
        {
            CompleteGameAction?.Invoke();
        }
        protected abstract void OnInitialize();
        protected abstract void OnStartGame();
        protected abstract void OnCloseGame();
    }
}