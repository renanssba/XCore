using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

  [Header("- Pilots and Relationships -")]
  public TextAsset pilotsFile;
  public List<Pilot> pilots;
  public Relationship[] relationships;

  public int currentRelationshipId;

  [Header("- System Data -")]
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
    currentRelationshipId = 0;

    VsnSaveSystem.SetVariable("money", 0);
    VsnSaveSystem.SetVariable("max_days", 14);
    VsnSaveSystem.SetVariable("observation_played", 0);
    VsnSaveSystem.SetVariable("day", 0);

    pilots = new List<Pilot>();

    SpreadsheetData spreadsheetData = SpreadsheetReader.ReadTabSeparatedFile(pilotsFile, 1);
    

    foreach(Dictionary<string, string> dic in spreadsheetData.data) {
      Pilot newPilot = new Pilot();

      newPilot.id = int.Parse(dic["id"]);
      newPilot.nameKey = dic["nameKey"];
      newPilot.isMale = dic["isMale"] == "TRUE";
      newPilot.attributes = new int[]{
        int.Parse(dic["maxHp"]),
        int.Parse(dic["movementRange"]),
        int.Parse(dic["attack"]),
        int.Parse(dic["agility"]),
        int.Parse(dic["dodgeRate"])};

      pilots.Add(newPilot);
    }

    ResourcesManager.instance.GenerateCharacterSprites(new string[] {"marcus", "agnes", "maya"});


    relationships = new Relationship[3];
    for(int i = 0; i < 2; i++) {
      relationships[i] = new Relationship {
        id = i,
        people = new Pilot[] { pilots[0], pilots[i + 1] }
      };
    }
    relationships[2] = new Relationship {
      id = 2,
      people = new Pilot[] { pilots[1], pilots[2] }
    };

    /// RELATIONSHIP SKILLTREES
    relationships[0].skilltree.InitializeSkillIds(new int[] { 0 });
    relationships[1].skilltree.InitializeSkillIds(new int[] { 0 });
    relationships[2].skilltree.InitializeSkillIds(new int[] { 0 });
  }


  public int CurrentCharacterAttribute(int attr) {

    if(BattleController.instance.GetCurrentEnemy() == null) {
      return 0;
    }

    int personId = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    if(personId >= BattleController.instance.partyMembers.Length) {
      return 0;
    }

    Pilot currentCharacter = BattleController.instance.partyMembers[personId];

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

  public Pilot GetDateablePerson(Pilot p) {
    List<Pilot> dateable = new List<Pilot>();
    foreach(Pilot p2 in pilots) {
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
    for(int i=0; i<pilots.Count; i++) {
      inv.itemListings = pilots[i].inventory.itemListings;
      VsnSaveSystem.SetVariable("person"+i+"_inventory", JsonUtility.ToJson(inv));
      //Debug.LogWarning("JSON person "+i+" inventory: " + JsonUtility.ToJson(inv));

      inv.itemListings = pilots[i].giftsReceived.itemListings;
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
    for(int i = 0; i < pilots.Count; i++) {
      inv = JsonUtility.FromJson<InventorySaveStruct>(VsnSaveSystem.GetStringVariable("person" + i + "_inventory"));
      pilots[i].inventory.itemListings = inv.itemListings;

      inv = JsonUtility.FromJson<InventorySaveStruct>(VsnSaveSystem.GetStringVariable("person" + i + "_gifts_received"));
      pilots[i].giftsReceived.itemListings = inv.itemListings;
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
