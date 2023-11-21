using System;
using System.IO;
using _School_Seducer_.Editor.Scripts.Chat;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BranchData))]
    public class BranchEditor : UnityEditor.Editor
    {
        private SerializedProperty _messagesProperty;
        private СonversationData _mainData;
        
        private void OnEnable()
        {
            string selectedObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            
            string parentDirectory = Path.GetDirectoryName(selectedObjectPath);

            Debug.Log($"Parent directory of selected object: {parentDirectory}");
            
            string grandparentDirectory = Path.GetDirectoryName(parentDirectory);

            Debug.Log($"Grandparent directory: {grandparentDirectory}");
            
            Type conversationDataType = typeof(СonversationData);
            
            string[] conversationDataGuids = AssetDatabase.FindAssets($"t:{conversationDataType.Name}", new[] { grandparentDirectory });
            
            if (conversationDataGuids.Length > 0)
            {
                string conversationDataPath = AssetDatabase.GUIDToAssetPath(conversationDataGuids[0]);
                СonversationData mainConversationData = AssetDatabase.LoadAssetAtPath<СonversationData>(conversationDataPath);

                if (mainConversationData != null)
                {
                    _mainData = mainConversationData;
                    Debug.Log("Main ConversationData loaded successfully.");
                }
                else
                {
                    Debug.LogError("Failed to load Main ConversationData.");
                }
            }
            else
            {
                Debug.LogError("ConversationData not found in the selected directory.");
            }
            
            if (target is BranchData)
            {
                _messagesProperty = serializedObject.FindProperty("Messages");
            }
            else
            {
                Debug.LogError("Invalid target type for BranchEditor");
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
                        // Правая кнопка мыши была нажата над элементом массива
                        HandleContextMenu(i);
                        Event.current.Use(); // Помечаем событие как обработанное
                    }

                    EditorGUI.PropertyField(elementRect, messageProperty, true);
	                EditorGUILayout.Space(170);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private void HandleContextMenu(int index)
        {
		    GenericMenu menu = new GenericMenu();
		
		    menu.AddItem(new GUIContent("Set as LeftActor"), false, SetLeftActor, index);
		    menu.AddItem(new GUIContent("Set as RightActor"), false, SetRightActor, index);
		    menu.AddItem(new GUIContent("Set as StoryTeller"), false, SetStoryTeller, index);
	        menu.AddItem(new GUIContent("Remove Selected Element"), false, RemoveElementAtIndex, index);
	        menu.AddItem(new GUIContent("Add New Element"), false, () => 
	        {
	        	AddNewElement();
	        });
		
		    menu.ShowAsContext();
		    Event.current.Use();
        }
        
	    private void AddNewElement()
	    {
            BranchData scriptableObject = (BranchData)target;
            
		    MessageData[] newArray = new MessageData[scriptableObject.Messages.Length + 1];
            
		    for (int i = 0; i < scriptableObject.Messages.Length; i++)
		    {
			    newArray[i] = scriptableObject.Messages[i];
		    }
            
		    MessageData newElement = new MessageData();
		    newArray[newArray.Length - 1] = newElement;
            
		    scriptableObject.Messages = newArray;
	    }

        private void SetStoryTeller(object index)
        {
            int selectedIndex = (int)index;

            MessageData msgData = FindMessage(selectedIndex);
            
	        СonversationData conversationData = _mainData;

            msgData.Sender = MessageSender.StoryTeller;
	        msgData.ActorIcon = conversationData.Config.StoryTellerSprite;
            Debug.Log($"Sender of message {selectedIndex} was chosen as {msgData.Sender}");
        }

        private void SetRightActor(object index)
        {
            int selectedIndex = (int)index;

            MessageData msgData = FindMessage(selectedIndex);

            СonversationData conversationData = _mainData;

            msgData.Sender = MessageSender.ActorRight;
            msgData.ActorIcon = conversationData.ActorRightSprite;
            Debug.Log($"Sender of message {selectedIndex} was chosen as {msgData.Sender}");
        }

        private void SetLeftActor(object index)
        {
            int selectedIndex = (int)index;

            MessageData msgData = FindMessage(selectedIndex);
            
            СonversationData conversationData = _mainData;

            msgData.Sender = MessageSender.ActorLeft;
            msgData.ActorIcon = conversationData.ActorLeftSprite;
            Debug.Log($"Sender of message {selectedIndex} was chosen as {msgData.Sender}");
        }

        private MessageData FindMessage(object index)
        {
            int selectedIndex = (int)index;
            
            if (selectedIndex >= 0 && selectedIndex < _messagesProperty.arraySize)
            {
	            BranchData branchData = target as BranchData;

                return branchData.Messages[selectedIndex];
            }
            
            return null;
        }

        private void RemoveElementAtIndex(object index)
        {
            int selectedIndex = (int)index;

            if (selectedIndex >= 0 && selectedIndex < _messagesProperty.arraySize)
            {
                SerializedProperty messageProperty = _messagesProperty.GetArrayElementAtIndex(selectedIndex);

                messageProperty.objectReferenceValue = null;

                _messagesProperty.DeleteArrayElementAtIndex(selectedIndex);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}