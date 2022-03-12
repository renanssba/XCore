using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacticalUIController : MonoBehaviour {
  public static TacticalUIController instance;
  
  [Header("- Character Detail Panel -")]
  public BattlerInfoPanel battlerInfoPanel;

  [Header("- Skip Turn Button -")]
  public GameObject skipTurnButton;
  public GameObject clickMapButton;


  public void Awake() {
    instance = this;
  }

  public void Update() {
    skipTurnButton.SetActive(GameController.instance.gameState != GameState.noInput &&
                             GameController.instance.gameState != GameState.battlePhase);
  }

  public void EnterBattlePhase() {
    Select(null);
    clickMapButton.SetActive(false);
  }

  public void EndBattlePhase() {
    clickMapButton.SetActive(true);
  }

  public void Select(CharacterToken character) {
    if(character == null) {
      battlerInfoPanel.canvasGroup.alpha = 0f;
      return;
    }

    battlerInfoPanel.canvasGroup.alpha = 1f;
    battlerInfoPanel.Initialize(character.battler);
    battlerInfoPanel.SkipHpBarAnimation();
  }
}
