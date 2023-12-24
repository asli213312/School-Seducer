using System;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public interface IObserverCustom<T>
    {
        void AddObserver(IObservableCustom<T> observable);
        void RemoveObserver(IObservableCustom<T> observable);
        void Notify();
    }
}