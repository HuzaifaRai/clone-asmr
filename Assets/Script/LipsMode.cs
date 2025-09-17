using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Coffee.UIEffects;
using ScratchCardAsset;

public class LipsMode : MonoBehaviour
{
    public static LipsMode instance;
    public static LipsMode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LipsMode();
            }
            return instance;
        }

    }
    // Start is called before the first frame update

    public enum ActionType
    {
        cureinjury, popPimples, cutHairs, tubescar, twizer, cream, lipstick
    }
    public ActionType actionType;
    [Header("GameObject")]
    public GameObject adPanel;
    public GameObject NotAvalible;
    public GameObject winPanel;
    [Header("Dragables")]
    public GameObject injuryEarbud;
    public GameObject PimplePopper, hairCutter, tube, twizeer, cream, lipstick;
    [Header("Image")]
    public Image lipsCreamImage;
    public Image redLipsImage;
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
    public ScratchCardManager lipsCreamCardManager;
    public ScratchCardManager lipsRedCardManager;
    private float creamProgress, lipsprogress;
    private bool startPaint, creamComplete, lipsComplete, checkProgress;

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
        StartCoroutine(ObjectActivation(injuryEarbud,0.5f,true));
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
        if(actionType == ActionType.cureinjury)
        {
            StartCoroutine(ObjectActivation(injuryEarbud,0f,false));
            StartCoroutine(ObjectActivation(PimplePopper, 0.5f,true));
            actionType = ActionType.popPimples;
        }
        else if(actionType == ActionType.popPimples)
        {
            StartCoroutine(ObjectActivation(PimplePopper, 0f,false));
            StartCoroutine(ObjectActivation(hairCutter, 0.5f,true));
            actionType = ActionType.cutHairs;
        }
        else if(actionType == ActionType.cutHairs)
        {
            StartCoroutine(ObjectActivation(hairCutter, 0f,false));
            StartCoroutine(ObjectActivation(tube, 0.5f,true));
            TaskBar.SetActive(false);
            actionType = ActionType.tubescar;
        }
        else if(actionType == ActionType.tubescar)
        {
            StartCoroutine(ObjectActivation(tube, 0f,false));
            StartCoroutine(ObjectActivation(twizeer, 0.5f,true));
            TaskBar.SetActive(true);
            actionType = ActionType.twizer;
        }
        else if(actionType == ActionType.twizer)
        {
            StartCoroutine(ObjectActivation(twizeer, 0f,false));
            StartCoroutine(ObjectActivation(cream, 0.5f,true));
            //skinAnim.transform.SetSiblingIndex(-1);
            StartCoroutine(ScratchcreamCompletely());
            actionType = ActionType.cream;
        }
        else if(actionType == ActionType.cream)
        {
            if (lipsCreamCardManager) lipsCreamCardManager.gameObject.SetActive(false);
            StartCoroutine(ObjectActivation(cream, 0f,false));
            StartCoroutine(ObjectActivation(lipstick, 0.5f,true));
            StartCoroutine(ScratchlipsCompletely());
            actionType = ActionType.lipstick;
        }
        else if(actionType == ActionType.lipstick)
        {
            if (lipsRedCardManager) lipsRedCardManager.gameObject.SetActive(false);
            StartCoroutine(ObjectActivation(lipstick, 0f,false));
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
            if (!creamComplete)
            {
                if (lipsCreamCardManager)
                {
                    creamProgress = lipsCreamCardManager.Progress.GetProgress();
                    TaskfillBar.fillAmount =1- lipsCreamCardManager.Progress.GetProgress();
                    if (creamProgress > 0.5f)
                    {
                        checkProgress = true;
                    }
                    if (creamProgress <= 0.03f && checkProgress)
                    {
                        if (!creamComplete)
                        {
                            creamComplete = true;
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
            else if (!lipsComplete)
            {
                if (lipsRedCardManager)
                {
                    lipsprogress = lipsRedCardManager.Progress.GetProgress();
                    TaskfillBar.fillAmount = 1- lipsRedCardManager.Progress.GetProgress();
                    if (lipsprogress > 0.5f)
                    {
                        checkProgress = true;
                    }
                    if (lipsprogress <= 0.03f && checkProgress)
                    {
                        if (!lipsComplete)
                        {
                            lipsComplete = true;
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
        if (actionType == ActionType.cream)
        {
            if (lipsCreamCardManager) lipsCreamCardManager.Card.InputEnabled = inputValue;
        }
        else if (actionType == ActionType.lipstick)
        {
            if (lipsRedCardManager) lipsRedCardManager.Card.InputEnabled = inputValue;
        }
    }
    #endregion

    #region ScratchGlueCompletely
    IEnumerator ScratchcreamCompletely()
    {
        if (lipsCreamCardManager) lipsCreamCardManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (lipsCreamCardManager.Card)
        {
            lipsCreamCardManager.Card.FillInstantly();
            lipsCreamCardManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            lipsCreamCardManager.Card.InputEnabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        if (lipsCreamImage)
        {
            lipsCreamImage.gameObject.SetActive(true);
            lipsCreamImage.enabled = true;
        }
        startPaint = true;
    }
    #endregion
    
    #region ScratchGlueCompletely
    IEnumerator ScratchlipsCompletely()
    {
        if (lipsRedCardManager) lipsRedCardManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (lipsRedCardManager.Card)
        {
            lipsRedCardManager.Card.FillInstantly();
            lipsRedCardManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            lipsRedCardManager.Card.InputEnabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        if (redLipsImage)
        {
            redLipsImage.gameObject.SetActive(true);
            redLipsImage.enabled = true;
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

