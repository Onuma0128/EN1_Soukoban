using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManageScript : MonoBehaviour
{
    int[,] map;
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    GameObject[,] field;

    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null)
                {
                    continue;
                }
                if (field[y, x].tag == "Player")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
    Vector2Int GetBoxIndex()
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (field[y, x] == null)
                {
                    continue;
                }
                if (field[y, x].tag == "Box")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo)
    {
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y,moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        field[moveTo.y,moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        field[moveTo.y, moveTo.x].transform.position = new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
        return true;
    }

    //Start is called before the first frame update
    void Start()
    {
        //配列の実態の生成と初期化

        map = new int[,]
        {
            {0,0,0,0,0},
            {0,2,1,2,0},
            {0,0,0,0,0},
        };
        field = new GameObject
        [
            map.GetLength(0),
            map.GetLength(1)
        ];

        for(int y = 0; y < map.GetLength(0); y++)
        {
            for(int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y,x] == 1)
                {
                    field[y,x] = Instantiate(
                       playerPrefab,
                       new Vector3(x , map.GetLength(0) - y, 0),
                       Quaternion.identity
                    );
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                       boxPrefab,
                       new Vector3(x, map.GetLength(0) - y, 0),
                       Quaternion.identity
                    );
                }
            }
        }
    }

    //update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //Vector2Int boxindex = GetBoxIndex();
            //移動処理のメソッド化
            MoveNumber(playerindex, playerindex + new Vector2Int(1, 0)) ;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //Vector2Int boxindex = GetBoxIndex();
            //移動処理のメソッド化
            MoveNumber(playerindex, playerindex + new Vector2Int(-1, 0));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //Vector2Int boxindex = GetBoxIndex();
            //移動処理のメソッド化
            MoveNumber(playerindex, playerindex + new Vector2Int(0, -1));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //Vector2Int boxindex = GetBoxIndex();
            //移動処理のメソッド化
            MoveNumber(playerindex, playerindex + new Vector2Int(0, 1));
        }
    }
}
