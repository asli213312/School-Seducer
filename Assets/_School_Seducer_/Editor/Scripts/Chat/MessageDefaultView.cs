using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;
using RectTransform = UnityEngine.RectTransform;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class MessageDefaultView : MessageViewBase, IMessage
    {
        [SerializeField] private Image background;
        [SerializeField] private Button audioButtonLeftActor;
        [SerializeField] private Button audioButtonRightActor;

        private List<LocalizedScriptableObject.LocalizedData> _localizedData;
        
        private Button _currentAudioButtonActor;

        public MessageSender MessageSender => Sender;
        public GameObject PaddingForward {get; private set;}
        public ContentSizeFitter ContentFitter => transform.GetComponent<ContentSizeFitter>();

        private Transform _content;
        private SoundHandler _soundHandler;
        private string _currentLanguageCode;

        public void Initialize(OptionButton[] optionButtons)
        {
            OptionButtons = optionButtons;
        }

        public bool NeedPaddings() => Sender != MessageSender.StoryTeller && Sender != MessageSender.ActorRight;

        public void SetBlockBackground(Sprite leftActor, Sprite rightActor, Sprite storyTeller)
        {
            Sprite selectedBlock = null;
            
            switch (Sender)
            {
                case MessageSender.ActorLeft: selectedBlock = leftActor; break;
                case MessageSender.ActorRight: selectedBlock = rightActor; break;
                case MessageSender.StoryTeller: selectedBlock = storyTeller; break;
            }

            background.sprite = selectedBlock;
        }

        public void SetFontColor(Color actorLeft, Color actorRight, Color storyTeller)
        {
            Color selectedColor = Color.clear;
            
            switch (Sender)
            {
                case MessageSender.ActorLeft: selectedColor = actorLeft; break;
                case MessageSender.ActorRight: selectedColor = actorRight; break;
                case MessageSender.StoryTeller: selectedColor = storyTeller; break;
            }
            
            msgText.color = selectedColor;
        }

        public void InstallLanguageCode(string languageCode) => _currentLanguageCode = languageCode;

        public void InitContentSpace(Transform content)
        {
            _content = content;
        }

        public void InitSoundInvoker(SoundHandler soundHandler)
        {
            _soundHandler = soundHandler;
        }

        public void RenderGeneralData(MessageData data, Sprite actorLeft, Sprite actorRight, Sprite storyTeller, bool needIconStoryTeller)
        {
            SetSenderMsg(actorRight, actorLeft, storyTeller, data, needIconStoryTeller);
            SetAudioButton();
            SetParentActor();
            SetOptions(data);
        }

        public void TranslateText(string languageCode)
        {
            msgText.text = Data.TranslateTextMsg(languageCode);
        }

        public void TranslateAudio(string languageCode)
        {
            var (isTranslated, translatedAudioMsg) = Data.TranslateAudioMsg(languageCode);

            if (isTranslated)
            {
                if (_currentAudioButtonActor != null)
                    _currentAudioButtonActor.gameObject.Activate();
                else
                    Debug.LogWarning("CurrentAudioButtonActor is null to change status view");
                
                Debug.Log("AudioMessage was translated: " + translatedAudioMsg.name);
            }
            else
            {
                if (_currentAudioButtonActor != null)
                    _currentAudioButtonActor.gameObject.Deactivate();
                
                if (translatedAudioMsg != null)
                    Debug.Log("AudioMessage wasn't translated: " + translatedAudioMsg.name);
            }

            _currentLanguageCode = languageCode;
            //Data.AudioMsg = translatedAudioMsg;
        }

        public void SetNameActors(string leftActor, string rightActor, string storyTeller)
        {
            SetName(leftActor, rightActor, storyTeller);
        }

        public void InstallStatusTranslatedAudioButtons()
        {
            var translatedAudio = Data.TranslateAudioMsg(_currentLanguageCode);

            if (translatedAudio.isTranslated)
            {
                if (Sender == MessageSender.ActorLeft)
                {
                    audioButtonLeftActor.gameObject.Activate();
                    audioButtonRightActor.gameObject.Deactivate();
                }
                else if (Sender == MessageSender.ActorRight)
                {
                    audioButtonRightActor.gameObject.Activate();
                    audioButtonLeftActor.gameObject.Deactivate();
                }
                else if (Sender == MessageSender.StoryTeller)
                {
                    audioButtonLeftActor.gameObject.Activate();
                    audioButtonRightActor.gameObject.Deactivate();
                }
            }
        }

        public void EnableAlignBubbleSpeech()
        {
            transform.GetComponent<ContentSizeFitter>().enabled = true;
            transform.GetComponent<VerticalLayoutGroup>().enabled = true;
        }

        private void SetParentActor()
        {
            AdjustLeftActor(_content, Sender);
            AdjustRightActor(_content, Sender);
            AdjustStoryTeller(_content, Sender);
        }

        private void SetAudioButton()
        {
            if (Data.IsPictureMsg() == false)
            {
                if (Sender == MessageSender.ActorLeft)
                {
                    audioButtonLeftActor.onClick.AddListener(InvokeAudioMsg);
                    audioButtonRightActor.gameObject.Deactivate();
                    _currentAudioButtonActor = audioButtonLeftActor;
                }
                else if (Sender == MessageSender.ActorRight)
                {
                    audioButtonRightActor.onClick.AddListener(InvokeAudioMsg);
                    //audioButtonRightActor.gameObject.Activate();
                    audioButtonLeftActor.gameObject.Deactivate();
                    _currentAudioButtonActor = audioButtonRightActor;
                }
                else if (Sender == MessageSender.StoryTeller)
                {
                    audioButtonLeftActor.onClick.AddListener(InvokeAudioMsg);
                    audioButtonRightActor.gameObject.Deactivate();
                    _currentAudioButtonActor = audioButtonLeftActor;

                    RectTransform audioLeftRect = audioButtonLeftActor.GetComponent<RectTransform>();
                    audioLeftRect.anchorMax = new Vector2(0, 0.5f);
                    audioLeftRect.anchorMin = new Vector2(0, 0.5f);
                    audioLeftRect.Translate(Vector2.left * 1.3f);
                }

                //_soundHandler.InvokeClipAfterExistClip(InvokeAudioMsg);
                if (_soundHandler.IsClipPlaying())
                {
                    _soundHandler.StopClip();
                    InvokeAudioMsg();
                }
                else
                    InvokeAudioMsg();
            }
        }

        private void InvokeAudioMsg()
        {
            //_soundHandler.AutoManagePlayback(Data.AudioMsg);

            var (isTranslated, translatedAudioMsg) = Data.TranslateAudioMsg(_currentLanguageCode);

            if (isTranslated)
            {
                if (_currentAudioButtonActor != null)
                    _currentAudioButtonActor.gameObject.Activate();
                else
                    Debug.LogWarning("CurrentAudioButtonActor is null to change status view");
                
                if (_soundHandler.IsClipPlaying())
                    _soundHandler.StopClip();
                else
                    _soundHandler.InvokeClipAfterPlayback(() =>_soundHandler.InvokeOneClip(Data.AudioMsg));
            }
            else
                _currentAudioButtonActor.gameObject.Deactivate();
        }
        
        private void AdjustStoryTeller(Transform content, MessageSender sender)
        {
            if (sender != MessageSender.StoryTeller) return;

            GameObject parent = CreateParentObject("Parent_StoryTeller", content);

            RectTransform rectParent = parent.AddComponent<RectTransform>();
            RectTransform mainRect = GetComponent<RectTransform>();
            
            parent.transform.position = gameObject.transform.position;

            rectParent.pivot = new Vector2(1, 0.5f);

            rectParent.sizeDelta = new Vector2(3.4f, 0);
            
            AlignSizeDelta alignMain = gameObject.AddComponent<AlignSizeDelta>();
            alignMain.Initialize(rectParent, gameObject.GetComponent<RectTransform>(), msgText.text.Length);

            gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;

            mainRect.pivot = new Vector2(0.5f, 1f);

            int indexMessage = gameObject.transform.GetSiblingIndex();
            parent.transform.SetSiblingIndex(indexMessage - 1);

            gameObject.transform.SetParent(parent.transform);
        }
        
        private void AdjustLeftActor(Transform content, MessageSender sender)
        {
            if (sender != MessageSender.ActorLeft) return;

            gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom = 24;
        }

        private void AdjustRightActor(Transform content, MessageSender sender)
        {
            if (sender != MessageSender.ActorRight) return;

            Debug.Log("Parent for right actor installed");

            GameObject parent = CreateParentObject("ParentRight", content);
            GameObject contentRight = CreateChildObject("ContentRight", parent.transform);
            GameObject paddingForward = CreateChildObject("Padding Forward", parent.transform);
            PaddingForward = paddingForward;

            SetSiblingIndexAlongParent(paddingForward.transform, content, parent.transform.GetSiblingIndex() + 1);

            RectTransform parentRect = parent.AddComponent<RectTransform>();
            contentRight.AddComponent<RectTransform>();
            paddingForward.AddComponent<RectTransform>();

            AlignSizeDelta alignMain = gameObject.AddComponent<AlignSizeDelta>();
            alignMain.Initialize(parentRect, gameObject.GetComponent<RectTransform>(), msgText.text.Length);
            
            VerticalLayoutGroup speechBubbleLayout = transform.GetChild(1).GetComponent<VerticalLayoutGroup>();
            speechBubbleLayout.padding.left = 18;
            speechBubbleLayout.padding.right = 30;

            SetPivotPosTopMode(leftBorderActor.GetComponent<RectTransform>());

            AdjustSizesAndPositions(parent, contentRight, paddingForward);
        }

        private GameObject CreateParentObject(string name, Transform parent)
        {
            GameObject parentObject = new GameObject(name);
            parentObject.transform.SetParent(parent);
            return parentObject;
        }

        private GameObject CreateChildObject(string name, Transform parent)
        {
            GameObject childObject = new GameObject(name);
            childObject.transform.SetParent(parent);
            return childObject;
        }

        private void SetSiblingIndexAlongParent(Transform target, Transform parent, int index)
        {
            target.SetParent(parent);
            target.SetSiblingIndex(index);
        }
        
        private void AdjustSizesAndPositionsLeft(GameObject parent, GameObject contentLeft, GameObject paddingForward)
        {
            RectTransform leftBorderRect = leftBorderActor.GetComponent<RectTransform>();
            RectTransform rightBorderRect = rightBorderActor.GetComponent<RectTransform>();
            RectTransform rectMain = gameObject.GetComponent<RectTransform>();

            //SetPivotPosTopMode(rightBorderRect);

            parent.GetComponent<RectTransform>().sizeDelta = rectMain.sizeDelta + new Vector2(0, rightBorderRect.sizeDelta.y / 24);
            contentLeft.GetComponent<RectTransform>().sizeDelta = rectMain.sizeDelta;

            leftBorderActor.transform.SetParent(contentLeft.transform);
            transform.SetParent(contentLeft.transform);
            leftBorderActor.transform.position = parent.transform.position;
            transform.position = parent.transform.position;

            leftBorderRect.Translate(Vector2.left * -5.03f);
            rectMain.position = leftBorderRect.position;

            SetStretchModeLeft(contentLeft.GetComponent<RectTransform>());

            paddingForward.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0.7f);
        }

        private void AdjustSizesAndPositions(GameObject parent, GameObject contentRight, GameObject paddingForward)
        {
            RectTransform leftBorderRect = leftBorderActor.GetComponent<RectTransform>();
            RectTransform rightBorderRect = rightBorderActor.GetComponent<RectTransform>();
            RectTransform rectMain = gameObject.GetComponent<RectTransform>();

            SetPivotPosTopMode(leftBorderRect);

            parent.GetComponent<RectTransform>().sizeDelta = rectMain.sizeDelta + new Vector2(0, leftBorderRect.sizeDelta.y / 24);
            contentRight.GetComponent<RectTransform>().sizeDelta = rectMain.sizeDelta;

            rightBorderActor.transform.SetParent(contentRight.transform);
            transform.SetParent(contentRight.transform);
            rightBorderActor.transform.position = parent.transform.position;
            transform.position = parent.transform.position;

            rightBorderRect.Translate(-Vector2.left * 7.03f);
            rectMain.position = rightBorderRect.position;

            SetStretchMode(contentRight.GetComponent<RectTransform>());

            paddingForward.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0.7f);
        }

        private void SetPivotPosTopMode(RectTransform leftBorderRect)
        {
            leftBorderRect.anchorMin = new Vector2(0f, 0f);
            leftBorderRect.anchorMax = new Vector2(1f, 1f);

            leftBorderRect.pivot = new Vector2(0.5f, 0.5f);

            leftBorderRect.sizeDelta = new Vector2(leftBorderRect.sizeDelta.x, 50f);
        }
        
        private void SetStretchModeLeft(RectTransform contentRect)
        {
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(0f, 0f);

            contentRect.pivot = new Vector2(0f, 0f);
        }

        private void SetStretchMode(RectTransform contentRightRect)
        {
            contentRightRect.anchorMin = new Vector2(0.5f, 1f);
            contentRightRect.anchorMax = new Vector2(0.5f, 1f);

            contentRightRect.pivot = new Vector2(0.5f, 1f);
        }

        private void OnDestroy()
        {
            if (Data.AudioMsg != null)
            {
                audioButtonLeftActor.RemoveListener(InvokeAudioMsg);
                audioButtonRightActor.RemoveListener(InvokeAudioMsg);
            }
        }
    }
}