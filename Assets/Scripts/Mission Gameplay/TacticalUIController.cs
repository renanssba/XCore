using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacticalUIController : MonoBehaviour {
  public static TacticalUIController instance;
  
  [Header("- Character Detail Panel -")]
  public CanvasGroup detailPanel;
  public Slider hpSlider;
  public TextMeshProUGUI characterName;
  public TextMeshProUGUI hpText;
  public Image characterImage;

  [Header("- Skip Turn Button -")]
  public GameObject skipTurnButton;


  public void Awake() {
    instance = this;
  }

  public void Update() {
    skipTurnButton.SetActive(GameController.instance.gameState != GameState.noInput);
  }

  public void Select(Character character) {
    if(character == null) {
      detailPanel.alpha = 0f;
      return;
    }

    detailPanel.alpha = 1f;

    // face and name
    characterImage.sprite = TacticalResources.instance.faceSprites[(int)character.id];
    characterName.text = character.id.ToString().ToTitleCase();

    // hp bar
    hpSlider.maxValue = character.maxHp;
    hpSlider.value = character.hp;
    hpText.text = character.hp.ToString() + " /"+ character.maxHp.ToString();
  }
}
