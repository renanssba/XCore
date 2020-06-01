using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  public List<Person> people;
  public Relationship[] relationships;

  public int currentRelationshipId;

  public int boysToGenerate = 5;
  public int girlsToGenerate = 5;

  public float playtime = 0f;

  public int saveToLoad = -1;

  public bool hideTutorials = false;



  public static GlobalData instance;

  void Awake() {
    if(instance == null) {
      instance = this;
    } else if(instance != this) {
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);
  }

  public void InitializeChapter() {
    Person newPerson;
    List<string> usedNames = new List<string>();
    string auxName;

    people = new List<Person>();
    relationships = new Relationship[boysToGenerate + girlsToGenerate - 1];
    currentRelationshipId = 0;

    VsnSaveSystem.SetVariable("money", 0);
    VsnSaveSystem.SetVariable("max_days", 14);
    VsnSaveSystem.SetVariable("observation_played", 0);
    VsnSaveSystem.SetVariable("day", 0);

    for(int i = 0; i < boysToGenerate; i++) {
      auxName = GetNewName(usedNames, true);
      if(ModsManager.instance.GetName(i) != null) {
        auxName = ModsManager.instance.GetName(2 * i);
      }
      newPerson = new Person { isMale = true };
      newPerson.Initialize(i);
      people.Add(newPerson);
    }
    for(int i = 0; i < girlsToGenerate; i++) {
      auxName = GetNewName(usedNames, false);
      if(ModsManager.instance.GetName(5 + i) != null) {
        auxName = ModsManager.instance.GetName(5 + i);
      }
      newPerson = new Person { isMale = false };
      newPerson.Initialize(5 + i);
      people.Add(newPerson);
    }
    usedNames.Clear();
  }

  public void InitializeChapterAlpha() {
    boysToGenerate = 1;
    girlsToGenerate = 3;

    people = new List<Person>();
    Person p = new Person() {
      nameKey = "daniel",
      isMale = true,
      id = 0,
      faceId = 0,
      attributes = new int[] { 5, 5, 5, 4 }
    };
    people.Add(p);
    p = new Person() {
      nameKey = "anna",
      isMale = false,
      id = 1,
      faceId = 5,
      attributes = new int[] { 5, 3, 3, 8 }
    };
    people.Add(p);
    p = new Person() {
      nameKey = "beatrice",
      isMale = false,
      id = 2,
      faceId = 6,
      attributes = new int[] { 4, 7, 4, 3 }
    };
    people.Add(p);
    p = new Person() {
      nameKey = "claire",
      isMale = false,
      id = 3,
      faceId = 7,
      attributes = new int[] { 4, 4, 6, 4 }
    };
    people.Add(p);

    p = new Person() {
      nameKey = "fertiliel",
      isMale = false,
      id = 10,
      faceId = 11
    };
    people.Add(p);

    ResourcesManager.instance.GenerateCharacterSprites(new string[] { "daniel", "anna", "beatrice", "claire", "fertiliel" });


    relationships = new Relationship[3];
    for(int i = 0; i < girlsToGenerate; i++) {
      relationships[i] = new Relationship {
        id = i,
        people = new Person[] { people[0], people[i + 1] }
      };
    }

    /// RELATIONSHIP SKILLTREES
    relationships[0].skilltree.InitializeSkillIds(new int[] { 10, 11, 12, 22, 15, 13, 14, 23, 26, 26, 27, 26, 30 });
    relationships[1].skilltree.InitializeSkillIds(new int[] { 10, 11, 12, 22, 17, 4, 16, 24, 27, 28, 27, 26, 30 });
    relationships[2].skilltree.InitializeSkillIds(new int[] { 10, 11, 12, 22, 19, 20, 21, 25, 26, 27, 27, 26, 30 });


    relationships[1].skilltree.skills[9].affectsPerson = SkillAffectsCharacter.boy;
    

    /// INITIAL INVENTORIES
    /// PLAYER
    //people[0].inventory.AddItem("sports_clothes", 5);
    //people[0].inventory.AddItem("chocolate_cake", 5);
    people[0].inventory.AddItem("sensor", 1);
    //people[0].inventory.AddItem("strawberry_cake", 5);
    //people[0].inventory.AddItem("pepper_cake", 5);

    /// ANA
    people[1].inventory.AddItemWithOwnership("old_teddy_bear", 1, 1);
    people[1].inventory.AddItemWithOwnership("delicate_key", 1, 1);
    people[1].inventory.AddItem("sports_clothes", 1);
    //people[1].inventory.AddItemWithOwnership("sports_clothes", 1, 1);

    /// BEATRICE
    people[2].inventory.AddItemWithOwnership("delicate_key", 1, 2);
    people[2].inventory.AddItemWithOwnership("script_drafts", 1, 2);
    people[2].inventory.AddItemWithOwnership("fancy_dagger", 1, 2);
    people[2].inventory.AddItem("dark_dress", 1);

    /// CLARA
    people[3].inventory.AddItemWithOwnership("flower_dress", 2, 3);
    people[3].inventory.AddItemWithOwnership("delicate_key", 1, 3);
    people[3].inventory.AddItem("chocolate_cake", 1);


    VsnSaveSystem.SetVariable("money", 200);


    // DEBUG CONFESSION SKILL
    //relationships[0].skilltree.skills[12].isUnlocked = true;
    //relationships[1].skilltree.skills[12].isUnlocked = true;
    //relationships[2].skilltree.skills[12].isUnlocked = true;

    //// DEBUG: TESTING SKILLS IN BATTLE
    //relationships[0].exp = 100;
    //relationships[0].level = 7;
    //relationships[0].bondPoints = 10;

    //relationships[1].exp = 100;
    //relationships[1].level = 7;
    //relationships[1].bondPoints = 10;

    //relationships[2].exp = 100;
    //relationships[2].level = 7;
    //relationships[2].bondPoints = 10;
  }


  public string GetNewName(List<string> usedNames, bool isBoy) {
    string newName;

    do {
      newName = (isBoy) ? Utils.GetRandomBoyName() : Utils.GetRandomGirlName();
    } while(usedNames.Contains(newName));

    usedNames.Add(newName);
    return newName;
  }


  public string CurrentCoupleName() {
    if(GetCurrentRelationship() == null) {
      return "";
    }
    return Lean.Localization.LeanLocalization.GetTranslationText("char_name/couple");
  }

  public Person CurrentBoy() {
    if(GetCurrentRelationship() == null) {
      return null;
    }
    return GetCurrentRelationship().GetBoy();
  }

  public Person CurrentGirl() {
    if(GetCurrentRelationship() == null) {
      return null;
    }
    return GetCurrentRelationship().GetGirl();
  }


  public int CurrentCharacterAttribute(int attr) {

    if(BattleController.instance.GetCurrentEnemy() == null) {
      return 0;
    }

    int personId = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    if(personId >= BattleController.instance.partyMembers.Length) {
      return 0;
    }

    Person currentCharacter = BattleController.instance.partyMembers[personId];

    return currentCharacter.AttributeValue(attr);
  }

  public void PassTime() {
    int daytime = VsnSaveSystem.GetIntVariable("daytime");
    if(daytime >= 2) {
      VsnSaveSystem.SetVariable("daytime", 0);
      VsnSaveSystem.SetVariable("day", VsnSaveSystem.GetIntVariable("day") + 1);
    } else {
      VsnSaveSystem.SetVariable("daytime", daytime + 1);
    }
    UIController.instance.UpdateUI();
  }

  public Person GetDateablePerson(Person p) {
    List<Person> dateable = new List<Person>();
    foreach(Person p2 in people) {
      if(p.isMale != p2.isMale) {
        dateable.Add(p2);
      }
    }

    if(dateable.Count > 0) {
      int selected = Random.Range(0, dateable.Count);
      //Debug.Log("Dateable: selected "+selected+" from "+ dateable.Count);
      return dateable[selected];
    } else {
      return null;
    }
  }

  public Relationship[] GetCurrentDateableCouples() {
    List<Relationship> availableCouples = new List<Relationship>();

    foreach(Relationship r in relationships) {
      if(r.level >= 2) {
        availableCouples.Add(r);
      }
    }

    return availableCouples.ToArray();
  }

  public Relationship GetCurrentRelationship() {
    if(currentRelationshipId < 0 || currentRelationshipId >= relationships.Length) {
      return null;
    }    
    return relationships[currentRelationshipId];
  }


  public void AddExpForRelationship(Relationship rel, int expToAdd) {
    UIController.instance.relationshipUpAnimationCard.Initialize(rel);
    UIController.instance.relationshipUpAnimationCard.RaiseExp(expToAdd);
  }

  public Sprite GetFaceByName(string name) {
    if(name == "Fertiliel") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[0];
    }
    if(name == "Graciel") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[1];
    }
    if(name == "Hardiel") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[2];
    }
    if(name == "Carta") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[3];
    }
    if(name == "Valentão" || name == "Delinquente") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[4];
    }
    if(name == "Vendedor") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[5];
    }
    if(name == "Vendedora") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[6];
    }
    if(name == "Fotógrafo") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[7];
    }
    if(name == "Cachorro-quente") {
      return ResourcesManager.instance.fixedCharactersFaceSprites[8];
    }
    if(name == "Ana Sombria") {
      return ResourcesManager.instance.faceSprites[2];
    }
    if(name == "Beatrice Sombria") {
      return ResourcesManager.instance.faceSprites[3];
    }
    if(name == "Clara Sombria") {
      return ResourcesManager.instance.faceSprites[4];
    }
    foreach(Person p in people) {
      if(p.GetName() == name) {
        return ResourcesManager.instance.faceSprites[p.faceId];
      }
    }
    return null;
  }


  public void SavePersistantGlobalData() {
    VsnSaveSystem.SetVariable("playtime", playtime);

    SaveInventories();
    SaveRelationships();
  }

  public void SaveInventories() {
    InventorySaveStruct inv = new InventorySaveStruct();

    //Debug.LogWarning("SAVING INVENTORIES");
    for(int i=0; i<people.Count; i++) {
      inv.itemListings = people[i].inventory.itemListings;
      VsnSaveSystem.SetVariable("person"+i+"_inventory", JsonUtility.ToJson(inv));
      //Debug.LogWarning("JSON person "+i+" inventory: " + JsonUtility.ToJson(inv));

      inv.itemListings = people[i].giftsReceived.itemListings;
      VsnSaveSystem.SetVariable("person" + i + "_gifts_received", JsonUtility.ToJson(inv));
    }
  }

  public void SaveRelationships() {
    RelationshipSaveStruct inv;

    Debug.LogWarning("SAVING RELATIONSHIPS");
    for(int i = 0; i < relationships.Length; i++) {
      inv = new RelationshipSaveStruct(relationships[i]);
      VsnSaveSystem.SetVariable("relationship_" + i, JsonUtility.ToJson(inv));
      Debug.LogWarning("JSON relationship "+i+": " + JsonUtility.ToJson(inv));
    }
  }


  public void LoadPersistantGlobalData() {
    playtime = VsnSaveSystem.GetFloatVariable("playtime");
    LoadInventories();
    LoadRelationships();
  }

  public void LoadInventories() {
    InventorySaveStruct inv = new InventorySaveStruct();

    //Debug.LogWarning("LOADING INVENTORIES");
    for(int i = 0; i < people.Count; i++) {
      inv = JsonUtility.FromJson<InventorySaveStruct>(VsnSaveSystem.GetStringVariable("person" + i + "_inventory"));
      people[i].inventory.itemListings = inv.itemListings;

      inv = JsonUtility.FromJson<InventorySaveStruct>(VsnSaveSystem.GetStringVariable("person" + i + "_gifts_received"));
      people[i].giftsReceived.itemListings = inv.itemListings;
    }
  }

  public void LoadRelationships() {
    RelationshipSaveStruct inv;

    Debug.LogWarning("LOADING RELATIONSHIPS");
    for(int i = 0; i < relationships.Length; i++) {
      inv = JsonUtility.FromJson<RelationshipSaveStruct>(VsnSaveSystem.GetStringVariable("relationship_" + i));
      relationships[i].LoadFromStruct(inv);
      Debug.LogWarning("JSON relationship " + i + ": " + JsonUtility.ToJson(inv));
    }
  }
}
