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
  navegarNaDeepweb,
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
  namorarPelado,
  fazerArranjoDeFlores,
  calcinhas,
  caridade,
  fazerDinheiro,
  criancas,
  empreendedorismo,
  tristeza,
  usarLingerie,
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
}

