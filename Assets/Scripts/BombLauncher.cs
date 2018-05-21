using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

public class BombLauncher : MonoBehaviour
{
    private static BombLauncher _instance;
    public static BombLauncher instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            return new GameObject("BombLauncher").AddComponent<BombLauncher>();
        }
    }
    private float _time = 2f;
    private int _fireSize = 3;
    public int fireSize
    {
        get
        { 
            return _fireSize; 
        }
        set
        {
            _fireSize = value; 
        }
    }
    private int _maxBombs = 1;
    public  int maxBombs
    {
        get
        { 
            return _maxBombs; 
        }
        set
        { 
            _maxBombs = value; 
        }
    }
    private int _placedBombs; 

    private Queue<List<Vector2>> _bombsPosQueue = new Queue<List<Vector2>>();

    private void Awake()
    {
        if (_instance != null)
        {
            Component.Destroy(this);
            return;
        }
        _instance = this;
    }

    public void SetBomb(int x, int y)
    {
        if (_placedBombs < _maxBombs)
        {   
            _placedBombs++;
            GameObject bomb = ObjectCreator.CreateView("Bomb", new Vector2(x, y), 9);
            StartCoroutine(BombTimer(bomb, _time));
        }
    }
       
    private IEnumerator BombTimer(GameObject bomb, float time)
    {
        yield return new WaitForSeconds(time);

        Explosing((int)bomb.transform.position.x, (int)bomb.transform.position.y);
        GameObject.Destroy(bomb);
    }

    private IEnumerator Burning(float time)
    {
        _placedBombs--;

        List<Vector2> posList = _bombsPosQueue.Dequeue();

        int x;
        int y;
        int typeId;
        
        for (int i = 0; i<posList.Count; i++)
        {
            x = (int)posList[i].x;
            y = (int)posList[i].y;
            typeId = Level.map[x, y].typeId;

            if (typeId != 2)
            {
                Level.map[x, y].typeId = 4;
                GameObject fire = ObjectCreator.CreateView("Fire",new Vector2(x, y), 4, 2);
                Destroy(fire, time);
            }
            else
            {
                Level.map[x, y].view.sprite = GameManager.GetSprite(14);
            }
        }

        yield return new WaitForSeconds(time);

        for (int i = 0; i < posList.Count; i++)
        {
            x = (int)posList[i].x;
            y = (int)posList[i].y;
            typeId = Level.map[x, y].typeId;

            if (typeId == 4)
            {
                Level.map[x, y].typeId = 0;
            }
            else
            {
                if (typeId == 2)
                {
                    Level.map[x, y].view.sprite = GameManager.GetSprite(0);
                    Level.map[x, y].typeId = 0;
                    Bonus.GenerateBonus(x, y);
                }
            }
        }
    }

    private void Explosing(int x, int y)
    {
        List<Vector2> posList = new List<Vector2>();

        posList.Add(new Vector2(x, y));

        //_firePosList.Add(new Vector2(x, y));

        bool allow_pX = true;
        bool allow_nX = true;
        bool allow_pY = true;
        bool allow_nY = true;

        for (int i = 1; i < _fireSize; i++)
        {
            int pX = x + i;
            int nX = x + i * -1;
            int pY = y + i;
            int nY = y + i * -1;

            if (allow_pX)
            {
                CheckCell(pX, y, ref allow_pX, ref posList);
            }

            if (allow_nX)
            {
                CheckCell(nX, y, ref allow_nX, ref posList);
            }

            if (allow_pY)
            {
                CheckCell(x, pY, ref allow_pY, ref posList);
            }

            if (allow_nY)
            {
                CheckCell(x, nY, ref allow_nY, ref posList);
            }
        }

        _bombsPosQueue.Enqueue(posList);
            
        StartCoroutine(Burning(.5f));
    }

    private void CheckCell(int x, int y, ref bool allow, ref List<Vector2> posList)
    {
        

        if (Level.map[x, y].typeId != 1)
        {
            //_firePosList.Add(new Vector2(x, y));
            posList.Add(new Vector2(x, y));

            if (Level.map[x, y].typeId == 2)
            {

                allow = false;
            }
        }
        else
        {
            allow = false;
        }
    }
}
