using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BombLauncher))]
public class PlayerController : MonoBehaviour {

    private float _speed;
    private float _speedMax;
    private Transform _transform;
    private BombLauncher _bombLauncher;
    private bool _isImmortal;
    private Vector2 _target;
    private bool _isDead;


    void Awake()
    {
        Init();
    }
        
	// Update is called once per frame
	void Update () 
    {
        if (!_isDead)
        {
            Walk();

            if (!_isImmortal)
            {
                if (CheckEnemyCollision() || CheckFireCollision())
                {
                    _isDead = true;
                    GameManager.SendGameResult(false);
                }
            }

            if (CheckBonusCollision())
            {
                Vector2 bonusPos = (new Vector2(Mathf.RoundToInt(_transform.position.x), Mathf.RoundToInt(_transform.position.y)));
                BonusItem item = Bonus.GetBonus(bonusPos);
                UseBonus(item);
            }

            if (CheckPortalCollision())
            {
                GameManager.SendGameResult(true);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                int x = Mathf.RoundToInt(transform.position.x);
                int y = Mathf.RoundToInt(transform.position.y);
                _bombLauncher.SetBomb(x, y);
            }
        }
	} 

    void Init()
    {
        _transform = transform;
        _target = _transform.position;
        _speed = 3f;
        _speedMax = 7f;
        _isImmortal = false;
        _isDead = false;

        GameObject cam = new GameObject("Camera");
        cam.transform.SetParent(_transform);
        cam.transform.localPosition = Vector2.zero;

        Camera camComp = cam.AddComponent<Camera>();
        camComp.backgroundColor = Color.black;
        camComp.orthographic = true;
        camComp.orthographicSize = 5f;
        camComp.nearClipPlane = 0f;

        _bombLauncher = GetComponent<BombLauncher>();
    }

    #region movement

    void Walk()
    {
        Vector2 currPos = (Vector2)transform.position; 

        if (_target == currPos)
        {
            _target = GetNewPosition();
        }

        transform.position = Vector2.MoveTowards((Vector2)_transform.position, _target, _speed * Time.deltaTime);
    }

    private Vector2 GetNewPosition()
    {
        MapCell[,] map = Level.map;

        int currX = Mathf.RoundToInt(transform.position.x);
        int currY = Mathf.RoundToInt(transform.position.y);

        int x = (int)Input.GetAxisRaw("Horizontal");
        int y = (int)Input.GetAxisRaw("Vertical");

        int targX = currX + x;
        int targY = currY + y;

        if (x != 0)
        {
            if (map[targX, currY].typeId == 1 || map[targX, currY].typeId == 2 )
            {
                targX = currX;
            }
        }

        if (y != 0)
        {
            if (map[currX, targY].typeId == 1 || map[currX, targY].typeId == 2)
            {
                targY = currY;
            }
        }

        if (x != 0 && y != 0)
        {
            targX = currX;
            targY = currY;
        }
           
        return new Vector2(targX, targY);
    }

    #endregion

    #region bonuses

    private void UseBonus(BonusItem bonus)
    {
        if (bonus.fire)
        {
            _bombLauncher.fireSize++;
        }
        if (bonus.bomb)
        {
            _bombLauncher.maxBombs++;
        }
        if (bonus.roller)
        {
            _speed = Mathf.Clamp(_speed * 1.25f, _speed, _speedMax);
        }
        if(bonus.armor)
        {
            StartCoroutine(ImmortalTimer(30f));
        }
    }

    private IEnumerator ImmortalTimer(float time)
    {
        _isImmortal = true;
        yield return new WaitForSeconds(time);
        _isImmortal = false;
    }

    #endregion

    #region collisions

    private bool CheckEnemyCollision()
    {
        for(int i=0; i< MobSpawner.mobCount; i++)
        {
            if (((Vector2)MobSpawner.mobList[i].position - (Vector2)_transform.position).sqrMagnitude < 1f)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckFireCollision()
    {
        int x = Mathf.RoundToInt(_transform.position.x);
        int y = Mathf.RoundToInt(_transform.position.y);
        if (Level.map[x, y].typeId == 4)
        {
            return true;
        }
        return false;
    }
        
    private bool CheckBonusCollision()
    {
        foreach (KeyValuePair<Vector2, BonusItem> kvp in Bonus.currentItems)
        {
            if (((Vector2)kvp.Key - (Vector2)_transform.position).sqrMagnitude < .25f)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckPortalCollision()
    {
        int x = Mathf.RoundToInt(_transform.position.x);
        int y = Mathf.RoundToInt(_transform.position.y);
        if (Level.map[x, y].typeId == 5)
        {
            return true;
        }
        return false;
    }

    #endregion


}
