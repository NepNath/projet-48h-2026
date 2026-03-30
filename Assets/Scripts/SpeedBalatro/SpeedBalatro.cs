using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpeedBalatro;

namespace SpeedBalatro
{

    public class GameManager : MonoBehaviour
    {

        [Header("Round Settings")]
        public int targetScore;
        public float roundTimeLimit;

        [Header("Current Game State")]
        public List<Card> currentHand = new();
        public List<Card> selectedCards = new();

        private int currentScore;
        private float currentTime;
        private bool gameActive;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
