using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Level
{
    private static MapCell[,] _map;
    public static MapCell[,] map 
    {
        get{ return _map; }
        set { _map = value; }
    }

    private static Transform _mapTransform;
    private static int _sizeX;
    private static int _sizeY;
    public static int sizeX
    {
        get{ return _sizeX; }
    }
    public static int sizeY
    {
        get{ return _sizeY; }
    }
        
    // 0 - walkable
    // 1 - wall
    // 2 - destuctible wall
    // 3 - bonus
    // 4 - fire
        
    private static void GenerateMap()
    {
        _map = new MapCell[_sizeX, _sizeY];

        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                if (x % 2 == 0 && y % 2 == 0 || x == 0 || x == _sizeX - 1 || y == 0 || y == _sizeY - 1)
                {
                    _map[x, y].typeId = 1;
                }
                else
                {
                    _map[x, y].typeId = 0;
                }
            }
        }

        int mobCount = 5;
        int undestructCenterWalls;
        int destuctWalls;

        undestructCenterWalls = (int)((Level.sizeX - 2) * .5f) * (int)((Level.sizeY - 2) * .5f);
        destuctWalls = (Level.sizeX - 2) * (Level.sizeY - 2);
        destuctWalls -= undestructCenterWalls;
        destuctWalls -= mobCount;
        destuctWalls -= Random.Range((int)(destuctWalls / 2f), (int)(destuctWalls / 1.5f)); 
        destuctWalls -= 2; // two blocks for isolation player

        _map[3, Level.sizeY-2].typeId = 2;
        _map[1, Level.sizeY-4].typeId = 2;

        while (destuctWalls > 0)
        {
            int rndX = Random.Range(1, _sizeX);
            int rndY = Random.Range(1, _sizeY);

            if (rndX == 1 && rndY == _sizeY - 2 || rndX == 2 && rndY == _sizeY - 2 || rndX == 1 && rndY == _sizeY - 3)
            {
                continue;
            }

            if (_map[rndX, rndY].typeId == 0)
            {
                _map[rndX, rndY].typeId = 2;
                destuctWalls--;
            }
        }            
    }

    private static void SetCell(int spriteId, int x, int y)
    {
        GameObject cell = ObjectCreator.CreateView("Cell", new Vector2(x, y), spriteId, 0);
        cell.transform.SetParent(_mapTransform);

        SpriteRenderer view = cell.GetComponent<SpriteRenderer>();
        /*view.sprite = GameManager.GetSprite(id);
        view.sharedMaterial.SetFloat("PixelSnap", 1f);*/
        _map[x, y].view = view;
    }

    public static void Build(int sizeX, int sizeY)
    {
        _mapTransform = new GameObject("Map").transform;

        _sizeX = sizeX;
        _sizeY = sizeY;

        GenerateMap();

        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                switch (_map[x, y].typeId)
                {
                    case 0:
                        {
                            SetCell(0, x, y);
                            break;
                        }
                    case 1:
                        {
                            SetCell(1, x, y);
                            break;
                        }
                    case 2:
                        {
                            SetCell(2, x, y);
                            break;
                        }
                }
            }
        }
    }
        
    public static void PrintMap()
    {
        string arr = "";
        for (int y = 14; y >= 0; y--)
        {
            for (int x = 0; x < 15; x++)
            {
                arr += Level.map[x,y].typeId + ",";

            }
            arr+="\n";
        }
        Debug.Log(arr);
    }
}

public struct MapCell
{
    public int typeId;
    public SpriteRenderer view;
}
