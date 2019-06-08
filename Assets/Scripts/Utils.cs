using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum StageName {
  TitleScreen,
  Gameplay,
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

public enum RandomBoyPtBr {
  Alberto,
  Jorge,
  Daniel,
  Nando,
  Pedro,
  Gustavo,
  Humberto,
  Jonas,
  Luan,
  Renan,
  Roberto
}

public enum RandomGirlPtBr {
  Amanda,
  Bianca,
  Carla,
  Mariane,
  Fernanda,
  Gabriela,
  Priscila,
  Larissa,
  Jade,
  Valentina,
  Juliana,
  Paola
}

public enum RandomBoyEng {
  Albert,
  George,
  Daniel,
  Peter,
  Charlie,
  Joe,
  Robert,
  Oliver,
  Harry,
  Jacob
}

public enum RandomGirlEng {
  Chloe,
  Olivia,
  Emily,
  Amelia,
  Sophia,
  Lily,
  Charlotte,
  Jade,
  Mia,
  Ruby,
  Daisy
}


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
  oSetindoDaVida,
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
    if (VsnSaveSystem.GetStringVariable("language") == "pt_br") {
      int selected = Random.Range(0, System.Enum.GetNames(typeof(RandomBoyPtBr)).Length);
      return System.Enum.GetName(typeof(RandomBoyPtBr), selected);
    } else {
      int selected = Random.Range(0, System.Enum.GetNames(typeof(RandomBoyEng)).Length);
      return System.Enum.GetName(typeof(RandomBoyEng), selected);
    }
  }

  public static string GetRandomGirlName() {
    if(VsnSaveSystem.GetStringVariable("language") == "pt_br"){
      int selected = Random.Range(0, System.Enum.GetNames(typeof(RandomGirlPtBr)).Length);
      return System.Enum.GetName(typeof(RandomGirlPtBr), selected);
    } else{
      int selected = Random.Range(0, System.Enum.GetNames(typeof(RandomGirlEng)).Length);
      return System.Enum.GetName(typeof(RandomGirlEng), selected);
    }
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
  }
}

