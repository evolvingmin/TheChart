using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReturnToEarth.UI
{
    public class MainUIController : MonoBehaviour
    {
        public void SkipToBattleScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
        }
    }

}

