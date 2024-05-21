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
        // �͈̓`�F�b�N
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        // �ړI�n�Ƀ{�b�N�X������ꍇ�A���̃{�b�N�X���ړ�
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        // �v���C���[�����������̃p�[�e�B�N���𐶐�
        if (field[moveFrom.y, moveFrom.x] != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Instantiate(playerParticlePrefab, field[moveFrom.y, moveFrom.x].transform.position, Quaternion.identity);
            }
        }
        // ����3�Ԃɓ��B�������̃p�[�e�B�N���𐶐�
        if (field[moveFrom.y, moveFrom.x] != null && field[moveFrom.y, moveFrom.x].tag == "Box" && map[moveTo.y, moveTo.x] == 3)
        {
            for (int i = 0; i < 20; i++)
            {
                Instantiate(ParticlePrefab, IndexToPosition(moveTo), Quaternion.identity);
            }
        }
        // �ړ�����
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
        List<Vector2Int> goals=new List<Vector2Int>();
        for(int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y,x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }
        for(int i=0;i<goals.Count;i++)
        {
            GameObject f = field[goals[i].y,goals[i].x];
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

    //Start is called before the first frame update
    void Start()
    {
        //�z��̎��Ԃ̐����Ə�����

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
                       IndexToPosition(new Vector2Int(x,y)),
                       Quaternion.identity
                    );
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                       boxPrefab,
                       IndexToPosition(new Vector2Int(x, y)),
                       Quaternion.identity
                    );
                }
                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                       goalPrefab,
                       new Vector3(
                       x - map.GetLength(1) / 2 + 0.5f,
                       0,
                       -y + map.GetLength(0) / 2),
                    Quaternion.identity
                    );
                }
            }
        }
    }
    //update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // �����̃I�u�W�F�N�g���폜
            foreach (GameObject obj in field)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            Start();
            clearText.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //�ړ������̃��\�b�h��
            if (playerindex != new Vector2Int(-1, -1))
            {
                MoveNumber(playerindex, playerindex + new Vector2Int(1, 0));
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //�ړ������̃��\�b�h��
            if (playerindex != new Vector2Int(-1, -1))
            {
                MoveNumber(playerindex, playerindex + new Vector2Int(-1, 0));
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //�ړ������̃��\�b�h��
            if (playerindex != new Vector2Int(-1, -1))
            {
                MoveNumber(playerindex, playerindex + new Vector2Int(0, -1));
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2Int playerindex = GetPlayerIndex();
            //�ړ������̃��\�b�h��
            if (playerindex != new Vector2Int(-1, -1))
            {
                MoveNumber(playerindex, playerindex + new Vector2Int(0, 1));
            }
        }
        if (IsCleard())
        {
            Debug.Log("Clear");
            // �p�[�e�B�N���𐶐��i�N���A�e�L�X�g�̈ʒu�j
            Instantiate(ParticlePrefab, new Vector3(0.5f,0,-1), Quaternion.identity);
            clearText.SetActive(true);
        }
    }
}
