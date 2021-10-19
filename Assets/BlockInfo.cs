using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfo : MonoBehaviour
{
    public BlockType blockType;
    void Start()
    {
        GetComponent<Renderer>().material.SetColor("_Color",
            ConvertColor(blockType)); 
    }
    Color ConvertColor(BlockType type)
    {
        switch (type)
        {
            case BlockType.Walkable:    return Color.black;
            case BlockType.Card1: return Color.red;
            case BlockType.Card2: return Color.blue;
            default:
                Debug.LogError($"���� ���� ���� Ÿ�� : {type}");
                return Color.white;
        }
    }

    private void OnMouseDown()
    {
        print($"blockType:{blockType}, {ConvertColor(blockType)}");
        BlockManager.Instance.FindPath(this);
    }
}