using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    public class TransitionImage : Transition, ITransition
    {
        [SerializeField] private Image image;
        private Image _dataParent;
        private Sprite _data;

        public override void SetData(ITransitionData data)
        {
            if (data is TransitionDataSprite newData)
            {
                _data = newData.dataSprite;
            }
            else Debug.LogError("Data for TransitionSprite is not TransitionDataSprite", gameObject);
        }

        public override void Transit()
        {
            if (_data != null) image.sprite = _data;
            else if (_dataParent != null) image.sprite = _dataParent.sprite;
        }

        public override void SetDataParent(IDataParent dataParent)
        {
            if (dataParent is DataParentImage imageDataParent)
            {
                if (imageDataParent.fieldName == name)
                {
                    _dataParent = imageDataParent.image;
                }
                else
                    Debug.LogError($"DataParentImage name is not equal to TransitionImage field name = {imageDataParent.fieldName}", imageDataParent.image.gameObject);
            }
        }
    }
}