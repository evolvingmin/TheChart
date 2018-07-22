using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class Detecter : MonoBehaviour
    {
        public enum Type
        {
            Origin,
            ToInput,
            Input
        }

        public Type type;
        private List<Block> DetectedBlock = new List<Block>();

        public void ClearBlock()
        {
            DetectedBlock.Clear();
        }

        public void Add(Block block)
        {
            if(!DetectedBlock.Contains(block))
            {
                DetectedBlock.Add(block);
            }
        }

        public void Remove(Block block)
        {
            if(DetectedBlock.Contains(block))
            {
                DetectedBlock.Remove(block);
            }
        }

        public List<Cell> GetDetectedCells()
        {
            List<Cell> refCells = new List<Cell>(DetectedBlock.Count);
            foreach (var item in DetectedBlock)
            {
                refCells.Add(item.RefCell);
            }

            return refCells;
        }
    }
}

