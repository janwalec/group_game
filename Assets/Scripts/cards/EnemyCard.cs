using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCard
{
    public int hitPoints; // how much we draw
    public string boatType; // regular or advanced

    public EnemyCard(int hp, string type)
    {
        this.hitPoints = hp;
        this.boatType = type;
    }

}