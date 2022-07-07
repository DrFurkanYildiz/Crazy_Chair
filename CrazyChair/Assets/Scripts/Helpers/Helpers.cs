using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;

    /// <summary>
    /// IEnumerator için yeni bekleme zamanı ekler.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }
    /// <summary>
    /// Mouseun UI üzerinde olup olmadığını kontrol eder.
    /// </summary>
    /// <returns></returns>
    public static bool IsOverUI()
    {
#if UNITY_EDITOR
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
#elif PLATFORM_ANDROID
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.touches[0].position };
#endif
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }
    /// <summary>
    /// UI pozisyonunu obje pozisyonuna eşitler.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera.main, out var result);
        return result;
    }
    /// <summary>
    /// Obje pozisyonunu çocukların pozisyonuna ortalar.
    /// </summary>
    /// <param name="aParent"></param>
    public static void CenterOnChildred(this Transform aParent)
    {
        var childs = aParent.Cast<Transform>().ToList();
        var pos = Vector3.zero;
        foreach (var C in childs)
        {
            pos += C.position;
            C.parent = null;
        }
        pos /= childs.Count;
        aParent.position = pos;
        foreach (var C in childs)
            C.parent = aParent;
    }
    public static int FixedValue(this int value, int min, int max)
    {
        if (value >= min && value <= max)
            return value;
        else if (value > max)
            return max;
        else if (value < min)
            return min;
        else return 1;
    }
}
