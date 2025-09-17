using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Coffee.UIEffects;
using ScratchCardAsset;

public class BellyButtoMode : MonoBehaviour
{
    public static BellyButtoMode instance;
    public static BellyButtoMode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BellyButtoMode();
            }
            return instance;
        }

    }
    // Start is called before the first frame update

    public enum ActionType
    {
        removePin, removepus, cleanbutton, cleanholedust, soaping, cottonClean, spray, cleanagain, wearPin
    }
    public ActionType actionType;
    [Header("GameObject")]
    public GameObject adPanel;
    public GameObject winPanel;
    public GameObject NotAvalible;
    [Header("Dragables")]
    public GameObject pinBox;
    public GameObject pin, pusRemover, dirtcleaner, tweezer, soap, cotton, spray, cotton2;
    [Header("Image")]
    public BoxCollider2D[] bubblecollider;
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
    public int Index;
    [HideInInspector]


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
        StartCoroutine(ObjectActivation(pinBox,0.5f,true));
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
        if(actionType == ActionType.removePin)
        {
            StartCoroutine(ObjectActivation(pinBox,0f,false));
            StartCoroutine(ObjectActivation(pusRemover, 0.5f,true));
            actionType = ActionType.removepus;
        }
        else if(actionType == ActionType.removepus)
        {
            StartCoroutine(ObjectActivation(pusRemover, 0f,false));
            StartCoroutine(ObjectActivation(dirtcleaner, 0.5f,true));
            actionType = ActionType.cleanbutton;
        }
        else if(actionType == ActionType.cleanbutton)
        {
            StartCoroutine(ObjectActivation(dirtcleaner, 0f,false));
            StartCoroutine(ObjectActivation(tweezer, 0.5f,true));
            actionType = ActionType.cleanholedust;
        }
        else if(actionType == ActionType.cleanholedust)
        {
            StartCoroutine(ObjectActivation(tweezer, 0f,false));
            StartCoroutine(ObjectActivation(soap, 0.5f,true));
            actionType = ActionType.soaping;
        }
        else if(actionType == ActionType.soaping)
        {
            StartCoroutine(ObjectActivation(soap, 0f,false));
            StartCoroutine(ObjectActivation(cotton, 0.5f,true));
            for (int i = 0; i < bubblecollider.Length; i++)
            {
                bubblecollider[i].enabled = true;
            }
            actionType = ActionType.cottonClean;
        }
        else if(actionType == ActionType.cottonClean)
        {
            StartCoroutine(ObjectActivation(cotton, 0f,false));
            StartCoroutine(ObjectActivation(spray, 0.5f,true));
            actionType = ActionType.spray;
        }
        else if(actionType == ActionType.spray)
        {
            StartCoroutine(ObjectActivation(spray, 0f,false));
            StartCoroutine(ObjectActivation(cotton2, 0.5f,true));
            actionType = ActionType.cleanagain;
        }
        else if(actionType == ActionType.cleanagain)
        {
            StartCoroutine(ObjectActivation(cotton2, 0f,false));
            StartCoroutine(ObjectActivation(pinBox, 0.5f,true));
            actionType = ActionType.wearPin;
        }
        else if(actionType == ActionType.wearPin)
        {
            StartCoroutine(ObjectActivation(pinBox, 0f,false));
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
    public void Next()
    {
        if(GameManager.Instance.selectedlevel < SaveData.Instance.modeProps.ModeLocked.Count-1)
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
        if (AudioManager.Instance)
        {
            if (AudioManager.Instance.BtnSfx)
                AudioManager.Instance.BtnSfx.Play();
        }
    }
    public void Play(string str)
    {
        if (AudioManager.Instance)
        {
            if (AudioManager.Instance.BtnSfx)
                AudioManager.Instance.BtnSfx.Play();
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
}

