using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectCreator 
{

    public static GameObject CreateView(string name, Vector2 pos, int spriteId, int order=1)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = pos;
        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.GetSprite(spriteId);
        spriteRenderer.sortingOrder = order;

        return obj;
    }


}

