using UnityEngine;

public static class InputHelper
{
    private static Camera _mainCamera;

    // Constructor
    private static Camera MainCamera => _mainCamera ?? (_mainCamera = Camera.main);

    public static Vector2 MouseWorldPos => MainCamera.ScreenToWorldPoint(Input.mousePosition);

    public static bool MouseOverUIObject()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    public static GameObject GetObjectUnderMouse2D(LayerMask mask)
    {
        Vector2 mouse = MouseWorldPos;
        var hit = Physics2D.GetRayIntersection(new Ray(new Vector3(mouse.x, mouse.y, -100), Vector3.forward), float.MaxValue, mask);
        if (hit.collider)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public static bool AnyOfTheseKeysDown(params KeyCode[] keys)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                return true;
            }
        }
        return false;
    }
}