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
            Selected
        }

        private BoardManager boardManager;

        private Vector2 index;

        private Vector3 location;
        private Image image;

        public Rect Rect { get; private set; }
        private BlockState state;

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

        public GameDefine.Result Initialize(BoardManager boardManager, Vector2 _index, Vector3 _location, Vector3 _scale)
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

            name = "Block(" + index.x + "," + index.y + ")";

            state = BlockState.Default;

            return GameDefine.Result.OK;
        }

        public override string ToString()
        {
            return name;
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
            UpdateState(BlockState.Selected);
            boardManager.SetSelected(this);
        }

        private void UpdateState(BlockState newState)
        {
            if (state == newState)
                return;

            state = newState;
            Color nextColor = boardManager.StateColors[(int)state];
            image.color = nextColor;
        }
    }

}
