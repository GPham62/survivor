using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Controller
{
    public class GameStateManager : MonoBehaviour
    {
        public void PlayGame() => GameStatus.CurrentState = GameState.Playing;

        public void PauseGame()
        {
            GameStatus.CurrentState = GameState.Pausing;
            this.PostEvent(MethodNameDefine.OnGamePaused);
        }

        public void ResumeGame()
        {
            PlayGame();
            this.PostEvent(MethodNameDefine.OnGameResumed);
        }

        public void GameLost()
        {
            GameStatus.CurrentState = GameState.GameOver;
            this.PostEvent(MethodNameDefine.OnGameOver);
        }

        public void GameWin()
        {
            GameStatus.CurrentState = GameState.GameWin;
            this.PostEvent(MethodNameDefine.OnGameWin);
        }
    }
}