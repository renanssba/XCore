using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour {

	public void PlayConfirmSfx() {
    VsnAudioManager.instance.PlaySfx("ui_confirm");
  }
  public void PlayBigConfirmSfx() {
    VsnAudioManager.instance.PlaySfx("ui_big_confirm");
  }

  public void PlaySelectSfx() {
    VsnAudioManager.instance.PlaySfx("ui_select2");
  }

  public void PlayCancelSfx() {
    //VsnAudioManager.instance.PlaySfx("ui_locked");
  }

  public void PlayForbbidenSfx() {
    VsnAudioManager.instance.PlaySfx("ui_locked");
  }
}
