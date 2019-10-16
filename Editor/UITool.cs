using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UITool {

    [MenuItem("Tools/UI/UIAssistant")]
    public static void OpenUIAssistant()
    {
        UIAssistantWindow windows = EditorWindow.GetWindow<UIAssistantWindow>();
        windows.autoRepaintOnSceneChange = true;
        windows.titleContent = new GUIContent("UIAssistant");
        windows.maxSize = new Vector2(1600, 1000);
        windows.minSize = new Vector2(200, 500);

        windows.Show();
    }
}
