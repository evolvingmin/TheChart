using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class Block : MonoBehaviour
    {
        private Vector2 index;

        private Vector3 location;
        
        public GameDefine.Result Initialize(Vector2 _index, Vector3 _location)
        {
            index= _index;
            location = _location;

            return GameDefine.Result.OK;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
