using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class EnemyAnimationTestPlayer : MonoBehaviour {

#if UNITY_EDITOR
  public Actor2D[] enemies;
  public UnityEngine.UI.Dropdown enemyList;
  public UnityEngine.UI.Dropdown stateList;
  public UnityEngine.UI.Button playAgainButton;
  private GameObject enemyDisplay;
  private Animator enemyAnimator;

  // Use this for initialization
  void Start () {
    //print(Resources.FindObjectsOfTypeAll<Actor2D>().Length);
    //Resources.
    List<string> enemyNames = new List<string>();
    foreach(Actor2D en in enemies)
    {
      enemyNames.Add(en.gameObject.name);
    }
    enemyList.ClearOptions();
    enemyList.AddOptions(enemyNames);

    enemyList.onValueChanged.AddListener(LoadEnemy);
    enemyDisplay = new GameObject("Enemy Display");
    enemyDisplay.transform.parent = this.transform;
    stateList.onValueChanged.AddListener(PlayState);
    playAgainButton.onClick.AddListener(PlayCurrentState);
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void LoadEnemy(int id) {
    print(enemies[id].gameObject.name);
    Destroy(enemyDisplay);
    enemyDisplay = Instantiate(enemies[id].gameObject);
    enemyDisplay.transform.parent = this.transform;
    LoadAnimationStateList(enemyDisplay);
  }

  void LoadAnimationStateList(GameObject enemy) {
    enemyAnimator = enemy.transform.Find("Body").GetComponent<Animator>();
    stateList.ClearOptions();
    stateList.AddOptions(GetAllAnimationStates(enemyAnimator));
  }

  List<string> GetAllAnimationStates(Animator animator) {
    AnimatorControllerLayer[] animatorLayers = GetAnimatorController(animator).layers;

    List<string> stateNames = new List<string>();

    for(int i = 0; i < animatorLayers.Length; i++)
    {
      ChildAnimatorState[] states = animatorLayers[i].stateMachine.states;

      for(int j = 0; j < states.Length; j++)
      {
        print(states[j].state.name);
          stateNames.Add(states[j].state.name);
      }
    }
    return stateNames;
  }

  AnimatorController GetAnimatorController(Animator animator) {

    var runtimeController = animator.runtimeAnimatorController;
    AnimatorController controller;
    if(runtimeController == null)
    {
      Debug.LogErrorFormat("RuntimeAnimatorController must not be null.");
      return null;
    }
    string path = UnityEditor.AssetDatabase.GetAssetPath(runtimeController);
    if(path.Contains(".overrideController"))
    {
      AnimatorOverrideController over = runtimeController as AnimatorOverrideController;
      controller = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(over.runtimeAnimatorController));
    }
    else
    {
      controller = runtimeController as AnimatorController;
    }
    if(controller == null)
    {
      Debug.LogErrorFormat("AnimatorController must not be null.");
      return null;
    }

    return controller;
  }

  void PlayState(int id) {
    enemyAnimator.Play(stateList.options[id].text);
  }
  void PlayCurrentState() {
    enemyAnimator.Play(stateList.options[stateList.value].text);
  }
  private void OnDestroy() {
    enemyList.onValueChanged.RemoveListener(LoadEnemy);
    stateList.onValueChanged.RemoveListener(PlayState);
  }
#endif
}
