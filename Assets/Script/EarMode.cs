using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Coffee.UIEffects;
using ScratchCardAsset;

public class EarMode : MonoBehaviour
{
    public static EarMode instance;
    public static EarMode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EarMode();
            }
            return instance;
        }

    }
    // Start is called before the first frame update

    public enum ActionType
    {
        removeEarring, popus, cleanEaringpus, popPimples, cleanEar, cleanEardirt, cleanearholeDirt, cleanearhole, wearEarring
    }
    public ActionType actionType;
    [Header("GameObject")]
    public GameObject adPanel;
    public GameObject NotAvalible;
    public GameObject winPanel;
    public GameObject earHoleParent;
    [Header("Dragables")]
    public GameObject earringBox;
    public GameObject earring, pusRemover, pimplepopper, dirtcleaner, earcleaner, holedirtCleaner, holeCleaner;
    [Header("Image")]
    public Image earshadeImage;
    public Image holeshadeImage;
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
    [Header("ScratchCard")]
    public ScratchCardManager earCardManager;
    public ScratchCardManager holeCardManager;
    private float earProgress, holeprogress;
    private bool startPaint, earComplete, holeComplete, checkProgress;

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
        StartCoroutine(ObjectActivation(earringBox,0.5f,true));
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
        if(actionType == ActionType.removeEarring)
        {
            StartCoroutine(ObjectActivation(earringBox,0f,false));
            StartCoroutine(ObjectActivation(pusRemover, 0.5f,true));
            actionType = ActionType.cleanEaringpus;
        }
        else if(actionType == ActionType.cleanEaringpus)
        {
            StartCoroutine(ObjectActivation(pusRemover, 0f,false));
            StartCoroutine(ObjectActivation(pimplepopper, 0.5f,true));
            actionType = ActionType.popPimples;
        }
        else if(actionType == ActionType.popPimples)
        {
            StartCoroutine(ObjectActivation(pimplepopper, 0f,false));
            StartCoroutine(ObjectActivation(dirtcleaner, 0.5f,true));
            StartCoroutine(ScratchearCompletely());
            actionType = ActionType.cleanEardirt;
        }
        else if(actionType == ActionType.cleanEardirt)
        {
            StartCoroutine(ObjectActivation(dirtcleaner, 0f,false));
            StartCoroutine(ObjectActivation(earcleaner, 0.5f,true));
            StartCoroutine(ScratchearCompletely());
            actionType = ActionType.cleanEar;
        }
        else if(actionType == ActionType.cleanEar)
        {
            if (earCardManager) earCardManager.gameObject.SetActive(false);
            earHoleParent.SetActive(true);
            StartCoroutine(ObjectActivation(earcleaner, 0f,false));
            StartCoroutine(ObjectActivation(holedirtCleaner, 0.5f,true));
            actionType = ActionType.cleanearholeDirt;
        }
        else if(actionType == ActionType.cleanearholeDirt)
        {
            StartCoroutine(ObjectActivation(holedirtCleaner, 0f,false));
            StartCoroutine(ObjectActivation(holeCleaner, 0.5f,true));
            StartCoroutine(ScratchearholeCompletely());
            actionType = ActionType.cleanearhole;
        }
        else if(actionType == ActionType.cleanearhole)
        {
            if (holeCardManager) holeCardManager.gameObject.SetActive(false);
            StartCoroutine(ObjectActivation(holeCleaner, 0f,false));
            earHoleParent.SetActive(false);
            StartCoroutine(ObjectActivation(earringBox, 0.5f,true));
            actionType = ActionType.wearEarring;
        }
        else if(actionType == ActionType.wearEarring)
        {
            StartCoroutine(ObjectActivation(earringBox, 0f,false));
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

    #region ManagerScratch
    private void ManagerScratch()
    {
        if (startPaint)
        {
            if (!earComplete)
            {
                if (earCardManager)
                {
                    earProgress = earCardManager.Progress.GetProgress();
                    if (actionType == ActionType.cleanEar)
                        TaskfillBar.fillAmount =  earProgress;
                    if (earProgress > 0.5f)
                    {
                        checkProgress = true;
                    }
                    if (earProgress >= 0.93f && checkProgress)
                    {
                        if (!earComplete)
                        {
                            earComplete = true;
                            checkProgress = false;
                            if (NextBtn) NextBtn.gameObject.SetActive(true);
                            if (AudioManager.Instance)
                            {
                                if (AudioManager.Instance.AppearSfx)
                                {
                                    AudioManager.Instance.AppearSfx.Play();
                                }
                            }
                        }
                    }
                }
            }
            else if (!holeComplete)
            {
                if (holeCardManager)
                {
                    holeprogress = holeCardManager.Progress.GetProgress();
                    if (actionType == ActionType.cleanearhole)
                        TaskfillBar.fillAmount =  holeprogress;
                    if (holeprogress > 0.5f)
                    {
                        checkProgress = true;
                    }
                    if (holeprogress >= 0.93f && checkProgress)
                    {
                        if (!holeComplete)
                        {
                            holeComplete = true;
                            checkProgress = false;
                            if (NextBtn) NextBtn.gameObject.SetActive(true);
                            if (AudioManager.Instance)
                            {
                                if (AudioManager.Instance.AppearSfx)
                                {
                                    AudioManager.Instance.AppearSfx.Play();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Inputs
    public void SetScratchInput(bool inputValue = false)
    {
        StartCoroutine(ManageInput(inputValue));
    }
    IEnumerator ManageInput(bool inputValue = false)
    {
        yield return new WaitForEndOfFrame();
        if (actionType == ActionType.cleanEar)
        {
            if (earCardManager) earCardManager.Card.InputEnabled = inputValue;
        }
        else if (actionType == ActionType.cleanearhole)
        {
            if (holeCardManager) holeCardManager.Card.InputEnabled = inputValue;
        }
    }
    #endregion

    #region ScratchGlueCompletely
    IEnumerator ScratchearCompletely()
    {
        if (earCardManager) earCardManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (earCardManager.Card)
        {
            earCardManager.Card.ClearInstantly();
            earCardManager.Card.Mode = ScratchCard.ScratchMode.Erase;
            earCardManager.Card.InputEnabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        if (earshadeImage)
        {
            earshadeImage.gameObject.SetActive(true);
            earshadeImage.enabled = true;
        }
        startPaint = true;
    }
    #endregion
    
    #region ScratchGlueCompletely
    IEnumerator ScratchearholeCompletely()
    {
        if (holeCardManager) holeCardManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (holeCardManager.Card)
        {
            holeCardManager.Card.ClearInstantly();
            holeCardManager.Card.Mode = ScratchCard.ScratchMode.Erase;
            holeCardManager.Card.InputEnabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        if (holeshadeImage)
        {
            holeshadeImage.gameObject.SetActive(true);
            holeshadeImage.enabled = true;
        }
        startPaint = true;
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        ManagerScratch();
    }
    #endregion
    public void Next()
    {
        if (AudioManager.Instance)
        {
            if (AudioManager.Instance.BtnSfx)
                AudioManager.Instance.BtnSfx.Play();
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

