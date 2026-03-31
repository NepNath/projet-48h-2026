using UnityEngine;
using UnityEngine.UI;

public class GestionnaireMangeur : MonoBehaviour
{
    public Image spriteRenderer; 

    public Sprite spriteNormal;
    public Sprite spriteAppuye;
    public int Renderer = 1;

    [Header("Réglages Mouvement")]
    public int distanceDuPas = 500; // Distance de décalage à chaque clic
    public Vector3 direction = Vector3.right;

    public void ChangeMangeur()
    {
        if (Renderer == 1) {
            spriteRenderer.sprite = spriteAppuye;
            Renderer = 0;
        } else {
            spriteRenderer.sprite = spriteNormal;
            Renderer = 1;
        }
    }
}