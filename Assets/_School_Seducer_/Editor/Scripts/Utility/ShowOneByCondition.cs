using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ShowOneByCondition : MonoBehaviour
    {
        private enum ConditionType { Collection, Single}

        [SerializeField] private ConditionType conditionOneImage;
        [SerializeField] private GameObject gameObjectOne;
        [SerializeField] private GameObject gameObjectTwo;
        [SerializeField] private bool needParentObjects;

        [Header("Options")] 
        [SerializeField, ShowIf(nameof(needParentObjects))] private GameObject[] objectsToParent;
        [SerializeField, ShowIf(nameof(IsCollectionAndNeedParent))] private Transform targetsContent;
        
        [SerializeField, ShowIf(nameof(conditionOneImage), ConditionType.Collection)] private Transform content; 
        [SerializeField, ShowIf(nameof(conditionOneImage), ConditionType.Collection)] private int amountItems;

        private Transform _parentOne;
        private Transform _parentTwo;

        private void Start()
        {
            _parentOne = objectsToParent[0].transform.parent;
            _parentTwo = objectsToParent[1].transform.parent;
        }

        private void Update()
        {
            UpdateSelectedImage();
        }

        private void UpdateSelectedImage()
        {
            if (conditionOneImage == ConditionType.Collection)
            {
                if (content.childCount == amountItems)
                {
                    if (gameObjectOne.activeSelf == false) gameObjectOne.Activate();
                    if (gameObjectTwo.activeSelf) gameObjectTwo.Deactivate();
                    
                    if (needParentObjects == false) return;
                    
                    objectsToParent[0].transform.position = content.GetChild(0).transform.position;
                    objectsToParent[1].transform.position = content.GetChild(1).transform.position;
                }
                else
                {
                    if (gameObjectTwo.activeSelf == false) gameObjectTwo.Activate();
                    if (gameObjectOne.activeSelf) gameObjectOne.Deactivate();
                }
            }
        }

        private bool IsCollectionAndNeedParent() => conditionOneImage == ConditionType.Collection && needParentObjects;
    }
}