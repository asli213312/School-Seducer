using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class InputExtensions
    {
        public static bool CheckTap()
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    return true;
                }

                return false;
            }
        }
    }
}