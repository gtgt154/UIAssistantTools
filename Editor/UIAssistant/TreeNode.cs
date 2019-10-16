using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

public class TreeNode
{
    public int batchCount = 0;
    private static float nodeDefaultMarginX = 20f;
    private static float nodeDefaultWidth = 100f;
    private static float nodeDefaultWidthObject = 180f;
    private static float nodeDefaultHeight = 20f;

    private bool isShow = true;
    private bool isDestroy = false;
    private bool isRoot = false;
    public bool IsRoot 
    {
        set 
        {
            isRoot = value;
        }
        get 
        {
            return isRoot;
        }
    }

    private int depth = 0;
    public int Depth 
    {
        set 
        {
            depth = value;
        }
        get 
        {
            return depth;
        }
    }

    private string title = null;
    private string subTitle = null;
    private RectTransform rectTransform = null;
    private GUIStyle subTitleGuiStyle = null;
    private TreeNode parent = null;
    private GameObject assetObject = null;
    public GameObject AssetObject 
    {
        get 
        {
            return assetObject;        
        }
    }
    private List<TreeNode> children = new List<TreeNode>();
    private Rect position = new Rect(0, 0, nodeDefaultWidth, nodeDefaultHeight);
    private float marginX = nodeDefaultMarginX;
    private Vector2 recursiveSize = new Vector2(nodeDefaultWidth, nodeDefaultHeight);
    public Vector2 RecursiveSize
    {
        set
        {
            recursiveSize = value;
        }
        get
        {
            return recursiveSize;
        }
    }

    private UINodeInfo nodeInfo;
    public UINodeInfo NodeInfo 
    {
        get 
        {
            return nodeInfo;        
        }
    }

    public TreeNode(string title, GameObject assetObject = null, int depth = 0)
    {
        this.title = title;
        this.assetObject = assetObject;
        this.depth = depth;

        GenInfo();

        if (title == null)
        {
            position.width = nodeDefaultWidthObject;
        }
    }

    public void OnGUI()
    {
        if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition) && Event.current.button == 0)
        {
            Selection.activeGameObject = assetObject;
        }

        isShow = EditorGUI.Foldout(position, isShow, title, UIAssistantTools.GetStyles(nodeInfo.BatchID % UIAssistantTools.DefaultDepth));
        if (subTitle != null)
        {
            EditorGUI.LabelField(new Rect(position.x + position.width + 5, position.y, position.width, position.height), subTitle, subTitleGuiStyle);
        }

        if (isShow)
        {
            float childY = position.y + position.height;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null)
                {
                    children[i].position.y = childY;
                    children[i].OnGUI();
                    childY = childY + children[i].recursiveSize.y;
                }
            }
        }

        CalcRecursiveSize();
    }

    public TreeNode Clone()
    {
        TreeNode cloneNode = new TreeNode(title, assetObject, depth);
        cloneNode.SetSubTitle(subTitle, subTitleGuiStyle != null ? subTitleGuiStyle.normal.textColor : Color.green);
        cloneNode.SetParent(parent);
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                cloneNode.AddChild(children[i].Clone());
            }
        }

        return cloneNode;
    }

    public void Destroy()
    {
        SetParent(null);
        assetObject = null;
        isDestroy = true;

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                children[i].Destroy();
            }
        }
        children.Clear();
    }

    public void PrintNodeInfo() 
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                children[i].PrintNodeInfo();
            }
        }
    }

    public void UpdataInfo() 
    {
        nodeInfo.RefreshInfo();
        if (nodeInfo.Use) 
        {
            if (nodeInfo.check > -1) 
            {
                title = string.Format("{0} {1}   info : ( {2} / {3} / {4} )       check : {5}      BatchID : {6} )", nodeInfo.HierarychyOrder, nodeInfo.Name, nodeInfo.Depth, nodeInfo.MaterialInstanceID, nodeInfo.TextureID, nodeInfo.check, nodeInfo.BatchID);
            }
            else 
            {
                title = string.Format("{0} {1}   info : ( {2} / {3} / {4} )       BatchID : {5}", nodeInfo.HierarychyOrder, nodeInfo.Name, nodeInfo.Depth, nodeInfo.MaterialInstanceID, nodeInfo.TextureID, nodeInfo.BatchID);
            }

        }
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                children[i].UpdataInfo();
            }
        }
    }

    public List<KeyValuePair<TreeNode, UINodeInfo>> GetNodesInfo() 
    {
        List<KeyValuePair<TreeNode, UINodeInfo>> list = new List<KeyValuePair<TreeNode, UINodeInfo>>();
        list.Add(new KeyValuePair<TreeNode, UINodeInfo>(this, nodeInfo));

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                List<KeyValuePair<TreeNode, UINodeInfo>> childList = children[i].GetNodesInfo();
                if(childList.Count > 0) 
                {
                    list.AddRange(childList);
                }
            }
        }
        return list;
    }

    public void AddChild(TreeNode node)
    {
        if (node == null)
        {
            return;
        }

        if (node == this)
        {
            return;
        }

        if (node.isDestroy)
        {
            return;
        }

        if (node.parent == this)
        {
            return;
        }

        node.SetParent(null);
        children.Add(node);
        node.parent = this;
        node.SetPositionX(position.x + marginX);
        node.Depth = depth + 1;
    }

    private void RemoveChild(TreeNode node)
    {
        if (node == null)
        {
            return;
        }

        int index = -1;
        for (int i = 0; i < children.Count; i++)
        {
            if (node == children[i])
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            children[index].parent = null;
            children.RemoveAt(index);
        }
    }

    private void RemoveAllChild()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                children[i].parent = null;
            }
        }
        children.Clear();
    }

    private void DestroyChild(TreeNode node)
    {
        if (node == null)
        {
            return;
        }

        int index = -1;
        for (int i = 0; i < children.Count; i++)
        {
            if (node == children[i])
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            children[index].Destroy();
        }
    }

    private void DestroyAllChild()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                children[i].Destroy();
            }
        }
        children.Clear();
    }

    private void SetParent(TreeNode node)
    {
        if (node == null)
        {
            if (parent != null)
            {
                parent.RemoveChild(this);
                parent = null;
            }
            return;
        }

        if (node == this)
        {
            return;
        }

        if (node.isDestroy)
        {
            return;
        }

        if (node != parent)
        {
            if (parent != null)
            {
                parent.RemoveChild(this);
                parent = null;
            }
            node.AddChild(this);
        }
    }

    private int GetShowChildNum()
    {
        int num = 0;
        if (isShow)
        {
            num++;
        }

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null && children[i].IsShowRecursively())
            {
                num++;
            }
        }

        return num;
    }

    private void SetSize(float width, float height)
    {
        position.width = width;
        position.height = height;
    }

    private void SetMarginX(float marginX)
    {
        this.marginX = marginX;

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                children[i].SetPositionX(position.x + this.marginX);
            }
        }
    }

    private void SetSubTitle(string subTitle, Color titleColor)
    {
        this.subTitle = subTitle;

        if (subTitle != null)
        {
            if (subTitleGuiStyle == null)
            {
                subTitleGuiStyle = new GUIStyle();
                subTitleGuiStyle.fontSize = 11;
                subTitleGuiStyle.alignment = TextAnchor.MiddleLeft;
            }
            subTitleGuiStyle.normal.textColor = titleColor;
        }
    }

    //calculate node size
    private void CalcRecursiveSize()
    {
        recursiveSize.x = position.width;
        recursiveSize.y = position.height;

        if (isShow)
        {
            float childMaxWidth = 0f;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null)
                {
                    childMaxWidth = Math.Max(childMaxWidth, children[i].recursiveSize.x);
                    recursiveSize.y = recursiveSize.y + children[i].recursiveSize.y;
                }
            }

            //all nodes have same size
            if (childMaxWidth > 0f)
            {
                recursiveSize.x = childMaxWidth + Math.Abs(marginX);
            }
        }
    }

    private void SetPositionX(float x)
    {
        position.x = x;

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                children[i].SetPositionX(position.x + marginX);
            }
        }
    }

    private bool IsShowRecursively()
    {
        if (!isShow)
        {
            return false;
        }

        if (parent == null)
        {
            return isShow;
        }

        return parent.IsShowRecursively();
    }

    private void GenInfo() 
    {
        rectTransform = assetObject.GetComponent<RectTransform>();
        MaskableGraphic ui = assetObject.GetComponent<MaskableGraphic>();
        nodeInfo = new UINodeInfo(ui != null, assetObject.GetComponent<Canvas>() != null, assetObject.name);
        nodeInfo.Depth = depth;
        if(ui != null) 
        {
            nodeInfo.MaterialInstanceID = ui.material.GetInstanceID();
            nodeInfo.TextureID = ui.mainTexture.GetInstanceID();
        }
        rectTransform.GetWorldCorners(nodeInfo.Corners);
    }
}