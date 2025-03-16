using TMPro;
using UnityEngine;

namespace Utilities
{
    public static class ExtensionMethods
    {
        public static void LimitX(this Transform t,float min, float max)
        {
            var newX = Mathf.Clamp(t.transform.position.x, min, max);
            t.transform.position = new Vector3(newX, t.transform.position.y, t.transform.position.z);
        }
        
        public static void LimitY(this Transform t,float min, float max)
        {
            var newY = Mathf.Clamp(t.transform.position.y, min, max);
            t.transform.position = new Vector3(t.transform.position.x, newY, t.transform.position.z);
        }
        
        public static void LimitZ(this Transform t,float min, float max)
        {
            var newZ = Mathf.Clamp(t.transform.position.z, min, max);
            t.transform.position = new Vector3(t.transform.position.x, t.transform.position.y, newZ);
        }

        public static void SetAlpha(this TMP_Text text, float alpha)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
    }
}