using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class MonoImage : MonoBase
    {
        [Header("Component options")] 
        [SerializeField] private Image graphic;
        
        protected override void UpdateEngineObject()
        {
            if (FoundedEngineObject is Sprite imageObject) graphic.sprite = imageObject;
        }

        protected override void UpdateObject()
        {
            
        }
    }
}