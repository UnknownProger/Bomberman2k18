using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bonus
{
    private static Dictionary<Vector2, BonusItem> _currentItems = new Dictionary<Vector2, BonusItem>(); 
    public static Dictionary<Vector2, BonusItem> currentItems
    {
        get { return _currentItems; }
    } 

    private static float[] _probabilities =
    {
        50f,    //0 - none
        20f,    //1 - more fire
        10f,    //2 - more bombs
        20f,    //3 - roller
        10f     //4 - armor
    };

    public static bool GenerateBonus(int x, int y)
    {
        bool isGenerated = false;
        int id = GetRandomId(_probabilities);

        switch (id)
        {
            case 1:
                {
                    InitBonus("BonusFire", new Vector2(x,y), 10, true, false, false, false);  
                    break;
                }
            case 2:
                {
                    InitBonus("BonusBomb", new Vector2(x,y), 11, false, true, false, false);
                    break;
                }
            case 3:
                {
                    InitBonus("BonusRoller", new Vector2(x,y), 12, false, false, true, false);
                    break;
                }
            case 4:
                {
                    InitBonus("BonusArmor", new Vector2(x,y), 13, false, false, false, true);
                    break;
                }
        }

        if (id != 0)
        {
            isGenerated = true;
        }
        return isGenerated;
    }
        
    private static int GetRandomId(float[] probs)
    {
        float total = 0;

        foreach (float el in probs)
        {
            total += el;
        }

        float randVal = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randVal < probs[i])
            {
                return i;
            }
            else
            {
                randVal -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    private static void InitBonus(string name, Vector2 pos, int spriteId, bool fire, bool bomb, bool roller, bool armor)
    {
        GameObject view = ObjectCreator.CreateView(name, pos, spriteId);
        _currentItems.Add(pos, new BonusItem(view, fire, bomb, roller, armor)); 
    }

    public static BonusItem GetBonus(Vector2 pos)
    {
        BonusItem bonus = _currentItems[pos];
        GameObject.Destroy(bonus.view);
        _currentItems.Remove(pos);
        return bonus;
    }
}

public struct BonusItem
{
    public BonusItem(GameObject view, bool fire, bool bomb, bool roller, bool armor)
    {
        this.view = view;
        this.fire = fire;
        this.bomb = bomb;
        this.roller = roller;
        this.armor = armor;
    }

    public GameObject view;
    public bool fire;
    public bool bomb;
    public bool roller;
    public bool armor;
}


