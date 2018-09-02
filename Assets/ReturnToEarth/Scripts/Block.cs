using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReturnToEarth
{
    public class Block : MonoBehaviour
    {
        public enum BlockState
        { 
            Default,
            Over,
            Selected,
            None
        }

        private BoardManager boardManager;

        private Vector2 index;

        private Vector3 location;
        private Image image;

        public Rect Rect { get; private set; }
        private BlockState state = BlockState.None;


        private Unit placed;
        public BlockState State
        {
            get
            {
                return state;
            }

            set
            {
                UpdateState(value);
            }
        }

        
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public Define.Result Initialize(BoardManager boardManager, Vector2 _index, Vector3 _location, Vector3 _scale)
        {
            this.boardManager = boardManager;
            image = GetComponent<Image>();
            index = _index;
            transform.Reset();
            location = _location;

            float x = location.x - _scale.x / 2.0f;
            float y = location.y + _scale.y / 2.0f;

            Rect = new Rect(x, y, rectTransform.sizeDelta.x* _scale.x, rectTransform.sizeDelta.y * _scale.y);
            
            transform.position = location;
            transform.localScale = _scale;

            State = BlockState.Default;

            return Define.Result.OK;
        }

        public void PlaceUnit(Unit unit)
        {
            placed = unit;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}] ({2},{3})", name, state, index.x, index.y);
        }

        public void OnPointerEnter()
        {
            if (state == BlockState.Selected)
                return;
            UpdateState(BlockState.Over);
        }

        public void OnPointerExit()
        {
            if (state == BlockState.Selected)
                return;

            UpdateState(BlockState.Default);
        }

        public void OnPointerClick()
        {
            boardManager.SetSelected(this);
            Debug.Log(ToString());
        }

        private void UpdateState(BlockState newState)
        {
            if (state == newState)
                return;

            state = newState;
            Color nextColor = boardManager.StateColors[(int)state];
            image.color = nextColor;

            if (placed == null)
                return;

            placed.UpdateState(state);
        }
    }

}
