namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class SystemExtensions
    {
        public static void IsNullReturn<T>(this T value)
        {
            if (value == null) return;
        }
    }
}