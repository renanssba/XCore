using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum StageName {
  TitleScreen,
  CityMap,
  Investigation,
  CustomizeScreen
};

public enum SongNames {
  conceito_loop,
  sewers_loop,
  night_loop,
  conceito_full
};

public enum LikeLevel {
  hates = -1,
  neutral = 0,
  loves = 1
};

public enum MainTastes {
  animais = 0,
  esportes = 1,
  videogames = 2,
  livros = 3,
  trending = 4
};


public enum CharacterTraits {
  estudioso,
  correTerEQui,
  shoppingSegEQua,
  delinquente,
  menteResistente,
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
  Ammanda,
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
  hackear,
  imitarTartarugas,
  arquitetura,
  artesanato,
  andarACavalo,
  rezar,
  meditacao,
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
  //  vidaDosOutros,
  //pudim,
  //tortas,
  //agua,
  natacao,
  tomarBanho,
  usarAppsDeNamoro,
  yaoi,
  //waifus,
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
  fazerPoses
}

[System.Serializable]
public class Suggestion {
  public SuggestionType type;
  public int target;
  public int topic;
  public string phrase;

  //public Suggestion(int newTarget, SuggestionType suggestionType, int newTopic, string newPhrase){
  //  target = newTarget;
  //  type = suggestionType;
  //  topic = newTopic;
  //  phrase = newPhrase;
  //}
};

[System.Serializable]
public class Opinion {
  public int personEncounterId;
  public int topic;
  public LikeLevel likeValue;

  public Opinion(int id, int newTopic, LikeLevel newLikeValue) {
    personEncounterId = id;
    topic = newTopic;
    likeValue = newLikeValue;
  }
};

public enum SuggestionType {
  replyTaste = 1,
  replyHobby = 2,
  replyCrush = 3,
  changeSubject = 4,
  giveGift = 5,
  leaveEncounter = 6,
  invite = 7,
  replyHate = 8,
  talkAboutHobby = 9,
  talkAboutHate = 10,
  talkAboutCrush = 11,
  sayLikeTasteEvaluated = 12,
  sayHateTasteEvaluated = 13
}

public enum MatterTypes {
  ask_opinion = 1,
  whats_up = 2,
  ask_crush = 3,
  what_you_hate = 4,
  specific_taste_like = 5,
  specific_taste_hate = 6,
  be_quiet = 5
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

  public static LikeLevel GetRandomTasteValue() {
    int selected = Random.Range(-1, 2);
    LikeLevel value = (LikeLevel)selected;

    return value;
  }

  public static int LevelOfAgree(LikeLevel taste1, LikeLevel taste2) {
    int a = (int)taste1;
    int b = (int)taste2;

    if (a == b &&
       (taste1 == LikeLevel.loves || taste1 == LikeLevel.hates)) {
      return 1;
    }

    if ((taste1 == LikeLevel.hates && taste2 == LikeLevel.loves) ||
       (taste1 == LikeLevel.loves && taste2 == LikeLevel.hates)) {
      return -1;
    }

    return 0;
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

  public static string GetPrintableTrait(string traitName, bool capitalLetters) {
    if (traitName.Length > 7) {
      switch (traitName.Substring(traitName.Length - 7, 7)) {
        case "SegEQua":
          return GetPrintableString(traitName.Substring(0, traitName.Length - 7), capitalLetters) + " (Seg/Qua)";
        case "TerEQui":
          return GetPrintableString(traitName.Substring(0, traitName.Length - 7), capitalLetters) + " (Ter/Qui)";
      }
    }

    return GetPrintableString(traitName, capitalLetters);
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

