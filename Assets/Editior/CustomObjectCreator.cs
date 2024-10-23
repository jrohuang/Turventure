using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LittleTurtle.System_;

public class CustomObjectCreator {
    #if UNITY_EDITOR

    [MenuItem("-----ForTest/Chinese", priority = 0)]
    private static void Lanaguage_Chinese() {
        EditorPrefs.SetString("Language", Language.Chinese.ToString());
    }
    [MenuItem("-----ForTest/English", priority = 0)]
    private static void Lanaguage_English() {
        EditorPrefs.SetString("Language", Language.English.ToString());
    }

    #region -- UI --
    [MenuItem("GameObject/---Text", false, -11)]
    public static void CreateCustomTextPrefab() {
        GameObject customPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/-----Prefabs/UI/InEditiing/Text_.prefab");
        GameObject customObject = PrefabUtility.InstantiatePrefab(customPrefab) as GameObject;

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
            customObject.transform.SetParent(selectedObject.transform);
            customObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        Selection.activeGameObject = customObject;
        Undo.RegisterCreatedObjectUndo(customObject, "Create Custom Object");
    }
    [MenuItem("GameObject/---Button", false, -10)]
    public static void CreateCustomButtonPrefab() {
        GameObject customPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/-----Prefabs/UI/InEditiing/Btn_.prefab");
        GameObject customObject = PrefabUtility.InstantiatePrefab(customPrefab) as GameObject;

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
            customObject.transform.SetParent(selectedObject.transform);
            customObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        Selection.activeGameObject = customObject;
        Undo.RegisterCreatedObjectUndo(customObject, "Create Custom Object");
    }
    #endregion
    #endif
}