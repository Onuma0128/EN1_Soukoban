using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManageScript : MonoBehaviour
{
    int[,] map;
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    public GameObject ParticlePrefab;
    public GameObject playerParticlePrefab;
    GameObject[,] field;
    List<GameObject> spawnedObjects = new List<GameObject>();
    bool next;

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
        // 範囲チェック
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        // 目的地にボックスがある場合、そのボックスを移動
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        // プレイヤーが動いた時のパーティクルを生成
        if (field[moveFrom.y, moveFrom.x] != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Instantiate(playerParticlePrefab, field[moveFrom.y, moveFrom.x].transform.position, Quaternion.identity);
            }
        }
        // 箱が3番に到達した時のパーティクルを生成
        if (field[moveFrom.y, moveFrom.x] != null && field[moveFrom.y, moveFrom.x].tag == "Box" && map[moveTo.y, moveTo.x] == 3)
        {
            for (int i = 0; i < 20; i++)
            {
                Instantiate(ParticlePrefab, IndexToPosition(moveTo), Quaternion.identity);
            }
        }
        // 移動処理
        if (field[moveFrom.y, moveFrom.x] != null)
        {
            Vector3 moveToPosition = IndexToPosition(moveTo);
            Move moveComponent = field[moveFrom.y, moveFrom.x].GetComponent<Move>();
            if (moveComponent != null)
            {
                moveComponent.MoveTo(moveToPosition);
                field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
                field[moveFrom.y, moveFrom.x] = null;
            }
            else
            {
                Debug.LogError("Move component not found on the object.");
            }
        }
        return true;
    }

    bool IsCleard()
    {
        List<Vector2Int> goals = new List<Vector2Int>();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                return false;
            }
        }
        return true;
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(
                index.x - map.GetLength(1) / 2 + 0.5f,
                0,
                -index.y + map.GetLength(0) / 2);
    }

    void InitializeMap()
    {
        if (!next)
        {
            map = new int[,]
            {
                {2,2,2,2,2,2,2,2,2},
                {2,3,2,0,0,0,2,3,2},
                {2,0,0,0,1,0,0,0,2},
                {2,0,2,0,0,0,2,0,2},
                {2,0,0,0,0,0,0,0,2},
                {2,3,0,0,0,0,0,3,2},
                {2,2,2,2,2,2,2,2,2},
            };
        }
        if (next)
        {
            map = new int[,]
            {
                {2,2,2,2,2,2,2,2,2},
                {2,0,2,0,0,0,0,3,2},
                {2,3,0,0,0,0,0,2,2},
                {2,0,0,0,1,0,0,0,2},
                {2,2,2,0,0,0,0,0,2},
                {2,0,3,0,0,0,0,3,2},
                {2,2,2,2,2,2,2,2,2},
            };
        }

        field = new GameObject[map.GetLength(0), map.GetLength(1)];
    }

    void SetupField()
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                       playerPrefab,
                       IndexToPosition(new Vector2Int(x, y)),
                       Quaternion.identity
                    );
                    spawnedObjects.Add(field[y, x]);
                }
                else if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                       boxPrefab,
                       IndexToPosition(new Vector2Int(x, y)),
                       Quaternion.identity
                    );
                    spawnedObjects.Add(field[y, x]);
                }
                else if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                       goalPrefab,
                       IndexToPosition(new Vector2Int(x, y)),
                       Quaternion.identity
                    );
                    spawnedObjects.Add(field[y, x]);
                }
            }
        }
    }

    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        InitializeMap();
        SetupField();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 既存のオブジェクトを削除
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            spawnedObjects.Clear();
            InitializeMap();
            SetupField();
            clearText.SetActive(false);
        }

        Vector2Int playerIndex = GetPlayerIndex();
        if (playerIndex != new Vector2Int(-1, -1))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveNumber(playerIndex, playerIndex + new Vector2Int(1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveNumber(playerIndex, playerIndex + new Vector2Int(-1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveNumber(playerIndex, playerIndex + new Vector2Int(0, -1));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveNumber(playerIndex, playerIndex + new Vector2Int(0, 1));
            }
        }

        if (IsCleard())
        {
            Debug.Log("Clear");
            // パーティクルを生成（クリアテキストの位置）
            Instantiate(ParticlePrefab, new Vector3(0.5f, 0, -1), Quaternion.identity);
            clearText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space) && !next)
            {
                foreach (GameObject obj in spawnedObjects)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
                spawnedObjects.Clear();
                clearText.SetActive(false);
                next = true;
                InitializeMap();
                SetupField();
            }
        }
    }
}