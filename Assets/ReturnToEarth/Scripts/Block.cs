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

        public GameDefine.Result Initialize(Vector2 _index, Vector3 _location, Vector3 _scale)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            index = _index;
            transform.Reset();
            location = _location;

            float x = location.x - _scale.x / 2.0f;
            float y = location.y + _scale.y / 2.0f;

            Rect = new Rect(x, y, spriteRenderer.size.x * _scale.x, spriteRenderer.size.y * _scale.y);

            transform.position = location;
            transform.localScale = _scale;

            name = "Block(" + index.x + "," + index.y + ")";

            return GameDefine.Result.OK;
        }

        public override string ToString()
        {
            return name;
        }
    }

}
