using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Coffee.UIEffects;
using ScratchCardAsset;

public class EyeMode : MonoBehaviour
{
    public static EyeMode instance;
    public static EyeMode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EyeMode();
            }
            return instance;
        }

    }
    // Start is called before the first frame update

    public enum ActionType
    {
        CutEyePimple, CleanEyePimple, popPimples, eyebrowCut, newEyeBrow, cureSkin, removeEyePus, Poureyedrop, FixLens
    }
    public ActionType actionType;
    [Header("GameObject")]
    public GameObject adPanel;
    public GameObject NotAvalible;
    public GameObject winPanel;
    public GameObject skinAnim;
    [Header("Dragables")]
    public GameObject PimpleCutter;
    public GameObject PusRemover, PimplePopper, eyeBrowCutter, eyeBrowPencile, paste, cottonBud, Droper, lens;
    [Header("Image")]
    public Image EyeBrowDarkImage;
    public Image EyePusImage, EyeDropImage, LensImage;
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
    public ScratchCardManager eyebrowCardManager;
    public ScratchCardManager eyepusCardManager;
    private float eyebrowProgress, eyePusprogress;
    private bool startPaint, eyebrowComplete, eyepusComplete, checkProgress;
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
        //TotalCoins.text = SaveData.Instance.Coins.ToString();
        StartCoroutine(ObjectActivation(PimpleCutter,0.5f,true));
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
        if(actionType == ActionType.CutEyePimple)
        {
            StartCoroutine(ObjectActivation(PimpleCutter,0f,false));
            StartCoroutine(ObjectActivation(PusRemover,0.5f,true));
            actionType = ActionType.CleanEyePimple;
        }
        else if(actionType == ActionType.CleanEyePimple)
        {
            StartCoroutine(ObjectActivation(PusRemover, 0f,false));
            StartCoroutine(ObjectActivation(PimplePopper,0.5f,true));
            actionType = ActionType.popPimples;
        }
        else if(actionType == ActionType.popPimples)
        {
            StartCoroutine(ObjectActivation(PimplePopper, 0f,false));
            StartCoroutine(ObjectActivation(eyeBrowCutter,0.5f,true));
            actionType = ActionType.eyebrowCut;
        }
        else if(actionType == ActionType.eyebrowCut)
        {
            StartCoroutine(ObjectActivation(eyeBrowCutter, 0f,false));
            StartCoroutine(ObjectActivation(eyeBrowPencile,0.5f,true));
            StartCoroutine(ScratchGlueCompletely());
            actionType = ActionType.newEyeBrow;
        }
        else if(actionType == ActionType.newEyeBrow)
        {
            if (eyebrowCardManager) eyebrowCardManager.gameObject.SetActive(false);
            StartCoroutine(ObjectActivation(eyeBrowPencile, 0f,false));
            StartCoroutine(ObjectActivation(paste,0.5f,true));
            skinAnim.transform.SetSiblingIndex(-1);
            actionType = ActionType.cureSkin;
        }
        else if(actionType == ActionType.cureSkin)
        {
            StartCoroutine(ScratcheyePusCompletely());
            StartCoroutine(ObjectActivation(paste, 0f,false));
            StartCoroutine(ObjectActivation(cottonBud,0.5f,true));
            actionType = ActionType.removeEyePus;
        }
        else if(actionType == ActionType.removeEyePus)
        {
            if (eyepusCardManager) eyepusCardManager.gameObject.SetActive(false);
            StartCoroutine(ObjectActivation(cottonBud, 0f,false));
            StartCoroutine(ObjectActivation(Droper,0.5f,true));
            actionType = ActionType.Poureyedrop;
        }
        else if(actionType == ActionType.Poureyedrop)
        {
            StartCoroutine(ObjectActivation(Droper, 0f,false));
            StartCoroutine(ObjectActivation(lens,0.5f,true));
            actionType = ActionType.FixLens;
        }
        else if(actionType == ActionType.FixLens)
        {
            StartCoroutine(ObjectActivation(lens, 0f,false));
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

    #region ManagerScratch
    private void ManagerScratch()
    {
        if (startPaint)
        {
            if (!eyebrowComplete)
            {
                if (eyebrowCardManager)
                {
                    eyebrowProgress = eyebrowCardManager.Progress.GetProgress();
                    if(actionType == ActionType.newEyeBrow)
                    TaskfillBar.fillAmount = 1 - eyebrowProgress;
                    if (eyebrowProgress > 0.5f)
                    {
                        checkProgress = true;
                    }
                    if (eyebrowProgress <= 0.03f && checkProgress)
                    {
                        if (!eyebrowComplete)
                        {
                            eyebrowComplete = true;
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
            else if (!eyepusComplete)
            {
                if (eyepusCardManager)
                {
                    eyePusprogress = eyepusCardManager.Progress.GetProgress();
                    if (actionType == ActionType.removeEyePus)
                        TaskfillBar.fillAmount =  eyePusprogress;
                    if (eyePusprogress > 0.5f)
                    {
                        checkProgress = true;
                    }
                    if (eyePusprogress >= 0.93f && checkProgress)
                    {
                        if (!eyepusComplete)
                        {
                            eyepusComplete = true;
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
        if (actionType == ActionType.newEyeBrow)
        {
            if (eyebrowCardManager) eyebrowCardManager.Card.InputEnabled = inputValue;
        }
        else if (actionType == ActionType.removeEyePus)
        {
            if (eyepusCardManager) eyepusCardManager.Card.InputEnabled = inputValue;
        }
    }
    #endregion

    #region ScratchGlueCompletely
    IEnumerator ScratchGlueCompletely()
    {
        if (eyebrowCardManager) eyebrowCardManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (eyebrowCardManager.Card)
        {
            eyebrowCardManager.Card.FillInstantly();
            eyebrowCardManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            eyebrowCardManager.Card.InputEnabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        if (EyeBrowDarkImage)
        {
            EyeBrowDarkImage.gameObject.SetActive(true);
            EyeBrowDarkImage.enabled = true;
        }
        startPaint = true;
    }
    #endregion
    
    #region ScratchGlueCompletely
    IEnumerator ScratcheyePusCompletely()
    {
        if (eyepusCardManager) eyepusCardManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (eyepusCardManager.Card)
        {
            eyepusCardManager.Card.ClearInstantly();
            eyepusCardManager.Card.Mode = ScratchCard.ScratchMode.Erase;
            eyepusCardManager.Card.InputEnabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        if (EyePusImage)
        {
            EyePusImage.gameObject.SetActive(true);
            EyePusImage.enabled = true;
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

