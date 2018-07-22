using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public static class Preferences
    {
        public static string Email
        {
            get { return PlayerPrefs.GetString("Email", ""); }
            set { PlayerPrefs.SetString("Email", value); }
        }

        public static string ID
        {
            get { return PlayerPrefs.GetString("ID", ""); }
            set { PlayerPrefs.SetString("ID", value ); }
        }

        public static string Password
        {
            get { return PlayerPrefs.GetString("Password", ""); }
            set { PlayerPrefs.SetString("Password", value); }
        }

    }

}
