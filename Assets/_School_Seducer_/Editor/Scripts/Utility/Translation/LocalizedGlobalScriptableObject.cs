using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public static class LocalizedGlobalScriptableObject
    {
        private static readonly List<LocalizedScriptableObject> LocalizedObjects = new();

        public static void AddLocalizedData(LocalizedScriptableObject localizedObject)
        {
            if (IsOriginalLocalizedData(localizedObject))
            {
                LocalizedObjects.Add(localizedObject);
                Debug.Log("Localized object is added");
            }
        }

        public static void UpdateLocalizedData()
        {
            Debug.Log("Count localized objects: " + LocalizedObjects.Count);

            if (LocalizedObjects.Count == 0)
            {
                Debug.LogWarning("Count localizedObjects = 0 to update them");
                return;
            }
            
            foreach (var localizedObject in LocalizedObjects)
            {
                FieldInfo[] fields = localizedObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(string) && field.DeclaringType == localizedObject.GetType())
                    {
                        string value = (string)field.GetValue(localizedObject);
                    
                        if (value != null)
                        {
                            LocalizedScriptableObject.LocalizedField localizedField = localizedObject.LocalizedFields.Find(x => x.fieldLabel == field.Name);

                            if (localizedField != null)
                            {
                                var localizedDataForLanguage = localizedField.localizedDataList.Find(x => x.languageCode == GlobalSettings.GlobalCurrentLanguage);
                                
                                if (localizedDataForLanguage == null) continue;
                                
                                field.SetValue(localizedObject, localizedDataForLanguage.key);
                            }
                        }
                    }
                }
            }

            foreach (var localizedObject in LocalizedObjects)
            {
                Debug.Log(localizedObject.name); 
            }
            
            Debug.Log("Update localizedObjects was completed");
        }
        
        private static bool IsOriginalLocalizedData(LocalizedScriptableObject localizedObject)
        {
            return LocalizedObjects.Contains(localizedObject) == false;
        }
    }
}