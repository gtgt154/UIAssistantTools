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
}