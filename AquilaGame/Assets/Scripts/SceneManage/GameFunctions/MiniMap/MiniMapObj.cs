using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapObj : MonoBehaviour
{
    public UnityEngine.UI.Image Img = null;
    public void SetUp(Character character)
    {
        Color color;
        switch (character.characterType)
        {
            case CharacterType.Enemy:
                color = new Color(0.8679245f, 0.02046992f, 0.2820109f);
                break;
            case CharacterType.Neutral:
                color = new Color(0.8980392f, 0.8252381f, 0.2509804f);
                break;
            case CharacterType.Friend:
                color = new Color(0, 0.7486f, 1);
                break;
            default:
                color = Color.white;
                break;
        }
        Img.color = color;
    }
}
