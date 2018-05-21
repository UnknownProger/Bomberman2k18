using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobOrange : MonoBehaviour, IMob {

    private Transform _transform;
    private Transform _targTrans;
    private Vector2 _currPos;
    private Vector2 _targPos;
    private Vector2 _nextPos;

    private int _targX;
    private int _targY;

    private int[,] _pathMap;

    private float _speed;

    private bool isFollow;

    private int _bufferedId;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        _targTrans = GameManager.GetPlayerTransform();
    }

    private void Update()
    {
        Move();
        CheckFire();
        if (!isFollow)
        {
            isFollow = CheckVisibility();
        }
    }

    public void Move()
    {
        _currPos = (Vector2)_transform.position;
        _targPos = (Vector2)_targTrans.position;

        int currX;
        int currY;

        if (_currPos.x != _nextPos.x || _currPos.y != _nextPos.y)
        {
            _transform.position = Vector2.MoveTowards(_currPos, _nextPos, _speed * Time.deltaTime);
        }
        else
        {
            currX = Mathf.RoundToInt(_currPos.x);
            currY = Mathf.RoundToInt(_currPos.y);
            int targX = Mathf.RoundToInt(_targPos.x);
            int targY = Mathf.RoundToInt(_targPos.y);

            if (isFollow)
            {
                if (_targX != targX || _targY != targY)
                {
                    _pathMap = PathFinder.FindPath(currX, currY, targX, targY);
                    _targX = targX;
                    _targY = targY;
                }

                if (_pathMap == null)
                {
                    isFollow = false;
                    return;
                }
                _nextPos = PathFinder.GetNextPosition(_pathMap, currX, currY);
            }
            else
            {
                List<Vector2> aroundList = new List<Vector2>();

                if (Level.map[currX - 1, currY].typeId == 0)
                {
                    aroundList.Add(new Vector2(currX - 1, currY));
                }
                if (Level.map[currX + 1, currY].typeId == 0)
                {
                    aroundList.Add(new Vector2(currX + 1, currY));
                }
                if (Level.map[currX, currY - 1].typeId == 0)
                {
                    aroundList.Add(new Vector2(currX, currY - 1));
                }
                if (Level.map[currX, currY + 1].typeId == 0)
                {
                    aroundList.Add(new Vector2(currX, currY + 1));
                }

                if (aroundList.Count > 0)
                {
                    System.Random rand = new System.Random();
                    int randId = rand.Next(aroundList.Count);

                    _nextPos = aroundList[randId];
                }
                else
                {
                    _nextPos = _currPos;
                }
            }
                
        }
            
    }
        
    private bool CheckDistance(float minDist)
    {
        return (_targTrans.position - _transform.position).sqrMagnitude <= minDist * minDist;
    }

    private bool CheckVisibility()
    {
  
        bool isVisible = true;

        if (CheckDistance(3))
        {
            int x = Mathf.RoundToInt(_transform.position.x);
            int y = Mathf.RoundToInt(_transform.position.y);
            int currX = x;
            int currY = y;
            int targX = Mathf.RoundToInt(_targTrans.position.x);
            int targY = Mathf.RoundToInt(_targTrans.position.y);

            int direct;

            if (currY == targY && currX != targX)
            {
                if (currX > targX)
                {
                    int tmp;
                    tmp = x;
                    x = targX;
                    targX = tmp;
                }

                for (; x <= targX; x++)
                {
                    int typeId = Level.map[x, y].typeId;

                    if (typeId != 0)
                    {
                        isVisible = false;
                        break;
                    }
                }
            }
            else if (currX == targX && currY != targY)
            {
                if (currY < targY)
                {
                    int tmp;
                    tmp = y;
                    y = targY;
                    targY = tmp;
                }

                for (; y <= targY; y++)
                {
                    int typeId = Level.map[x, y].typeId;

                    if (typeId != 0)
                    {
                        isVisible = false;
                        break;
                    }
                }
            }
            else
            {
                isVisible = false;
            }
        }
        else
        {
            isVisible = false;
        }
        return isVisible;
    }

    private void Init()
    {
        _transform = transform;
        _currPos = _transform.position;
        _nextPos = _currPos;
        _speed = 3f;
    }

    private void CheckFire()
    {
        int x = Mathf.RoundToInt(_transform.position.x);
        int y = Mathf.RoundToInt(_transform.position.y);

        if (Level.map[x, y].typeId == 4)
        {
            Level.map[x, y].typeId = 0;
            MobSpawner.KillMob(_transform);
        }
    }
}
