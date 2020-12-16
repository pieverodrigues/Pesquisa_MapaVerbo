using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameInfo", menuName = "Game Info", order = 1)]
[System.Serializable]
public class GameInfo : ScriptableObject
{
    [NaughtyAttributes.ShowAssetPreview(128, 128)]
    public Sprite cover;
    [Space]
    public string gameName;
    public string autor;
    public string gender;

    [Tooltip("Tags separadas por virgula.")]
    [TextArea]
    public string tags;


    private string[] tagsArray;


    private void OnValidate()
    {
        SetupTagArray();
    }


    public void SetupTagArray()
    {
        tagsArray = tags.Split(new[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);
    }


    public string[] GetTagsArray()
    {
        if(tagsArray.Length == 0)
        {
            SetupTagArray();
        }

        return tagsArray;
    }
}
