using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "card_input")]
  public class CardInputCommand : VsnCommand {

    public override void Execute() {
      for(int i = 0; i < 7; i++) {
        GameController.instance.dateCards[i].gameObject.SetActive(false);
      }
      GameController.instance.ShuffleNewCardHand();
      GameController.instance.dateCardsPanel.ShowPanel();
      GameController.instance.StartCoroutine(ShowCardByCard());
      VsnController.instance.state = ExecutionState.WAITINGCUSTOMINPUT;
    }

    public IEnumerator ShowCardByCard() {
      for(int i = 0; i < GameController.instance.cardsHand.Count; i++) {
        GameController.instance.dateCards[i].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.18f);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}