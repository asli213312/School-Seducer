using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class LocalizedScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<LocalizedField> localizedFields = new List<LocalizedField>();

        public List<LocalizedField> LocalizedFields { get => localizedFields; private set => localizedFields = value; }

        [Serializable]
        public class LocalizedData
        {
            [SerializeField] public string languageCode;
            [SerializeField] public string key;
        }

        [System.Serializable]
        public class LocalizedField
        {
            public string fieldLabel;
            public LocalizedField dampedField;
            [SerializeField] public List<LocalizedData> localizedDataList = new List<LocalizedData>();
        }

        protected void OnValidate()
        {
            Update();
        }

        private void Update()
        {
            LocalizedGlobalScriptableObject.AddLocalizedData(this);
            
            FindAndAddStringFields();
            CheckLanguageCode();
        }
        
        private void FindAndAddStringFields()
        {
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(string) && field.DeclaringType == this.GetType())
                {
                    string value = (string)field.GetValue(this);

                    // Если нашли и ключ совпадает с текущим полем, то присваиваем значение ключа
                    if (value != null)
                    {
                        // Проверяем, есть ли localizedField с тем же fieldLabel и dampedField.fieldLabel не равен fieldLabel
                        LocalizedField localizedField = localizedFields.Find(x => x.fieldLabel == field.Name);

                        // Проверяем, есть ли уже localizedField с тем же fieldLabel
                        bool fieldExists = localizedFields.Exists(x => x.fieldLabel == field.Name);

                        // Если localizedField не существует и fieldExists равен false, то добавляем новый localizedField
                        if (localizedField == null && !fieldExists)
                        {
                            Debug.Log("original localized field was founded");
                            localizedField = new LocalizedField();
                            localizedField.fieldLabel = field.Name;
                            localizedField.localizedDataList.Clear();
                            localizedFields.Add(localizedField);
                            localizedField.dampedField = localizedField;
                            localizedField.dampedField.fieldLabel = localizedField.fieldLabel;
                        }
                    }
                }
            }
        }

        private void CheckLanguageCode()
        {
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(string) && field.DeclaringType == this.GetType() 
                    )
                {
                    string value = (string)field.GetValue(this);
                    
                    if (value != null)
                    {
                        LocalizedField localizedField = localizedFields.Find(x => x.fieldLabel == field.Name);

                        if (localizedField != null)
                        {
                            var localizedDataForLanguage = localizedField.localizedDataList.Find(x => x.languageCode == GlobalSettings.GlobalCurrentLanguage);

                            if (localizedDataForLanguage == null) continue;
                            
                            if (localizedDataForLanguage.key.IsNullOrWhitespace() == false)
                                field.SetValue(this, localizedDataForLanguage.key);
                            Debug.Log("localizedDataForLanguage founded");   
                        }
                    }
                }
            }
        }
    }
}