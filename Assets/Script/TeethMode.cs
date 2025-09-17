using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Coffee.UIEffects;
using ScratchCardAsset;

public class TeethMode : MonoBehaviour
{
    public static TeethMode instance;
    public static TeethMode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TeethMode();
            }
            return instance;
        }

    }
    // Start is called before the first frame update

    public enum ActionType
    {
       popPimples, cavityRemoving, twister, Brush, tongCleaning
    }
    public ActionType actionType;
    [Header("GameObject")]
    public GameObject adPanel;
    public GameObject NotAvalible;
    public GameObject winPanel;
    [Header("Dragables")]
    public GameObject PimplePopper;
    public GameObject  cavityRemover, twister, brush, tongcleaner;
    [Header("Loading")]
    public GameObject loadingPanel;
    public Image fillBar;
    [Header("Others")]
    public ParticleSystem TaskPartical;
    public GameObject FinalPartical;
    public GameObject NextBtn;
    public GameObject TaskBar;
    public Image TaskfillBar;
    [HideInInspector]
    public int pimpleIndex;
    #region Awake
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    #endregion

    #region Start
    void Start()
    {
        if (GAManager.Instance) GAManager.Instance.LogDesignEvent("Scene:" + SceneManager.GetActiveScene().name + SceneManager.GetActiveScene().buildIndex);
        if (GameManager.Instance.Initialized == false)
        {
            GameManager.Instance.Initialized = true;
            Rai_SaveLoad.LoadProgress();
        }
        StartCoroutine(ObjectActivation(PimplePopper,0.5f,true));
        TaskBar.SetActive(true);
    }
    #endregion

    #region TaskDone
    public void TaskDone()
    {
        TaskfillBar.fillAmount = 0;
        if (AudioManager.Instance) AudioManager.Instance.taskkSfx.Play();
        if (NextBtn) NextBtn.SetActive(false);
        if (TaskPartical) TaskPartical.Play();
        if(actionType == ActionType.popPimples)
        {
            StartCoroutine(ObjectActivation(PimplePopper,0f,false));
            StartCoroutine(ObjectActivation(cavityRemover,0.5f,true));
            actionType = ActionType.cavityRemoving;
        }
        else if(actionType == ActionType.cavityRemoving)
        {
            StartCoroutine(ObjectActivation(cavityRemover, 0f,false));
            StartCoroutine(ObjectActivation(twister,0.5f,true));
            actionType = ActionType.twister;
        }
        else if(actionType == ActionType.twister)
        {
            StartCoroutine(ObjectActivation(twister, 0f,false));
            StartCoroutine(ObjectActivation(brush,0.5f,true));
            actionType = ActionType.Brush;
        }
        else if(actionType == ActionType.Brush)
        {
            StartCoroutine(ObjectActivation(brush, 0f,false));
            StartCoroutine(ObjectActivation(tongcleaner,0.5f,true));
            actionType = ActionType.tongCleaning;
        }
        else if(actionType == ActionType.tongCleaning)
        {
            StartCoroutine(ObjectActivation(tongcleaner, 0f,false));
            winPanel.SetActive(true);
            FinalPartical.SetActive(true);
        }
    }
    #endregion

    #region ObjectActivation
    IEnumerator ObjectActivation(GameObject obj, float Delay, bool IsTrue)
    {
        yield return new WaitForSeconds(Delay);
        obj.SetActive(IsTrue);
    }
    #endregion

    #region Play
    public void Play(string str)
    {
        if (AudioManager.Instance)
        {
            if (AudioManager.Instance.BtnSfx)
            {
                AudioManager.Instance.BtnSfx.Play();
            }
        }
        StartCoroutine(ShowInterstitialAD());
        StartCoroutine(LoadingScene(str));
    }

    IEnumerator LoadingScene(string str)
    {
        yield return new WaitForSeconds(0.2f);
        loadingPanel.SetActive(true);
        fillBar.fillAmount = 0;
        while (fillBar.fillAmount < 1)
        {
            fillBar.fillAmount += Time.deltaTime / 4;
            yield return null;
        }
        SceneManager.LoadScene(str);
    }
    #endregion

    #region ShowInterstitialAD
    IEnumerator ShowInterstitialAD()
    {
        if (MyAdsManager.instance)
        {
            if (MyAdsManager.instance.IsInterstitialAvailable())
            {
                if (adPanel) adPanel.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                if (adPanel) adPanel.SetActive(false);
                MyAdsManager.instance.ShowInterstitialAds();
            }
        }
    }
    #endregion

    public void Next()
    {
        if (AudioManager.Instance)
        {
            if (AudioManager.Instance.BtnSfx)
            {
                AudioManager.Instance.BtnSfx.Play();
            }
        }
        if (GameManager.Instance.selectedlevel < SaveData.Instance.modeProps.ModeLocked.Count-1)
        {
            GameManager.Instance.selectedlevel++;
            if (SaveData.Instance.modeProps.ModeLocked[GameManager.Instance.selectedlevel] == true)
            {
                SaveData.Instance.modeProps.ModeLocked[GameManager.Instance.selectedlevel] = false;
            }
            Rai_SaveLoad.SaveProgress();
            StartCoroutine(LoadingScene(LevelSelection.Instance.SceneNames[GameManager.Instance.selectedlevel]));
            StartCoroutine(ShowInterstitialAD());
        }
        else
        {
            GameManager.Instance.selectedlevel = 0;
            StartCoroutine(LoadingScene(LevelSelection.Instance.SceneNames[GameManager.Instance.selectedlevel]));
            StartCoroutine(ShowInterstitialAD());
        }
    }

}

