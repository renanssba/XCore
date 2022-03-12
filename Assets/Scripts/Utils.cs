using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum StageName {
  TitleScreen
};


public class Utils {
  public static void UnselectButton() {
    EventSystem.current.SetSelectedGameObject(null);
  }

  public static void SetButtonDisabledGraphics(Button but) {
    SpriteState st = new SpriteState();
    st.disabledSprite = ResourcesManager.instance.buttonSprites[3];
    st.highlightedSprite = ResourcesManager.instance.buttonSprites[4];
    st.pressedSprite = ResourcesManager.instance.buttonSprites[5];
    but.spriteState = st;
    but.GetComponent<Image>().sprite = ResourcesManager.instance.buttonSprites[3];
    //Debug.LogWarning("Setting ");
  }

  public static void SetButtonEnabledGraphics(Button but) {
    SpriteState st = new SpriteState();
    st.disabledSprite = ResourcesManager.instance.buttonSprites[0];
    st.highlightedSprite = ResourcesManager.instance.buttonSprites[1];
    st.pressedSprite = ResourcesManager.instance.buttonSprites[2];
    but.spriteState = st;
    but.GetComponent<Image>().sprite = ResourcesManager.instance.buttonSprites[0];
  }



  public static string GetPrintableString(string name, bool capitalLetters) {
    char initialLetter = name[0];

    for(int i = 1; i < name.Length; i++) {
      if(System.Char.IsUpper(name[i])) {
        name = name.Substring(0, i) + " " + System.Char.ToLower(name[i]) + name.Substring(i + 1, name.Length - i - 1);
      }
    }

    if(capitalLetters)
      return initialLetter.ToString().ToUpper() + name.Substring(1, name.Length - 1);
    else
      return name;
  }

  public void ShuffleList(List<int> list) {
    int n = list.Count;
    while(n > 1) {
      n--;
      int k = UnityEngine.Random.Range(0, n + 1);
      int value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
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
    for(int i = 0; i < loadedTags.Length; i++) {
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
    for(int i = 0; i < loadedTags.Length; i++) {
      ints[i] = int.Parse(loadedTags[i].Trim());
    }
    return ints;
  }

  public static string GetStringArgument(string clause) {
    int start = clause.IndexOf("(");
    int end = clause.IndexOf(")");

    if(start == -1 || end == -1) {
      return null;
    }

    string argumentName = clause.Substring(start + 1, (end - start - 1));
    Debug.Log("GET ARGUMENT: '" + argumentName + "'  ");
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


  public static bool AreAllConditionsMet(Skill usedSkill, string[] allConditions, SkillTarget skillUserId) {
    string conditionArgument;
    int currentPlayerTurn = VsnSaveSystem.GetIntVariable("currentPlayerTurn");
    Battler user = BattleController.instance.GetBattlerByTargetId(skillUserId);
    Battler main = BattleController.instance.GetBattlerByTargetId(SkillTarget.partyMember1);


    foreach(string condition in allConditions) {
      conditionArgument = GetStringArgument(condition);

      //Debug.LogWarning("Check for skill activation. Checking condition: " + condition);

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

      if(condition.StartsWith("turn_minimum")) {
        if(VsnSaveSystem.GetIntVariable("currentBattleTurn") < int.Parse(conditionArgument)) {
          return false;
        }
      }

      if(condition.StartsWith("turn_is_multiple")) {
        if(VsnSaveSystem.GetIntVariable("currentBattleTurn") % int.Parse(conditionArgument) != 0) {
          return false;
        }
      }

      // TODO: improve this with a more thorough/accurate checking
      if(condition.StartsWith("cured_status")) {
        if(BattleController.instance.selectedActionType[currentPlayerTurn] == TurnActionType.useItem &&
           BattleController.instance.selectedTargetPartyId[currentPlayerTurn] == skillUserId &&
           TagIsInArray(conditionArgument, BattleController.instance.selectedItems[currentPlayerTurn].healsConditionNames)) {
          continue;
        }
        if(BattleController.instance.selectedActionType[currentPlayerTurn] == TurnActionType.useSkill &&
           BattleController.instance.selectedTargetPartyId[currentPlayerTurn] == skillUserId &&
           TagIsInArray(conditionArgument, BattleController.instance.selectedSkills[currentPlayerTurn].healsConditionNames)) {
          continue;
        }
        return false;
      }

      if(condition.StartsWith("received_item_with_tag")) {
        //Debug.LogWarning("Checking received_item_with_tag. Action: " + BattleController.instance.selectedActionType[currentPlayerTurn]);
        if(BattleController.instance.selectedActionType[currentPlayerTurn] != TurnActionType.useItem) {
          return false;
        }
        //Debug.LogWarning("Condition argument: " + conditionArgument);
        if(!TagIsInArray(conditionArgument, BattleController.instance.selectedItems[currentPlayerTurn].tags) ||
           BattleController.instance.selectedTargetPartyId[currentPlayerTurn] != skillUserId) {
          return false;
        }
      }

      if(condition.StartsWith("hp_percent_is_less")) {
        float hpPercent = ((float)user.CurrentHP()) / (float)user.MaxHP();
        //Debug.LogWarning("Current HP percent: " + hpPercent + ". argument: " + float.Parse(conditionArgument));
        if(hpPercent > float.Parse(conditionArgument)) {
          return false;
        }
      }

      if(condition.StartsWith("main_hp_percent_is_less")) {
        float hpPercent = ((float)main.CurrentHP()) / (float)main.MaxHP();
        //Debug.LogWarning("Current HP percent: " + hpPercent + ". argument: " + float.Parse(conditionArgument));
        if(hpPercent > float.Parse(conditionArgument)) {
          return false;
        }
      }

      if(condition.StartsWith("has_status_condition")) {
        if(user.CurrentStatusConditionStacks(conditionArgument) <= 0) {
          return false;
        }
      }

      if(condition.StartsWith("dont_have_status_condition")) {
        if(user.CurrentStatusConditionStacks(conditionArgument) > 0) {
          return false;
        }
      }

      if(condition == "was_attacked_for_damage") {
        switch(skillUserId) {
          case SkillTarget.enemy1:
          case SkillTarget.enemy2:
          case SkillTarget.enemy3:
            if(BattleController.instance.selectedActionType[currentPlayerTurn] != TurnActionType.useSkill) {
              return false;
            }
            if(BattleController.instance.selectedSkills[currentPlayerTurn].damagePower <= 0) {
              return false;
            }
            break;
        }
      }

      if(condition == "was_attacked") {
        switch(skillUserId) {
          case SkillTarget.enemy1:
          case SkillTarget.enemy2:
          case SkillTarget.enemy3:
            if(BattleController.instance.selectedActionType[currentPlayerTurn] != TurnActionType.useSkill) {
              return false;
            }
            if(BattleController.instance.selectedTargetPartyId[currentPlayerTurn] != skillUserId) {
              return false;
            }
            break;
        }
      }


      if(condition == "ally_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") == (int)skillUserId) {
        return false;
      }

      if(condition == "self_targeted" && VsnSaveSystem.GetIntVariable("enemyAttackTargetId") != (int)skillUserId) {
        return false;
      }

      if(condition == "defending" && !TheaterController.instance.GetActorByIdInParty(skillUserId).battler.IsDefending()) {
        return false;
      }


      /// CHECKING FOR SKILL-DEPENDANT TRIGGERS
      if(usedSkill == null) {
        continue;
      }

      if(condition.StartsWith("limit_uses_in_battle")) {
        if(user.CheckSkillUsesInBattle(usedSkill.id) >= int.Parse(conditionArgument)) {
          return false;
        }
      }

      if(condition.StartsWith("cooldown")) {
        if(user.CheckTimeSinceLastUsedSkill(usedSkill.id) < int.Parse(conditionArgument)) {
          return false;
        }
      }
    }
    return true;
  }

  public static string GetTimeFormattedAsString(float timePassed) {
    string timeString = "";
    int timeInt = (int)timePassed;

    timeString = (timeInt / 60).ToString();

    timeString += ":";

    if(timeInt % 60 < 10) {
      timeString += "0" + (timeInt % 60);
    } else {
      timeString += (timeInt % 60).ToString();
    }

    return timeString;
  }
}

public static class MyExtensions {
  private static System.Random rng = new System.Random();

  public static void Shuffle<T>(this IList<T> list) {
    int n = list.Count;
    while(n > 1) {
      n--;
      int k = rng.Next(n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }

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

  public static Vector2 Rotate(this Vector2 v, float degrees) {
    float radians = degrees * Mathf.Deg2Rad;
    float sin = Mathf.Sin(radians);
    float cos = Mathf.Cos(radians);

    float tx = v.x;
    float ty = v.y;

    return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
  }

  public static Vector3 Rotate(this Vector3 v, float degrees) {
    float radians = degrees * Mathf.Deg2Rad;
    float sin = Mathf.Sin(radians);
    float cos = Mathf.Cos(radians);

    float tx = v.x;
    float ty = v.y;

    return new Vector3(cos * tx - sin * ty, sin * tx + cos * ty, 0f);
  }

  public static Vector2 ReflectFromNormal(this Vector2 v, Vector2 normal) {
    return v - 2 * Vector2.Dot(v, normal) * normal;
  }

  public static Toggle GetSelected(this ToggleGroup group) {
    foreach(Toggle t in group.ActiveToggles()) {
      if(t.isOn) {
        return t;
      }
    }
    return null;
  }

  public static int GetSelectedId(this ToggleGroup group) {
    int count = 0;
    foreach(Toggle t in group.ActiveToggles()) {
      if(t.isOn) {
        return count;
      }
      count++;
    }
    return -1;
  }


  public static Vector3 SnapToGrid(this Vector3 position) {
    return new Vector3(Mathf.FloorToInt(position.x + 0.5f), Mathf.FloorToInt(position.y + 0.5f), 0f);
  }

  public static Vector3Int SnapToGridInt(this Vector3 position) {
    return new Vector3Int(Mathf.FloorToInt(position.x + 0.5f), Mathf.FloorToInt(position.y + 0.5f), 0);
  }

  public static Vector2 SnapToGrid(this Vector2 position) {
    return new Vector2(Mathf.FloorToInt(position.x + 0.5f), Mathf.FloorToInt(position.y + 0.5f));
  }

  public static Vector2Int SnapToGridInt(this Vector2 position) {
    return new Vector2Int(Mathf.FloorToInt(position.x + 0.5f), Mathf.FloorToInt(position.y + 0.5f));
  }

  public static Vector3 GridToWorldPosition(this Vector3Int tilePosition) {
    return new Vector3((float)tilePosition.x - 0.5f, (float)tilePosition.y - 0.5f, 0f);
  }

  public static Vector2 GridToWorldPosition(this Vector2Int tilePosition) {
    return new Vector2((float)tilePosition.x - 0.5f, (float)tilePosition.y - 0.5f);
  }

  // return the angle in a float format: from 0 to 360
  public static float GetAngleRotationZeroToThreeSixty(this Vector2 dist) {
    float angle = Vector2.Angle(dist, Vector2.right);
    if(dist.y < 0f) {
      return 360f - angle;
    }
    return angle;
  }

  public static Vector3Int ToVector3Int(this Vector2Int value) {
    return new Vector3Int(value.x, value.y, 0);
  }

  public static string CleanObjectName(this string value) {
    int index = value.IndexOf("(");
    if(index < 0) {
      return value;
    }
    return value.Substring(0, index).Trim();
  }


  public static int CountNonNull(this GameObject[] array) {
    int count = 0;
    for(int i = 0; i < array.Length; i++) {
      if(array[i] != null) count++;
    }
    return 1;
  }

  public static int CountNull(this GameObject[] array) {
    int count = 0;
    for(int i = 0; i < array.Length; i++) {
      if(array[i] == null) count++;
    }
    return 1;
  }

  public static GameObject FindObject(this GameObject parent, string name) {
    Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
    foreach(Transform t in trs) {
      if(t.name == name) {
        return t.gameObject;
      }
    }
    return null;
  }

  public static bool Contains(this List<Vector3> list, Vector3 pos) {
    foreach(Vector3 v in list) {
      if(Vector3.SqrMagnitude(v - pos) < 0.001f) {
        return true;
      }
    }
    return false;
  }

  public static bool Contains(this string[] array, string text) {
    foreach(string s in array) {
      if(s == text) {
        return true;
      }
    }
    return false;
  }

  public static int FindId(this Material[] array, Material mat) {
    for(int i = 0; i < array.Length; i++) {
      if(array[i] == mat) {
        return i;
      }
    }
    return -1;
  }

  public static int FindId(this Sprite[] array, Sprite sprite) {
    for(int i = 0; i < array.Length; i++) {
      if(array[i] == sprite) {
        return i;
      }
    }
    return -1;
  }


  public static void ClearChildren(this Transform content) {
    int childCount = content.childCount;

    for(int i = 0; i < childCount; i++) {
      GameObject.Destroy(content.GetChild(i).gameObject);
    }
  }

  public static string ToTitleCase(this string name) {
    return name[0].ToString().ToUpper() + name.ToString().Substring(1, name.Length - 1).ToLower();
  }

  public static TileBase GetTile(this Tilemap tilemap, Vector2Int pos) {
    return tilemap.GetTile(new Vector3Int(pos.x, pos.y, 0));
  }

  public static Vector3 CellToWorld(this Tilemap tilemap, Vector2Int cellPosition) {
    return tilemap.CellToWorld(cellPosition.ToVector3Int());
  }

  public static Vector3 CellToWorld(this Grid grid, Vector2Int cellPosition) {
    return grid.CellToWorld(cellPosition.ToVector3Int());
  }

  public static void SetTile(this Tilemap tilemap, Vector2Int position, TileBase tile) {
    tilemap.SetTile(position.ToVector3Int(), tile);
  }
}

public static class TweenAnimations {
  public static void AnimTweenPopAppear(this Transform t) {
    t.gameObject.transform.localScale = Vector3.zero;
    t.transform.DOScale(1.1f, 0.5f).SetUpdate(true)
      .OnComplete(() => {
        t.transform.DOScale(1f, 0.2f).SetUpdate(true);
      });
  }

  public static void SubtlePulseAnimation(this Transform t) {
    DOTween.Kill(t.gameObject.transform);
    t.gameObject.transform.localScale = Vector3.one;
    t.transform.DOScale(1.03f, 0.12f).SetUpdate(true)
      .OnComplete(() => {
        t.transform.DOScale(1f, 0.12f).SetUpdate(true);
      });
  }
}
