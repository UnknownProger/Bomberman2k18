using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCoin : MonoBehaviour, IMob
{
    private Transform _transform;
    private Transform _targTrans;
    private Vector2 _currPos;
    private Vector2 _targPos;
    private Vector2 _nextPos;

    private int _targX;
    private int _targY;

    private int[,] _pathMap;
    private float _speed;
    private int[] _ignore;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        _targTrans = GameManager.GetPlayerTransform();;
    }

    private void Update()
    {
        Move();
        CheckFire();
    }
        
    public void Move()
    {
        _currPos = (Vector2)_transform.position;
        _targPos = (Vector2)_targTrans.position;



        if (_currPos.x != _nextPos.x || _currPos.y != _nextPos.y)
        {
            _transform.position = Vector2.MoveTowards(_currPos, _nextPos, _speed * Time.deltaTime);
        }
        else
        {
            int currX = Mathf.RoundToInt(_currPos.x);
            int currY = Mathf.RoundToInt(_currPos.y);
            int targX = Mathf.RoundToInt(_targPos.x);
            int targY = Mathf.RoundToInt(_targPos.y);

            if (_targX != targX || _targY != targY)
            {
                _pathMap = PathFinder.FindPath(currX, currY, targX, targY, _ignore);
                _targX = targX;
                _targY = targY;
            }

            _nextPos = PathFinder.GetNextPosition(_pathMap, currX, currY);
        }
            
    }

    private void Init()
    {
        _transform = transform;
        _currPos = _transform.position;
        _nextPos = _currPos;
        _speed = 3.5f;
        _ignore = new int[2];
        _ignore[0] = 1;
        _ignore[1] = 2;
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
