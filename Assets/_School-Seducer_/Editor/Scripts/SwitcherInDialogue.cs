using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class SwitcherInDialogue : MonoBehaviour
    {
        private Image _emote;
        private Image _selectedSprite;
        public void Initialize(Image emote, Image selectedSprite)
        {
            _emote = emote;
            _selectedSprite = selectedSprite;
        }

        public void SwitchEmote(Sprite emoteSprite)
        {
            if (emoteSprite == null)
            {
                Debug.LogError("This sprite is null: " + emoteSprite);
                return;
            }
            
            _emote.gameObject.Activate();
            _emote.sprite = emoteSprite;
        }

        public void SwitchSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.LogError("This sprite is null: " + sprite);
                return;
            }
            
            if (_emote.gameObject.activeSelf) _emote.gameObject.Deactivate();
            
            _selectedSprite.sprite = sprite;
        }

        public void Reset()
        {
            _emote = null;
            _selectedSprite = null;
        }
    }
}