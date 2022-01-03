using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;


public class OptionsPanel : Panel {
  [Header("Graphics Options")]
  public Toggle fullscreenToggle;
  public TMP_Dropdown resolutionDropdown;
  public TextMeshProUGUI resolutionText;

  public AudioMixer mixer;

  private Resolution windowedResolution;

  [Header("Volume Options")]
  public Slider masterVolumeSlider;
  public Slider musicVolumeSlider;
  public Slider sfxVolumeSlider;

  //[Header("Gameplay Options")]
  //public Toggle skipTutorialsToggle;



  public Resolution SelectedResolution {
    get {
      Resolution r = new Resolution();
      //Debug.LogWarning("Resolution selected: " + resolutionText.text);
      string[] dimensions = resolutionText.text.Split('x');

      r.width = int.Parse(dimensions[0]);
      r.height = int.Parse(dimensions[1]);
      return r;
    }
  }


  public override void PreShowPanel() {
    float groupVolume = GetGroupVolume(VsnAudioManager.masterVolumeParameter);
    float converted = ConvertGroupVolumeToSliderValue(groupVolume);
    Debug.LogWarning("groupVolume: "+groupVolume+". converted: "+converted);

    //masterVolumeSlider.value = converted;
    masterVolumeSlider.value = ConvertGroupVolumeToSliderValue(GetGroupVolume(VsnAudioManager.masterVolumeParameter));
    musicVolumeSlider.value = ConvertGroupVolumeToSliderValue(GetGroupVolume(VsnAudioManager.musicVolumeParameter));
    sfxVolumeSlider.value = ConvertGroupVolumeToSliderValue(GetGroupVolume(VsnAudioManager.sfxVolumeParameter));

    InitializeResolutions();
    fullscreenToggle.isOn = GetIsFullscreen();
    GetFullscreenResolution();
  }


  public void OnFullscreenToggleChanged() {
    if(fullscreenToggle.isOn) {
      //Screen.SetResolution(GetFullscreenResolution().width, GetFullscreenResolution().height, true);
      Screen.SetResolution(SelectedResolution.width, SelectedResolution.height, true);
      //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    } else {
      //Screen.SetResolution(windowedResolution.width, windowedResolution.height, false);
      // TODO: consider a good windowed resolution
      //Screen.SetResolution(1280, 720, false);
      Screen.SetResolution(SelectedResolution.width, SelectedResolution.height, false);
      //Screen.fullScreenMode = FullScreenMode.Windowed;
    }
  }

  public void InitializeResolutions() {
    Resolution[] resolutions = Screen.resolutions;
    List<string> displayOptions = new List<string>();
    string newRes;
    int selectedId = 0;
    string currentResolution = Screen.width.ToString() + "x" + Screen.height;

    // Print the resolutions
    foreach(Resolution res in resolutions) {
      newRes = res.width.ToString() + "x" + res.height;
      if(!displayOptions.Contains(newRes)) {
        displayOptions.Add(newRes);
        if(newRes == currentResolution) {
          selectedId = displayOptions.Count - 1;
        }
      }      
    }
    
    resolutionDropdown.ClearOptions();
    resolutionDropdown.AddOptions(displayOptions);

    resolutionDropdown.value = selectedId;
    //WaitThenUpdate();
  }

  //public IEnumerator WaitThenUpdate() {
  //  yield return null;
  //  string currentResolution = Screen.width.ToString() + "x" + Screen.height;
  //  int optionId = 0;

  //  for(int i=0; i<resolutionDropdown.options.Count; i++) {
  //    if(resolutionDropdown.options[i].text == currentResolution) {
  //      optionId = i;
  //    }
  //  }
  //  resolutionDropdown.value = optionId;
  //  //resolutionText.text = Screen.width.ToString() + "x" + Screen.height;
  //}

  public Resolution GetFullscreenResolution() {
    Resolution[] resolutions = Screen.resolutions;
    //foreach(var res in resolutions) {
    //  Debug.LogWarning(res.width + "x" + res.height + " : " + res.refreshRate);
    //}
    return resolutions[resolutions.Length-1];
  }


  public void OnMasterSliderValueChanged() {
    PlayerPrefs.SetInt(VsnAudioManager.masterVolumeParameter, (int)masterVolumeSlider.value);
    PlayerPrefs.Save();
    VsnAudioManager.instance.SetGroupVolume(VsnAudioManager.masterVolumeParameter, (int)masterVolumeSlider.value);
  }

  public void OnMusicSliderValueChanged() {
    PlayerPrefs.SetInt(VsnAudioManager.musicVolumeParameter, (int)musicVolumeSlider.value);
    PlayerPrefs.Save();
    VsnAudioManager.instance.SetGroupVolume(VsnAudioManager.musicVolumeParameter, (int)musicVolumeSlider.value);
  }

  public void OnSfxSliderValueChanged() {
    PlayerPrefs.SetInt(VsnAudioManager.sfxVolumeParameter, (int)sfxVolumeSlider.value);
    PlayerPrefs.Save();
    VsnAudioManager.instance.SetGroupVolume(VsnAudioManager.sfxVolumeParameter, (int)sfxVolumeSlider.value);
    VsnAudioManager.instance.PlaySfx("paint tile");
  }


  public void OnResolutionDropdownChanged() {
    //Debug.LogWarning("Resolution selected: "+ resolutionText.text);
    Screen.SetResolution(SelectedResolution.width, SelectedResolution.height, fullscreenToggle.isOn);
  }



  bool GetIsFullscreen() {
    if(Screen.fullScreenMode == FullScreenMode.Windowed) {
      windowedResolution = Screen.currentResolution;
    }
    return Screen.fullScreenMode != FullScreenMode.Windowed;
  }

  float GetGroupVolume(string groupName) {
    float volume = 0f;
    mixer.GetFloat(groupName, out volume);
    return volume;
  }

  float ConvertGroupVolumeToSliderValue(float Db) {
    float converted = Mathf.Pow(10f, Db/20f);
    return converted * 10f;
  }



  public void ClickEnglishButton() {
    Lean.Localization.LeanLocalization.CurrentLanguage = "English";
  }

  public void ClickPortugueseButton() {
    Lean.Localization.LeanLocalization.CurrentLanguage = "Portuguese";
  }


  public void ClickedExitButton() {
    HidePanel();
  }

}
