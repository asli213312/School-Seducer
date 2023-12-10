using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class WheelFortuneData : ScriptableObject
    {
        [Header("Data")] 
        public Sprite iconMoney;
        public Sprite[] girlsSprites;

        [Header("Wheel parameters")]
        public int moneyForSpin;
        [SerializeField] public int timeShowStopButton;
        [InfoBox("If you want to show the stop button immediately, set the time to 0")]
        [Space(10)]
        [SerializeField] public float rotationSpeed;
        [ShowIf("showDebugParameters")] public float decelerationMin;
        [ShowIf("showDebugParameters")] public float decelerationMax;

        [Space(10)]
        [Header("Debug")] 
        public bool showDebugParameters;
        
        [ShowIf("showDebugParameters"), SerializeField] public GameObject[] hints;
        private GameObject[] _originalHints;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (_originalHints == null || _originalHints.Length != hints.Length)
                {
                    _originalHints = new GameObject[hints.Length];
                    for (int i = 0; i < hints.Length; i++)
                    {
                        _originalHints[i] = hints[i];
                    }
                }
                else
                {
                    for (int i = 0; i < hints.Length; i++)
                    {
                        hints[i] = _originalHints[i];
                    }
                }
            }
        }
#endif

        [Button]
        private bool ShowHintsIndex()
        {
            if (hints == null) return false;
            
            foreach (var hint in hints)
            {
                hint.Activate();

                if (hint == hints.Last())
                    return true;
            }

            return false;
        }

        [Button]
        private bool HideHintsIndex()
        {
            if (hints == null) return false;
            
            foreach (var hint in hints)
            {
                hint.Deactivate();

                if (hint == hints.Last())
                    return true;
            }

            return false;
        }
    }
}