using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName="AngledPartSettings",menuName="Universe/Angled Part Settings")]
public class PlaceAngledSettings : ScriptableObject
{
    public List<Part> parts;
    
    [Serializable]
    public class Part
    {
        public Transform part;
        public Transform endPoint;
        public Vector3 angle;
        public float randomAngleStep = 90f;
        public bool use = true;
    }
}

