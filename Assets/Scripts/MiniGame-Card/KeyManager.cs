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

    private Vector2 startPosition;
    private float timeLeft;
    private bool gameActive = false;
    private bool isDragging = false;
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        startPosition = card.anchoredPosition;
        StartGame();
    }

    void StartGame()
    {
        card.anchoredPosition = startPosition;
        timeLeft = GameSettings.timeLimit;
        gameActive = true;
        isDragging = false;
        resultText.text = "";
    }

    void Update()
    {
        if (!gameActive || !isDragging) return;

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
        if (!gameActive) return;

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
        resultText.text = win ? "ACCESS GRANTED" : "ACCESS DENIED";

        Invoke(nameof(StartGame), 2f);
    }
}