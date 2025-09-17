using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Coffee.UIEffects;
using ScratchCardAsset;
using DG.Tweening;

public class FootMode : MonoBehaviour
{
    public static FootMode instance;
    public static FootMode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FootMode();
            }
            return instance;
        }

    }
    // Start is called before the first frame update

    public enum ActionType
    {
        spray, clothclean, ankleSkinRemoving, twizeer, applyCream, buffing, nailCutting, oilApply, foamspreding
    }
    public ActionType actionType;
    [Header("GameObject")]
    public GameObject adPanel;
    public GameObject NotAvalible;
    public GameObject winPanel;
    [Header("Dragables")]
    public GameObject spray;
    public GameObject cloth, ankleskinremover, twizeer, pot, cream, buffer, nailcutter, oilBottle, foam;
    [Header("Image")]
    public Image creamlayer;
    public Image Lips;
    [Header("colliders")]
    public BoxCollider2D[] creamcollider;
    public BoxCollider2D[] oilcollider;
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
        StartCoroutine(ObjectActivation(spray,0.5f,true));
        TaskBar.SetActive(true);

    }
    #endregion

    #region TaskDone
    public void TaskDone()
    {
        TaskfillBar.fillAmount = 0;
        if(AudioManager.Instance) AudioManager.Instance.taskkSfx.Play();
        if (NextBtn) NextBtn.SetActive(false);
        if (TaskPartical) TaskPartical.Play();
        if(actionType == ActionType.spray)
        {
            StartCoroutine(ObjectActivation(spray,0f,false));
            StartCoroutine(ObjectActivation(cloth, 0.5f,true));
            actionType = ActionType.clothclean;
        }
        else if(actionType == ActionType.clothclean)
        {
            StartCoroutine(ObjectActivation(cloth, 0f,false));
            StartCoroutine(ObjectActivation(ankleskinremover, 0.5f,true));
            actionType = ActionType.ankleSkinRemoving;
        }
        else if(actionType == ActionType.ankleSkinRemoving)
        {
            StartCoroutine(ObjectActivation(ankleskinremover, 0f,false));
            StartCoroutine(ObjectActivation(twizeer, 0.5f,true));
            StartCoroutine(ObjectActivation(pot, 0.5f,true));
            actionType = ActionType.twizeer;
        }
        else if(actionType == ActionType.twizeer)
        {

            pot.transform.DOLocalMove(new Vector3(1500, -650, 0), 1f).SetDelay(0.5f); ;
            StartCoroutine(ObjectActivation(pot, 1f,false));
            StartCoroutine(ObjectActivation(twizeer, 0f,false));
            StartCoroutine(ObjectActivation(cream, 0.5f,true));
            actionType = ActionType.applyCream;
        }
        else if(actionType == ActionType.applyCream)
        {
            StartCoroutine(ObjectActivation(cream, 0f,false));
            for (int i = 0; i < creamcollider.Length; i++)
            {
                creamcollider[i].enabled = true;
            }
            StartCoroutine(ObjectActivation(buffer, 0.5f,true));
            actionType = ActionType.buffing;
        }
        else if(actionType == ActionType.buffing)
        {
            creamlayer.GetComponent<DOTweenAnimation>().DOPlay();
            StartCoroutine(ObjectActivation(creamlayer.gameObject, 1f,false));
            StartCoroutine(ObjectActivation(buffer, 0f,false));
            StartCoroutine(ObjectActivation(nailcutter, 0.5f,true));
            actionType = ActionType.nailCutting;
        }
        else if(actionType == ActionType.nailCutting)
        {
            StartCoroutine(ObjectActivation(nailcutter, 0f,false));
            StartCoroutine(ObjectActivation(oilBottle, 0.5f,true));
            actionType = ActionType.oilApply;
        }
        else if(actionType == ActionType.oilApply)
        {
            StartCoroutine(ObjectActivation(oilBottle, 0f,false));
            for (int i = 0; i < oilcollider.Length; i++)
            {
                oilcollider[i].enabled = true;
            }
            StartCoroutine(ObjectActivation(foam, 0.5f,true));
            actionType = ActionType.foamspreding;
        }
        else if(actionType == ActionType.foamspreding)
        {
            Lips.enabled = false;
            Lips.transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(ObjectActivation(foam, 0f,false));
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

