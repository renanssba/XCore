using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour {

	public void PlayConfirmSfx() {
    StaticPlayConfirmSfx();
  }
  public void PlayBigConfirmSfx() {
    StaticPlayBigConfirmSfx();
  }

  public void PlaySelectSfx() {
    StaticPlaySelectSfx();
  }

  public void PlayCancelSfx() {
    StaticPlayCancelSfx();
  }

  public void PlayForbbidenSfx() {
    StaticPlayForbbidenSfx();
  }



  public static void StaticPlayConfirmSfx() {
    VsnAudioManager.instance.PlaySfx("ui_confirm");
  }

  public static void StaticPlayBigConfirmSfx() {
    VsnAudioManager.instance.PlaySfx("ui_big_confirm");
  }

  public static void StaticPlaySelectSfx() {
    VsnAudioManager.instance.PlaySfx("ui_select2");
  }

  public static void StaticPlayCancelSfx() {
    VsnAudioManager.instance.PlaySfx("ui_back");
  }

  public static void StaticPlayForbbidenSfx() {
    VsnAudioManager.instance.PlaySfx("ui_locked");
  }
}
