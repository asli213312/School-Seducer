using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    public class TransitionImage : Transition, ITransition
    {
        [SerializeField] private Image image;
        [SerializeField] private Image dataParent;
        private Sprite _data;

        public override void SetData(ITransitionData data)
        {
            if (data is TransitionDataSprite newData)
            {
                if (newData.fieldName != name) return;
                
                _data = newData.dataSprite;
            }
            else Debug.LogError("Data for TransitionSprite is not TransitionDataSprite", gameObject);
        }

        public override void Transit()
        {
            if (_data != null) image.sprite = _data;
            else if (dataParent != null) image.sprite = dataParent.sprite;
        }

        public override void SetDataParent(IDataParent dataRequestedParent)
        {
            if (dataRequestedParent is DataParentImage imageDataParent)
            {
                if (imageDataParent.fieldName == name)
                {
                    dataParent = imageDataParent.image;
                }
                else if (imageDataParent.fieldName == null)
                    dataParent = imageDataParent.image;
                //else
                    //Debug.LogError($"DataParentImage name is not equal to TransitionImage field name = {imageDataParent.fieldName}", imageDataParent.image.gameObject);
            }
            
            Transit();
        }
    }
}