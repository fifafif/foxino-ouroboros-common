using System;
using UnityEngine;

namespace Ouroboros.Common.Utils.GameObjects
{
    public class MouseClickObjectEnabler : MonoBehaviour
    {
        [Serializable]
        public class KeyCodeObject
        {
            public KeyCode KeyCode;
            public GameObject GameObject;
        }

        [SerializeField] private GameObject target;
        [SerializeField] private KeyCodeObject[] keyCodeObjects;

        private void Start()
        {
            if (target != null)
            {
                target.SetActive(false);
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (target != null)
            {
                if (Input.GetMouseButtonDown(0))
                { 
                    target.SetActive(!target.activeSelf);
                }
            }
#endif

            int index = -1;

            for (int i = 0; i < keyCodeObjects.Length; i++)
            {
                if (Input.GetKeyDown(keyCodeObjects[i].KeyCode))
                {
                    index = i;
                }
            }

            if (index >= 0)
            {
                for (int i = 0; i < keyCodeObjects.Length; i++)
                {
                    keyCodeObjects[i].GameObject.SetActive(i == index);
                }
            }
        }
    }
}