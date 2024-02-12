using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Environment.Map
{
    public class SurvivorObstacle : MonoBehaviour
    {
        [SerializeField] private float size;

        public float Size
        {
            get { return size; }
            private set { }
        }
    }
}