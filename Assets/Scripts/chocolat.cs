using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GestionnaireChoco : MonoBehaviour
{
    [Header("Chocolat")]
    public Image imageDuBouton; 
    public List<Sprite> etapesChocolat;
    private int indexActuel = 0;

    [Header("Timer Visuel")]
    public Image imageAfficheTimer; 
    public List<Sprite> spritesTimer; 
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
            GererVisuelTimer();

            if (tempsRestant <= 0)
            {
                TerminerJeu();
            }
        }
    }

    // Vérifie bien l'ordre ici : public, puis void, puis le nom
    public void GererVisuelTimer() 
    {
        if (spritesTimer.Count < 3) return;

        float pourcentage = tempsRestant / tempsTotal;

        if (pourcentage > 0.66f) {
            imageAfficheTimer.sprite = spritesTimer[0];
        }
        else if (pourcentage > 0.33f) {
            imageAfficheTimer.sprite = spritesTimer[1];
        }
        else {
            imageAfficheTimer.sprite = spritesTimer[2];
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
            Debug.Log("Gagné !");
        }
    }

    void TerminerJeu()
    {
        jeuFini = true;
        Time.timeScale = 0f; 
        Debug.Log("GAME OVER");
    }
}