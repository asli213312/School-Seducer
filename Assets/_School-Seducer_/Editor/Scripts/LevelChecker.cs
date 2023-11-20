using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using DialogueEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class LevelChecker : MonoBehaviour
    {
        private Text _levelText; 
        private Image _image;
        private Color _initialColor;

        private const float _offset = 0.8f;

        private void Awake()
        {
            _levelText ??= GetComponentInChildren<Text>();
            _image ??= GetComponent<Image>();

            _initialColor = _image.color;
        }

        public void Enter(Character character)
        {
	        gameObject.Activate();

            transform.position = character.transform.position + new Vector3(_offset, 0);
            _levelText.text = "Level " + character.Data.RequiredLevel;

            Invoke("InvokeFadeOut", 1);
        }

        public void Exit()
        {
            InvokeExit();
        }

        private void InvokeFadeOut()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(_image.FadeOut(1f));
                Invoke("Deactivate", 0.9f);
            }
        }

        private void InvokeExit()
        {
            _image.color = _initialColor;
            gameObject.Deactivate();
        }

        private void Deactivate()
        {
            gameObject.Deactivate();
        }
    }
}