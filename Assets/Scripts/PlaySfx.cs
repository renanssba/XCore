using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySfx : MonoBehaviour {

  public string sfxName;

	public void PlaySfxName() {
    VsnAudioManager.instance.PlaySfx(sfxName);
  }
}
