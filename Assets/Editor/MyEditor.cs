using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MyEditor : EditorWindow
{
    private ObjectField TileMapField;
    private ObjectField SavePlaceableSOField;
    private TextField FilePath;
    private Button SaveLevel;
    private Button LoadLevel;

    [MenuItem("Tools/MyEditor")]
    public static void OpenEditorWindow()
    {
        MyEditor window = GetWindow<MyEditor>();
        window.titleContent = new GUIContent("MyEditor");
        window.maxSize = new Vector2(315, 205);
        window.minSize = window.maxSize;
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkitDemo/MyDoc.uxml");
        VisualElement tree = visualTree.Instantiate();
        root.Add(tree);

        //Assign Properties
        TileMapField = root.Q<ObjectField>("TilemapField");
        SavePlaceableSOField = root.Q<ObjectField>("SaveSOField");

        SaveLevel = root.Q<Button>("SaveButton");
        LoadLevel = root.Q<Button>("LoadButton");

        //Assign Callbacks

        SaveLevel.clicked += StartSave;
        LoadLevel.clicked += StartLoad;
    }


    private void StartSave()
    {
        CheckIfNull();
        PlaceableSO placeableSO = SavePlaceableSOField.value as PlaceableSO;
        placeableSO.SetData(TileMapField.value as Tilemap);
        Debug.Log("Saved Successfully");
    }
    private void StartLoad()
    {
        CheckIfNull();
        PlaceableSO placeableSO = SavePlaceableSOField.value as PlaceableSO;
        placeableSO.ViewData(TileMapField.value as Tilemap, AssetDatabase.LoadAssetAtPath<GroundRuleTile>("Assets/Palletes/Green.asset"), AssetDatabase.LoadAssetAtPath<GroundRuleTile>("Assets/Palletes/Orange.asset"));
        Debug.Log("Loaded Successfully");
    }

    private bool CheckIfNull()
    {
        bool result = true;
        if (TileMapField.value == null)
        {
            Debug.Log("Tilemap Field is Empty");
            result = false;
        }

        PlaceableSO placeableSO = SavePlaceableSOField.value as PlaceableSO;
        if (placeableSO == null)
        {
            Debug.Log("Attach a PlaceableSO");
            result = false;
        }
        return result;
    }
}
