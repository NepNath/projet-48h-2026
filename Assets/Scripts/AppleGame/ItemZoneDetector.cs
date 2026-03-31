using UnityEngine;

public class ItemZoneDetector : MonoBehaviour
{
    public int applesInBasket;
    private Collider2D basketCollider;
    private bool finished;
    private int lastApplesInBasket = 0;

    [Header("Settings")]
    public float timeLimit = 5f;
    private float timeLeft;

    void Start()
    {
        applesInBasket = 0;
        basketCollider = GetComponent<Collider2D>();
        timeLeft = timeLimit;
    }
    
    void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            Finish(false);
            return;
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            basketCollider.bounds.size,
            0f
        );

        applesInBasket = 0;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Apple"))
                applesInBasket++;
        }

        // Play basket sound when a new apple is detected in the basket
        if (applesInBasket > lastApplesInBasket)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayBasketSound();
        }
        lastApplesInBasket = applesInBasket;

        if (applesInBasket >= RandomAppleSpawner.MaxAppleForGoal)
        {
            Finish(true);
        }
    }

    void Finish(bool win)
    {
        if (finished) return;
        finished = true;
        TransitionManager.LoadScene(SceneFlow.CompleteMiniGame(win));
    }
}
