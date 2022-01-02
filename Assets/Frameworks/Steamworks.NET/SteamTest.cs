using UnityEngine;
using Steamworks;
using TMPro;

public class SteamTest : MonoBehaviour {

  public TextMeshProUGUI usernameText;
  public TMP_InputField stageInputField;

  protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;



  private void OnEnable() {
    if(SteamManager.Initialized) {
      m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
    }
  }

  private void OnGameOverlayActivated(GameOverlayActivated_t pCallback) {
    if(pCallback.m_bActive != 0) {
      Debug.Log("Steam Overlay has been activated");
    } else {
      Debug.Log("Steam Overlay has been closed");
    }
  }



  void Start() {
    if(!SteamManager.Initialized) {
      Debug.LogError("SteamManager is not initialized");
      return;
    }

    //AchievementsController.InitializeAchievementsAndStats();

    string name = SteamFriends.GetPersonaName();
    usernameText.text = "Steam username: " + name;
  }

  


  public void ClickGetAchievement() {
    AchievementsController.ReceiveAchievement(stageInputField.text);
  }

  public void ClickResetAchievement() {
    int stageId = int.Parse(stageInputField.text);
    AchievementsController.ResetAchievement(stageId);
  }

  public void ClickResetAllAchievements() {
    AchievementsController.ResetAllAchievements();
  }

}
