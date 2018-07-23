using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class Block : MonoBehaviour
    {
        private Vector2 index;

        private Vector3 location;
        private SpriteRenderer spriteRenderer;

        public Rect Rect { get; private set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public GameDefine.Result Initialize(Vector2 _index, Vector3 _location, Vector3 _scale)
        {
            index= _index;
            location = _location;

            float x = _location.x - _scale.x / 2.0f;
            float y = _location.y + _scale.y / 2.0f;

            Rect = new Rect(x, y, _scale.x, _scale.y);

            transform.localScale = _scale;

            name = "Block(" + _index.x + "," + _index.y + ")";

            return GameDefine.Result.OK;
        }
    }

}
