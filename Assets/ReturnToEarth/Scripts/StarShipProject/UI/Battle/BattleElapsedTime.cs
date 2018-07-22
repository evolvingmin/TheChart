using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class BattleElapsedTime : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private string format;
        // Use this for initialization
        void Start()
        {
            text.text = string.Format(format, Time.time);
        }

        // Update is called once per frame
        void Update()
        {
            text.text = string.Format(format, Time.time);
        }
    }

}
