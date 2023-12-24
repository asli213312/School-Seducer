using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [System.Serializable]
    public class MessageData
    {
        [ShowAssetPreview(32, 32), SerializeField, Sirenix.OdinInspector.HideLabel] public Sprite ActorIcon;
        [ResizableTextArea] [NaughtyAttributes.HideIf("IsPictureMsg")] public string Msg;
        public OptionalMsgData optionalData;
        [NaughtyAttributes.HideIf("IsPictureMsg"), Sirenix.OdinInspector.HideLabel] public AudioClip AudioMsg;

        [field: NaughtyAttributes.ReadOnly] public MessageSender Sender { get; set; }
        
        [HideInEditorMode, SerializeField] private List<Translator.Languages> localizedData;

        public List<Translator.Languages> SetLocalizedData(List<Translator.Languages> localizedDataList)
        {
            return localizedData = localizedDataList;
        }

        public string TranslateMsg(string languageCode)
        {
            for (int i = 0; i < localizedData.Count; i++)
            {
                if (localizedData[i].languageCode == languageCode)
                {
                    Translator.Languages currentLanguage = localizedData[i];
                    return currentLanguage.key;
                }  
            }

            return "not translated";
        }

        public bool IsPictureMsg() => optionalData.GallerySlot != null;
    }
}