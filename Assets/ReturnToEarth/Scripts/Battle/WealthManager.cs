using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;


namespace StarShip {

    public class WealthManager  {

        [SerializeField]
        private Text wealthDisplayer;

        private int debris;
        //private int supplies;
        //private int rearmetals;
        //private int biometals;
        const string display = "{0} Debris {1} Supplies";

        // Use this for initialization
        void Start() {
            debris = 0;
            //supplies = 0;
            //rearmetals = 0;
            //biometals = 0;
        }

        // Update is called once per frame
        void Update() {

            wealthDisplayer.text = string.Format(display, debris, debris);
        }

        public void IncreaseDebris() {
            debris++;
            Debug.Log("Debris = " + debris);

        }

    }

}