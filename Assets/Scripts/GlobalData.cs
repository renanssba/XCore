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


  public void Start() {
    InitializeChapter();
  }

  public void InitializeChapter() {
    if(VsnSaveSystem.GetBoolVariable("global_data_initialized") == true) {
      return;
    }
    VsnSaveSystem.SetVariable("global_data_initialized", true);

    currentRelationshipId = 0;

    VsnSaveSystem.SetVariable("money", 0);
    VsnSaveSystem.SetVariable("max_days", 14);
    VsnSaveSystem.SetVariable("observation_played", 0);
    VsnSaveSystem.SetVariable("day", 0);
    

    boysToGenerate = 1;
    girlsToGenerate = 2;

    people = new List<Person>();
    Person p = new Person() {
      nameKey = "marcus",
      isMale = true,
      id = 0,
      faceId = 0,
      attributes = new int[] { 2, 2, 2, 0 }
    };
    people.Add(p);
    p = new Person() {
      nameKey = "agnes",
      isMale = false,
      id = 1,
      faceId = 5,
      attributes = new int[] { 5, 5, 5, 0 }
    };
    people.Add(p);
    p = new Person() {
      nameKey = "maya",
      isMale = false,
      id = 2,
      faceId = 6,
      attributes = new int[] { 3, 3, 3, 0 }
    };
    people.Add(p);

    ResourcesManager.instance.GenerateCharacterSprites(new string[] {"marcus", "agnes", "maya"});


    relationships = new Relationship[3];
    for(int i = 0; i < girlsToGenerate; i++) {
      relationships[i] = new Relationship {
        id = i,
        people = new Person[] { people[0], people[i + 1] }
      };
    }
    relationships[2] = new Relationship {
      id = 2,
      people = new Person[] { people[1], people[2] }
    };

    /// RELATIONSHIP SKILLTREES
    relationships[0].skilltree.InitializeSkillIds(new int[] { 10, 11, 12, 22, 15, 13, 14, 23, 26, 26, 27, 26, 30 });
    relationships[1].skilltree.InitializeSkillIds(new int[] { 10, 11, 12, 22, 31, 4, 32, 29, 27, 28, 27, 26, 30 });
    relationships[2].skilltree.InitializeSkillIds(new int[] { 10, 11, 12, 22, 31, 4, 32, 29, 27, 28, 27, 26, 30 });

    relationships[1].skilltree.skills[9].affectsPerson = SkillAffectsCharacter.boy;
    //VsnSaveSystem.SetVariable("money", 200);
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

  public AudioClip GetDialogSfxByName(string name) {
    foreach(VsnCharacterData chara in ResourcesManager.instance.vsnCharacterData) {
      if(name == Lean.Localization.LeanLocalization.GetTranslationText(chara.nameKey)) {
        return chara.dialogSfx;
      }
    }
    return null;
  }

  public float GetPitchByName(string name) {
    foreach(VsnCharacterData chara in ResourcesManager.instance.vsnCharacterData) {
      if(name == Lean.Localization.LeanLocalization.GetTranslationText(chara.nameKey)) {
        return chara.pitch;
      }
    }
    return 1f;
  }

  public Sprite GetFaceByName(string name) {
    foreach(VsnCharacterData chara in ResourcesManager.instance.vsnCharacterData) {
      if(name == Lean.Localization.LeanLocalization.GetTranslationText(chara.nameKey)) {
        return chara.faceSprite;
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
