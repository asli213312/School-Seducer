using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Attributes;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using NaughtyAttributes;
using OneLine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    //[CreateAssetMenu(fileName = "BranchData", menuName = "Game/Data/Chat/BranchData", order = 0)]
    public class BranchData : LocalizedScriptableObject, IСonversation
    {
        [FormerlySerializedAs("translator")] [SerializeField, OneLine] private LocalizatorMessages localizator;
        public LocalizatorMessages Localizator {get => localizator; set => localizator = value;}
        private string _currentLanguage;
        [SerializeField] [ResizableTextArea] public string BranchName;
        public AudioClip audioMsg;
        [field: NonSerialized] public ChatConfig Config { get; set; }
        [field: ShowAssetPreview(32, 32), SerializeField] public Sprite StoryTellerSprite { get; set; }
        [field: SerializeField, ShowAssetPreview(32, 32), HorizontalGroup("Sprites", LabelWidth = 15)] public Sprite ActorRightSprite { get; set; }
        [field: SerializeField, ShowAssetPreview(32, 32), HorizontalGroup("Sprites", LabelWidth = 15)] public Sprite ActorLeftSprite { get; set; }
        
        [SerializeField] public MessageData[] Messages;
        
        #region Context Menu Commands

        [ContextMenu("Install Translation")]
        public void InstallTranslation()
        {
            localizator ??= Localizator;

            if (localizator.translationJson == null)
            {
                Debug.LogError("Translation File is not assigned!");
                return;
            }

            for (int i = 0; i < Messages.Length; i++)
            {
                Messages[i].SetLocalizedData(localizator.GetLanguages(i));
            }
        }
        
        [ContextMenu("Translate Messages")]
        public void TranslateMessages()
        {
            localizator ??= Localizator;

            if (localizator.translationJson == null)
            {
                Debug.LogError("Translation File is not assigned!");
                return;
            }

            for (int i = 0; i < Messages.Length; i++)
            {
                Messages[i].Msg = localizator.GetTranslatedMessage(GlobalSettings.GlobalCurrentLanguage, i);

                var (isTranslated, translatedAudioMsg) = Messages[i].TranslateAudioMsg(GlobalSettings.GlobalCurrentLanguage);
                Messages[i].AudioMsg = translatedAudioMsg;
            }
            
            Debug.Log("Messages translated for conversation: " + name);
        }

        [ContextMenu(nameof(ResetLanguageContainer))]
        private void ResetLanguageContainer()
        {
            localizator.ResetTranslations();
        }

        #endregion

        private void OnEnable()
        {
            GlobalSettings.LanguageChanged += InstallActuallyTranslation;
        }

        private void OnDisable()
        {
            GlobalSettings.LanguageChanged -= InstallActuallyTranslation;
        }

        private new void OnValidate()
        {
            SetActors();
            CheckBranchesIsLast();
            
            base.OnValidate();
        }

        private void InstallActuallyTranslation()
        {
            _currentLanguage = GlobalSettings.GlobalCurrentLanguage;
            InstallTranslation();
            TranslateMessages();
            
            Debug.Log("Installed new translation for conversation: " + name);
        }

        private void SetActors()
        {
            if (ActorLeftSprite == null || ActorRightSprite == null || StoryTellerSprite == null)
            {
                Debug.LogWarning("Set actor sprites in inspector!");
            }
            else
            {
                foreach (var msg in Messages)
                {
                    Sprite spriteToSet;
                    if (msg.ActorIcon == ActorLeftSprite)
                    {
                        spriteToSet = ActorLeftSprite;
                        msg.Sender = MessageSender.ActorLeft;
                    }
                    else if (msg.ActorIcon == ActorRightSprite)
                    {
                        spriteToSet = ActorRightSprite;
                        msg.Sender = MessageSender.ActorRight;
                    }
                    else
                    {
                        spriteToSet = Config.StoryTellerSprite;
                        msg.Sender = MessageSender.StoryTeller;
                    }

                    msg.ActorIcon = spriteToSet;
                }
            }
        }

        private void CheckBranchesIsLast()
        {
            for (int i = 0; i < Messages.Length; i++)
            {
                if (Messages[i].optionalData.Branches.Length > 0 && Messages[i] != Messages[^1])
                {
                    MessageData[] newArray = new MessageData[i + 1];
                    Array.Copy(Messages, newArray, i + 1);

                    Array.Resize(ref Messages, i + 1);
                    Array.Copy(newArray, Messages, i + 1);
                    Debug.LogWarning("Messages after install branches were removed");
                }
            }
        }
    }
}