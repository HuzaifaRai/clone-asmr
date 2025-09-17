using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Links : MonoBehaviour
{

    public void MoreGames()
    {
        Application.OpenURL("https://www.amazon.com/s?i=mobile-apps&rh=p_4%3ACGS%2BLLP&search-type=ss");
        if(AudioManager.Instance) AudioManager.Instance.BtnSfx.Play();
    }
    public void RateUS()
    {
        Application.OpenURL("http://www.amazon.com/gp/mas/dl/android?p=com.cgsllp.skincarespaasmr.games");
        if (AudioManager.Instance) AudioManager.Instance.BtnSfx.Play();
    }
    public void PP()
    {
        Application.OpenURL("https://sites.google.com/view/policycolorgame");
        if(AudioManager.Instance) AudioManager.Instance.BtnSfx.Play();
    }

}
