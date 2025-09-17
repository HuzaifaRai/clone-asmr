using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public static LevelSelection instance;
    public static LevelSelection Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LevelSelection();
            }
            return instance;
        }
    }


    public GameObject AdPenl;
    public GameObject ModeScroller;
    public GameObject videoNotAvalible;
    public GameObject loadingPanel;
    public Image fillBar;
    [HideInInspector]
    public string[] SceneNames = { "EyeMode", "TeethMode", "LipsMode", "EarMode", "BellyButtonMode", "FootMode"};
    public GameObject[] levels;
    private int selectedIndex;
    [HideInInspector]
    public List<ItemInfo> modeList = new List<ItemInfo>();
    private ItemInfo tempItem;
    private enum RewardType
    {
        none, Coins, SelectionItem
    }
    private RewardType rewardType;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void Start()
    {
        if (GAManager.Instance) GAManager.Instance.LogDesignEvent("Scene:" + SceneManager.GetActiveScene().name + SceneManager.GetActiveScene().buildIndex);
        if (GameManager.Instance.Initialized == false)
        {
            GameManager.Instance.Initialized = true;
            Rai_SaveLoad.LoadProgress();
        }
        GameManager.Instance.TotalMode = levels.Length;
        #region Initialing Mode
        if (ModeScroller)
        {
            var modeinfo = ModeScroller.GetComponentsInChildren<ItemInfo>();
            for (int i = 0; i < modeinfo.Length; i++)
            {
                modeList.Add(modeinfo[i]);
            }
        }
        SetupData(SaveData.Instance.modeProps.ModeLocked, modeList);
        #endregion
        GetItemsInfo();
        UpdateLevelTexts();
    }

    #region EnableDisable
    void OnEnable()
    {
        if (MyAdsManager.Instance != null)
        {
            MyAdsManager.Instance.onRewardedVideoAdCompletedEvent += OnRewardedVideoComplete;
        }
    }

    void OnDisable()
    {
        if (MyAdsManager.Instance != null)
        {
            MyAdsManager.Instance.onRewardedVideoAdCompletedEvent -= OnRewardedVideoComplete;
        }
    }
    #endregion

    #region SetupItemData
    public void SetupData(List<bool> unlockItems, List<ItemInfo> _ItemsInfo)
    {
        if (_ItemsInfo.Count > 0)
        {
            if (unlockItems.Count < _ItemsInfo.Count)
            {
                for (int i = 0; i < _ItemsInfo.Count; i++)
                {
                    if (unlockItems.Count <= i)
                    {
                        // Add new data to SaveData file in case the file is empty or new data is available
                        unlockItems.Add(_ItemsInfo[i].isLocked);
                    }
                }
            }
            // Setting up Hairs Properties to actual Properties from SaveData file  
            for (int i = 0; i < _ItemsInfo.Count; i++)
            {
                _ItemsInfo[i].isLocked = unlockItems[i];
            }
            //Adding Click listeners to btns 
            for (int i = 0; i < _ItemsInfo.Count; i++)
            {
                int Index = i;
                if (_ItemsInfo[i].itemBtn)
                {
                    _ItemsInfo[i].itemBtn.onClick.AddListener(() =>
                    {
                        selectedIndex = Index;
                        SelectItem(Index);
                    });
                }
            }
        }
    }
    #endregion

    #region SelectItem
    public void SelectItem(int index)
    {
        CheckSelectedItem(modeList);
        GetItemsInfo();
    }
    #endregion

    #region CheckSelectedItem
    public void CheckSelectedItem(List<ItemInfo> itemInfoList)
    {
        rewardType = RewardType.SelectionItem;
        if (itemInfoList.Count > selectedIndex)
        {
            tempItem = itemInfoList[selectedIndex];
            if (itemInfoList[selectedIndex].isLocked)
            {
                if (itemInfoList[selectedIndex].videoUnlock)
                {
                    CheckVideoStatus();
                }
            }
            else
            {
                if (AudioManager.Instance.BtnSfx) AudioManager.Instance.BtnSfx.Play();
                GameManager.Instance.modeName = SceneNames[selectedIndex];
                GameManager.Instance.selectedlevel = selectedIndex;
                Play();
            }
        }
    }
    #endregion

    private void UpdateLevelTexts()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            Transform levelChild1 = levels[i].transform.GetChild(1); 

            if (levelChild1 != null)
            {
                Transform grandChild = levelChild1; 

                if (grandChild != null)
                {
                    Text levelText = grandChild.GetComponent<Text>();

                    if (levelText != null)
                    {
                        levelText.text = "Level " + (i + 1); 
                    }
                }
            }
        }
    }

    #region CheckVideoStatus
    public void CheckVideoStatus()
    {
        if (MyAdsManager.Instance != null)
        {
            if (MyAdsManager.Instance.IsRewardedAvailable())
            {
                StartCoroutine(ShowRewardedAd());
            }
            else
            {
                if (AudioManager.Instance.PopinSfx) AudioManager.Instance.PopinSfx.Play();
                videoNotAvalible.SetActive(true);
                Invoke("videoPanelOf", 1.3f);
            }
        }
        else
        {
            if (AudioManager.Instance.PopinSfx) AudioManager.Instance.PopinSfx.Play();
            videoNotAvalible.SetActive(true);
            Invoke("videoPanelOf", 1.3f);
        }
    }
    #endregion



    #region RewardedVideoCompleted
    public void OnRewardedVideoComplete()
    {
        if (rewardType == RewardType.SelectionItem)
        {
            if (tempItem != null) tempItem.isLocked = false;
            UnlockSingleItem();
            GetItemsInfo();
            //SelectItem(selectedIndex);
        }
        if (AudioManager.Instance) AudioManager.Instance.purchaseSFX.Play();
    }
    #endregion

    #region UnlockSingleItem
    public void UnlockSingleItem()
    {
        SaveData.Instance.modeProps.ModeLocked[selectedIndex] = false;
        Rai_SaveLoad.SaveProgress();
    }
    #endregion

    #region videoPanelOf
    public void videoPanelOf()
    {
        videoNotAvalible.SetActive(false);
    }
    #endregion

    public void SelectedScene(string scene)
    {
        if (AudioManager.Instance.BtnSfx) AudioManager.Instance.BtnSfx.Play();
        GameManager.Instance.modeName = scene;
    }
    public void Play()
    {
        if (AudioManager.Instance.BtnSfx) AudioManager.Instance.BtnSfx.Play();
        StartCoroutine(ShowInterstitialAD());
        StartCoroutine(Loading(GameManager.Instance.modeName));
        loadingPanel.gameObject.SetActive(true);
    }
    public void Back()
    {
        if (AudioManager.Instance.BtnSfx) AudioManager.Instance.BtnSfx.Play();
        StartCoroutine(ShowInterstitialAD());
        StartCoroutine(Loading("MyMainMenu"));
        loadingPanel.gameObject.SetActive(true);
    }
    IEnumerator Loading(string str)
    {
        fillBar.fillAmount = 0;
        while (fillBar.fillAmount < 1)
        {
            fillBar.fillAmount += Time.deltaTime / 4;
            yield return null;
        }

        SceneManager.LoadScene(str);
    }

    IEnumerator ShowInterstitialAD()
    {
        if (MyAdsManager.Instance)
        {
            if (MyAdsManager.Instance.IsInterstitialAvailable())
            {
                if (AdPenl)
                {
                    AdPenl.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                    AdPenl.SetActive(false);
                }
                MyAdsManager.Instance.ShowInterstitialAds();
            }
        }
    }
    IEnumerator ShowRewardedAd()
    {
        if (MyAdsManager.Instance)
        {
            if (MyAdsManager.Instance.IsRewardedAvailable())
            {
                if (AdPenl)
                {
                    AdPenl.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                    AdPenl.SetActive(false);
                }
                MyAdsManager.Instance.ShowRewardedVideos();
            }
        }
    }

    #region GetItemsInfo
    private void GetItemsInfo()
    {
        SetItemsInfo(modeList);
    }
    #endregion

    #region SetItemsInfo
    private void SetItemsInfo(List<ItemInfo> _ItemInfo)
    {
        if (_ItemInfo == null) return;
        for (int i = 0; i < _ItemInfo.Count; i++)
        {
            if (_ItemInfo[i].isLocked)
            {
                if (_ItemInfo[i].LockIcon) _ItemInfo[i].LockIcon.SetActive(true);
                if (_ItemInfo[i].videoUnlock)
                {
                    if (_ItemInfo[i].VideoSlot) _ItemInfo[i].VideoSlot.SetActive(true);

                }

            }
            else
            {
                if (_ItemInfo[i].VideoSlot) _ItemInfo[i].VideoSlot.SetActive(false);
                if (_ItemInfo[i].LockIcon) _ItemInfo[i].LockIcon.SetActive(false);
            }
        }
    }
    #endregion
}
