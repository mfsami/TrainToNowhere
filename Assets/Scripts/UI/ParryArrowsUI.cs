using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class ParryArrowsUI : MonoBehaviour
{
    // Fixed positions
    public RectTransform leftOuter;
    public RectTransform rightOuter;
    public RectTransform leftInner;    
    public RectTransform rightInner;

    float leftOuterStartX, rightOuterStartX;

    private void Awake()
    {
        leftOuterStartX = leftOuter.anchoredPosition.x;
        rightOuterStartX = rightOuter.anchoredPosition.x;
        
    }

    

    public void UpdateArrows(float startDist, float remainingDist)
    {
        float t = Mathf.InverseLerp(startDist, 30f, remainingDist);

        float xL = Mathf.Lerp(leftOuterStartX, leftInner.anchoredPosition.x, t);
        float xR = Mathf.Lerp(rightOuterStartX, rightInner.anchoredPosition.x,t);

        leftOuter.anchoredPosition = new Vector2(xL, leftOuter.anchoredPosition.y);
        rightOuter.anchoredPosition = new Vector2(xR, rightOuter.anchoredPosition.y);
    }
}
