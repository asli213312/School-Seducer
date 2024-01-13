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

        [SerializeField] private List<Translator.LanguageAudioClip> localizedAudioClips;
        private List<Translator.Languages> _localizedData;

        public List<Translator.Languages> SetLocalizedData(List<Translator.Languages> localizedDataList)
        {
            return _localizedData = localizedDataList;
        }

        public (bool isTranslated, AudioClip translatedAudio) TranslateAudioMsg(string languageCode)
        {
            if (localizedAudioClips.Count == 0) return (false, AudioMsg);
            
            foreach (var localizedAudioClip in localizedAudioClips)
            {
                if (localizedAudioClip.languageCode == languageCode)
                {
                    Translator.LanguageAudioClip currentAudioClip = localizedAudioClip;
                    return (true, currentAudioClip.key);
                }
            }

            return (false, AudioMsg);
        }

        public string TranslateTextMsg(string languageCode)
        {
            if (_localizedData.Count == 0) return Msg;
            
            for (int i = 0; i < _localizedData.Count; i++)
            {
                if (_localizedData[i].languageCode == languageCode)
                {
                    Translator.Languages currentLanguage = _localizedData[i];
                    return currentLanguage.key;
                }  
            }

            return Msg;
        }

        public bool IsPictureMsg() => optionalData.GallerySlot != null;
    }
}