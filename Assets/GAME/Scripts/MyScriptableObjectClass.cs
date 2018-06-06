using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class MyScriptableObjectClass : ScriptableObject
{
    public string Level1 = "Level1";
    public int EnemiesKilled = 0;
}