using System.Collections.Generic;
using UnityEngine;

public class UIAssistantTools
{
    private static Color[] m_colors = new Color[6] { Color.red, Color.yellow, Color.blue, Color.green, Color.black, Color.white };
    private static GUIStyle[] styles = null;
    private static List<DrawRectInfo> list = new List<DrawRectInfo>();

    public static int DefaultDepth = 1000;

    public static void InitStyles() 
    {
        styles = new GUIStyle[6];
        for (int i = 0; i < styles.Length; ++i) 
        {
            styles[i] = new GUIStyle();
            styles[i].normal.textColor = m_colors[i];
        }
    }

    public static GUIStyle GetStyles(int id) 
    {
        if(styles == null) 
        {
            InitStyles();
        }

        return styles[id % 6];
    }

    public static void GenTreeInfo(TreeNode node)
    {
        if (node.IsRoot) 
        {
            Dictionary<int, KeyValuePair<TreeNode, List<DrawRectInfo>>> rectInfos = new Dictionary<int, KeyValuePair<TreeNode, List<DrawRectInfo>>>();
            rectInfos.Add(1, new KeyValuePair<TreeNode, List<DrawRectInfo>>(node, new List<DrawRectInfo>()));
            List<KeyValuePair<TreeNode, UINodeInfo>> infos = node.GetNodesInfo();
            int idx = 0;
            for(int i = 0; i < infos.Count; ++i) 
            {
                if (infos[i].Value.Use) 
                {
                    DrawRectInfo rectInfo = new DrawRectInfo(infos[i].Value.Name, infos[i].Value.IsOnlyCanvas);
                    rectInfo.Depth = infos[i].Value.Depth;
                    rectInfo.HierarychyOrder = idx++;
                    rectInfo.MaterialInstanceID = infos[i].Value.MaterialInstanceID;
                    rectInfo.TextureID = infos[i].Value.TextureID;
                    rectInfo.Corners = infos[i].Value.Corners;
                    infos[i].Value.ConnectInfo(rectInfo);
                    if (!rectInfos.ContainsKey(rectInfo.Depth / DefaultDepth)) 
                    {
                        rectInfos[rectInfo.Depth / DefaultDepth] = new KeyValuePair<TreeNode, List<DrawRectInfo>>(infos[i].Key, new List<DrawRectInfo>());
                    }
                    rectInfos[rectInfo.Depth / DefaultDepth].Value.Add(rectInfo);
                }
            }

            if(rectInfos.Count > 0) 
            {
                int allBatchCount = 0;
                foreach(var keyValue in rectInfos) 
                {
                    allBatchCount += CalculateDepthInCanvas(keyValue.Value.Key, keyValue.Value.Value);
                }
                node.batchCount = allBatchCount;
            }
        }
    }

    private static int CalculateDepthInCanvas(TreeNode node, List<DrawRectInfo> rectInfos) 
    {
        if(rectInfos.Count == 0) 
        {
            return 0;
        }
        list.Clear();
        list.Add(rectInfos[0]);

        for (int i = 1; i < rectInfos.Count; ++i)
        {
            CalculateDepth(rectInfos[i]);
            list.Add(rectInfos[i]);
        }

        //Print
        rectInfos.Sort();

        rectInfos[0].BatchID = rectInfos[0].Depth;
        for (int i = 1; i < rectInfos.Count; ++i)
        {
            if ((rectInfos[i].Depth == rectInfos[i - 1].Depth && rectInfos[i].CanBatch(rectInfos[i - 1])) || rectInfos[i].IsOnlyCanvas)
            {
                rectInfos[i].BatchID = rectInfos[i - 1].BatchID;
            }
            else
            {
                rectInfos[i].BatchID = rectInfos[i - 1].BatchID + 1;
            }
            //rectInfos[i].Print();
        }

        for (int i = 0; i < rectInfos.Count - 2; ++i)
        {
            for (int j = i + 1; j < rectInfos.Count; ++j)
            {
                if (rectInfos[i].BatchID != rectInfos[j].BatchID && rectInfos[i].CanBatch(rectInfos[j]))
                {
                    rectInfos[i].check = rectInfos[j].HierarychyOrder;
                    rectInfos[j].check = rectInfos[i].HierarychyOrder;
                }
            }
        }

        node.batchCount = rectInfos[rectInfos.Count - 1].BatchID - rectInfos[0].Depth + 1;
        node.UpdataInfo();
        DrawRectLine ui = node.AssetObject.GetComponent<DrawRectLine>();
        if (ui == null)
        {
            ui = node.AssetObject.AddComponent<DrawRectLine>();
        }
        ui.SetDrawRectInfoList(rectInfos);
        return node.batchCount;
    }

    private static void CalculateDepth(DrawRectInfo a) 
    {
        a.Depth = list[0].Depth;
        for(int i = 0; i < list.Count; ++i) 
        {
            if(Intersects(a, list[i]) || Intersects(list[i], a)) 
            {
                int depth = list[i].Depth + 1;
                if (list[i].CanBatch(a)) 
                {
                    depth = list[i].Depth;
                }
                a.Depth = depth > a.Depth ? depth : a.Depth;
            }
        }
    }

    private static bool Intersects(DrawRectInfo a, DrawRectInfo b) 
    {
        Rect rect = a.GetRect();
        for(int i = 0; i < b.Corners.Length; ++i) 
        {
            if (rect.Contains(b.Corners[i])) 
            {
                return true;
            }
        }

        return false;
    }
}