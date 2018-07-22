using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

namespace StarShip
{
    public class PlayerHPText : MonoBehaviour
    {
        //[SerializeField]
        //private TextMeshProUGUI text;

        private StarShip starShip;

        public bool Initialize(StarShip starship)
        {
            this.starShip = starship;
            Canvas canvas = Camera.main.GetComponentInChildren<Canvas>();

            transform.SetParent(canvas.transform);
            transform.localScale = Vector3.one;
            transform.position = starship.transform.position;
            gameObject.SetActive(true);
            return true;
        }

        private void Update()
        {
            if (starShip == null)
                return;

            transform.position = starShip.transform.position;
            //text.text = string.Format("{0} / {1} ", starShip.BattleObject.StatuesInfo.currentHP, starShip.BattleObject.StatuesInfo.hp);
        }
    }

}


