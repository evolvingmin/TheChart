using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit.Pattern;


namespace ChallengeKit
{
    public class UIComponent : MonoBehaviour
    {
        public virtual void InvalidateUI(params object[] inputs) { }
    }
}


