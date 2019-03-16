using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit.Pattern;


namespace ChallengeKit
{
    public class UIComponent : MonoBehaviour
    {
        public virtual void HandleSwipe(float startX, float startY, float endX, float endY, float velocityX, float velocityY) { }
        public virtual void InvalidateUI(params object[] inputs) { }

        public virtual void BeginDrag(float screenX, float screenY) { }
        public virtual void DragTo(float screenX, float screenY) { }
        public virtual void EndDrag(float velocityXScreen, float velocityYScreen) { }
    }
}


