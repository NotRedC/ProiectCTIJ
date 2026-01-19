using UnityEngine;
using TMPro;
using System.Collections;

public class TauntManager : MonoBehaviour
{
    public static TauntManager Instance;

    public TextMeshProUGUI textElement;
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 2f;
    public float displayDuration = 3f;

    void Awake()
    {
        Instance = this;
        if (canvasGroup != null) canvasGroup.alpha = 0;
    }

    public void ShowMessage(string message, TauntType type)
    {
        
        StopAllCoroutines();
        RectTransform rect = textElement.GetComponent<RectTransform>();
        if(type == TauntType.Hint)
        {
            rect.anchoredPosition = new Vector2(0, 1000);
            textElement.fontSize = 35;
        }
        else
        {
            rect.anchoredPosition = new Vector2(0, 0);
            textElement.fontSize = 40;
        }
            StartCoroutine(DisplayRoutine(message, type));
    }

    public void HideMessage()
    {
        StopAllCoroutines();
        StartCoroutine(HideRoutine());
    }

    IEnumerator DisplayRoutine(string message, TauntType type)
    {
        textElement.text = message;
        Debug.Log("Displaying message: " + message);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        if (type != TauntType.Hint)
        {
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(HideRoutine());
        }
    }

    IEnumerator HideRoutine()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed * 2;
            yield return null;
        }
    }
}