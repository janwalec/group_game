using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class CoinController: ModifierController
{
    


    public CoinController() : base(0, 2)
    {

    }

 
    public override int influenceOutput(int nextValue)
    {
        return currentTotal * nextValue;
    }

    public override void calculateCurrentTotal(ModifierController prev)
    {
        if (prev == null)
        {
            currentTotal = currentValue;
        }
        else
        {
            currentTotal = prev.getCurrentTotal() * currentValue;
        }
        
        updateText();
    }

    

    public override void Roll()
    {
        base.Roll();        //sets currentValue to 0 or 1
        currentValue *= 2;  //scales to 0 or 2
        
    }


}