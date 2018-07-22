using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using StarShip.UI;

namespace StarShip
{
    public class Deck
    {
        private List<UICard> cards = new List<UICard>();

        public void Generate(List<string> cardNames, GameObject cardUIPrefab, Transform deckHolder)
        {
            foreach (var cardName in cardNames)
            {
                UICard currentUICard = GameObject.Instantiate(cardUIPrefab).GetComponent<UI.UICard>();
                currentUICard.transform.SetParent(deckHolder);
                currentUICard.transform.Reset();
                currentUICard.Initialize(cardName);
                cards.Add(currentUICard);
            }
            cards.Shuffle();

        }

        public List<UICard> Draw(int Count)
        {
            int actualCount = Mathf.Min(Count, cards.Count);

            if (actualCount <= 0)
                return null;

            List<UICard> Picked = new List<UICard>(cards.GetRange(0, actualCount));
            cards.RemoveRange(0, actualCount);

            return Picked;
        }
    }

}
