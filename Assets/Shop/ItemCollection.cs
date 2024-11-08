using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[CreateAssetMenu(fileName = "NewItemCollection", menuName = "Shop/ItemCollection")]
public class ItemCollection : ScriptableObject
{
    public ShopItem[] items;
}
