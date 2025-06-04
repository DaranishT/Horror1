// TextButton.cs (attach to each button)
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private TMP_Text text;

    void Start()
    {
        originalScale = transform.localScale;
        text = GetComponent<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * 1.1f;
        text.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        text.color = new Color(0.7f, 0.7f, 0.7f);
    }
}