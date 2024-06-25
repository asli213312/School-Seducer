using System;
using System.Collections.Generic;
using UnityEngine;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using TMPro;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class LocalizedMonoBehaviour : MonoBehaviour
    {
    	private LocalizedGlobalMonoBehaviour _localizer;


    	protected TextMeshProUGUI Host
	    {
	        get { return _host; }
	        set
	        {
	            if (_host != value)
	            {
	                _host = value;
	                if (_localizer != null && _host != null && _selectedField != null)
	                {
	                    if (_host.text != _currentKey)
	                    {
	                        _host.text = UpdateLocalizedDataById(_selectedField.fieldLabel);
	                    }

	                    UpdateHost();
	                }
	            }
	        }
	    }

    	private TextMeshProUGUI _host;
        private List<LocalizedScriptableObject.LocalizedField> localizedFieldsRuntime = new();

        private LocalizedScriptableObject.LocalizedField _selectedField;
        private string _currentKey;

        protected string UpdateLocalizedDataById(string fieldLabel) 
        {
        	if (_localizer == null) 
        	{
        		Debug.LogWarning($"<color=yellow>LOCALIZED MONO:</color> {name} can't update data, install localizer!");	
        		return "";
        	}

        	if (localizedFieldsRuntime.Count == 0) 
        	{
        		Debug.LogWarning($"<color=yellow>LOCALIZED MONO:</color> {name} can't update data, install first!");
        		return "";
        	} 

        	LocalizedScriptableObject.LocalizedField selectedField = localizedFieldsRuntime.Find(x => x.fieldLabel == fieldLabel);

        	if (selectedField == null) 
        	{
        		Debug.LogError($"<color=yellow>LOCALIZED MONO:</color> {name} can't update data, they not found by id: {fieldLabel}!");
        		return "";
        	}

        	_selectedField = selectedField;

        	LocalizedScriptableObject.LocalizedData currentLanguageData = selectedField.localizedDataList.Find(y => y.languageCode == _localizer.GlobalLanguageCodeRuntime);

        	if (currentLanguageData == null) 
        	{
        		Debug.LogError($"<color=yellow>LOCALIZED MONO:</color> {name} can't update data, data not found for current language: {_localizer.GlobalLanguageCodeRuntime}!");
        		return "";
        	}

        	_currentKey = currentLanguageData.key;
        	return currentLanguageData.key;
        }

        protected void InstallLocalizedData(LocalizedScriptableObject localizedObject) 
        {
        	localizedFieldsRuntime = localizedObject.LocalizedFields;
        }

        protected void ProvideLocalizer(LocalizedGlobalMonoBehaviour localizer) 
        {
        	_localizer = localizer;
        }

        private void UpdateHost() 
        {
        	if (_localizer.TextOptions.font != null)
        		_host.font = _localizer.TextOptions.font;
        }
    }
}