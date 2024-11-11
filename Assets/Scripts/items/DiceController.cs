using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class DiceController : ModifierController
{
    public DiceController() : base(1,7)
    {
        
    }
   

    public override int influenceOutput(int nextValue)
    {
        return currentTotal + nextValue;
    }

    public override void calculateCurrentTotal(ModifierController prev)
    {
        {
            if (prev == null)
            {
                currentTotal = currentValue;
            }
            else
            {
                currentTotal = prev.influenceOutput(currentValue);
            }
            updateText();
        }
    }
}