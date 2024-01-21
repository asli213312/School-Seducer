using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ChooseOneImage : MonoBehaviour
    {
        private enum ConditionType { Collection, Single}

        [SerializeField] private ConditionType conditionOneImage;
        [SerializeField] private GameObject gameObjectOne;
        [SerializeField] private GameObject gameObjectTwo;

        [Header("Options")] 
        [SerializeField, ShowIf(nameof(conditionOneImage), ConditionType.Collection)] private Transform content; 
        [SerializeField, ShowIf(nameof(conditionOneImage), ConditionType.Collection)] private int amountItems;

        private void Update()
        {
            UpdateSelectedImage();
        }

        private void UpdateSelectedImage()
        {
            if (conditionOneImage == ConditionType.Collection)
            {
                if (content.childCount == amountItems) gameObjectOne.Activate();
                else gameObjectTwo.Activate();
            }
        }
    }
}