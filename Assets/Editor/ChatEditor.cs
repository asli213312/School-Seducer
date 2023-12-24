using System;
using System.IO;
using System.Reflection;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(СonversationData))]
    public class ChatEditor : UnityEditor.Editor
    {
        private SerializedProperty _messagesProperty;
        private GlobalSettings _globalSettings;

        private void OnEnable()
        {
            string globalSettingsPath = Enums.Strings.GLOBAL_SETTINGS;
            
            GlobalSettings globalSettingsData = AssetDatabase.LoadAssetAtPath<GlobalSettings>(globalSettingsPath);

            if (globalSettingsData != null)
            {
                _globalSettings = globalSettingsData;
                Debug.Log("Global Settings loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to load Global Settings.");
            }

            if (target is СonversationData)
            {
                _messagesProperty = serializedObject.FindProperty("Messages");
                СonversationData conversationData = (СonversationData)target;
                conversationData.StoryTellerSprite = conversationData.Config.StoryTellerSprite;
            }
            else
            {
                Debug.LogError("Invalid target type for ChatEditor");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            if (_messagesProperty != null)
            {
                for (int i = 0; i < _messagesProperty.arraySize; i++)
                {
                    SerializedProperty messageProperty = _messagesProperty.GetArrayElementAtIndex(i);
                    Rect elementRect = EditorGUILayout.GetControlRect();

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && elementRect.Contains(Event.current.mousePosition))
                    {
                        Event.current.Use();
                    }

                    //EditorGUI.PropertyField(elementRect, messageProperty, true);
	                //EditorGUILayout.Space(170);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private void EmptyMethodCallback()
        {
            // Пустой метод
            Debug.Log("Empty Method Called");
        }
        
	    private void AddNewElement()
	    {
		    // Получаем целевой объект (ScriptableObject)
		    СonversationData scriptableObject = (СonversationData)target;

		    // Создаем временный массив с увеличенным размером
		    MessageData[] newArray = new MessageData[scriptableObject.Messages.Length + 1];

		    // Копируем существующие элементы в новый массив
		    for (int i = 0; i < scriptableObject.Messages.Length; i++)
		    {
			    newArray[i] = scriptableObject.Messages[i];
		    }

		    // Создаем новый элемент и добавляем его в конец массива
		    MessageData newElement = new MessageData();
		    newArray[newArray.Length - 1] = newElement;

		    // Присваиваем новый массив свойству Messages
		    scriptableObject.Messages = newArray;
	    }

        private void DoSomething(int selectedIndex)
        {
            Debug.Log("Doing something with element at index: " + selectedIndex);
        }

        private void HandleContextMenu(int index)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Set as LeftActor"), false, SetLeftActor, index);
            menu.AddItem(new GUIContent("Set as RightActor"), false, SetRightActor, index);
            menu.AddItem(new GUIContent("Set as StoryTeller"), false, SetStoryTeller, index);
            menu.AddItem(new GUIContent("Remove element"), false, RemoveElementAtIndex, index);
            menu.AddItem(new GUIContent("Add New Element"), false, () =>
            {
                AddNewElement();
            });

            menu.ShowAsContext();
            Event.current.Use();
        }
        
        private void SetStoryTeller(object index)
        {
            int selectedIndex = (int)index;

            MessageData msgData = FindMessage(selectedIndex);
            
            СonversationData conversationData = target as СonversationData;

            msgData.Sender = MessageSender.StoryTeller;
            msgData.ActorIcon = conversationData.Config.StoryTellerSprite;
            Debug.Log($"Sender of message {selectedIndex} was chosen as {msgData.Sender}");
        }
        
        private void SetRightActor(object index)
        {
            int selectedIndex = (int)index;

            MessageData msgData = FindMessage(selectedIndex);
            
            IСonversation conversationData = serializedObject.targetObject as IСonversation;

            msgData.Sender = MessageSender.ActorRight;
            msgData.ActorIcon = conversationData.ActorRightSprite;
            Debug.Log($"Sender of message {selectedIndex} was chosen as {msgData.Sender}");
        }

        private void SetLeftActor(object index)
        {
            int selectedIndex = (int)index;

            MessageData msgData = FindMessage(selectedIndex);
            
            IСonversation conversationData = serializedObject.targetObject as IСonversation;

            msgData.Sender = MessageSender.ActorLeft;
            msgData.ActorIcon = conversationData.ActorLeftSprite;
            Debug.Log($"Sender of message {selectedIndex} was chosen as {msgData.Sender}");
        }

        private MessageData FindMessage(object index)
        {
            int selectedIndex = (int)index;
            
            
                СonversationData conversationData = target as СonversationData;

            
                return conversationData.Messages[selectedIndex];
        }

        private void RemoveElementAtIndex(object index)
        {
            int selectedIndex = (int)index;

            if (selectedIndex >= 0 && selectedIndex < _messagesProperty.arraySize)
            {
                // Получаем SerializedProperty для элемента массива
                SerializedProperty messageProperty = _messagesProperty.GetArrayElementAtIndex(selectedIndex);

                // Освобождаем ссылку на объект
                messageProperty.objectReferenceValue = null;

                // Удаляем элемент из массива
                _messagesProperty.DeleteArrayElementAtIndex(selectedIndex);

                // Применяем изменения
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    public class ActorSettingInfo
    {
        public int Index { get; }
        public MessageSender ActorType { get; }

        public ActorSettingInfo(int index, MessageSender actorType)
        {
            Index = index;
            ActorType = actorType;
        }
    }
}