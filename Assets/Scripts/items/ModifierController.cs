using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using TMPro;
using Unity.VisualScripting;

public abstract class ModifierController : MonoBehaviour
{
    protected int currentValue = 0;
    protected int currentTotal = 0;
    protected int minVal;
    protected int maxVal;
    protected System.Random rand = new System.Random();
    public Canvas canvas;
    public GameObject multiplicationSigns;
    public GameObject additionSigns;
    protected AudioSource audioSource;

    protected Animator animator;
    [SerializeField] protected AudioClip onRollingSound;
    public ModifierController(int min, int max) {
        minVal = min;
        maxVal = max;
    }
    protected void Start()
    {
        updateText();
        canvas.gameObject.SetActive(false);
        

        EraseOperations();
        //SetOperation(ChainGenerator.Direction.TOP_RIGHT, ChainGenerator.Operation.MULTIPLICATION);
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {   
        
    }

    public virtual void ChangeAnimation() {
        animator.SetTrigger("Flip");
    }

    public virtual void Roll()
    {
        currentValue = rand.Next(minVal, maxVal);
        //updateText();
        audioSource.PlayOneShot(onRollingSound, AudioListener.volume);
    }

    public void updateText()
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = currentValue.ToString() + "       " + currentTotal.ToString();
    }
    public void setCurrentTotal(int currentTotal_)
    {
        currentTotal = currentTotal_;
    }
    public abstract int influenceOutput(int nextValue);
    public abstract void calculateCurrentTotal(ModifierController prev);
    public int getCurrentValue()
    {
        return currentValue;
    }
    public int getCurrentTotal()
    {
        return currentTotal;
    }
    public void activateCanvas()
    {
        canvas.gameObject.SetActive(true);
    }
    public void deactivateCanvas()
    {
        canvas.gameObject.SetActive(false);
    }

    public void SetOperation(ChainGenerator.Direction direction, ChainGenerator.Operation operation)
    {
        GameObject currentSign = operation == ChainGenerator.Operation.MULTIPLICATION ? multiplicationSigns : additionSigns;
        String position = "";
        switch (direction)
        {
            case ChainGenerator.Direction.MIDDLE_LEFT:
                position = "MiddleLeft";
                break;
            case ChainGenerator.Direction.MIDDLE_RIGHT:
                position = "MiddleRight";
                break;
            case ChainGenerator.Direction.BOTTOM_LEFT:
                position = "BottomLeft";
                break;
            case ChainGenerator.Direction.BOTTOM_RIGHT:
                position = "BottomRight";
                break;
            case ChainGenerator.Direction.TOP_LEFT:
                position = "TopLeft";
                break;
            case ChainGenerator.Direction.TOP_RIGHT:
                position = "TopRight";
                break;
        }
        currentSign.transform.Find(position).gameObject.SetActive(true);
    }

    public void EraseOperations()
    {
        int childrenNum = multiplicationSigns.transform.childCount;
        for (int i = 0; i < childrenNum; i++)
        {
            multiplicationSigns.transform.GetChild(i).gameObject.SetActive(false);
        }


        childrenNum = additionSigns.transform.childCount;
        for (int i = 0; i < childrenNum; i++)
        {
            additionSigns.transform.GetChild(i).gameObject.SetActive(false);
        }

    }
}