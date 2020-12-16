using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{
    public TMPro.TMP_Text textGameName;
    public TMPro.TMP_Text textAutor;
    public TMPro.TMP_Text textGenre;
    public TMPro.TMP_Text textTags;
    public Image image;


    public void SetGameName(string gameName)
    {
        textGameName.text = gameName;
    }


    public void SetAutor(string autor)
    {
        textAutor.text = autor;
    }


    public void SetGenre(string genre)
    {
        textGenre.text = genre;
    }


    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }


    public void SetTags(string tags)
    {
        if(textTags != null) textTags.text = tags;
    }
}
