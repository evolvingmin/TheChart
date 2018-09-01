using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class Cell 
    {
        public Vector3 Index { get { return index; } }
        private Vector3 index;
        public Rect rect;
        private float positionZ;
        private Board.LineType lineType;

        public Vector3 LeftTop {get{return new Vector3(rect.x, rect.y, positionZ); } }
        public Vector3 RightTop{get{return new Vector3(rect.x + rect.width , rect.y, positionZ);}}
        public Vector3 LeftBotom{get{return new Vector3(rect.x, rect.y - rect.height, positionZ);}}
        public Vector3 RightBottom{get{return new Vector3(rect.x + rect.width, rect.y - rect.height, positionZ);}}
        public Vector3 Center { get {return new Vector3(rect.center.x, rect.y - rect.height/2.0f, positionZ); } }

        public bool Occupied {  get { return occupied; } }
        private bool occupied = false;
        private Enemy current = null;

        public Vector3 Initialize(Vector3 current, float lengthX, float lengthY, Vector3 index)
        {
            this.index = index;
            rect = new Rect(current.x, current.y, lengthX, lengthY);
            positionZ = current.z;
            lineType = (Board.LineType)index.x;
            return new Vector3(rect.x + rect.width, rect.y, positionZ);
        }

        public void Occupy(Enemy enemy)
        {
            current = enemy;
            occupied = true;
        }

        public Enemy Release()
        {
            Enemy enemy = current;
            occupied = false;
            current = null;
            GameManager.Instance.Board.IncreaseAvailableLine(lineType);
            return enemy;
        }

    }

}
