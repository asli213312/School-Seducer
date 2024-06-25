using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;

namespace _School_Seducer_.Editor.Scripts
{
    public abstract class InfoMiniGameDataBase : ScriptableObject
    {
        [Header("Base")] 
        [SerializeField, SerializeReference] public InfoMiniGameBase prefab;
        [HideInInspector] private int blank;

        [Header("Additional")]
        [HideInInspector] private int blank2;
    }
}