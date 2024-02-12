using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Controller
{
    public class LevelUIManager : MonoBehaviour
    {
        public GamePlayUI gameplayUI;

        public void Init()
        {
            gameplayUI.Init();
        }
    }
}