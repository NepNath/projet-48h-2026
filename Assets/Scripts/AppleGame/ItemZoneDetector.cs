using UnityEngine;

public class ItemZoneDetector : MonoBehaviour
{
    public int applesInBasket;
    private Collider2D basketCollider;

    void Start()
    {
        applesInBasket = 0;
        basketCollider = GetComponent<Collider2D>();
    }
    
    void Update()
    {
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

        if (applesInBasket >= RandomAppleSpawner.MaxAppleForGoal)
        {
            Debug.Log("Player Won");
        }
    }
}