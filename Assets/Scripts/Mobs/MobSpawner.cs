//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MobSpawner
{
    private static List<Transform> _mobList;
    public static List<Transform> mobList
    {
        get
        {
            if (_mobList == null)
            {
                _mobList = new List<Transform>();
            }
            return _mobList;
        }
    }
    public static int mobCount
    {
        get
        {
            return mobList.Count;
        }
    }

    //0 - MobBall
    //1 - MobOrange
    //2 - MobGhost
    //3 - MobCoin

    public static void SpawnMobs(int count)
    {
        System.Random rand = new System.Random();
        _mobList = new List<Transform>();

        int id;
        int x;
        int y;

        while (count > 0)
        {
            x = rand.Next(1, Level.sizeX - 1);
            y = rand.Next(1, Level.sizeY - 1);


            if (x == 1 && y == Level.sizeY - 2 || x == 2 && y == Level.sizeY - 2 || x == 1 && y == Level.sizeY - 3 || Level.map[x, y].typeId != 0)
            {
                continue;
            }

            id = rand.Next(3);

            Transform mob = null;

            switch (id)
            {
                case 0:
                    {
                        mob = Spawn<MobBall>(x, y, 5).transform;
                        break;
                    }
                case 1:
                    {
                        mob = Spawn<MobOrange>(x, y, 6).transform;
                        break;
                    }
                case 2:
                    {
                        mob = Spawn<MobGhost>(x, y, 7).transform;
                        break;
                    }
            }

            if(mob != null)
                _mobList.Add(mob);

            count--;
        }
    }

    public static void SpawnBosses()
    {
        _mobList.Add(Spawn<MobCoin>(Level.sizeX-2, Level.sizeY-2, 8).transform);
        _mobList.Add(Spawn<MobCoin>(Level.sizeX-2, 1, 8).transform);
    }

    private static T Spawn<T>(int x, int y, int spriteId) where T : MonoBehaviour, IMob
    {
        GameObject gameObject = ObjectCreator.CreateView(typeof(T).Name, new Vector2(x, y), spriteId); 
        return gameObject.AddComponent<T>();
    }

    public static void KillMob(Transform mob)
    {
        _mobList.Remove(mob);
        GameObject.Destroy(mob.gameObject);

        if (_mobList.Count == 0)
        {
            GameManager.SpawnPortal();
        }
    }

}
