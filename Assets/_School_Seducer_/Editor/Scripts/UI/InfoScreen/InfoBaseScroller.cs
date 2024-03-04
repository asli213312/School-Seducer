using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public abstract class InfoBaseScroller : MonoBehaviour
    {
        protected abstract void Next();
        protected abstract void Previous();
    }
}