using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReturnToEarth.UI
{
    public class ObjectDisplay : MonoBehaviour
    {
        [SerializeField]
        private Transform contents;

        [SerializeField]
        private ResourceManager resourceManager;

        private const string prefab = "ObjectItem";

        public void Display()
        {

        }
    }

}
