using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using IEnumerator = System.Collections.IEnumerator;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoMiniGameUndress : InfoMiniGameBase
    {
        [Serializable]
        private class MiniGameUndressPointTarget
        {
            public MiniGameUndressPointType type;
            public Collider2D target;
        }

        [Serializable]
        private class MiniGameUndressPointTargetContainer
        {
            public MiniGameUndressPointTarget[] levelTargets;
        }
        
        [Header("Data")]
        [SerializeField] private InfoMiniGameDataUndress data;
        [SerializeField] private List<MiniGameUndressPointTargetContainer> pointTargetsByType;
        
        [Header("UI")]
        [SerializeField] private Image characterImage;
        [SerializeField] private Image pointBack;
        [SerializeField] private Image flashImage;
        [SerializeField] private RectTransform pointsContent;
        [SerializeField] private InfoMiniGameViewPointUndress pointPrefab;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Image timerBar;

        [Header("Options")] 
        [SerializeField] private float progressBarSpeed;
        [SerializeField] private float progressBarMaxValue;
        [SerializeField] private float progressBarMultiplier;

        private List<MiniGameUndressPointTargetContainer> PointTargets => pointTargetsByType;

        private InfoMiniGameDataUndressLevel _currentLevel;
        private InfoMiniGameViewPointUndress _currentPoint;
        private List<InfoMiniGameDataUndressPoint> _currentPointsToTouch = new();
        private List<InfoMiniGameViewPointUndress> _points = new();
        
        private InfoMiniGameDataUndressPoint _nextPointToTouch;
        private MiniGameUndressPointTargetContainer _currentPointsTarget;

        private float _timer;
        private float _nextProgressBarMinValue;
        private int _currentLevelIndex;
        private int _currentPointIndex;

        private bool _timerStarted;
        private bool _isInitialized;

        protected override void Awake() 
        {
            base.Awake();
            progressBar.onValueChanged.AddListener(OnProgressChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            progressBar.onValueChanged.RemoveListener(OnProgressChanged);
        }

        private void Update() 
        {

        }

        private void OnProgressChanged(float value)  
        {
            if (value >= 66f)
                _nextProgressBarMinValue = 66.66f;
            else if (value >= 33f)
                _nextProgressBarMinValue = 33.33f;
        }

        protected override void OnStartGame()
        {
            OnInitialize();
        }

        protected override void OnCloseGame()
        {
            ResetTimer();
        }
        
        protected override void OnComplete()
        {
            base.OnComplete();
            StartCoroutine(AnimateFlashImage(() =>
            {
                characterImage.sprite = data.finalCharacterImage;
            }));

            _points[0].transform.parent.parent.gameObject.SetActive(false);

            progressBar.gameObject.SetActive(false);
            timerBar.gameObject.SetActive(false);
        }

        protected override void OnInitialize()
        {
            InitializeLevel(0);
            progressBar.value = 0;

            if (_points.Count > 0)
                _points[0].transform.parent.parent.gameObject.SetActive(true);

            progressBar.gameObject.SetActive(true);
            timerBar.gameObject.SetActive(true);

            if (_isInitialized) return;
            
            if (_points.Count > 0) _points.Clear();

            foreach (var pointData in data.levels.SelectMany(x => x.pointsToTouch))
            {
                if (_points.Exists(x => x.Type == pointData.type)) continue;
                
                Image viewBack = Instantiate(pointBack, pointsContent);
                Image pointStatic = Instantiate(pointBack, viewBack.transform);
                pointStatic.transform.localPosition = Vector3.zero;
                pointStatic.sprite = pointData.icon;

                InfoMiniGameViewPointUndress view = CreatePoint(viewBack.transform, pointData);
                pointStatic.rectTransform.sizeDelta = view.GetComponent<RectTransform>().sizeDelta;
                
                _points.Add(view);
            }
            
            progressBar.maxValue = progressBarMaxValue;

            _isInitialized = true;
        }

        private void InitializeLevel(int index)
        {
            _currentPointIndex = 0;
            _currentLevelIndex = index;

            _currentLevel = data.levels[_currentLevelIndex];
            
            _currentPointsToTouch = _currentLevel.pointsToTouch.ToList();
            _nextPointToTouch = _currentPointsToTouch.First();
            _currentPointsTarget = PointTargets[_currentLevelIndex];

            characterImage.sprite = _currentLevel.nextImage;

            _timer = _currentLevel.timeToComplete;
        }

        private InfoMiniGameViewPointUndress CreatePoint(Transform parent, InfoMiniGameDataUndressPoint pointData)
        {
            InfoMiniGameViewPointUndress view = Instantiate(pointPrefab, parent);
            view.transform.localPosition = Vector3.zero;
            view.Render(pointData);
            view.OnClick = OnClickPoint;
            view.OnPointerUpAction = OnPointerUpPoint;

            return view;
        }

        private void OnClickPoint(InfoMiniGameViewPointUndress point)
        {
            _currentPoint = point;
            point.ImageComponent.raycastTarget = false;
            StartCoroutine(point.ImageComponent.FadeIn(0.3f));
            this.WaitForSeconds(1f, () => point.ImageComponent.raycastTarget = true);
        }

        private void OnPointerUpPoint(InfoMiniGameViewPointUndress currentPoint)
        {
            bool targetFound = false;

            foreach (var pointToTouch in _currentPointsTarget.levelTargets)
            {
                if (_currentPoint.transform.IsIntersects2D(pointToTouch.target.transform) == false) continue;

                if (_currentPoint.Type == _nextPointToTouch.type
                    && pointToTouch.type == _nextPointToTouch.type)
                {
                    OnSuccessPoint();
                    _currentPointIndex++;

                    if (_currentPointIndex < _currentPointsToTouch.Count)
                    {
                        _nextPointToTouch = _currentPointsToTouch[_currentPointIndex];
                        
                        Debug.Log("Success point!");
                    }
                    else if (_currentPointIndex >= _currentPointsToTouch.Count)
                    {
                        OnFinishedLevel();
                    }

                    targetFound = true;
                    break;
                }
            }

            if (targetFound == false)
                OnFailedPoint();

            StartCoroutine(ReturnPointProcess());
        }

        private void OnSuccessPoint()
        {
            float barValue = progressBar.value;
            float newValue = barValue + progressBar.maxValue / (_currentPointsToTouch.Count * progressBarMultiplier);
            AnimateProgressBar(newValue);
            
            if (newValue > progressBar.maxValue)
            {
                AnimateProgressBar(progressBar.maxValue);
            }

            if (_timerStarted == false) 
            {
                _timerStarted = true;
                StartCoroutine(AnimateTimerBarProcess());
            }
        }

        private void OnFailedPoint()
        {
            float barValue = progressBar.value;
            float newValue = barValue - progressBar.maxValue / (_currentPointsToTouch.Count * (progressBarMultiplier + 1));
            bool valueOverMin = false;

            if (newValue <= _nextProgressBarMinValue)
            {
                AnimateProgressBar(_nextProgressBarMinValue);
                valueOverMin = true;
            }
            
            if (valueOverMin == false)
                AnimateProgressBar(newValue);

            if (progressBar.value < 0)
            {
                AnimateProgressBar(0);
            }

            if (_currentPointIndex > 0)
            {
                _currentPointIndex--;
            }
            else _currentPointIndex = 0;
                
            _nextPointToTouch = _currentPointsToTouch[_currentPointIndex];
        }

        private IEnumerator AnimateTimerBarProcess() 
        {
            float initialTimerValue = _currentLevel.timeToComplete;
            _timer = initialTimerValue;

            while (_timer > 0 && _timerStarted) 
            {
                if (IsLevelCompleted()) 
                {
                    yield break;
                }

                _timer -= Time.deltaTime;
                timerBar.fillAmount = _timer / initialTimerValue;

                yield return null;
            }

            OnFinishTimer();
        }

        private bool IsLevelCompleted() 
        {
            if (_currentLevelIndex == _currentLevelIndex + 1) 
            {
                _timerStarted = false;
                return true;
            }

            return false;
        }

        private void OnFinishTimer() 
        {
            _timerStarted = false;
            progressBar.value = _nextProgressBarMinValue;
            timerBar.fillAmount = 1;

            Debug.Log("Invoked finish timer");
        }

        private void ResetTimer() 
        {
            StopCoroutine("AnimateTimerBarProcess");
            this.WaitForSeconds(0.09f, () => _timerStarted = false);
        }

        private void AnimateProgressBar(float newValue)
        {
            StartCoroutine(progressBar.AnimateProgressionBySpeed(newValue, progressBarSpeed));
        }

        private IEnumerator ReturnPointProcess()
        {
            StartCoroutine(_currentPoint.GetComponent<RectTransform>().DoLocalScale(new Vector3(1.5f, 1.5f, 1.5f)));
            StartCoroutine(_currentPoint.GetComponent<Image>().FadeOut(0.3f));
            
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(_currentPoint.GetComponent<Image>().Fade(0.3f, 0.5f));
            StartCoroutine(_currentPoint.GetComponent<RectTransform>().DoLocalScale(new Vector3(1f, 1f, 1f)));
            yield return new WaitForSeconds(0.2f);
            yield return _currentPoint.GetComponent<RectTransform>().DoLocalMove(_currentPoint.InitialPosition);
        }

        private void OnFinishedLevel()
        {
            _currentLevelIndex++;

            if (_currentLevelIndex <= data.levels.Length - 1) 
            {
                ResetTimer();

                InitializeLevel(_currentLevelIndex);
                StartCoroutine(AnimateFlashImage()); 
            }

            if (_currentLevelIndex == data.levels.Length) 
            {
                ResetTimer();

                OnComplete();
            }
        }

        private IEnumerator AnimateFlashImage(Action onComplete = null)
        {
            yield return flashImage.FadeIn(0.1f);
            yield return flashImage.FadeOut(0.1f);
            onComplete?.Invoke();
        }
    }
}