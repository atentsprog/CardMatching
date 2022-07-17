using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BlockInfo : MonoBehaviour
{
    public BlockType blockType;
    public BlockInfo parent;
    [SerializeField] Color debugColor = Color.red;
    [SerializeField] float debugTweenTime = 0.5f;

    private void OnMouseDown()
    {
        print($"blockType:{blockType}");
        FindPathParent.Instance.FindPath(this);
    }

    internal void SetActiveState()
    {
        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * 0.5f, 0.3f);

        var mat = GetComponentInChildren<Renderer>().material;
        mat.color = debugColor;
        mat.DOColor(Color.white, debugTweenTime);
    }

    internal void SetTexture(Texture2D texture)
    {
        var r = GetComponentInChildren<Renderer>();
        r.sharedMaterial.mainTexture = texture;
    }
    public override string ToString() => blockType.ToString();
}