using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using TMPro;

public abstract class ModifierController : MonoBehaviour
{
    protected int currentValue = 0;
    protected int currentTotal = 0;
    protected int minVal;
    protected int maxVal;
    protected System.Random rand = new System.Random();
    public Canvas canvas;
    public ModifierController(int min, int max) {
        minVal = min;
        maxVal = max;
    }
    protected void Start()
    {
        updateText();
    }

    private void Update()
    {

    }
    public virtual void Roll()
    {
        currentValue = rand.Next(minVal, maxVal);
        //updateText();
    }
    public void updateText()
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = currentValue.ToString() + "      " + currentTotal.ToString();
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
}