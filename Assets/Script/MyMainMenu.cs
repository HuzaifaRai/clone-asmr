using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MyMainMenu : MonoBehaviour
{
    [Header("Profile")]
    public GameObject SettingPanel;
    [Header("Loading")]
    public GameObject loadingPanel;
    public Image fillBar;
    public Slider BGMSlider;
    public Slider VolumeSlider;
    float volumevalue;
    float bgmvalue;
    public void Start()
    {
        if (GAManager.Instance) GAManager.Instance.LogDesignEvent("Scene:" + SceneManager.GetActiveScene().name + SceneManager.GetActiveScene().buildIndex);
        if (GameManager.Instance.Initialized == false)
        {
            GameManager.Instance.Initialized = true;
            Rai_SaveLoad.LoadProgress();
        }
    }
    public void Save()
    {
        if (AudioManager.Instance) AudioManager.Instance.BtnSfx.Play();
        SaveData.Instance.ProfileCreated = true;
        Rai_SaveLoad.SaveProgress();
    }
    public void SettingIsTrue(bool IsTrue)
    {
        if(IsTrue == true)
        {
            if (AudioManager.Instance) AudioManager.Instance.PopinSfx.Play();
        }
        else if(IsTrue == false)
        {
            if (AudioManager.Instance) AudioManager.Instance.PopOutSfx.Play();
        }
        SettingPanel.SetActive(IsTrue);
    }

    public void Volume()
    {
        volumevalue = VolumeSlider.value;
        AudioManager.Instance.Sound(volumevalue);
    }
    public void BGM()
    {
        print(bgmvalue);
        bgmvalue = BGMSlider.value;
        AudioManager.Instance.Music(bgmvalue);
    }
    public void Play()
    {
        if(AudioManager.Instance)AudioManager.Instance.BtnSfx.Play();
        StartCoroutine(Loading("MyModeSelection"));
    }
    IEnumerator Loading(string scene)
    {
        yield return new WaitForSeconds(0.5f);
        loadingPanel.SetActive(true);
        fillBar.fillAmount = 0f;
        while (fillBar.fillAmount < 1)
        {
            fillBar.fillAmount += Time.deltaTime / 4;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }
}
