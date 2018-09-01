using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



// 카드 를 드레그하는 시스템. 
// UI 가 가변이 되서는 안된다.
// 3개들어가면 3개, 내개 들어가면 4개, 
// 즉 핸드 -> 슬롯 -> 카드 이 시스템이 되어야 함.

// State의 변경은 여기서 하지 않도록 함.

namespace StarShip.UI
{
    public class UICard : MonoBehaviour
    {
        public enum State
        {
            Deck,
            Hand,
            Decision,
            Play,
            Preplay,
        }

        public State CardState {  get { return state; } }
        private State state = State.Hand;

        private CardInfo card;

        private RectTransform slot;

        public bool Initialize(string cardName/*Transform slot*/)
        {
            //this.slot = slot;
            Type elementType = Type.GetType(string.Format("StarShip.CardInfo_{0}", cardName));
            card = (CardInfo)Activator.CreateInstance(elementType);
            gameObject.SetActive(false);
            return card.Initialize();
        }

        public void AttachToHand(RectTransform slot)
        {
            this.slot = slot;
            slot.offsetMin = Vector2.zero;
            slot.offsetMax = Vector2.zero;
            transform.SetParent(slot);
            transform.Reset();
            gameObject.SetActive(true);
        }

        public void BeginDrag()
        {
            switch (state)
            {
                case State.Deck:
                    break;
                case State.Hand:
                    DeckManager.Instance.CardBoard.Select(this);
                    break;
                case State.Decision:
                    break;
                case State.Play:
                    break;
                case State.Preplay:
                    break;
            }

            Debug.Log("BeginDrag");
        }

        public void WhileDrag()
        {
            Vector2 onOverayPosition = GUIUtility.ScreenToGUIPoint(Input.mousePosition);

            switch (state)
            {
                case State.Deck:
                    break;
                case State.Hand:
                    transform.position = onOverayPosition;
                    transform.localScale = Vector3.one;
                    break;
                case State.Decision:
                    break;
                case State.Play:
                    break;
                case State.Preplay:
                    transform.position = DeckManager.Instance.CardBoard.preplayArea.position;
                    transform.localScale = Vector3.one * 2.0f;
                    break;
            }
        }

        public void EndDrag()
        {
            switch (state)
            {
                case State.Deck:
                    break;
                case State.Hand:
                    transform.position = slot.transform.position;
                    DeckManager.Instance.CardBoard.DeSelect();
                    break;
                case State.Decision:
                    break;
                case State.Play:
                    DeckManager.Instance.CardBoard.CollectCard(this);
                    break;
                case State.Preplay:
                    break;
            }
            Debug.Log("End drag");
        }
         

        public void ChangeState(State newState)
        {
            switch (newState)
            {
                case State.Deck:
                    break;
                case State.Hand:
                    card.OnHand();
                    break;
                case State.Decision:
                    card.OnDecision();
                    break;
                case State.Play:
                    card.OnPlay();
                    break;
                case State.Preplay:
                    card.OnPreplay();
                    break;
            }
            state = newState;
        }
    }
}


