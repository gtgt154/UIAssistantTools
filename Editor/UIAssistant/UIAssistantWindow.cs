using UnityEngine;
using UnityEditor;

public class UIAssistantWindow : EditorWindow
{
    private int defaultDepth = UIAssistantTools.DefaultDepth;
    private int depthIdx = 1;
    private GameObject m_selectObj = null;
    private TreeNode m_treeRootNode = null;
    private Vector2 m_ScrollPos = Vector2.zero;
    private Color[] m_colors = new Color[6] { Color.red, Color.yellow, Color.blue, Color.green, Color.black, Color.white };

    void OnGUI()
    {
        GUILabelType();
        GUILayout.Label("UIAssistant");

        GUILabelType(TextAnchor.UpperLeft);
        GUILayout.Space(2);
        CreateSplit();
        
        GUILayout.BeginHorizontal();
        for(int i = 0; i < m_colors.Length; ++i) 
        {
            GUI.color = m_colors[i];
            GUILayout.Label(string.Format("{0} : ", i));
            GUILayout.Box("", GUILayout.Width(15), GUILayout.Height(15));
            GUILayout.Space(140);
        }
        GUILayout.EndHorizontal();

        GUILabelType(TextAnchor.UpperLeft);
        GUILayout.Space(2);
        CreateSplit();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Catch"))
        {
            Catch();
        }
        if (GUILayout.Button("Refresh"))
        {
            Refresh();
        }
        if (GUILayout.Button("Clear"))
        {
            Reset();
        }
        GUILayout.EndHorizontal();

        GUILabelType(TextAnchor.UpperLeft);
        GUILayout.Space(2);
        CreateSplit();

        ShowCatchUI();
    }

    private void ShowCatchUI() 
    {
        if (m_selectObj != null) 
        {
            GUILabelType(TextAnchor.UpperLeft);
            GUILayout.Space(2);
            GUILayout.Label(m_treeRootNode == null ? "Result: " : string.Format("Result: {0}", m_treeRootNode.batchCount));
            GUILayout.Space(2);

            m_ScrollPos = GUI.BeginScrollView(new Rect(10, 140, 800, position.height - 100), m_ScrollPos, new Rect(0, 0, m_treeRootNode.RecursiveSize.x, m_treeRootNode.RecursiveSize.y), true, true);
            m_treeRootNode.OnGUI();
            GUI.EndScrollView();
        }
    }

    private void Catch()
    {
        if(Selection.activeGameObject == null) 
        {
            EditorUtility.DisplayDialog("Tips", "Select Object is null!", "close");
            return;
        }

        if (Selection.activeGameObject.layer == LayerMask.NameToLayer("UI")) 
        {
            m_selectObj = Selection.activeGameObject;
            Refresh();
        }
    }

    private void Refresh()
    {
        if (m_treeRootNode != null)
        {
            m_treeRootNode.Destroy();
        }

        if (m_selectObj != null)
        {
            depthIdx = 1;
            m_treeRootNode = new TreeNode(m_selectObj.name, m_selectObj, depthIdx * defaultDepth);
            m_treeRootNode.IsRoot = true;
            GenChildNodes(m_treeRootNode, m_selectObj.transform);
            UIAssistantTools.GenTreeInfo(m_treeRootNode);
        }
    }

    private void GenChildNodes(TreeNode node, Transform transform)
    {
        if(transform.childCount > 0) 
        {
            int depth = 0;
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                if(child.gameObject.activeSelf) 
                {
                    depth = node.Depth + 1;
                    if (child.GetComponent<Canvas>() != null) 
                    {
                        ++depthIdx;
                        depth = depthIdx * defaultDepth;
                    }
                    TreeNode childNode = new TreeNode(child.name, child.gameObject, depth);
                    GenChildNodes(childNode, child);
                    node.AddChild(childNode);
                }
            }
        }
    }

    private void Reset()
    {
        m_selectObj = null;
        m_treeRootNode = null;
        m_ScrollPos = Vector2.zero;
    }

    public GUIStyle GUILabelType(TextAnchor anchor = TextAnchor.UpperCenter)
    {
        GUIStyle labelstyle = GUI.skin.GetStyle("Label");
        labelstyle.alignment = anchor;
        return labelstyle;
    }

    public void CreateSplit()
    {
        GUILayout.Label("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
    }
}