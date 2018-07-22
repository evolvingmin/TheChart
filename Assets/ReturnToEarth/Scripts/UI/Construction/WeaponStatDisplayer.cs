using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace StarShip.UI
{
    public class WeaponStatDisplayer : MonoBehaviour
    {
        [SerializeField]
        private Text description = null;


        private DataProvider dataProvider = null;

        public void UpdateDisplayInfo(TableHandler.Row info)
        {
            if (dataProvider == null)
                dataProvider = new DataProvider("WeaponList", info);
            else
                dataProvider.Update(info);


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("name :" + dataProvider.Get<string>("name"));
            stringBuilder.AppendLine("attackspeed :" + dataProvider.GetStat("attackspeed"));
            stringBuilder.AppendLine("bullet_speed :" + dataProvider.GetStat("bulletspeed"));
            stringBuilder.AppendLine("damage :" + dataProvider.GetStat("damage"));
            description.text = stringBuilder.ToString();
        }
    }
}
