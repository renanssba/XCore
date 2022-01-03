using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchievementsController : MonoBehaviour {


  public void Start() {
    if(!SteamManager.Initialized) {
      return;
    }
    InitializeAchievementsAndStats();
  }


  public static void InitializeAchievementsAndStats() {
    if(!SteamManager.Initialized) {
      Debug.LogError("SteamManager is not initialized");
      return;
    }
    SteamUserStats.RequestCurrentStats();
  }


  public static void ReceiveAchievement(string achievementName) {
    if(!SteamManager.Initialized) {
      Debug.LogError("SteamManager is not initialized");
      return;
    }
    Debug.LogWarning("Receiving achievement: " + achievementName);
    SteamUserStats.SetAchievement(achievementName);
    SteamUserStats.StoreStats();
  }

  public static void CompleteStageAchievement(int stage) {
    if(!SteamManager.Initialized) {
      Debug.LogError("SteamManager is not initialized");
      return;
    }
    Debug.LogWarning("Calling achievement for this stage: "+stage);
    SteamUserStats.SetAchievement("STAGE_"+stage);
    SteamUserStats.StoreStats();
  }

  public static void StatProgress(string statName, int value) {
    if(!SteamManager.Initialized) {
      Debug.LogError("SteamManager is not initialized");
      return;
    }
    Debug.LogWarning("Updating stat: " + statName + ". ading value: " + value);
    int currentValue;
    SteamUserStats.GetStat(statName, out currentValue);
    SteamUserStats.SetStat(statName, currentValue + value);
    SteamUserStats.StoreStats();
  }




  public static void ResetAchievement(int stage) {
    if(!SteamManager.Initialized) {
      Debug.LogError("SteamManager is not initialized");
      return;
    }
    Debug.LogWarning("Resetting achievement for this stage: " + stage);
    SteamUserStats.ClearAchievement("STAGE_" + stage);
    SteamUserStats.StoreStats();
  }

  public static void ResetAllAchievements() {
    if(!SteamManager.Initialized) {
      Debug.LogError("SteamManager is not initialized");
      return;
    }
    Debug.LogWarning("Resetting all achievements and stats");
    SteamUserStats.ResetAllStats(true);
    SteamUserStats.StoreStats();
  }

}
