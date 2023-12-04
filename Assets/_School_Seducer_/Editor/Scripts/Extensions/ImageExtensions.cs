using UnityEngine;
using UnityEngine.UI;

namespace _Kittens__Kitchen.Editor.Scripts.Utility.Extensions
{
    public static class ImageExtensions
    {
        private const float PRESSED = 0.5f;
        private const float NOT_PRESSED = 1f;
        
        public static void SetNotPressed(this Image images)
        {
            SetVisible(images,NOT_PRESSED);
        }

        public static void SetPressed(this Image images)
        {
            SetVisible(images,PRESSED);
        }
        
        private static void SetVisible(this Image images, float value)
        {
            if (images.GetComponentsInChildren<Image>() != null)
            {
                foreach (var image in images.GetComponentsInChildren<Image>())
                {
                    image.color = SetColorImage(image, value);
                }
            }
            else
                images.color = SetColorImage(images, value);
        }

        private static Color SetColorImage(Image image, float alpha)
        {
            return new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }
}