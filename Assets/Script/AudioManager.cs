using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AudioManager();
            }
            return instance;
        }
    }

    public AudioSource BGM;
    public AudioSource BtnSfx;
    public AudioSource AppearSfx;
    public AudioSource PopinSfx;
    public AudioSource PopOutSfx;
    public AudioSource fireworkSfx;
    public AudioSource CelebrateSfx;
    public AudioSource purchaseSFX;
    public AudioSource taskkSfx;
    public AudioSource rubbing;
    public AudioSource trigger;
    public AudioSource bubble;
    public AudioSource spray;
    public AudioSource water;
    public AudioSource itemPick;
    public AudioSource itemDrop;
    public AudioSource machine;
    public AudioSource brush;

    #region Awake
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.Instance.Initialized)
        {
            Rai_SaveLoad.LoadProgress();
            GameManager.Instance.Initialized = true;
        }
        DontDestroyOnLoad(gameObject);
    }
    public void Music(float value)
    {
        if (BtnSfx) BtnSfx.Play();
        if(BGM)BGM.volume = value;
    }
    public void Sound(float value)
    {
        if (BtnSfx) BtnSfx.Play();
        if (AppearSfx) AppearSfx.volume = value;
        if (BtnSfx) BtnSfx.volume = value;
        if (taskkSfx) taskkSfx.volume = value;
        if (PopinSfx) PopinSfx.volume = value;
        if (PopOutSfx) PopOutSfx.volume = value;
        if (CelebrateSfx) CelebrateSfx.volume = value;
        if (fireworkSfx) fireworkSfx.volume = value;
        if (purchaseSFX) purchaseSFX.volume = value;
        if (rubbing) rubbing.volume = value;
        if (trigger) trigger.volume = value;
        if (bubble) bubble.volume = value;
        if (spray) spray.volume = value;
        if (itemPick) itemPick.volume = value;
        if (itemDrop) itemDrop.volume = value;
        if (machine) machine.volume = value;
        if (brush) brush.volume = value;
    }
}
