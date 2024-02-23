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
        public System.Collections.Generic.List<WheelSlotData> characters;

        [Header("Wheel parameters")]
        public int moneyForSpin;
        [SerializeField] public int timeShowStopButton;
        [InfoBox("If you want to show the stop button immediately, set the time to 0")]
        [SerializeField] public int secondsSlowSpdSlots;
        
        [Space(10)]
        [ShowIf(nameof(showDebugParameters)), SerializeField] public float rotationSpeedCharacters;
        [ShowIf("showDebugParameters"), SerializeField] public float rotationSpeed;
        [ShowIf("showDebugParameters"), SerializeField] public float rotationSlowSpeed;
        [ShowIf("showDebugParameters"), SerializeField] public float decelerationCharactersWheel;
        [ShowIf("showDebugParameters")] public float decelerationMin;
        [ShowIf("showDebugParameters")] public float decelerationMax;

        [Space(10)]
        [Header("Debug")] 
        public bool showDebugParameters;
        
        [ShowIf("showDebugParameters"), SerializeField] public GameObject[] hints;
        private GameObject[] _originalHints;

        public bool CanSpin(int moneyPlayer, float multiplier = 1f)
        {
             return moneyPlayer >= moneyForSpin * multiplier;
        }

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

            foreach (var character in characters)
            {
                if (character.useTest == false)
                {
                    if (character.slotType != WheelCategorySlotEnum.Character)
                        characters.Remove(character);
                }
                else
                    if (character.slotTypeTest != WheelCategorySlotEnum.Character)
                        characters.Remove(character);
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