using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static Component FindComponentByInstanceID(this MonoBehaviour monoObject, int instanceID)
        {
            Component[] components = monoObject.GetComponents<Component>();
            
            return System.Array.Find(components, c => c.GetInstanceID() == instanceID);
        } 
    }
}