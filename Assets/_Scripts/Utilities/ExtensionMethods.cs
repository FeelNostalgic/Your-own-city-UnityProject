using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public static class ExtensionMethods
    {
        #region Transform

        public static void AddMoveToX(this Transform t, float moveX)
        {
            t.position = new Vector3(t.position.x + moveX, t.position.y, t.position.z);
        }

        public static void AddMoveToY(this Transform t, float moveY)
        {
            t.position = new Vector3(t.position.x, t.position.y + moveY, t.position.z);
        }

        public static void SetZ(this Transform t, float z)
        {
            t.position = new Vector3(t.position.x, t.position.y, z);
        }

        public static void SetY(this Transform t, float y)
        {
            t.position = new Vector3(t.position.x, y, t.position.z);
        }

        public static void SetZLocalRotation(this Transform t, float zRotation)
        {
            t.localRotation = Quaternion.Euler(new Vector3(t.transform.localRotation.eulerAngles.x, t.transform.localRotation.eulerAngles.y, zRotation));
        }

        public static void AddZLocalRotation(this Transform t, float zToAdd)
        {
            t.localRotation = Quaternion.Euler(new Vector3(t.transform.localRotation.eulerAngles.x, t.transform.localRotation.eulerAngles.y, t.transform.localRotation.eulerAngles.z + zToAdd));
        }

        public static void SetXLocalRotation(this Transform t, float xRotation)
        {
            t.localRotation = Quaternion.Euler(new Vector3(xRotation, t.transform.localRotation.eulerAngles.y, t.transform.localRotation.eulerAngles.z));
        }

        public static float AnglePointingTo(this Transform t, Vector3 towards)
        {
            return Mathf.Atan2(towards.y - t.position.y, towards.x - t.position.x) * Mathf.Rad2Deg;
        }

        public static float InverseAnglePointingTo(this Transform t, Vector3 towards)
        {
            return Mathf.Atan2(t.position.y - towards.y, t.position.x - towards.x) * Mathf.Rad2Deg;
        }

        public static void SetZRotation(this Transform t, float rotationZ)
        {
            var rotationEuler = t.transform.rotation.eulerAngles;
            t.transform.rotation = Quaternion.Euler(new Vector3(rotationEuler.x, rotationEuler.y, rotationZ));
        }
        
        public static void LimitX(this Transform t, float min, float max)
        {
            var newX = Mathf.Clamp(t.transform.position.x, min, max);
            t.transform.position = new Vector3(newX, t.transform.position.y, t.transform.position.z);
        }

        public static void LimitY(this Transform t, float min, float max)
        {
            var newY = Mathf.Clamp(t.transform.position.y, min, max);
            t.transform.position = new Vector3(t.transform.position.x, newY, t.transform.position.z);
        }

        public static void LimitZ(this Transform t, float min, float max)
        {
            var newZ = Mathf.Clamp(t.transform.position.z, min, max);
            t.transform.position = new Vector3(t.transform.position.x, t.transform.position.y, newZ);
        }

        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }

        #endregion

        #region Color

        public static bool CompareColor(this Color c, Color c1)
        {
            return Mathf.Approximately(c.r, c1.r) && Mathf.Approximately(c.g, c1.g) && Mathf.Approximately(c.b, c1.b);
        }

        public static Color SetAlpha(this Color c, float alpha)
        {
            var newColor = c;
            newColor.a = alpha;
            return newColor;
        }

        #endregion

        #region Image

        public static void SetAlpha(this Image image, float alpha)
        {
            var newColor = image.color.SetAlpha(alpha);
            image.color = newColor;
        }

        #endregion

        #region int

        public static string KiloFormat(this int num)
        {
            return num switch
            {
                0 => "0",
                >= 100000000 => (num / 1000000).ToString("#,0M"),
                >= 10000000 => (num / 1000000).ToString("0.#") + "M",
                >= 100000 => (num / 1000).ToString("#,0K"),
                >= 10000 => (num / 1000).ToString("0.#") + "K",
                _ => num.ToString("#,0")
            };
        }

        #endregion

        #region ICollection

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        public static bool IsNull<T>(this ICollection<T> collection)
        {
            return collection is null;
        }

        public static bool IsNotEmpty<T>(this ICollection<T> collection)
        {
            return !collection.IsEmpty();
        }

        public static bool IsNotNull<T>(this ICollection<T> collection)
        {
            return !collection.IsNull();
        }

        #endregion

        #region STRING[]

        public static bool IsEmpty(this string[] obj)
        {
            return obj is not { Length: > 0 };
        }

        public static bool IsNotEmpty(this string[] obj)
        {
            return !obj.IsEmpty();
        }

        public static bool IsNull(this string[] obj)
        {
            return obj is null;
        }

        public static bool IsNotNull(this string[] obj)
        {
            return !obj.IsNull();
        }

        #endregion

        #region OBJECT[]

        public static bool IsEmpty(this object[] obj)
        {
            return obj is not { Length: > 0 };
        }

        public static bool IsNotEmpty(this object[] obj)
        {
            return !obj.IsEmpty();
        }

        public static bool IsNull(this object[] obj)
        {
            return obj is not { Length: > 0 };
        }

        public static bool IsNotNull(this object[] obj)
        {
            return !obj.IsEmpty();
        }

        #endregion

        #region OBJECT

        public static bool IsNull(this object obj)
        {
            return obj is null;
        }

        public static bool IsNotNull(this object obj)
        {
            return !obj.IsNull();
        }

        #endregion

        #region UNITY OBJECT

        public static bool IsNull(this UnityEngine.Object obj)
        {
            return obj is null;
        }

        public static bool IsNotNull(this UnityEngine.Object obj)
        {
            return !obj.IsNull();
        }

        #endregion

        #region GameObject

        public static bool IsNull(this GameObject gb)
        {
            return gb is null;
        }

        public static bool IsNotNull(this GameObject gb)
        {
            return !gb.IsNull();
        }

        #endregion

        #region STRING

        public static bool IsNull(this string obj)
        {
            return obj is null;
        }

        public static bool IsNotNull(this string obj)
        {
            return !obj.IsNull();
        }

        public static bool IsEmpty(this string obj)
        {
            return obj is { Length: <= 0 } or "";
        }

        public static bool IsNotEmpty(this string obj)
        {
            return !obj.IsEmpty();
        }

        public static bool NotEquals(this string str1, string str2)
        {
            return !str1.Equals(str2);
        }

        #endregion

        #region Vector2

        public static Vector2 RandomNormalizedDirection(this Vector2 v2)
        {
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        public static Vector2 NormalClockwise(this Vector2 v2)
        {
            return new Vector2(v2.y, -v2.x);
        }

        public static Vector2 NormalCounterClockwise(this Vector2 v2)
        {
            return new Vector2(-v2.y, v2.x);
        }

        public static Vector2 GetVector2DirectionFromToNormalized(this Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }

        #endregion

        #region Vector3

        public static Vector3 AddXOffset(this Vector3 v3, float offset)
        {
            return new Vector3(v3.x + offset, v3.y, v3.z);
        }

        public static Vector3 AddYOffset(this Vector3 v3, float offset)
        {
            return new Vector3(v3.x, v3.y + offset, v3.z);
        }

        public static Vector3 AddZOffset(this Vector3 v3, float offset)
        {
            return new Vector3(v3.x, v3.y, v3.z + offset);
        }

        public static Vector3 GetVector3DirectionFromToNormalized(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        #endregion

        #region bool

        public static bool Not(this bool b)
        {
            return !b;
        }

        #endregion

        #region RectTransform

        public static Vector3 GetVector3X(this RectTransform rt, float newX)
        {
            return new Vector3(newX, rt.anchoredPosition3D.y, rt.anchoredPosition3D.z);
        }

        public static Vector3 GetVector3Y(this RectTransform rt, float newY)
        {
            return new Vector3(rt.anchoredPosition3D.x, newY, rt.anchoredPosition3D.z);
        }

        #endregion

        #region SpriteRenderer

        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            spriteRenderer.color = spriteRenderer.color.SetAlpha(alpha);
        }

        #endregion

        #region Button

        public static void ConfigureNewExplicitNavigation(this Button button, Selectable Right = null, Selectable Left = null, Selectable Up = null, Selectable Down = null)
        {
            var nav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = Right,
                selectOnLeft = Left,
                selectOnUp = Up,
                selectOnDown = Down
            };

            button.navigation = nav;
        }

        #endregion

        #region Selectable

        public static void ConfigureNewExplicitNavigation(this Selectable selectable, Selectable Right = null, Selectable Left = null, Selectable Up = null, Selectable Down = null)
        {
            var nav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = Right,
                selectOnLeft = Left,
                selectOnUp = Up,
                selectOnDown = Down
            };

            selectable.navigation = nav;
        }

        #endregion

        #region Dictionary

        public static void Print<T1, T2>(this Dictionary<T1, T2> dictionary)
        {
            Debug.Log($"---Dictionary<{dictionary.Types()}>---");
            foreach (var (key, value) in dictionary)
            {
                Debug.Log($"{key} : {value}");
            }
        }

        public static string Types<T1, T2>(this Dictionary<T1, T2> dictionary)
        {
            return $"{typeof(T1)},{typeof(T2)}";
        }

        #endregion

        #region TMP_Text

        public static void SetAlpha(this TMP_Text text, float alpha)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }

        #endregion
    }
}