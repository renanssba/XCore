using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum StageName {
  TitleScreen,
  Park,
  CustomizeScreen
};

public enum SongNames {
  conceito_loop,
  sewers_loop,
  night_loop,
  conceito_full
};

public enum LocationBg {
  Park,
  Shopping,
  City
}


public enum CharacterTraits {
  estudioso,
  correTerEQui,
  shoppingSegEQua,
  delinquente,
  menteResistente,
  teimoso, // needs more AP to be persuaded
  //  menteVulneravel,
  seApaixonaFacil,
  timido
};


public enum RandomTastes {
  comerMuitoFeijao,
  //  bdsm,
  //navegarNaDeepweb,
  dinossauros,
  praticarOcultismo,
  fazerAcrobacias,
  assistirTokusatsu,
  dancarSamba,
  //roguelikes,
  //datingSims,
  //cupidSimulators,
  //arroz,
  //batatas,
  acampar,
  oSentidoDaVida,
  hackear,
  imitarTartarugas,
  arquitetura,
  artesanato,
  andarACavalo,
  rezar,
  meditacao,
  memes,
  esgrima,
  //rpg,
  mercadoDeAcoes,
  robos,
  praticarNinjutsu,
  enfrentarNinjas,
  piratear,
  origami,
  //churros,
  //  minhaVida,
  aVidaDosOutros,
  //pudim,
  //tortas,
  //agua,
  natacao,
  tomarBanho,
  usarAppsDeNamoro,
  yaoi,
  waifus,
  //barbas,
  relacionamentos,
  cuidarDeBebes,
  //ratos,
  //  morte,
  //namorarPelado,
  fazerArranjoDeFlores,
  //calcinhas,
  caridade,
  fazerDinheiro,
  //criancas,
  empreendedorismo,
  tristeza,
  //usarLingerie,
  //  naoUsarRoupasDeBaixo,
  usarOculos,
  brincarDeBonecas,
  lutar,
  fazerPoses,
  count
}


public class Utils {
  public static string GetRandomTasteName() {
    int selected = Random.Range(0, System.Enum.GetNames(typeof(RandomTastes)).Length);

    return System.Enum.GetName(typeof(RandomTastes), selected);
  }

  public static CharacterTraits GetRandomTrait() {
    int selected = Random.Range(0, System.Enum.GetNames(typeof(CharacterTraits)).Length);

    Debug.LogWarning("selected: " + selected + ", possible values: " + System.Enum.GetNames(typeof(CharacterTraits)).Length);

    return (CharacterTraits)selected;
  }

  public static string GetRandomBoyName() {
    int selected = Random.Range(0, 10);
    return Lean.Localization.LeanLocalization.GetTranslationText("char_name/npc_boy_"+selected);
  }

  public static string GetRandomGirlName() {
    int selected = Random.Range(0, 10);
    return Lean.Localization.LeanLocalization.GetTranslationText("char_name/npc_girl_" + selected);
  }
  
  
  public static int GetRandomAge() {
    return Random.Range(15, 18);
  }


  public static void UnselectButton() {
    EventSystem.current.SetSelectedGameObject(null);
  }

  public static void SetButtonDisabledGraphics(Button but) {
    SpriteState st = new SpriteState();
    st.disabledSprite     = ResourcesManager.instance.buttonSprites[3];
    st.highlightedSprite  = ResourcesManager.instance.buttonSprites[4];
    st.pressedSprite      = ResourcesManager.instance.buttonSprites[5];
    but.spriteState = st;
    but.GetComponent<Image>().sprite = ResourcesManager.instance.buttonSprites[3];
    Debug.LogWarning("Setting ");
  }

  public static void SetButtonEnabledGraphics(Button but) {
    SpriteState st = new SpriteState();
    st.disabledSprite     = ResourcesManager.instance.buttonSprites[0];
    st.highlightedSprite  = ResourcesManager.instance.buttonSprites[1];
    st.pressedSprite      = ResourcesManager.instance.buttonSprites[2];
    but.spriteState = st;
    but.GetComponent<Image>().sprite = ResourcesManager.instance.buttonSprites[0];
  }



  public static string GroupNameString(List<Person> peopleGroup) {
    string currentString = "";

    if (peopleGroup.Count == 0)
      return "Ninguém";

    for (int i = 0; i < peopleGroup.Count; i++) {
      currentString += peopleGroup[i].name;
      if (i == peopleGroup.Count - 2) {
        currentString += " e ";
      } else if (i < peopleGroup.Count - 2) {
        currentString += ", ";
      }
    }
    return currentString;
  }

  public static string GroupNameString(Person[] peopleArray) {
    List<Person> peopleList = new List<Person>();

    foreach (Person p in peopleArray) {
      peopleList.Add(p);
    }

    return GroupNameString(peopleList);
  }

  public static string GetWeekdayName(int day) {
    switch (day % 5) {
      case 1:
        return "Seg";
      case 2:
        return "Ter";
      case 3:
        return "Qua";
      case 4:
        return "Qui";
      case 0:
        return "Sex";
    }
    return "???";
  }


  public static int CountBoys(List<Person> people) {
    int count = 0;
    foreach (Person p in people) {
      if (p.isMale)
        count++;
    }
    return count;
  }

  public static int CountGirls(List<Person> people) {
    int count = 0;
    foreach (Person p in people) {
      if (!p.isMale)
        count++;
    }
    return count;
  }

  public static string GetPrintableString(string name, bool capitalLetters) {
    char initialLetter = name[0];

    for (int i = 1; i < name.Length; i++) {
      if (System.Char.IsUpper(name[i])) {
        name = name.Substring(0, i) + " " + System.Char.ToLower(name[i]) + name.Substring(i + 1, name.Length - i - 1);
      }
    }

    if (capitalLetters)
      return initialLetter.ToString().ToUpper() + name.Substring(1, name.Length - 1);
    else
      return name;
  }

  public void ShuffleList(List<int> list) {
    int n = list.Count;
    while (n > 1) {
      n--;
      int k = Random.Range(0, n + 1);
      int value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }

  public static string RelationshipNameByHeartLocksOpened(int heartlocksOpened) {
    switch(heartlocksOpened) {
      case 0:
      default:
        return "Conhecidos";
      case 1:
        return "Amigos Próximos";
      case 2:
        return "Apaixonados";
    }
  }

  public static string GetRandomFamilyMember() {
    int selected = Random.Range(0, 4);
    switch (selected) {
      case 0:
        return "Minha tia ";
      case 1:
        return "Minha mãe ";
      case 2:
        return "Meu pai ";
      case 3:
        return "Meu avô ";
    }
    return "";
  }

  public static string GetRandomDisaster() {
    int selected = Random.Range(0, 6);
    switch (selected) {
      case 0:
        return "perdeu o emprego";
      case 1:
        return "sofreu graves ferimentos";
      case 2:
        return "entrou em depressão";
      case 3:
        return "virou alcoólatra";
      case 4:
        return "perdeu sua casa";
      case 5:
        return "se afogou em dívidas";
    }
    return "";
  }

  public static string AddSuggestedTextMark(string text) {
    return AddSuggestedTextMark(text, true);
  }

  public static string AddSuggestedTextMark(string text, bool isSuggested) {
    return isSuggested ? "<color=#79CBE1FF>" + text + "</color>" : text;
  }

  public static void SelectUiElement(GameObject toSelect) {
    EventSystem.current.SetSelectedGameObject(toSelect);
    if(JoystickController.instance != null) {
      //if(toSelect != null) {
      //  Debug.LogError("setting lastSelectedObject: " + toSelect.name);
      //} else {
      //  Debug.LogError("setting lastSelectedObject: null");
      //}      
      JoystickController.instance.CurrentContext().lastSelectedObject = toSelect;
    }
  }

  public static void GenerateNavigation(Button[] navigatableObjects) {
    Navigation navi = new Navigation {
      mode = Navigation.Mode.Explicit
    };

    if(navigatableObjects.Length <= 1) {
      return;
    }

    for(int i = 0; i < navigatableObjects.Length; i++) {
      if(i == 0) {
        navi.selectOnDown = navigatableObjects[i + 1];
        navi.selectOnUp = null;
      } else if(i == navigatableObjects.Length - 1) {
        navi.selectOnDown = null;
        navi.selectOnUp = navigatableObjects[i - 1];
      } else {
        navi.selectOnUp = navigatableObjects[i - 1];
        navi.selectOnDown = navigatableObjects[i + 1];
      }
      navigatableObjects[i].navigation = navi;
    }
  }

  public static Attributes GetAttributeByString(string attrName) {
    switch(attrName) {
      case "guts":
        return Attributes.guts;
      case "intelligence":
        return Attributes.intelligence;
      case "charisma":
        return Attributes.charisma;
      case "magic":
        return Attributes.endurance;
    }
    return Attributes.guts;
  }

  public static Color GetColorByString(string colorName) {
    switch(colorName) {
      case "red":
        return Color.red;
      case "green":
        return Color.green;
      case "blue":
        return Color.blue;
      case "yellow":
        return Color.yellow;
      case "cyan":
        return Color.cyan;
      case "magenta":
        return Color.magenta;
      case "gray":
      case "grey":
        return Color.gray;
      case "white":
        return Color.white;
      case "black":
        return Color.black;
    }
    Color c;
    if(ColorUtility.TryParseHtmlString(colorName, out c)) {
      return c;
    }
    return Color.magenta;
  }

  public static Color ChangeColorAlpha(Color c, float alpha) {
    c.a = alpha;
    return c;
  }

  public static string[] SeparateTags(string raw) {
    if(string.IsNullOrEmpty(raw)) {
      return new string[0];
    }

    string[] loadedTags = raw.Split(',');
    for(int i=0; i<loadedTags.Length; i++) {
      loadedTags[i] = loadedTags[i].Trim();
    }
    return loadedTags;
  }

  public static int[] SeparateInts(string raw) {
    if(string.IsNullOrEmpty(raw)) {
      return new int[0];
    }

    string[] loadedTags = raw.Split(',');
    int[] ints = new int[loadedTags.Length];
    for(int i=0; i<loadedTags.Length; i++) {
      ints[i] = int.Parse(loadedTags[i].Trim());
    }
    return ints;
  }

  public static string GetStringArgument(string clause) {
    int start = clause.IndexOf("(");
    int end = clause.IndexOf(")");

    if(start == -1 || end == -1){
      return null;
    }

    string argumentName = clause.Substring(start+1, (end-start-1));
    Debug.Log("GET ARGUMENT: '"+argumentName+"'  ");
    return argumentName;
  }


  public static bool TagIsInArray(string tagToCheck, string[] tags) {
    foreach(string tag in tags) {
      if(tag == tagToCheck) {
        return true;
      }
    }
    return false;
  }


  public static bool AreAllConditionsMet(Skill usedSkill, string[] allConditions, int partyMemberId) {
    string conditionArgument;
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");


    foreach(string condition in allConditions) {
      conditionArgument = GetStringArgument(condition);

      Debug.LogWarning("Check for skill activation. Checking condition: " + condition);

      if(condition.StartsWith("enemy_has_tag")) {
        if(!TagIsInArray(conditionArgument, BattleController.instance.GetCurrentEnemy().tags)) {
          return false;
        }
      }

      if(condition.StartsWith("is_turn")) {
        if(VsnSaveSystem.GetIntVariable("currentBattleTurn") != int.Parse(conditionArgument)) {
          return false;
        }
      }

      if(condition.StartsWith("turn_is_multiple")) {
        if(VsnSaveSystem.GetIntVariable("currentBattleTurn") % int.Parse(conditionArgument) != 0) {
          return false;
        }
      }

      if(condition.StartsWith("received_item_with_tag")) {
        Debug.LogWarning("Checking received_item_with_tag. Action: " + BattleController.instance.selectedActionType[currentPlayerTurn]);
        if(BattleController.instance.selectedActionType[currentPlayerTurn] != TurnActionType.useItem) {
          return false;
        }
        conditionArgument = GetStringArgument(condition);
        Debug.LogWarning("Condition argument: " + conditionArgument);
        if(!TagIsInArray(conditionArgument, BattleController.instance.selectedItems[currentPlayerTurn].tags) ||
           BattleController.instance.selectedTargetPartyId[currentPlayerTurn] != partyMemberId) {
          return false;
        }
      }

      if(condition.StartsWith("hp_percent_is_less")) {
        float hpPercent = ((float)BattleController.instance.GetBattlerByTargetId(partyMemberId).CurrentHP()) /
                          (float)BattleController.instance.GetBattlerByTargetId(partyMemberId).MaxHP();
        Debug.LogWarning("Current HP percent: " + hpPercent + ". argument: " + float.Parse(conditionArgument));
        if(hpPercent > float.Parse(conditionArgument)) {
          return false;
        }
      }


      if(condition == "ally_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") == partyMemberId) {
        return false;
      }

      if(condition == "self_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") != partyMemberId) {
        return false;
      }

      if(condition == "defending" && !TheaterController.instance.GetActorByIdInParty(partyMemberId).battler.IsDefending()) {
        return false;
      }


      /// CHECKING FOR SKILL-DEPENDANT TRIGGERS
      if(usedSkill == null) {
        continue;
      }

      if(condition.StartsWith("limit_uses_in_date")) {
        if(BattleController.instance.GetBattlerByTargetId(partyMemberId).CheckSkillUsesInDate(usedSkill.id)
           >= int.Parse(conditionArgument)) {
          return false;
        }
      }

      if(condition.StartsWith("limit_uses_in_battle")) {
        if(BattleController.instance.GetBattlerByTargetId(partyMemberId).CheckSkillUsesInBattle(usedSkill.id)
           >= int.Parse(conditionArgument)) {
          return false;
        }
      }
    }
    return true;
  }

}

public static class MyExtensions {
  public static void SetAlpha(this Image img, float alpha) {
    Color c = img.color;
    c.a = alpha;
    img.color = c;
  }

  public static void SetAlpha(this SpriteRenderer renderer, float alpha) {
    Color c = renderer.color;
    c.a = alpha;
    renderer.color = c;
  }
}
