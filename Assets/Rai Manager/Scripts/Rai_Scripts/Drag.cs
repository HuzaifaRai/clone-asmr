using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Drag : MonoBehaviour
{
    public bool isCanvasObject;
    public AudioSource MouseDownSFX;
    public GameObject MouseDownIndicator, MouseUpIndicator,oneTimeIndication;
    public ParticleSystem triggerParticles;
    public int downSortingOrder, upSortinggOrder;
    public int totalTriggers;
    private Vector2 InitialPosition;
    private Vector2 MousePosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private float deltaX, deltaY;
    private ScalePingPong pingPong;
    private bool isPosAssigned, restPos;
    private BoxCollider2D boxCollider;
    private List<BoxCollider2D> boxColliders = new List<BoxCollider2D>();
    private Animator m_Animator;
    private int didTrigger;
    private bool inTrigger = false;
    public UnityEvent mouseDown, mouseUp;
    // Start is called before the first frame update
    void Start()
    {
        restPos = true;
        boxCollider = GetComponent<BoxCollider2D>();
        pingPong = GetComponent<ScalePingPong>();
        m_Animator = GetComponentInChildren<Animator>();
        if (m_Animator)
            m_Animator.enabled = false;
        var _BoxColliders2D = FindObjectsOfType<BoxCollider2D>();
        for(int i = 0; i < _BoxColliders2D.Length; i++)
        {
            boxColliders.Add(_BoxColliders2D[i]);
        }
        if (MouseUpIndicator)
        {
            MouseUpIndicator.SetActive(true);
        }
        if (oneTimeIndication)
            oneTimeIndication.SetActive(true);
    }
    private void ColliderManager(bool isTrue)
    {
        for(int i = 0; i < boxColliders.Count; i++)
        {
            if(boxColliders[i] != boxCollider)
            {
                boxColliders[i].enabled = isTrue;
            }
        }
    }

    void OnMouseDown()
    {
        mouseDown.Invoke();
        var _Renderers = GetComponentsInChildren<Renderer>();
        for(int i = 0; i < _Renderers.Length; i++)
        {
            _Renderers[i].sortingOrder = downSortingOrder;
        }
        if (pingPong) pingPong.enabled = false;
        if (MouseDownIndicator) MouseDownIndicator.SetActive(true);
        if (MouseUpIndicator) MouseUpIndicator.SetActive(false);
        if (m_Animator) m_Animator.enabled = true;
        if (triggerParticles) triggerParticles.Play();
        if (MouseDownSFX) MouseDownSFX.Play();
        if (!isPosAssigned)
        {
            isPosAssigned = true;
            InitialPosition = transform.localPosition;
        }
        if (isCanvasObject)
        {
            screenPoint = Camera.main.WorldToScreenPoint(Input.mousePosition); // I removed this line to prevent centring 
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
        else
        {
            deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.localPosition.x;
            deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.localPosition.y;
        }
        if (EyeMode.Instance)
        {
            if (gameObject.name == "EyeBrowPencile" || gameObject.name == "EarBud")
            {
                EyeMode.Instance.SetScratchInput(true);
            }
        }
        if (LipsMode.Instance)
        {
            if (gameObject.name == "EarBudCream" || gameObject.name == "lipstick")
            {
                LipsMode.Instance.SetScratchInput(true);
            }
        }
        if (EarMode.Instance)
        {
            if (gameObject.name == "cotton")
            {
                EarMode.Instance.SetScratchInput(true);
            }
        }
        if (gameObject.name == "EarBud" || gameObject.name == "cottonOne" || gameObject.name == "cottonTwo" || gameObject.name == "cotton" || gameObject.name == "cloth" || gameObject.name == "foam" || gameObject.name == "EarBudInjury" || gameObject.name == "EarBudCream" 
            || gameObject.name == "lipstick"|| gameObject.name == "Tong")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.rubbing)
               AudioManager.Instance.rubbing.Play();
            }
        }
        if (gameObject.name == "Shawer")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.spray)
               AudioManager.Instance.spray.Play();
            }
        }
        if (gameObject.name == "mesagger" ||gameObject.name == "drySkinRemover")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.machine)
               AudioManager.Instance.machine.Play();
            }
        }
        if (gameObject.name == "Soap")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.bubble)
               AudioManager.Instance.bubble.Play();
            }
        }
        if (gameObject.name == "brush")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.brush)
               AudioManager.Instance.brush.Play();
            }
        }
        if (oneTimeIndication)
            oneTimeIndication.SetActive(false);
    }

    void OnMouseDrag()
    {
        if (isCanvasObject)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            transform.position = curPosition;
        }
        else
        {
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition = new Vector2(MousePosition.x - deltaX, MousePosition.y - deltaY);
        } 
    }

    void OnMouseUp()
    {
        mouseUp.Invoke();
        var _Renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < _Renderers.Length; i++)
        {
            _Renderers[i].sortingOrder = upSortinggOrder;
        }
        if (pingPong) pingPong.enabled = true;
        if (MouseDownIndicator) MouseDownIndicator.SetActive(false);
        if (m_Animator) m_Animator.enabled = false;
        if (triggerParticles) triggerParticles.Stop();
        if (restPos)
        {
            transform.localPosition = InitialPosition;
            if (MouseUpIndicator)
            {
                MouseUpIndicator.SetActive(true);
            }
        }
        if (EyeMode.Instance)
        {
            if (gameObject.name == "EyeBrowPencile" || gameObject.name == "EarBud")
            {
                EyeMode.Instance.SetScratchInput(false);
            }
        }
        if (LipsMode.Instance)
        {
            if (gameObject.name == "EarBudCream" || gameObject.name == "lipstick")
            {
                LipsMode.Instance.SetScratchInput(false);
            }
        }
        if (EarMode.Instance)
        {
            if (gameObject.name == "cotton")
            {
                EarMode.Instance.SetScratchInput(false);
            }
        }
        if (gameObject.name == "EarBud" || gameObject.name == "cottonOne" || gameObject.name == "cottonTwo" || gameObject.name == "cotton" || gameObject.name == "cloth" || gameObject.name == "foam" || gameObject.name == "EarBudInjury" || gameObject.name == "EarBudCream"
            || gameObject.name == "lipstick" || gameObject.name == "Tong")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.rubbing)
                    AudioManager.Instance.rubbing.Stop();
            }
        }
        if (gameObject.name == "Shawer")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.spray)
                    AudioManager.Instance.spray.Stop();
            }
        }
        if (gameObject.name == "mesagger" || gameObject.name == "drySkinRemover")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.machine)
                    AudioManager.Instance.machine.Stop();
            }
        }
        if (gameObject.name == "Soap")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.bubble)
                    AudioManager.Instance.bubble.Stop();
            }
        }
        if (gameObject.name == "brush")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.brush)
                    AudioManager.Instance.brush.Stop();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (EyeMode.Instance)
        {
            if (gameObject.name == "PimpleCutter" && col.name == "EyePimple")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.GetComponent<Image>().enabled = false;
                col.transform.GetChild(0).gameObject.SetActive(true);
                StartCoroutine(ObjectActivation(EyeMode.Instance.NextBtn, 2f, true));
                EyeMode.instance.TaskfillBar.DOFillAmount(
                 EyeMode.instance.TaskfillBar.fillAmount + 1f,
                 2f).SetEase(Ease.Linear);
                StartCoroutine(nextSound(2f));
                StartCoroutine(ObjectActivation(gameObject, 2f, false));
            }
            if (gameObject.name == "pusRemover" && col.name == "EyePimple")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                col.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<Image>().enabled = false;
                col.transform.GetChild(0).gameObject.SetActive(false);
                col.transform.GetChild(1).gameObject.SetActive(true);
                StartCoroutine(ObjectActivation(EyeMode.Instance.NextBtn, 2f, true));
                EyeMode.instance.TaskfillBar.DOFillAmount(
                 EyeMode.instance.TaskfillBar.fillAmount + 1f,
                 2f).SetEase(Ease.Linear);
                StartCoroutine(nextSound(2f));
                StartCoroutine(ObjectActivation(gameObject, 2f, false));
            }
            if (gameObject.name == "PimplePoper" && col.name == "Pimple")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                gameObject.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                Invoke("gameobjectImgecollider", 2f);
                EyeMode.Instance.pimpleIndex++;
                EyeMode.instance.TaskfillBar.DOFillAmount(
                 EyeMode.instance.TaskfillBar.fillAmount + 0.34f,
                 2f).SetEase(Ease.Linear);
                if (EyeMode.Instance.pimpleIndex > 2)
                {
                    StartCoroutine(ObjectActivation(EyeMode.Instance.NextBtn, 2f, true));
                    StartCoroutine(nextSound(2f));
                    StartCoroutine(ObjectActivation(gameObject, 2f, false));
                }
            }
            if (gameObject.name == "EarBrowCutter" && col.tag == "hair")
            {
                col.enabled = false;
                col.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                didTrigger++;
                EyeMode.instance.TaskfillBar.fillAmount += 0.026f;
                if (didTrigger > totalTriggers)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    EyeMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "Tube" && col.name == "SkinInfection")
            {
                EyeMode.instance.TaskfillBar.DOFillAmount(
                 EyeMode.instance.TaskfillBar.fillAmount + 1f,
                 5f).SetEase(Ease.Linear);
                gameObject.GetComponent<Image>().enabled = false;
                col.enabled = false;
                col.GetComponent<Animator>().enabled = true;
                StartCoroutine(ObjectActivation(EyeMode.Instance.NextBtn, 5f, true));
                StartCoroutine(nextSound(5f));
                StartCoroutine(ObjectActivation(gameObject, 5f, false));
            }
            if (gameObject.name == "Dropper" && col.name == "EyeDropAnim")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                gameObject.GetComponent<Image>().enabled = false;
                col.enabled = false;
                col.GetComponent<Animator>().enabled = true;
                StartCoroutine(ObjectActivation(EyeMode.Instance.NextBtn, 2f, true));
                EyeMode.instance.TaskfillBar.DOFillAmount(
                 EyeMode.instance.TaskfillBar.fillAmount + 1f,
                 2f).SetEase(Ease.Linear);
                StartCoroutine(nextSound(2f));
                StartCoroutine(ObjectActivation(gameObject.transform.parent.gameObject, 2f, false));
            }
            if (gameObject.name == "Lens" && col.name == "NewLens")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                EyeMode.instance.TaskfillBar.DOFillAmount(
                 EyeMode.instance.TaskfillBar.fillAmount + 1f,
                 1f).SetEase(Ease.Linear);
                gameObject.GetComponent<Image>().enabled = false;
                col.enabled = false;
                col.transform.GetChild(0).gameObject.SetActive(true);
                col.GetComponent<Image>().enabled = true;
                StartCoroutine(ObjectActivation(EyeMode.Instance.NextBtn, 1f, true));
                StartCoroutine(nextSound(1f));
                StartCoroutine(ObjectActivation(gameObject.transform.parent.gameObject, 1f, false));
            }
        }

        if (TeethMode.Instance)
        {
            if (gameObject.name == "pimpleremover" && col.name == "Pimple")
            {
                if (AudioManager.Instance.trigger)
                {
                    AudioManager.Instance.trigger.Play();
                }
                gameObject.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                TeethMode.instance.TaskfillBar.DOFillAmount(
                 TeethMode.instance.TaskfillBar.fillAmount + 0.5f,
                 2f).SetEase(Ease.Linear);
                Invoke("gameobjectImgecollider", 2.5f);
                TeethMode.Instance.pimpleIndex++;
                if (TeethMode.Instance.pimpleIndex > 1)
                {
                    TeethMode.Instance.pimpleIndex = 0;
                    StartCoroutine(ObjectActivation(TeethMode.Instance.NextBtn, 2.5f, true));
                    StartCoroutine(nextSound(2.5f));
                    StartCoroutine(ObjectActivation(gameObject, 2.5f, false));
                }
            }
            if (gameObject.name == "cavityRemover" && col.tag == "twister")
            {
                if (AudioManager.Instance.trigger)
                {
                    AudioManager.Instance.trigger.Play();
                }
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                TeethMode.Instance.pimpleIndex++;
                TeethMode.instance.TaskfillBar.DOFillAmount(
                TeethMode.instance.TaskfillBar.fillAmount + 0.125f,
                0.2f).SetEase(Ease.Linear);
                if (TeethMode.Instance.pimpleIndex > 7)
                {
                    TeethMode.Instance.pimpleIndex = 0;
                    StartCoroutine(ObjectActivation(TeethMode.Instance.NextBtn, 1f, true));
                    StartCoroutine(nextSound(1f));
                    StartCoroutine(ObjectActivation(gameObject, 1f, false));
                }
            }
            if (gameObject.name == "twiser" && col.tag == "tongdirt")
            {
                if (AudioManager.Instance.trigger)
                {
                    AudioManager.Instance.trigger.Play();
                }
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                TeethMode.Instance.pimpleIndex++;
                TeethMode.instance.TaskfillBar.fillAmount += 0.167f;
                if (TeethMode.Instance.pimpleIndex > 5)
                {
                    TeethMode.Instance.pimpleIndex = 0;
                    StartCoroutine(ObjectActivation(TeethMode.Instance.NextBtn, 1f, true));
                    StartCoroutine(nextSound(1f));
                    StartCoroutine(ObjectActivation(gameObject, 1f, false));
                }
            }
            if (gameObject.name == "brush" && col.name == "Teeth")
            {
                col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.1f);
                TeethMode.instance.TaskfillBar.fillAmount += 0.1f;
                if (col.GetComponent<Image>().color.a < 0)
                {
                    TeethMode.Instance.NextBtn.SetActive(true);
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "Tong" && col.name == "Toung")
            {
                col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.1f);
                TeethMode.instance.TaskfillBar.fillAmount += 0.1f;
                if (col.GetComponent<Image>().color.a < 0)
                {
                    TeethMode.Instance.NextBtn.SetActive(true);
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    gameObject.SetActive(false);
                }
            }

        }

        if (LipsMode.Instance)
        {
            if (gameObject.name == "EarBudInjury" && col.name == "Infection")
            {
                col.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, col.transform.GetChild(1).GetComponent<Image>().color.a + 0.1f);
                col.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, col.transform.GetChild(0).GetComponent<Image>().color.a - 0.1f);
                LipsMode.instance.TaskfillBar.fillAmount += 0.1f;
                if (col.transform.GetChild(1).GetComponent<Image>().color.a > 1)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    col.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    col.transform.GetChild(1).GetComponent<DOTweenAnimation>().DOPlay();
                    LipsMode.Instance.NextBtn.SetActive(true);;
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "PimplePoper" && col.name == "Pimple")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                gameObject.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                LipsMode.instance.TaskfillBar.DOFillAmount(
                 LipsMode.instance.TaskfillBar.fillAmount + 0.34f,
                 2f).SetEase(Ease.Linear);
                Invoke("gameobjectImgecollider", 2f);
                LipsMode.Instance.pimpleIndex++;
                if (LipsMode.Instance.pimpleIndex > 2)
                {
                    LipsMode.Instance.pimpleIndex = 0;
                    StartCoroutine(ObjectActivation(LipsMode.Instance.NextBtn, 2f, true));
                    StartCoroutine(nextSound(2f));
                    StartCoroutine(ObjectActivation(gameObject, 2f, false));
                }
            }
            if (gameObject.name == "hairCutter" && col.tag == "hair")
            {
                col.enabled = false;
                col.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                LipsMode.Instance.pimpleIndex++;
                LipsMode.instance.TaskfillBar.fillAmount += 0.009f;
                if (LipsMode.Instance.pimpleIndex > 106)
                {
                    LipsMode.Instance.pimpleIndex = 0;
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    LipsMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "Tube" && col.name == "Tube")
            {
                gameObject.GetComponent<Image>().enabled = false;
                col.enabled = false;
                col.GetComponent<Animator>().enabled = true;
                StartCoroutine(ObjectActivation(LipsMode.Instance.NextBtn, 6f, true));
                StartCoroutine(nextSound(6f));
                StartCoroutine(ObjectActivation(gameObject, 6f, false));
            }
            if (gameObject.name == "Twizeer" && col.name == "skin")
            {
                
                StartCoroutine(triggersfx(0.1f));
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(0).gameObject, 0f, false));
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(0).gameObject, 3f, true));
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                LipsMode.Instance.pimpleIndex++;
                LipsMode.instance.TaskfillBar.DOFillAmount(
                 LipsMode.instance.TaskfillBar.fillAmount + 0.2f,
                 2f).SetEase(Ease.Linear);
                if (LipsMode.Instance.pimpleIndex > 4)
                {
                    LipsMode.Instance.pimpleIndex = 0;
                    StartCoroutine(ObjectActivation(LipsMode.Instance.NextBtn, 3f, true));
                    StartCoroutine(nextSound(3f));
                    StartCoroutine(ObjectActivation(gameObject, 3f, false));
                }
            }
        }

        if (EarMode.Instance)
        {
            if (gameObject.name == "earring" && col.name == "earringinBox")
            {
                gameObject.transform.parent.GetChild(1).gameObject.SetActive(true);
                col.GetComponent<Image>().enabled = true;
                col.transform.parent.DOLocalMove(new Vector3(1500, -650, 0), 1f).SetDelay(0.5f);
                col.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                EarMode.instance.TaskfillBar.DOFillAmount(
                 EarMode.instance.TaskfillBar.fillAmount + 1f,
                 0.5f).SetEase(Ease.Linear);
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.AppearSfx)
                    {
                        AudioManager.Instance.AppearSfx.Play();
                    }
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                EarMode.Instance.NextBtn.SetActive(true);
                gameObject.SetActive(false);
            }
            if (gameObject.name == "pusRemover" && col.name == "earringparent")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                col.enabled = false;
                col.GetComponent<Animator>().enabled = true;
                gameObject.GetComponent<Image>().enabled = false;
                EarMode.instance.TaskfillBar.DOFillAmount(
                EarMode.instance.TaskfillBar.fillAmount + 1f,
                2f).SetEase(Ease.Linear);
                StartCoroutine(ObjectActivation(EarMode.Instance.NextBtn, 2f, true));
                StartCoroutine(nextSound(2f));
                StartCoroutine(ObjectActivation(gameObject, 2f, false));
            }
            if (gameObject.name == "PimplePoper" && col.name == "Pimple")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                gameObject.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                Invoke("gameobjectImgecollider", 2f);
                EarMode.Instance.pimpleIndex++;
                EarMode.instance.TaskfillBar.DOFillAmount(
                  EarMode.instance.TaskfillBar.fillAmount + 0.2f,
                  2f).SetEase(Ease.Linear);
                if (EarMode.Instance.pimpleIndex > 4)
                {
                    EarMode.Instance.pimpleIndex = 0;
                    StartCoroutine(ObjectActivation(EarMode.Instance.NextBtn, 2f, true));
                    StartCoroutine(nextSound(2f));
                    StartCoroutine(ObjectActivation(gameObject, 2f, false));
                }
            }
            if (gameObject.name == "EarBud" && col.tag == "mud")
            {
                col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.51f);
                if (col.GetComponent<Image>().color.a < 0)
                {
                    col.enabled = false;
                    EarMode.Instance.pimpleIndex++;
                    EarMode.Instance.TaskfillBar.fillAmount += 0.053f;
                    if (EarMode.Instance.pimpleIndex > 18)
                    {
                        EarMode.Instance.pimpleIndex = 0;
                        StartCoroutine(ObjectActivation(EarMode.Instance.NextBtn, 0f, true));
                        if (AudioManager.Instance)
                        {
                            if (AudioManager.Instance.AppearSfx)
                            {
                                AudioManager.Instance.AppearSfx.Play();
                            }
                        }
                        StartCoroutine(ObjectActivation(gameObject, 0f, false));
                    }
                }
            }
            if (gameObject.name == "Twizeer" && col.name == "skin")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(0).gameObject, 0f, false));
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(0).gameObject, 3f, true));
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                EarMode.Instance.pimpleIndex++;
                EarMode.instance.TaskfillBar.DOFillAmount(
                  EarMode.instance.TaskfillBar.fillAmount + 0.34f,
                  3f).SetEase(Ease.Linear);
                if (EarMode.Instance.pimpleIndex > 2)
                {
                    EarMode.Instance.pimpleIndex = 0;
                    StartCoroutine(ObjectActivation(EarMode.Instance.NextBtn, 3f, true));
                    StartCoroutine(nextSound(3f));
                    StartCoroutine(ObjectActivation(gameObject, 3f, false));
                }
            }
            if (gameObject.name == "earringinBox" && col.name == "hole")
            {
                col.transform.GetChild(0).gameObject.SetActive(true);
                gameObject.transform.parent.DOLocalMove(new Vector3(1500, -650, 0), 1f).SetDelay(0.5f);
                col.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                EarMode.instance.TaskfillBar.DOFillAmount(
                  EarMode.instance.TaskfillBar.fillAmount + 1f,
                  0.5f).SetEase(Ease.Linear);
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.AppearSfx)
                    {
                        AudioManager.Instance.AppearSfx.Play();
                    }
                }
                EarMode.Instance.NextBtn.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        if (BellyButtoMode.Instance)
        {
            if (gameObject.name == "pinDrag" && col.name == "pinBox")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                        AudioManager.Instance.trigger.Play();
                    if (AudioManager.Instance.AppearSfx)
                    {
                        AudioManager.Instance.AppearSfx.Play();
                    }
                }
                BellyButtoMode.instance.TaskfillBar.DOFillAmount(
                  BellyButtoMode.instance.TaskfillBar.fillAmount + 1f,
                  0.5f).SetEase(Ease.Linear);
                gameObject.transform.parent.GetChild(1).GetChild(0).gameObject.SetActive(true);
                col.GetComponent<Image>().enabled = true;
                col.GetComponent<Drag>().enabled = true;
                col.GetComponent<ScalePingPong>().enabled = true;
                col.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                col.transform.parent.DOLocalMove(new Vector3(1500, -650, 0), 1f).SetDelay(0.5f);
                BellyButtoMode.Instance.NextBtn.SetActive(true);
                gameObject.SetActive(false);
            }
            if (gameObject.name == "pusRemover" && col.name == "pusAnim")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                BellyButtoMode.instance.TaskfillBar.DOFillAmount(
                 BellyButtoMode.instance.TaskfillBar.fillAmount + 1f,
                 2f).SetEase(Ease.Linear);
                StartCoroutine(nextSound(2f));
                col.enabled = false;
                col.GetComponent<Animator>().enabled = true;
                gameObject.GetComponent<Image>().enabled = false;
                StartCoroutine(ObjectActivation(BellyButtoMode.Instance.NextBtn, 2f, true));
                StartCoroutine(ObjectActivation(gameObject, 2f, false));
            }
            if (gameObject.name == "EarBud" && col.name == "earbudDust")
            {
                col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.26f);
                BellyButtoMode.Instance.TaskfillBar.fillAmount += 0.06f;
                if (col.GetComponent<Image>().color.a < 0)
                {
                    col.enabled = false;
                    BellyButtoMode.Instance.Index++;
                    if (BellyButtoMode.Instance.Index > 3)
                    {
                        if (AudioManager.Instance)
                        {
                            if (AudioManager.Instance.AppearSfx)
                            {
                                AudioManager.Instance.AppearSfx.Play();
                            }
                        }
                        BellyButtoMode.instance.TaskfillBar.DOFillAmount(
                         BellyButtoMode.instance.TaskfillBar.fillAmount + 1f,
                         0.3f).SetEase(Ease.Linear);
                        BellyButtoMode.Instance.Index = 0;
                        StartCoroutine(ObjectActivation(BellyButtoMode.Instance.NextBtn, 0f, true));
                        StartCoroutine(ObjectActivation(gameObject, 0f, false));
                    }
                }
            }
            if (gameObject.name == "Twizeer" && col.name == "tweezerDust")
            {
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(0).gameObject, 0f, false));
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(0).gameObject, 3f, true));
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                BellyButtoMode.Instance.Index++;
                BellyButtoMode.instance.TaskfillBar.DOFillAmount(
                 BellyButtoMode.instance.TaskfillBar.fillAmount + 0.5f,
                 3f).SetEase(Ease.Linear);
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                        AudioManager.Instance.trigger.Play();
                }
                if (BellyButtoMode.Instance.Index > 1)
                {
                    StartCoroutine(nextSound(3f));
                    BellyButtoMode.Instance.Index = 0;
                    StartCoroutine(ObjectActivation(BellyButtoMode.Instance.NextBtn, 3f, true));
                    StartCoroutine(ObjectActivation(gameObject, 3f, false));
                }
            }
            if (gameObject.name == "Soap" && col.tag == "bubble")
            {
                col.enabled = false;
                col.GetComponent<Image>().enabled = true;
                BellyButtoMode.Instance.Index++;
                BellyButtoMode.Instance.TaskfillBar.fillAmount += 0.025f;
                if (BellyButtoMode.Instance.Index > 39)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    BellyButtoMode.Instance.Index = 0;
                    BellyButtoMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "cottonOne" && (col.tag == "bubble" || col.name == "dustnearbutton"))
            {
                if (gameObject.name == "cottonOne" && col.name == "dustnearbutton")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.1f);
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        col.enabled = false;
                        BellyButtoMode.Instance.Index++;
                        BellyButtoMode.Instance.TaskfillBar.fillAmount += 0.025f;
                    }
                }
                if (gameObject.name == "cottonOne" && col.tag == "bubble")
                {
                    col.enabled = false;
                    col.GetComponent<Image>().enabled = false;
                    BellyButtoMode.Instance.Index++;
                    BellyButtoMode.Instance.TaskfillBar.fillAmount += 0.025f;
                }
                if (BellyButtoMode.Instance.Index > 40)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    BellyButtoMode.Instance.Index = 0;
                    BellyButtoMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "Shawer" && col.name == "water")
            {
                col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a + 0.1f);
                BellyButtoMode.Instance.TaskfillBar.fillAmount += 0.1f;
                if (col.GetComponent<Image>().color.a > 1)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    BellyButtoMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "cottonTwo" && (col.tag == "mud" || col.name == "water"))
            {
                if (gameObject.name == "cottonTwo" && col.name == "water")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.1f);
                    BellyButtoMode.Instance.TaskfillBar.fillAmount += 0.1f;
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        col.enabled = false;
                        BellyButtoMode.Instance.Index++;
                    }
                }
                if (gameObject.name == "cottonTwo" && col.tag == "mud")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.51f);
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        col.enabled = false;
                        BellyButtoMode.Instance.Index++;
                    }
                }
                if (BellyButtoMode.Instance.Index > 6)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    BellyButtoMode.Instance.Index = 0;
                    BellyButtoMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "pinBox" && col.name == "Pinlast")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                        AudioManager.Instance.trigger.Play();
                }
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.AppearSfx)
                    {
                        AudioManager.Instance.AppearSfx.Play();
                    }
                }
                BellyButtoMode.instance.TaskfillBar.DOFillAmount(
                 BellyButtoMode.instance.TaskfillBar.fillAmount + 1f,
                 0.5f).SetEase(Ease.Linear);
                col.GetComponent<Image>().enabled = true;
                col.enabled = false;
                col.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                gameObject.transform.parent.DOLocalMove(new Vector3(1500, -650, 0), 1f).SetDelay(0.5f);
                BellyButtoMode.Instance.NextBtn.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        if (FootMode.Instance)
        {
            if (gameObject.name == "Shawer" && col.name == "ShawerParticals")
            {
                col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a + 0.06f);
                FootMode.instance.TaskfillBar.fillAmount += 0.06f;
                if (col.GetComponent<Image>().color.a > 1)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    FootMode.instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "cloth" && (col.name == "ShawerParticals" || col.name == "FootDust"))
            {
                if (gameObject.name == "cloth" && col.name == "FootDust")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.06f);
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        col.enabled = false;
                        didTrigger++;
                    }
                }
                if (gameObject.name == "cloth" && col.name == "ShawerParticals")
                {
                    FootMode.instance.TaskfillBar.fillAmount += 0.06f;
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.06f);
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        col.enabled = false;
                        didTrigger++;
                    }
                }
                if(didTrigger > 1)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    FootMode.instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "drySkinRemover" && col.name == "AnkleSkin")
            {
                col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.1f);
                FootMode.instance.TaskfillBar.fillAmount += 0.1f;
                if (col.GetComponent<Image>().color.a < 0)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    col.enabled = false;
                    FootMode.instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "Twizeer" && (col.tag == "throable" || col.tag == "itemIntray"))
            {
                if (gameObject.name == "Twizeer" && col.tag == "throable")
                {
                    if(gameObject.transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>().sprite == null)
                    {
                        gameObject.transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>().sprite = col.GetComponent<Image>().sprite;
                        gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = true;
                        col.gameObject.SetActive(false);
                        if (AudioManager.Instance)
                        {
                            if (AudioManager.Instance.itemPick)
                            {
                                AudioManager.Instance.itemPick.Play();
                            }
                        }
                    }
                }
                if (gameObject.name == "Twizeer" && col.tag == "itemIntray")
                {
                    if(gameObject.transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>().sprite != null)
                    {
                        col.GetComponent<Image>().sprite = gameObject.transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>().sprite;
                        col.GetComponent<Image>().enabled = true;
                        col.transform.GetChild(0).gameObject.SetActive(true);
                        gameObject.transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>().sprite = null;
                        gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = false;
                        FootMode.instance.TaskfillBar.DOFillAmount(
                            FootMode.instance.TaskfillBar.fillAmount + 0.1f,
                            0.1f).SetEase(Ease.Linear);
                        col.enabled = false;
                        FootMode.Instance.Index++;
                        if (AudioManager.Instance)
                        {
                            if (AudioManager.Instance.itemDrop)
                            {
                                AudioManager.Instance.itemDrop.Play();
                            }
                        }
                    }
                }
                if (FootMode.Instance.Index > 9)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    FootMode.Instance.Index = 0;
                    FootMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "Creamtube" && col.name == "Cream")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                col.enabled = false;
                col.GetComponent<Image>().enabled = true;
                FootMode.instance.TaskfillBar.DOFillAmount(
                    FootMode.instance.TaskfillBar.fillAmount + 0.5f,
                    0.2f).SetEase(Ease.Linear);
                FootMode.Instance.Index++;
                if (FootMode.Instance.Index > 1)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    FootMode.Instance.Index = 0;
                    FootMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "mesagger" && (col.name == "Cream" || col.name == "skin" || col.name == "CreamFoot"))
            {
                if (gameObject.name == "mesagger" && col.name == "skin")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.2f);
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        col.enabled = false;
                        FootMode.Instance.Index++;
                    }
                }
                if (gameObject.name == "mesagger" && col.name == "Cream")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.2f);
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        col.enabled = false;
                        FootMode.Instance.Index++;
                    }
                }
                if (gameObject.name == "mesagger" && col.name == "CreamFoot")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a + 0.05f);
                    FootMode.instance.TaskfillBar.fillAmount += 0.05f;
                    if (col.GetComponent<Image>().color.a > 1)
                    {
                        col.enabled = false;
                        FootMode.Instance.Index++;
                    }
                }
                if (FootMode.Instance.Index > 5)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    FootMode.Instance.Index = 0;
                    FootMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "nailCutterRemover" && col.name == "NailParent")
            {
                StartCoroutine(triggersfx(0.65f));
                gameObject.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                gameObject.transform.GetChild(1).gameObject.SetActive(false);
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(0).gameObject, 2f, true));
                StartCoroutine(ObjectActivation(gameObject.transform.GetChild(1).gameObject, 2f, true));
                col.gameObject.GetComponent<Animator>().enabled = true;
                col.enabled = false;
                Invoke("gameobjectImgecollider", 2f);
                FootMode.Instance.Index++;
                FootMode.instance.TaskfillBar.DOFillAmount(
                    FootMode.instance.TaskfillBar.fillAmount + 0.2f,
                    2f).SetEase(Ease.Linear);
                if (FootMode.Instance.Index > 4)
                {
                    StartCoroutine(nextSound(2f));
                    FootMode.Instance.Index = 0;
                    StartCoroutine(ObjectActivation(FootMode.Instance.NextBtn, 2f, true));
                    StartCoroutine(ObjectActivation(gameObject, 2f, false));
                }
            }
            if (gameObject.name == "droper" && col.name == "oilraw")
            {
                if (AudioManager.Instance)
                {
                    if (AudioManager.Instance.trigger)
                    {
                        AudioManager.Instance.trigger.Play();
                    }
                }
                FootMode.instance.TaskfillBar.DOFillAmount(
                    FootMode.instance.TaskfillBar.fillAmount + 0.5f,
                    0.2f).SetEase(Ease.Linear);
                col.enabled = false;
                col.GetComponent<Image>().enabled = true;
                FootMode.Instance.Index++;
                if (FootMode.Instance.Index > 1)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    FootMode.Instance.Index = 0;
                    FootMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            if (gameObject.name == "foam" && (col.name == "oilraw" || col.name == "OilLayer"))
            {
                if (gameObject.name == "foam" && col.name == "oilraw")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a - 0.2f);
                    if (col.GetComponent<Image>().color.a < 0)
                    {
                        FootMode.Instance.Index++;
                        col.enabled = false;
                    }
                }
                if (gameObject.name == "foam" && col.name == "OilLayer")
                {
                    col.GetComponent<Image>().color = new Color(1, 1, 1, col.GetComponent<Image>().color.a + 0.1f);
                    FootMode.instance.TaskfillBar.fillAmount += 0.1f;
                    if (col.GetComponent<Image>().color.a > 1)
                    {
                        FootMode.Instance.Index++;
                        col.enabled = false;
                    }
                }
                if (FootMode.Instance.Index > 2)
                {
                    if (AudioManager.Instance)
                    {
                        if (AudioManager.Instance.AppearSfx)
                        {
                            AudioManager.Instance.AppearSfx.Play();
                        }
                    }
                    FootMode.Instance.Index = 0;
                    FootMode.Instance.NextBtn.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        inTrigger = true;
       // if (MouseDownIndicator) MouseDownIndicator.SetActive(false);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        inTrigger = false;
        //if (MouseDownIndicator) MouseDownIndicator.SetActive(true);
    }

    private void OnEnable()
    {
        didTrigger = 0;
        if (boxCollider) boxCollider.enabled = true;
        if (m_Animator) m_Animator.enabled = false;
        if (pingPong) pingPong.enabled = true;
        if (MouseDownIndicator)
        {
            if (MouseDownIndicator.GetComponentInChildren<Image>())
            {
                MouseDownIndicator.GetComponentInChildren<Image>().enabled = true;
            }
        }
        if (MouseUpIndicator)
        {
            if (MouseUpIndicator.GetComponentInChildren<Image>())
            {
                MouseUpIndicator.GetComponentInChildren<Image>().enabled = true;
            }
        }
    }
    private void OnDisable()
    {
        DisableObjects();
        if (MouseDownIndicator)
        {
            if (MouseDownIndicator.GetComponentInChildren<Image>())
            {
                MouseDownIndicator.GetComponentInChildren<Image>().enabled = false;
            }
        }
        if (MouseUpIndicator)
        {
            if (MouseUpIndicator.GetComponentInChildren<Image>())
            {
                MouseUpIndicator.GetComponentInChildren<Image>().enabled = false;
            }
        }
        if (isPosAssigned) transform.localPosition = InitialPosition;
        if (gameObject.name == "EarBud" || gameObject.name == "cottonOne" || gameObject.name == "cottonTwo" || gameObject.name == "cotton" || gameObject.name == "cloth" || gameObject.name == "foam" || gameObject.name == "EarBudInjury" || gameObject.name == "EarBudCream" 
            || gameObject.name == "lipstick" || gameObject.name == "Tong")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.rubbing)
                    AudioManager.Instance.rubbing.Stop();
            }
        }
        if (gameObject.name == "Shawer")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.spray)
                    AudioManager.Instance.spray.Stop();
            }
        }
        if (gameObject.name == "Soap")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.bubble)
                    AudioManager.Instance.bubble.Stop();
            }
        }
        if (gameObject.name == "mesagger" || gameObject.name == "drySkinRemover")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.machine)
                    AudioManager.Instance.machine.Stop();
            }
        }
        if (gameObject.name == "brush")
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.brush)
                    AudioManager.Instance.brush.Stop();
            }
        }
    }

    private void DisableObjects()
    {
        if (boxCollider) boxCollider.enabled = false;
        if (MouseDownIndicator) MouseDownIndicator.SetActive(false);
        if (MouseUpIndicator) MouseUpIndicator.SetActive(false);
    }
    void gameobjectImgecollider()
    {
        gameObject.GetComponent<Image>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
    }

    #region ObjectActivation
    IEnumerator ObjectActivation(GameObject obj, float Delay, bool IsTrue)
    {
        yield return new WaitForSeconds(Delay);
        obj.SetActive(IsTrue);
    }
    IEnumerator nextSound(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        if (AudioManager.Instance)
        {
            if (AudioManager.Instance.AppearSfx)
            {
                AudioManager.Instance.AppearSfx.Play();
            }
        }
    }
    IEnumerator triggersfx(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        if (AudioManager.Instance)
        {
            if (AudioManager.Instance.trigger)
            {
                AudioManager.Instance.trigger.Play();
            }
        }
    }
    #endregion

}
