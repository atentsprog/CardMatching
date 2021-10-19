using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConstructor : MonoBehaviour
{
    [System.Serializable]
    public class CardTexture
    {
        [ShowAssetPreview(128, 128)]
        public Texture2D texture;
        public BlockType blockType;

        [Button("선택된 오브젝트에 텍스쳐 적용", EButtonEnableMode.Always)]
        public void SetSelectedBlock()
        {
            print("SetSelectedBlock");
        }
    }

    [Button("선택된 오브젝트에 0텍스쳐 적용")] public void SetSelectedBlock0() { SetSelectedBlock(0); }
    [Button("선택된 오브젝트에 1텍스쳐 적용")] public void SetSelectedBlock1() { SetSelectedBlock(1); }
    [Button("선택된 오브젝트에 2텍스쳐 적용")] public void SetSelectedBlock2() { SetSelectedBlock(2); }
    [Button("선택된 오브젝트에 3텍스쳐 적용")] public void SetSelectedBlock3() { SetSelectedBlock(3); }
    [Button("선택된 오브젝트에 4텍스쳐 적용")] public void SetSelectedBlock4() { SetSelectedBlock(4); }
    [Button("선택된 오브젝트에 5텍스쳐 적용")] public void SetSelectedBlock5() { SetSelectedBlock(5); }
    [Button("선택된 오브젝트에 6텍스쳐 적용")] public void SetSelectedBlock6() { SetSelectedBlock(6); }
    [Button("선택된 오브젝트에 7텍스쳐 적용")] public void SetSelectedBlock7() { SetSelectedBlock(7); }

    private void SetSelectedBlock(int textureIndex)
    {
        print(textureIndex);
        CardTexture cardTexture = textures[textureIndex];
        foreach (var item in UnityEditor.Selection.objects)
        {
            BlockInfo blockInfo = ((GameObject)item).GetComponent<BlockInfo>();
            blockInfo.blockType = cardTexture.blockType;
            blockInfo.SetTexture(cardTexture.texture);
        }
    }

    public List<CardTexture> textures;

    public GameObject baseItem;
    public int xCount = 8;
    public int yCount = 7;

    void Start()
    {
        Init();
    }

    [ContextMenu("블락 배치 하기")]
    private void Init()
    {
        float width = baseItem.transform.localScale.x;
        float height = baseItem.transform.localScale.y;
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                var newBlock = Instantiate(baseItem, baseItem.transform.parent);
                newBlock.transform.localPosition = new Vector3(width * x, height * y, y * 0.1f - x * 0.001f);
            }
        }
    }

    //[ContextMenu("블럭 타입 설정")]
    //void SetBlockType()
    //{
    //    //텍스쳐를 블럭타입 정보로 바꿔서 설정
    //    var trexture = GetComponentInChildren<Renderer>().material.mainTexture;
    //}
}
