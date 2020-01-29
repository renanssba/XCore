using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameController : MonoBehaviour {

  public static GameController instance;

  public ItemSelectorScreen itemSelectorScreen;

  public ScreenTransitions girlInteractionPanel;
  public ScreenTransitions girlInteractionButtonsPanel;

  public GameObject[] girlInteractionButtons;
  public GameObject[] dateSelectButtons;

  public ScreenTransitions dateSelectPanel;
  public CoupleEntry coupleEntry;

  public ParticleGenerator babiesParticleGenerator;
  public Image[] engagementScreenImages;
  public GameObject engagementScreen;



  public void Awake() {
    instance = this;
  }

  public void Start() {
    //VsnAudioManager.instance.PlayMusic("observacao_intro", "observacao_loop");

    if (VsnSaveSystem.GetIntVariable("minigame_ended") == 1) {
      //datingPeopleCards[0].Initialize(GlobalData.instance.people[0]);
      //datingPeopleCards[1].Initialize(GlobalData.instance.people[1]);

      VsnSaveSystem.SetVariable("minigame_ended", 0);
      UIController.instance.UpdateUI();
      VsnController.instance.StartVSN("back_from_minigame");
    } else {
      GlobalData.instance.InitializeChapter();
      GlobalData.instance.PassTime();

      //VsnSaveSystem.SetVariable("observation_played", 1);
      //VsnController.instance.StartVSN("show_people_screen");

      UIController.instance.UpdateUI();

      if(GlobalData.instance.hideTutorials) {
        VsnSaveSystem.SetVariable("tutorial_date", 1);
        VsnSaveSystem.SetVariable("tutorial_date2", 1);
        VsnSaveSystem.SetVariable("tutorial_shop", 1);
        VsnSaveSystem.SetVariable("tutorial_choose_date", 1);
        VsnSaveSystem.SetVariable("tutorial_observation", 1);
      }
      //VsnController.instance.StartVSN("cap0_intro");
      //VsnController.instance.StartVSN("cap1_manha");
      VsnController.instance.StartVSN("cap1_manha");
      //VsnController.instance.StartVSN("tutorial_intro");
    }
  }


  public void Update() {
    if(Input.GetKeyDown(KeyCode.F4)){
      SceneManager.LoadScene(StageName.TitleScreen.ToString());
    }
    if (Input.GetKeyDown(KeyCode.F5)) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }


  

  public void ShowEngagementScreen(int babies) {
    VsnAudioManager.instance.PlaySfx("date_success");

    engagementScreenImages[0].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentBoy().faceId);
    engagementScreenImages[1].sprite = ResourcesManager.instance.GetFaceSprite(GlobalData.instance.CurrentGirl().faceId);
    babiesParticleGenerator.particlesToGenerate = babies;
    engagementScreen.SetActive(true);
    babiesParticleGenerator.DeleteSons();
    StartCoroutine(ShowEngagementScreenAnimation(5f+babies));
  }

  public IEnumerator ShowEngagementScreenAnimation(float waitTime){
    VsnController.instance.state = ExecutionState.WAITING;
    yield return new WaitForSeconds(waitTime);
    VsnController.instance.state = ExecutionState.PLAYING;
    HideEngagementScreen();
  }

  public void HideEngagementScreen() {
    engagementScreen.SetActive(false);
    babiesParticleGenerator.DeleteSons();
  }

  



  public void ShowGirlInteractionScreen() {
    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();
    if(currentRelationship == null){
      Debug.LogError("Error getting current relationship");
    }

    if(VsnSaveSystem.GetIntVariable("daytime") == 0) {
      UIController.instance.boyInteractionImage.sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[0].id, "uniform");
      UIController.instance.girlInteractionImage.sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[1].id, "uniform");
    } else {
      UIController.instance.boyInteractionImage.sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[0].id, "casual");
      UIController.instance.girlInteractionImage.sprite = ResourcesManager.instance.GetCharacterSprite(GlobalData.instance.observedPeople[1].id, "casual");
    }    
    UIController.instance.relationshipCard.Initialize(currentRelationship);

    coupleEntry.Initialize(currentRelationship);
    girlInteractionPanel.ShowPanel();
    girlInteractionButtonsPanel.canvasGroup.alpha = 0f;
    girlInteractionButtonsPanel.ShowPanel();
    Utils.SelectUiElement(girlInteractionButtons[0]);
    dateSelectPanel.gameObject.SetActive(false);
  }

  public void ShowDateSelectionPanel() {
    girlInteractionButtonsPanel.HidePanel();
    dateSelectPanel.ShowPanel();
    Utils.SelectUiElement(dateSelectButtons[0]);
  }

  public void ExitDateSelectionPanel() {
    girlInteractionButtonsPanel.ShowPanel();
    Utils.SelectUiElement(girlInteractionButtons[0]);
    dateSelectPanel.HidePanel();
  }

  public void HideGirlInteractionScreen() {
    girlInteractionPanel.HidePanel();
  }

  public void ClickConversationButton() {
    SfxManager.StaticPlayConfirmSfx();
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("conversation");
    VsnController.instance.GotCustomInput();
  }

  public void ClickGiveGiftButton() {
    SfxManager.StaticPlayConfirmSfx();
    HideGirlInteractionScreen();
    Command.GotoCommand.StaticExecute("give_gift");
    VsnController.instance.GotCustomInput();
  }


  public void ClickedDateButton(int dateId) {

    /// IF CANNOT GO TO DATE RIGHT NOW
    //if(VsnSaveSystem.GetIntVariable("daytime") == 0) {
    //  SfxManager.StaticPlayForbbidenSfx();
    //  return;
    //}

    Relationship currentRelationship = GlobalData.instance.GetCurrentRelationship();

    Debug.LogWarning("Clicked date button to " + currentRelationship.GetBoy().name + " and " + currentRelationship.GetGirl().name);

    BattleController.instance.StartBattle(currentRelationship.GetBoy(), currentRelationship.GetGirl(), dateId);

    SfxManager.StaticPlayBigConfirmSfx();
    HideGirlInteractionScreen();
    Command.EndScriptCommand.StaticExecute(new VsnArgument[0]);
    VsnController.instance.GotCustomInput();
    //VsnController.instance.StartVSN("date");
    VsnArgument[] args = new VsnArgument[1];
    args[0] = new VsnString("date_intro_" + dateId);
    switch(currentRelationship.GetGirl().id) {
      case 1:
        VsnController.instance.StartVSN("cap1_conversa_ana", args);
        break;
      case 2:
        VsnController.instance.StartVSN("cap1_conversa_beatrice", args);
        break;
      case 3:
        VsnController.instance.StartVSN("cap1_conversa_clara", args);
        break;
    }
  }
}
