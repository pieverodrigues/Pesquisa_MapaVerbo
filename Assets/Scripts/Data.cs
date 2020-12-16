using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MacroGroup
{
    [HorizontalLine(color: EColor.Pink)]
    public string tag;
    [HorizontalLine(color: EColor.Gray)]
    public GameInfo[] morePresents;
    [HorizontalLine(color: EColor.Gray)]
    public GameInfo[] lessPresents;
    
}


[CreateAssetMenu(fileName = "Data",menuName = "Data",order = 0)]
public class Data : ScriptableObject
{
    [ReorderableList]
    public List<MacroGroup> macroGroups;

    [Header("Most Seen Window")]
    [Range(1, 20)] public int contentColumnsMOLS;
    [Range(1, 20)] public int contentRowsMOLS;

    [Header("Subtag Window")]
    [Range(1, 20)] public int contentColumnsST;
    [Range(1, 20)] public int contentRowsST;
}
