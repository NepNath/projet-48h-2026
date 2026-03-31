using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class KeyCardManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public RectTransform card;
    public RectTransform reader;
    public TextMeshProUGUI resultText;

    [Header("Settings")]
    public float successDistance = 50f;
    public float timeLimit = 5f;

    private Vector2 startPosition;
    private float timeLeft;
    private bool gameActive = false;
    private bool isDragging = false;
    private Canvas canvas;
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found! KeyCardManager needs to be under a Canvas.");
            return;
        }
        if (card == null || reader == null || resultText == null)
        {
            Debug.LogError("Missing UI references in KeyCardManager!");
            return;
        }
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMiniGameMusic();
        startPosition = card.anchoredPosition;
        StartGame();
    }

    void StartGame()
    {
        CancelInvoke();
        card.anchoredPosition = startPosition;
        timeLeft = timeLimit;
        gameActive = true;
        isDragging = false;
        resultText.text = "";
    }

    void Update()
    {
        if (!gameActive) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            EndGame(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gameActive) return;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!gameActive || canvas == null) return;

        Vector2 delta = eventData.delta / canvas.scaleFactor;
        card.anchoredPosition += new Vector2(delta.x, 0);

        if (card.anchoredPosition.x < startPosition.x)
        {
            card.anchoredPosition = new Vector2(startPosition.x, card.anchoredPosition.y);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!gameActive) return;
        isDragging = false;

        float distance = Vector2.Distance(card.anchoredPosition, reader.anchoredPosition);

        if (distance <= successDistance)
        {
            EndGame(true);
        }
        else
        {
            card.anchoredPosition = startPosition;
            resultText.text = "RETRY!";
            Invoke(nameof(ClearResult), 1f);
        }
    }

    void ClearResult()
    {
        resultText.text = "";
    }

    void EndGame(bool win)
    {
        gameActive = false;
        CancelInvoke();
        resultText.text = win ? "ACCESS GRANTED" : "ACCESS DENIED";
        StartCoroutine(LoadNextScene(win));
    }

    System.Collections.IEnumerator LoadNextScene(bool win)
    {
        yield return new WaitForSeconds(0.5f);
        TransitionManager.LoadScene(SceneFlow.CompleteMiniGame(win));
    }
}
