using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // <--- OBLIGATOIRE : Ajoute ça en haut !

public class GestionnaireChoco : MonoBehaviour
{
    [Header("Chocolat")]
    public Image imageDuBouton; 
    public Image BatonChocolatImage; 
    public List<Sprite> etapesChocolat;
    private int indexActuel = 0;

    [Header("Timer Texte")]
    public TextMeshProUGUI texteTimer; // On utilise le composant Pro
    public float tempsTotal = 10f; 
    private float tempsRestant;
    private bool jeuFini = false;

    void Start()
    {
        tempsRestant = tempsTotal;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!jeuFini && tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;
            
            if (texteTimer != null)
            {
                // "f1" pour n'avoir qu'un chiffre après la virgule
                texteTimer.text = tempsRestant.ToString("f1") + "s";
            }

            if (tempsRestant <= 0)
            {
                TerminerJeu();
            }
        }
    }

    public void MangerUnMorceau()
    {
        if (jeuFini) return;

        if (indexActuel < etapesChocolat.Count - 1)
        {
            indexActuel++; 
            imageDuBouton.sprite = etapesChocolat[indexActuel];
        }
        else
        {
            jeuFini = true;
            Die();
        }
    }

    void TerminerJeu()
    {
        jeuFini = true;
        if (texteTimer != null) texteTimer.text = "0.0s";
        Time.timeScale = 0f; 
    }

    void Die()
    {
        if (imageDuBouton != null) 
        {
            imageDuBouton.gameObject.SetActive(false);
            BatonChocolatImage.gameObject.SetActive(false);
        }
    }
}