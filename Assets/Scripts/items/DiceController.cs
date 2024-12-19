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

    public override void ActivateMiniVersion()
    {
        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    public override void DeactivateMiniVersion()
    {
        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}