using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class DeckManager : MonoBehaviour
    {
        public CardBoard CardBoard { get { return cardBoard; } }
        private CardBoard cardBoard;

        [SerializeField]
        private Transform deckHolder;

        [SerializeField]
        private GameObject cardUIPrefab;

        private Deck currentDeck;

        private static DeckManager instance = null;
        public static DeckManager Instance
        {
            get
            {
                if (instance == null)
                    instance = (DeckManager)FindObjectOfType(typeof(DeckManager));

                return instance;
            }
        }

        private void Awake()
        {
            cardBoard = GetComponent<CardBoard>();
        }

        public bool Initialize(List<string> cardNames)
        {
            currentDeck = new Deck();
            currentDeck.Generate(cardNames, cardUIPrefab, deckHolder);

            return cardBoard.Initialize(currentDeck);
        }

    }
}

