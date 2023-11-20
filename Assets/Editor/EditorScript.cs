using System;
using System.IO;
using System.Linq;
using _School_Seducer_.Editor.Scripts.Chat;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EditorScript : UnityEditor.Editor
    {
	    //[MenuItem("MyMenu/Load Main ConversationData")]
        private static void LoadMainConversationData()
        {
            string selectedObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            // Получаем родительскую папку выбранного объекта
            string parentDirectory = Path.GetDirectoryName(selectedObjectPath);

            Debug.Log($"Parent directory of selected object: {parentDirectory}");

            // Получаем родительскую папку родительской папки
            string grandparentDirectory = Path.GetDirectoryName(parentDirectory);

            Debug.Log($"Grandparent directory: {grandparentDirectory}");

            // Получаем тип ConversationData
            Type conversationDataType = typeof(СonversationData);

            // Ищем активы с типом ConversationData в родительской папке родительской папки
            string[] conversationDataGuids = AssetDatabase.FindAssets($"t:{conversationDataType.Name}", new[] { grandparentDirectory });
            
            if (conversationDataGuids.Length > 0)
            {
                // Загружаем первый найденный ConversationData
                string conversationDataPath = AssetDatabase.GUIDToAssetPath(conversationDataGuids[0]);
                СonversationData mainConversationData = AssetDatabase.LoadAssetAtPath<СonversationData>(conversationDataPath);

                if (mainConversationData != null)
                {
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
        }
    }
}