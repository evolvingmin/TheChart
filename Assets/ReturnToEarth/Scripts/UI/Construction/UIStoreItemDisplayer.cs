using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class UIStoreItemDisplayer : MonoBehaviour
    {
        [SerializeField]
        private Text description;
        public void UpdateSelected(UIStoreItem current)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if(current.StoreMode == UIStore.Mode.Equipment)
            {
                switch (current.Category)
                {
                    
                    case UIStore.Category.Weapon:
                        DataProvider WeaponProvider = new DataProvider("WeaponList", current.GeneratedItem);
                        stringBuilder.AppendLine("name :" + WeaponProvider.Get<string>("name"));

                        stringBuilder.AppendLine("attackspeed :" + WeaponProvider.GetStat("attackspeed"));
                        stringBuilder.AppendLine("bullet_speed :" + WeaponProvider.GetStat("bulletspeed"));
                        stringBuilder.AppendLine("damage :" + WeaponProvider.GetStat("damage"));


                        break;
                    case UIStore.Category.Hull:
                        DataProvider HullProvider = new DataProvider("HullList", current.GeneratedItem);
                        stringBuilder.AppendLine("name :" + HullProvider.Get<string>("name"));

                        stringBuilder.AppendLine("defense :" + HullProvider.GetStat("defense"));
                        stringBuilder.AppendLine("hp :" + HullProvider.GetStat("hp"));
                        stringBuilder.AppendLine("avoid rate :" + HullProvider.GetStat("avoidrate"));
                        break;
                    case UIStore.Category.Utilities:
                        break;
                }
            }
            else if(current.StoreMode == UIStore.Mode.BluePrint)
            {
                // 재구현 해주세요
                /*
                switch (current.Category)
                {
                    case UIStore.Category.Weapon:
                        stringBuilder.AppendLine("name :" + current.Row.Get<string>("name"));
                        stringBuilder.AppendLine("attackspeed :" + current.Row.Get<string>("attackspeed"));
                        stringBuilder.AppendLine("attack_range :" + current.Row.Get<string>("attack_range"));
                        stringBuilder.AppendLine("bullet_speed :" + current.Row.Get<string>("bullet_speed"));
                        stringBuilder.AppendLine("damage :" + current.Row.Get<string>("damage"));
                        break;
                    case UIStore.Category.Hull:
                        stringBuilder.AppendLine("name :" + current.Row.Get<string>("name"));
                        stringBuilder.AppendLine("defense :" + current.Row.Get<string>("defense"));
                        stringBuilder.AppendLine("hp :" + current.Row.Get<string>("hp"));
                        stringBuilder.AppendLine("avoid rate :" + current.Row.Get<string>("avoid_rate"));
                        break;
                    case UIStore.Category.Utilities:
                        break;
                }
                */
            }

            description.text = stringBuilder.ToString();
        }
    }

}
