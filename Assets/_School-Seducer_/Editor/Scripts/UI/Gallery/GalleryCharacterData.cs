using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System;

namespace _School_Seducer_.Editor.Scripts.UI.Gallery
{
    [CreateAssetMenu]
    public class GalleryCharacterData : ScriptableObject
    {
        public List<GallerySlotData> AllSlots;

        public void AddSlotData(GallerySlotData slotData)
        {
            if (IsOriginalData(slotData)) AllSlots.Add(slotData);
        }

        public bool IsOriginalData(GallerySlotData data)
        {
            return AllSlots.Contains(data) == false;
        }
    }
}