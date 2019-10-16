#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections.Generic;

public class DrawRectInfo : IComparable<DrawRectInfo>
{
    public int check = -1;
    private int m_BatchID = 0;
    public int BatchID
    {
        set
        {
            m_BatchID = value;
        }
        get
        {
            return m_BatchID;
        }
    }

    private string m_name;
    public string Name
    {
        set
        {
            m_name = value;
        }
        get
        {
            return m_name;
        }
    }

    private int m_hierarychyOrder = 0;
    public int HierarychyOrder
    {
        set
        {
            m_hierarychyOrder = value;
        }
        get
        {
            return m_hierarychyOrder;
        }
    }

    private int m_depth = 0;
    public int Depth
    {
        set
        {
            m_depth = value;
        }
        get
        {
            return m_depth;
        }
    }

    private int m_materialInstanceID = 0;
    public int MaterialInstanceID
    {
        set
        {
            m_materialInstanceID = value;
        }
        get
        {
            return m_materialInstanceID;
        }
    }

    private int m_textureID = 0;
    public int TextureID
    {
        set
        {
            m_textureID = value;
        }
        get
        {
            return m_textureID;
        }
    }

    private Vector3[] m_corners = new Vector3[4];
    public Vector3[] Corners
    {
        set
        {
            m_corners = value;
        }
        get
        {
            return m_corners;
        }
    }

    private bool m_isOnlyCanvas;
    public bool IsOnlyCanvas 
    {
        get 
        {
            return m_isOnlyCanvas;
        }
    }

    public DrawRectInfo(string name, bool onlyCanvas) 
    {
        this.m_name = name;
        this.m_isOnlyCanvas = onlyCanvas;
    }

    public Rect GetRect()
    {
        float minX, minY, maxX, maxY;
        minX = maxX = m_corners[0].x;
        minY = maxY = m_corners[0].y;
        for (int i = 1; i < 4; ++i)
        {
            if (m_corners[i].x < minX)
            {
                minX = m_corners[i].x;
            }
            if (m_corners[i].x > maxX)
            {
                maxX = m_corners[i].x;
            }
            if (m_corners[i].y < minY)
            {
                minY = m_corners[i].y;
            }
            if (m_corners[i].y > maxY)
            {
                maxY = m_corners[i].y;
            }
        }
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    public bool CanBatch(DrawRectInfo b) 
    {
        return MaterialInstanceID == b.MaterialInstanceID && TextureID == b.TextureID;
    }

    public int CompareTo(DrawRectInfo b)
    {
        if (m_depth > b.Depth)
        {
            return 1;
        }
        else if (m_depth == b.Depth)
        {
            if(m_materialInstanceID > b.MaterialInstanceID) 
            {
                return 1;
            }
            else if(m_materialInstanceID == b.MaterialInstanceID) 
            {
                if(m_textureID > b.TextureID) 
                {
                    return 1;
                }
                else if(m_textureID == b.TextureID) 
                {
                    if (m_hierarychyOrder > b.HierarychyOrder)
                    {
                        return 1;
                    }
                    else if (m_hierarychyOrder == b.HierarychyOrder)
                    {
                        return 0;
                    }
                }
            }
        }
        return -1;
    }

    public void Print() 
    {
        Debug.LogError("------------------- Name: " + Name);
        Debug.LogError("------------------- Depth: " + Depth);
        //Debug.LogError("------------------- MaterialInstanceID: " + MaterialInstanceID);
        //Debug.LogError("------------------- TextureID: " + TextureID);
        Debug.LogError("------------------- HierarychyOrder: " + HierarychyOrder);
    }
}

public class DrawRectLine : MonoBehaviour {
    private Color[] m_colors = new Color[6] { Color.red, Color.yellow, Color.blue, Color.green, Color.black, Color.white};

    private List<DrawRectInfo> m_rectInfoList = new List<DrawRectInfo>();
    public void SetDrawRectInfoList(List<DrawRectInfo> rectInfoList) 
    {
        m_rectInfoList.Clear();
        m_rectInfoList = rectInfoList;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < m_rectInfoList.Count; ++i) 
        {
            Gizmos.color = m_colors[m_rectInfoList[i].BatchID % m_colors.Length];
            for (int j = 0; j < 4; ++j) 
            {
                Gizmos.DrawLine(m_rectInfoList[i].Corners[j], m_rectInfoList[i].Corners[(j + 1) % 4]);
            }
        }
    }
}
#endif