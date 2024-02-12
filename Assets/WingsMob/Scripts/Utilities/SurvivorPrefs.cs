using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorPrefs : MonoBehaviour
{
    const int TOTAL_LEVEL = 6;

    private static string State_Levels_Key = "StateLevels";
    private static string Current_Level_Key = "CurrentLevel";
    private static string Best_Time_Levels_Key = "BestTimeLevel";
    private static string Flash_Key = "Flash";
    private static string Gems_Key = "Gems";
    private static string Coins_Key = "Coins";
    private static string Current_Score_Key = "CurrentScore";
    private static string Score_Key = "Score";

    public static bool[] AllLevels()
    {
        if (!PlayerPrefs.HasKey(State_Levels_Key))
        {
            // 0: Unlock state
            // 1: Lock state

            string stateValue = "";
            string bestTimeValue = "";
            for(int i = 0; i < TOTAL_LEVEL; i++)
            {
                stateValue += i == 0 ? "0" : "1";
                bestTimeValue += "0,";
            }

            PlayerPrefs.SetString(State_Levels_Key, stateValue);
        }

        string stateString = PlayerPrefs.GetString(State_Levels_Key);
        bool[] stateLevels = new bool[stateString.Length];
        for(int i = 0; i < stateString.Length; i++)
        {
            if (stateString[i] == '0') stateLevels[i] = false;
            else stateLevels[i] = true;
        }

        return stateLevels;
    }

    public static void UnLockLevel(int level)
    {
        string stateString = PlayerPrefs.GetString(State_Levels_Key);

        string newState = "";
        for(int i = 0; i < stateString.Length; i++)
        {
            if (i == level)
                newState += "0";
            else
                newState += stateString[i];
        }

        PlayerPrefs.SetString(State_Levels_Key, newState);
    }

    public static bool CheckUnlockLevel(int level)
    {
        bool[] stateLevels = AllLevels();
        if (level < stateLevels.Length)
        {
            return stateLevels[level];
        }

        return false;
    }

    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(Current_Level_Key, 0);
    }

    public static void SelectLevel(int level)
    {
        PlayerPrefs.SetInt(Current_Level_Key, level);
    }

    public static float GetBestimeLevel(int level)
    {
        string bestTimeString = PlayerPrefs.GetString(Best_Time_Levels_Key);
        if (string.IsNullOrEmpty(bestTimeString))
        {
            string bestTimeValue = "";
            for (int i = 0; i < TOTAL_LEVEL; i++) bestTimeValue += "0,";

            PlayerPrefs.SetString(Best_Time_Levels_Key, bestTimeValue);
            bestTimeString = bestTimeValue;
        }

        string[] bestTimes = bestTimeString.Split(',');

        return float.Parse(bestTimes[level]);
    }

    public static void UpdateBestTimeLevel(int level, float time)
    {
        string bestTimeString = PlayerPrefs.GetString(Best_Time_Levels_Key);
        string[] bestTimes = bestTimeString.Split(',');

        string newBestTime = "";
        for(int i = 0; i < bestTimes.Length; i++)
        {
            if (i == level)
            {
                newBestTime += time.ToString();
            }
            else
            {
                newBestTime += bestTimes[i];
            }
        }

        PlayerPrefs.SetString(Best_Time_Levels_Key, newBestTime);
    }

    public static int Flash { get { return PlayerPrefs.GetInt(Flash_Key, 0); } set { PlayerPrefs.SetInt(Flash_Key, value); } }
    public static int Gems { get { return PlayerPrefs.GetInt(Gems_Key, 0); } set { PlayerPrefs.SetInt(Gems_Key, value); } }
    public static int Coins { get { return PlayerPrefs.GetInt(Coins_Key, 0); } set { PlayerPrefs.SetInt(Coins_Key, value); } }
    public static int CurrentScore { get { return PlayerPrefs.GetInt(Current_Score_Key, 0); } set { PlayerPrefs.SetInt(Current_Score_Key, value); } }
    public static float Score { get { return PlayerPrefs.GetFloat(Score_Key, 0); } set { PlayerPrefs.SetFloat(Score_Key, value); } }
}
