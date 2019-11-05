using System;
using UnityEngine;

public class UINodeInfo 
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

    private int m_hierarychyOrder = 0;
    public int HierarychyOrder
    {
        get
        {
            return m_hierarychyOrder;
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

    private bool m_isDrawGraphic = false;
    private bool m_isCanvas = false;
    public bool IsOnlyCanvas 
    {
        get 
        {
            return m_isCanvas && !m_isDrawGraphic;
        }
    }

    public bool Use 
    {
        get 
        {
            return m_isDrawGraphic || m_isCanvas;
        }
    }

    private int m_isInMask = -1;
    public int IsInMask
    {
        set 
        {
            m_isInMask = value;
        }
        get 
        {
            return m_isInMask; 
        }
    }

    private int m_isInMask2D = -1;
    public int IsInMask2D
    {
        set
        {
            m_isInMask2D = value;
        }
        get
        {
            return m_isInMask2D;
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

    private DrawRectInfo m_info;

    public UINodeInfo(bool drawGraphic, bool isCanvas, string name) 
    {
        m_isDrawGraphic = drawGraphic;
        m_isCanvas = isCanvas;
        m_name = name;
    }

    public void ConnectInfo(DrawRectInfo info) 
    {
        m_info = info;
    }

    public void RefreshInfo() 
    {
        if (m_info != null) 
        {
            check = m_info.check;
            m_BatchID = m_info.BatchID;
            m_depth = m_info.Depth;
            m_hierarychyOrder = m_info.HierarychyOrder;
        }
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

    public void Clip(Rect rect) 
    {
        Rect curRect = GetRect();
        if((curRect.xMax > rect.xMin) && (curRect.xMin < rect.xMax) && (curRect.yMax > rect.yMin) && (curRect.yMin < rect.yMax)) 
        {
            float minX = curRect.xMin > rect.xMin ? curRect.xMin : rect.xMin;
            float maxX = curRect.xMax < rect.xMax ? curRect.xMax : rect.xMax;
            float minY = curRect.yMin > rect.yMin ? curRect.yMin : rect.yMin;
            float maxY = curRect.yMax < rect.yMax ? curRect.yMax : rect.yMax;
            m_corners[0] = new Vector2(minX, minY);
            m_corners[1] = new Vector2(minX, maxY);
            m_corners[2] = new Vector2(maxX, maxY);
            m_corners[3] = new Vector2(maxX, minY);
        }
        else 
        {
            for(int i = 0; i < m_corners.Length; ++i) 
            {
                m_corners[i] = Vector2.zero;
            }
            m_isDrawGraphic = false;
        }
    }
}