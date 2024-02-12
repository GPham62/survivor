using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DarkTonic.PoolBoss;

namespace WingsMob.Survival.Utils
{
    public static class Utilities
    {
        private static string CharacterConfigFile = "CharacterConfig";

        private static Dictionary<string, CharacterConfig> m_dictCharacters;

        private static void ReadDataConfig()
        {
            var fileData = Resources.Load<TextAsset>(CharacterConfigFile).ToString();
            var lines = fileData.Split("\n"[0]);

            List<string[]> allData = new List<string[]>();
            for (int i = 1; i < lines.Length; i++)
            {
                var lineData = (lines[i].Trim()).Split(","[0]);
                allData.Add(lineData);
            }

            m_dictCharacters = new Dictionary<string, CharacterConfig>();
            foreach (string[] data in allData)
            {
                if (data == null || data.Length < 5) continue;

                CharacterConfig config = new CharacterConfig();
                config.name = data[0];
                config.health = int.Parse(data[1]);
                config.speed = int.Parse(data[2]);
                config.damage = int.Parse(data[3]);
                config.armor = int.Parse(data[4]);

                m_dictCharacters.Add(config.name, config);
            }
        }

        public static CharacterConfig GetConfig(string characterName)
        {
            if (m_dictCharacters == null)
            {
                ReadDataConfig();
            }

            if (m_dictCharacters.ContainsKey(characterName))
            {
                return m_dictCharacters[characterName];
            }

            return null;
        }

        #region Find Coordinates

        public static Vector2 GetPointWithFriction(Vector2 pos1, Vector2 pos2, float friction)
        {
            return new Vector2(pos1.x + (pos2.x - pos1.x) * friction, pos1.y + (pos2.y - pos1.y) * friction);
        }

        public static Vector2 GetMiddlePoint(Vector2 pos1, Vector2 pos2)
        {
            return GetPointWithFriction(pos1, pos2, 0.5f);
        }
        public static Vector2 CircleXY(float radius, float angle)
        {
            // Convert angle to radians
            angle = (angle - 90) * Mathf.PI / 180;

            return new Vector2(radius * Mathf.Cos(angle), -radius * Mathf.Sin(angle));
        }

        #endregion

        #region Sort transform

        public static List<Transform> TransformSort(List<Transform> targets, Transform center)
        {
            return targets.OrderBy(x => Vector2.Distance(center.position, x.position)).ToList();
        }

        #endregion

        #region transform
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
            return transform;
        }

        public static void ClearEditor(this Transform transform)
        {
            for (int i = transform.childCount; i > 0; --i)
                UnityEngine.Object.DestroyImmediate(transform.GetChild(0).gameObject);
        }

        public static Transform ClearBoss(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                PoolBoss.Despawn(child, true);
            }
            return transform;
        }

        public static Transform HideChilds(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            return transform;
        }

        public static Transform ShowChilds(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            return transform;
        }
        #endregion


        #region collider
        public static void FullScreenCollider(Camera cam, BoxCollider2D collider, float offset = 0f)
        {
            var bottomLeft = cam.ScreenToWorldPoint(Vector2.zero);
            var topLeft = cam.ScreenToWorldPoint(new Vector2(0, cam.pixelHeight));
            var topRight = cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight));
            collider.size = new Vector2(topRight.x - topLeft.x - offset, topLeft.y - bottomLeft.y - offset);
        }
        #endregion
    }

}