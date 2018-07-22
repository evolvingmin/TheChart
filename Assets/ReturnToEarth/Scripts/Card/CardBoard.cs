using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarShip.UI;

namespace StarShip
{
    public class CardBoard : MonoBehaviour
    {
        public enum State
        {
            None,
            Selected
        }

        private UICard Selected = null;
        private State state;

        public RectTransform preplayArea;

        [SerializeField]
        private RectTransform[] slots;

        private Deck currentDeck = null;

        [SerializeField]
        private float DrawDuration = 5.0f;

        public bool Initialize(Deck deck)
        {
            currentDeck = deck;
            List<UICard> cards = deck.Draw(4);

            for (int i = 0; i < cards.Count; i++)
                cards[i].AttachToHand(slots[i]);

            StartCoroutine(DrawCardFromDeck());
            return true;
        }

        public void Select(UICard Card)
        {
            Selected = Card;
            state = State.Selected;
        }

        public void DeSelect()
        {
            Selected = null;
            state = State.None;
        }

        public void CollectCard(UICard targetCard)
        {
            DeSelect();

            // 지금은 그냥 디스트로이 하자
            DestroyImmediate(targetCard.gameObject);
        }

        public void OnPointerEnterArea(string cardState)
        {
            if (state != State.Selected)
                return;

            UICard.State selectedState = (UICard.State)Enum.Parse(typeof(UICard.State), cardState);

            switch (selectedState)
            {
                case UICard.State.Deck:
                    break;
                case UICard.State.Hand:
                    Selected.ChangeState(UICard.State.Hand);
                    break;
                case UICard.State.Decision:
                    Selected.ChangeState(UICard.State.Decision);
                    break;
                case UICard.State.Play:
                    break;
                case UICard.State.Preplay:
                    if(Selected.CardState == UICard.State.Hand)
                        Selected.ChangeState(UICard.State.Preplay);
                    break;
            }
        }

        public void ChangeCurrentCardState(string cardState)
        {
            if (state != State.Selected)
                return;

            UICard.State selectedState = (UICard.State)Enum.Parse(typeof(UICard.State), cardState);

            Selected.ChangeState(selectedState);
        }

        private bool IsHandFull()
        {
            bool result = true;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].transform.childCount <= 0)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public RectTransform GetAvailableSlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].transform.childCount <= 0)
                {
                    return slots[i];
                }
            }
            return null;
        }

        IEnumerator DrawCardFromDeck()
        {
            while(true)
            {
                yield return new WaitForSeconds(DrawDuration);
                if (!IsHandFull())
                {
                    List<UICard> Cards = currentDeck.Draw(1);
                    if (Cards != null)
                    {
                        Cards[0].AttachToHand(GetAvailableSlot());
                    }
                }
            }
        }
    }
}

