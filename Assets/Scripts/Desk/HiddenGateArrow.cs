using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Viewport
{
    FullScreen,
    Left,
    UpperLeft
}

public class HiddenGateArrow : MonoBehaviour
{
    public static HiddenGateArrow Instance { get; private set; }

    public GameObject arrow;
    public float borderSize;

    GameObject target;
    Camera camera;
    RectTransform viewportRectTransform;
    RectTransform arrowRectTransform;

    (float left, float right, float top, float bottom) viewportBorders;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            viewportRectTransform = gameObject.GetComponent<RectTransform>();
            arrowRectTransform = arrow.GetComponent<RectTransform>();
            camera = Camera.main;
        }
    }

    void Start()
    {
        SetViewport(Viewport.Left);    
    }

    void Update()
    {
        Vector3 targetScreenPosition = camera.WorldToScreenPoint(target.transform.position);

        bool isOffScreen = IsOffScreen(targetScreenPosition);
        if (isOffScreen)
        {
            CalculateArrowPosition(targetScreenPosition);
            RotatePointer(targetScreenPosition);
        }

        arrow.SetActive(isOffScreen);
    }

    public void Activate(GameObject baseGate)
    {
        target = baseGate;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    //Is not used yet -> should be done in new UIManager
    public void SetViewport(Viewport viewport)
    {
        switch (viewport)
        {
            case (Viewport.FullScreen):
                {
                    viewportRectTransform.offsetMax = new Vector2(0, 0);
                    viewportRectTransform.offsetMin = new Vector2(0, 0);
                    break;
                }
            case (Viewport.Left):
                {
                    viewportRectTransform.offsetMax = new Vector2(-550, 0);
                    viewportRectTransform.offsetMin = new Vector2(0, 0);
                    break;
                }
            case (Viewport.UpperLeft):
                {
                    viewportRectTransform.offsetMax = new Vector2(-550, 0);
                    viewportRectTransform.offsetMin = new Vector2(0, 300);
                    break;
                }
        }

        viewportBorders = CalculateViewportBorders();
    }

    private bool IsOffScreen(Vector3 targetScreenPosition)
    {
        return  viewportBorders.left > targetScreenPosition.x ||
                viewportBorders.right < targetScreenPosition.x ||
                viewportBorders.top < targetScreenPosition.y ||
                viewportBorders.bottom > targetScreenPosition.y;
    }

    private void RotatePointer(Vector3 targetScreenPosition)
    {
        Vector3 direction = (targetScreenPosition - arrowRectTransform.position).normalized;
        float rad = Mathf.Acos(Vector3.Dot(direction, new Vector3(1, 0, 0)));

        if (direction.y < 0)
        {
            rad *= -1;
        }
        else if (direction.y == 0)
        {
            rad = direction.x > 0 ? 0 : Mathf.PI;
        }

        arrowRectTransform.localEulerAngles = new Vector3(0, 0, (rad / (2 * Mathf.PI)) * 360);
    }

    private (float left, float right, float top, float bottom) CalculateViewportBorders()
    {
        float left = viewportRectTransform.position.x - viewportRectTransform.rect.width / 2;
        float right = viewportRectTransform.position.x + viewportRectTransform.rect.width / 2;
        float top = viewportRectTransform.position.y + viewportRectTransform.rect.height / 2;
        float bottom = viewportRectTransform.position.y - viewportRectTransform.rect.height / 2;
        return (left, right, top, bottom);
    }

    private void CalculateArrowPosition(Vector3 targetScreenPosition)
    {
        Vector3 arrowScreenPosition = targetScreenPosition;
        if (arrowScreenPosition.x <= viewportBorders.left + borderSize) arrowScreenPosition.x = viewportBorders.left + borderSize;
        if (arrowScreenPosition.x >= viewportBorders.right - borderSize) arrowScreenPosition.x = viewportBorders.right - borderSize;
        if (arrowScreenPosition.y <= viewportBorders.bottom + borderSize) arrowScreenPosition.y = viewportBorders.bottom + borderSize;
        if (arrowScreenPosition.y >= viewportBorders.top - borderSize) arrowScreenPosition.y = viewportBorders.top - borderSize;

        arrowRectTransform.position = arrowScreenPosition;
    }
}