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

        [Button("���õ� ������Ʈ�� �ؽ��� ����", EButtonEnableMode.Always)]
        public void SetSelectedBlock()
        {
            print("SetSelectedBlock");
        }
    }

    [Button("���õ� ������Ʈ�� 0�ؽ��� ����")] public void SetSelectedBlock0() { SetSelectedBlock(0); }
    [Button("���õ� ������Ʈ�� 1�ؽ��� ����")] public void SetSelectedBlock1() { SetSelectedBlock(1); }
    [Button("���õ� ������Ʈ�� 2�ؽ��� ����")] public void SetSelectedBlock2() { SetSelectedBlock(2); }
    [Button("���õ� ������Ʈ�� 3�ؽ��� ����")] public void SetSelectedBlock3() { SetSelectedBlock(3); }
    [Button("���õ� ������Ʈ�� 4�ؽ��� ����")] public void SetSelectedBlock4() { SetSelectedBlock(4); }
    [Button("���õ� ������Ʈ�� 5�ؽ��� ����")] public void SetSelectedBlock5() { SetSelectedBlock(5); }
    [Button("���õ� ������Ʈ�� 6�ؽ��� ����")] public void SetSelectedBlock6() { SetSelectedBlock(6); }
    [Button("���õ� ������Ʈ�� 7�ؽ��� ����")] public void SetSelectedBlock7() { SetSelectedBlock(7); }

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

    [ContextMenu("��� ��ġ �ϱ�")]
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

    //[ContextMenu("�� Ÿ�� ����")]
    //void SetBlockType()
    //{
    //    //�ؽ��ĸ� ��Ÿ�� ������ �ٲ㼭 ����
    //    var trexture = GetComponentInChildren<Renderer>().material.mainTexture;
    //}
}
