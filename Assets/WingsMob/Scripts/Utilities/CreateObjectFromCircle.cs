using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Utils.Editor
{
    public class CreateObjectFromCircle : MonoBehaviour
    {
        [SerializeField] GameObject enemyPefab;
        [SerializeField] float radius = 1f;
        [SerializeField] int m_numbOfDot;
        [SerializeField] bool isRandomRotation;

        [Button(ButtonSizes.Large), GUIColor(1, 0, 0)]
        public void CreateAround()
        {
            CreateEnemiesAroundPoint(m_numbOfDot, transform.position, radius);
        }

        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void DeleteChilds()
        {
            for (int i = transform.childCount; i > 0; --i)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        public void CreateEnemiesAroundPoint(int num, Vector3 point, float radius)
        {
            for (int i = 0; i < num; i++)
            {
                /* Distance around the circle */
                var radians = 2 * Math.PI / num * i;

                /* Get the vector direction */
                float vertrical = (float)Math.Sin(radians);
                float horizontal = (float)Math.Cos(radians);

                var spawnDir = new Vector3(horizontal, vertrical, 0);

                /* Get the spawn position */
                var spawnPos = point + spawnDir * radius; // Radius is just the distance away from the point

                /* Now spawn */
                var enemy = Instantiate(enemyPefab, spawnPos, Quaternion.identity) as GameObject;

                /* Rotate the enemy to face towards player */
                //   enemy.transform.LookAt(point);

                /* Adjust height */
                enemy.transform.Translate(new Vector3(0, enemy.transform.localScale.y / 2, 0));

                enemy.transform.SetParent(transform);
                // random rotation
                if (isRandomRotation)
                    enemy.transform.eulerAngles = Vector3.forward * UnityEngine.Random.Range(0, 360);

            }
        }
    }

}
