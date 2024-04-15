using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManageScript : MonoBehaviour
{
    int[] map;

    void PrintArray()
    {
        string debugText = "";
        for (int i = 0; i < map.Length; i++)
        {
            //要素数を1つずつ出力
            debugText += map[i].ToString() + ", ";
        }
        Debug.Log(debugText);
    }
    int GetPlayerIndex()
    {
        for(int i = 0;i < map.Length;i++)
        {
            if (map[i] == 1)
            {
                return i;
            }
        }
        return -1;
    }
    bool MoveNumber(int number,int moveFrom,int moveTo)
    {
        if (moveTo < 0 || moveTo >= map.Length)
        {
            //動けない条件を先に書き、リターンする。早期リターン
            return false;
        }
        if (map[moveTo]==2)
        {
            int velocity = moveTo - moveFrom;
            bool success = MoveNumber(2, moveTo, moveTo + velocity);
            if (!success)
            {
                return false;
            }
        }
        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //配列の実態の生成と初期化
        map = new int[] { 0, 0, 2, 1, 0, 2, 2, 0, 0 };
        PrintArray();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            int playerIndex = GetPlayerIndex();
            //移動処理のメソッド化
            MoveNumber(1, playerIndex, playerIndex + 1);
            PrintArray();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int playerIndex = GetPlayerIndex();
            //移動処理のメソッド化
            MoveNumber(1, playerIndex, playerIndex - 1);
            PrintArray();
        }
    }
}
