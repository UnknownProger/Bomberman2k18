using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBall : MonoBehaviour, IMob
{
    private Transform _transform;
    private Vector2 _currPos;
    private Vector2 _nextPos;

    private float _speed;

    private void Awake()
    {
        Init();
    }
        
	// Update is called once per frame
	void Update () 
    {
        Move();
        CheckFire();
	}

    public void Move()
    {
        _currPos = (Vector2)_transform.position;

        if (_currPos.x != _nextPos.x || _currPos.y != _nextPos.y)
        {
            _transform.position = Vector2.MoveTowards(_currPos, _nextPos, _speed * Time.deltaTime);
        }
        else
        {
            int currX = Mathf.RoundToInt(_currPos.x);
            int currY = Mathf.RoundToInt(_currPos.y);

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

    private void Init()
    {
        _transform = transform;
        _currPos = _transform.position;
        _nextPos = _currPos;
        _speed = 1f;
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
