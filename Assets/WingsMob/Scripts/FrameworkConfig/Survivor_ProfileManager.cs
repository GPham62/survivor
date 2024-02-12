using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WingsMob
{
    public partial class ProfileManager
    {
        public int CurrentEnergy
        {
            get { return UserData.CurrentEnergy; }
            set
            {
                UserData.CurrentEnergy = value;
                SaveLocalUserData(true);
            }
        }

        /// <summary>
        /// Max level user passed
        /// </summary>
        public int PlayerUnlockedLevel
        {
            get
            {
                //Min level = 0
                if (UserData.PlayerUnlockedLevel < 0)
                {
                    UserData.PlayerUnlockedLevel = 0;
                }
                return UserData.PlayerUnlockedLevel;
            }
            set
            {
                if (value != UserData.PlayerUnlockedLevel)
                {
                    UserData.PlayerUnlockedLevel = value;
                    SaveLocalUserData(true);
                }
            }
        }
        public int FirstTimeLevel
        {
            get
            {
                //Min level = 0
                if (UserData.FirstTimeLevel < 0)
                {
                    UserData.FirstTimeLevel = 0;
                }
                return UserData.FirstTimeLevel;
            }
            set
            {
                if (value != UserData.FirstTimeLevel)
                {
                    UserData.FirstTimeLevel = value;
                    SaveLocalUserData(true);
                }
            }
        }

        private int m_currentLevel = 0;
        /// <summary>
        /// Current level playing
        /// </summary>
        public int CurrentLevel
        {
            get
            {
                if (m_currentLevel < 0)
                {
                    m_currentLevel = 0;
                }
                return m_currentLevel;
            }
            set
            {
                if (value != m_currentLevel)
                {
                    m_currentLevel = value;
                }
            }
        }

        public int GameCoins
        {
            get { return UserData.GameCoins; }
            set
            {
                UserData.GameCoins = value;
                SaveLocalUserData(true);
            }
        }

        public int PlayerAccountLevel
        {
            get
            {
                //Min level = 0
                if (UserData.PlayerAccountLevel < 1)
                {
                    UserData.PlayerAccountLevel = 1;
                }
                return UserData.PlayerAccountLevel;
            }
            set
            {
                if (value != UserData.PlayerAccountLevel)
                {
                    UserData.PlayerAccountLevel = value;
                    SaveLocalUserData(true);
                }
            }
        }

        public float PlayerAccountExp
        {
            get { return UserData.PlayerAccountExp; }
            set
            {
                UserData.PlayerAccountExp = value;
                SaveLocalUserData(true);
            }
        }
    }
}

