using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEmitter : MonoBehaviour {

  public void PlaySfx(string sfxName) {
    VsnAudioManager.instance.PlaySfx(sfxName);
  }
}
