using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Controller
{
    public class SkillCollection : MonoBehaviour
    {
        [SerializeField] private float m_baseChance = 10f;
        [SerializeField] private float m_chanceIncreasePerLevel = 2f;
        [ReadOnly] public List<SkillChance> weaponCollection;
        [ReadOnly] public List<SkillChance> augmentorCollection;
        private List<SkillCard> m_tempSkillCards;
        private List<SkillChance> m_tempSkillChance;

        public void Init()
        {
            weaponCollection = new List<SkillChance>();
            augmentorCollection = new List<SkillChance>();
            m_tempSkillCards = new List<SkillCard>();
            m_tempSkillChance = new List<SkillChance>();
            UpdateWeaponCollection();
            UpdateAugmentorCollection();
        }

        public void UpdateWeaponCollection()
        {
            m_tempSkillCards.Clear();
            Weapon[] playerWeaponCollection = LevelManager.Instance.playerController.weaponManager.weaponsArr;
            List<int> weaponIds = new List<int>();
            for (int i = 0; i < playerWeaponCollection.Length; i++)
            {
                if (playerWeaponCollection[i] != null)
                {
                    weaponIds.Add(playerWeaponCollection[i].GetId());
                    m_tempSkillCards.Add(playerWeaponCollection[i]);
                }
            }

            List<SkillCard> remainingWeaponsInCollection = GameAssets.Instance.weaponCollection.Where(weapon => !weaponIds.Contains(weapon.GetWeaponId())).Cast<SkillCard>().ToList();
            foreach (var skillCard in remainingWeaponsInCollection)
            {
                m_tempSkillCards.Add(skillCard);
            }

            if (playerWeaponCollection.Any(w => w != null && w.GetWeaponId() == 7 && w.isFinalForm))
                m_tempSkillCards.RemoveAll(skill => skill.GetId() == 8);

            InitCollection(weaponCollection, m_tempSkillCards);
        }

        private void UpdateAugmentorCollection()
        {
            List<SkillCard> augmentorList = GameAssets.Instance.weaponAugmentorCollection.Cast<SkillCard>().ToList();
            InitCollection(augmentorCollection, augmentorList);
        }

        private void InitCollection(List<SkillChance> collection, List<SkillCard> skills)
        {
            collection.Clear();
            int totalSkills = skills.Count(skill => skill.CanLevelUp);
            foreach (var skill in skills)
            {
                SkillChance skillChance = new SkillChance();
                skillChance.skill = skill;
                skillChance.dropChance = skill.CanLevelUp ? m_baseChance + skillChance.skill.GetCardLevel() * m_chanceIncreasePerLevel * LevelManager.Instance.gameStats.levelReached : 0f;
                collection.Add(skillChance);
            }
        }

        public void UpdateSkillChance(SkillCard skill)
        {
            if (skill is Weapon)
            {
                if (LevelManager.Instance.playerController.weaponManager.IsAllWeaponFilled())
                {
                    InitCollection(weaponCollection, LevelManager.Instance.playerController.weaponManager.weaponsArr.Cast<SkillCard>().ToList());
                }
                UpdateSkillChanceInCollection(skill, weaponCollection);
                
            }
            else if (skill is WeaponAugmentor)
            {
                if (LevelManager.Instance.playerController.weaponAugmentorManager.IsAllAugmentorFilled())
                {
                    InitCollection(augmentorCollection, LevelManager.Instance.playerController.weaponAugmentorManager.weaponAugmentors.Cast<SkillCard>().ToList());
                }
                UpdateSkillChanceInCollection(skill, augmentorCollection);
            }
            UpdateCollectionChance(weaponCollection);
            UpdateCollectionChance(augmentorCollection);
        }

        private void UpdateSkillChanceInCollection(SkillCard skill, List<SkillChance> collection)
        {
            SkillChance skillChance = collection.FirstOrDefault(s => s.skill.GetId() == skill.GetId());
            if (skillChance != null)
                skillChance.skill = skill;
        }

        private void UpdateCollectionChance(List<SkillChance> collection)
        {
            foreach (var skillChance in collection)
            {
                skillChance.dropChance = skillChance.skill.CanLevelUp ? m_baseChance + skillChance.skill.GetCardLevel() * m_chanceIncreasePerLevel * LevelManager.Instance.gameStats.levelReached : 0f;
            }
        }

        public List<SkillCard> GetRandomWeaponsFromCollection(int length)
        {
            m_tempSkillChance = new List<SkillChance>(weaponCollection);
            return GetRandomCardsFromCollection(m_tempSkillChance, length);
        }

        public List<SkillCard> GetRandomAugmentorsFromCollection(int length)
        {
            m_tempSkillChance = new List<SkillChance>(augmentorCollection);
            return GetRandomCardsFromCollection(m_tempSkillChance, length);
        }

        public List<SkillCard> GetRandomSkillCardsFromCollection(int length)
        {
            m_tempSkillChance = new List<SkillChance>(weaponCollection);
            m_tempSkillChance.AddRange(augmentorCollection);
            return GetRandomCardsFromCollection(m_tempSkillChance, length);
            
        }

        private float m_totalWeight, m_cardChance, m_weightCounter;
        private List<SkillCard> GetRandomCardsFromCollection(List<SkillChance> collection, int length)
        {
            List<SkillCard> result = new List<SkillCard>();
            for (int i = 0; i < length; i++)
            {
                m_totalWeight = 0f;
                foreach (var item in collection)
                    m_totalWeight += item.dropChance;
                m_cardChance = UnityEngine.Random.Range(0, m_totalWeight);
                m_weightCounter = 0f;
                foreach (var item in collection)
                {
                    if (item.dropChance > 0f)
                    {
                        m_weightCounter += item.dropChance;
                        if (m_weightCounter >= m_cardChance)
                        {
                            result.Add(item.skill);
                            collection.Remove(item);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        [System.Serializable]
        public class SkillChance
        {
            public SkillCard skill;
            public float dropChance;
        }
    }
}