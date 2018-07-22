using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class Block : MonoBehaviour
    {
        public enum State
        {
            None,
            Detectable,
        }

        private BoxCollider2D boxCollider;
        private SpriteRenderer spriteRenderer;

        private State state = State.None;

        public Cell RefCell {  get { return refCell; } }
        private Cell refCell;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool Initialize(Cell cell)
        {
            refCell = cell;
            SetState(Block.State.None);
            return true;
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.None:
                    boxCollider.enabled = false;
                    spriteRenderer.enabled = false;
                    break;
                case State.Detectable:
                    boxCollider.enabled = true;
                    spriteRenderer.enabled = false;
                    break;
            }
            this.state = state;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (state != State.Detectable)
                return;

            if (collision.CompareTag("Detecter"))
            { 
                collision.GetComponentInParent<Detecter>().Add(this);
                spriteRenderer.enabled = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (state != State.Detectable)
                return;

            if (collision.CompareTag("Detecter"))
            {
                collision.GetComponentInParent<Detecter>().Remove(this);
                spriteRenderer.enabled = false;
            }
        }
    }
}
