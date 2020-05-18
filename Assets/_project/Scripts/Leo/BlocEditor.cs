using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CustomEditor(typeof(BlocCreator))]
//[CanEditMultipleObjects]
public class BlocEditor : Editor
{
    PhysicsScene2D editorPS;
    // painter
    Vector3 worldCursor;
    Texture2D[] palleteImages;
    GameObject stamp;
    BlocCreator bc;
    List<GameObject> erase = new List<GameObject>();
    List<Bounds> overlaps = new List<Bounds>();
    List<GameObject> overlappedGameObjects = new List<GameObject>();

    private void OnEnable()
    {
        stamp = new GameObject("Stamp");
        stamp.hideFlags = HideFlags.HideAndDontSave;
        bc = target as BlocCreator;
        if (bc.SelectedPrefab != null)
            CreateNewStamp();

        var scriptObject = bc.gameObject;
        editorPS = PhysicsSceneExtensions2D.GetPhysicsScene2D(scriptObject.scene);
    }

    private void OnDisable()
    {
        if (stamp != null)
            DestroyImmediate(stamp);
    }

    void RefreshPaletteImages(BlocCreator bc)
    {
        if (palleteImages == null || palleteImages.Length != bc.prefabPallete.Length)
        {
            palleteImages = new Texture2D[bc.prefabPallete.Length];
            for (var i = 0; i < bc.prefabPallete.Length; i++)
            {
                palleteImages[i] = AssetPreview.GetAssetPreview(bc.prefabPallete[i]);
            }
        }
    }

    string blocName = "Enter Bloc Name"; // bloc name to store
    int selectedName = 0;
    Bloc.BlocArea blocArea = Bloc.BlocArea.COUNTRY; // bloc area to store

    Vector2 blocYRange;
    Vector4 gOffset;
    Vector2 rotOff;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (bc.rootTransform == null)
        {
            EditorGUILayout.HelpBox("You must assign the root transform for new painted instances.", MessageType.Error);
            bc.rootTransform = (Transform)EditorGUILayout.ObjectField("Root Transform", bc.rootTransform, typeof(Transform), true);
            return;
        }
        EditorGUILayout.HelpBox("Stamp: Left Click\nErase: Ctrl + Left Click\nRotate: Shift + Scroll\nRevert/Redo: Ctrl+Z/Ctrl+Y", MessageType.Info);
        base.OnInspectorGUI();

        GUILayout.Space(16);
        if (bc.prefabPallete != null && bc.prefabPallete.Length > 0)
        {
            RefreshPaletteImages(bc);
            var tileSize = 96;
            var xCount = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth / tileSize + 1) / 2);
            var gridHeight = GUILayout.Height(palleteImages.Length / xCount * tileSize);
            var buttonStyle = EditorStyles.miniButton;
            buttonStyle.fixedHeight = palleteImages.Length / xCount * tileSize;
            var newIndex = GUILayout.SelectionGrid(bc.selectedPrefabIndex, palleteImages, xCount, buttonStyle, gridHeight);
            if (newIndex != bc.selectedPrefabIndex)
            {
                bc.selectedPrefabIndex = newIndex;
                CreateNewStamp();
            }
            GUILayout.Space(16);
        }

        // ADDITIONAL BLOC PARAMETERS
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("MISC PARAMETERS:", GUILayout.MaxWidth(250));
        if (GUILayout.Button("Reset Misc", GUILayout.MaxWidth(100)))
        {
            blocYRange = Vector2.zero;
            gOffset = Vector4.zero;
            rotOff = Vector2.zero;
        }
        EditorGUILayout.EndHorizontal();
        // Bloc Y Range parameter
        GUILayout.Space(6);
        MinMaxIntSliderGUI("Bloc Y Range", ref blocYRange.x, ref blocYRange.y, -9, 9);
        GUILayout.Space(3);
        // Global offset parameters
        MinMaxIntSliderGUI("Global Obs X Offset" , ref gOffset.x, ref gOffset.y, -6, 6);
        MinMaxIntSliderGUI("Global Obs Y Offset", ref gOffset.w, ref gOffset.z, -9, 9);
        GUILayout.Space(3);
        // Global Rotation parameter
        MinMaxIntSliderGUI("Global Rotation Offset", ref rotOff.x, ref rotOff.y, -16, 16);

        GUILayout.Space(8);

        // BLOC SAVING
        GUILayout.Label("BLOC SAVING:");
        GUILayout.Space(6);
        GetSavedBlocsNames();
        EditorGUI.BeginChangeCheck();
        blocName = EditorGUILayout.TextField("Bloc Name : ", blocName);
        if (EditorGUI.EndChangeCheck())
        {
            if (bc.blocNames.Contains(blocName))
                selectedName = bc.blocNames.IndexOf(blocName);
        }
        EditorGUI.BeginChangeCheck();
        selectedName = EditorGUILayout.Popup("Available Blocs Selection : ", selectedName, bc.blocNames.ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            blocName = bc.blocNames[selectedName];
        }

        blocArea = (Bloc.BlocArea)EditorGUILayout.EnumPopup("Bloc Area : ", blocArea);
        GUILayout.Space(8);
        var scriptableStoredBlocs = bc.blocScriptable.storedBlocs;
        var presavedBloc = scriptableStoredBlocs.FirstOrDefault(b => b.blocName == blocName); // is there a block with that name saved already ?
        EditorGUI.BeginDisabledGroup(presavedBloc == null);
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Load existing Bloc"))
        {
            // load bloc parameters
            LoadBlocParameters(presavedBloc);
            LoadSpawnablesFromBloc(presavedBloc);
        }
        GUILayout.Space(6);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete existing bloc"))
        {
            scriptableStoredBlocs.Remove(presavedBloc);
        }
        GUI.backgroundColor = Color.white;
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(8);
        EditorGUI.BeginDisabledGroup(blocName == "Enter Bloc Name");
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save Bloc to Scriptable"))
        {
            var spawnables = bc.rootTransform.GetComponentsInChildren<SpawnableObject>().Select(o => o.GetSpawnable()).ToArray();
            int blocLength = 0;
            foreach(Spawnable osbP in spawnables)
            {
                var obsRightBoundX = osbP.BoundsSize.x + osbP.BlocPosition.x;
                if (obsRightBoundX > blocLength) blocLength = Mathf.CeilToInt(obsRightBoundX);
            }
            Bloc newBloc = new Bloc(blocArea, bc.rootTransform.childCount, blocLength, blocName, spawnables);

            // set misc parameters
            if (blocYRange != Vector2.zero)
            {
                newBloc.blocYRange = new Vector2Int((int)blocYRange.x, (int)blocYRange.y);
            }
            if (gOffset != Vector4.zero)
            {
                newBloc.globalOffsetRange = gOffset;
            }
            if (rotOff != Vector2.zero)
            {
                newBloc.globalRotationOffsetRange = new Vector2Int((int)rotOff.x, (int)rotOff.y);
            }

            if (presavedBloc != null)
            {
                scriptableStoredBlocs[scriptableStoredBlocs.IndexOf(presavedBloc)] = newBloc;
            }
            else
            {
                bc.blocScriptable.storedBlocs.Add(newBloc);

                bc.blocNames.Add(newBloc.blocName); // add bloc name to list of names
                selectedName = bc.blocNames.IndexOf(blocName);  // set pop field as equal to the new bloc name
            }
        }
        EditorGUI.EndDisabledGroup();
    }

    void MinMaxIntSliderGUI(in string _label, ref float _minVal, ref float _maxVal, in int _minBound, in int _maxBound)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_label, GUILayout.MaxWidth(125));
        _minVal = EditorGUILayout.IntField((int)_minVal, GUILayout.MaxWidth(25));
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.MinMaxSlider(ref _minVal, ref _maxVal, _minBound, _maxBound, GUILayout.MaxWidth(300));
        if (EditorGUI.EndChangeCheck())
        {
            _minVal = Mathf.RoundToInt(_minVal);
            _maxVal = Mathf.RoundToInt(_maxVal);
        }
        _maxVal = EditorGUILayout.IntField((int)_maxVal, GUILayout.MaxWidth(25));
        EditorGUILayout.EndHorizontal();
    }
    void GetSavedBlocsNames()
    {
        bc.blocNames = bc.blocScriptable.storedBlocs.Select(w => w.blocName).ToList();
    }

    void CreateNewStamp()
    {
        while (stamp.transform.childCount > 0)
            DestroyImmediate(stamp.transform.GetChild(0).gameObject);

        GameObject dummy;
        dummy = PrefabUtility.InstantiatePrefab(bc.SelectedPrefab as GameObject) as GameObject;
        foreach (var c in dummy.GetComponentsInChildren<Collider>())
            c.enabled = false;
        dummy.transform.parent = stamp.transform;
        dummy.transform.localPosition = Vector3.zero;
        dummy.transform.localRotation = Quaternion.identity;
    }

    void PerformErase()
    {
        foreach (var g in erase)
            Undo.DestroyObjectImmediate(g);
        erase.Clear();
    }

    void PerformStamp()
    {
        var dummy = stamp.transform.GetChild(0);
        if (dummy.gameObject.activeSelf)
        {
            var stampObject = dummy;
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(stampObject.gameObject);
            var g = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(g, "Stamp");

            g.transform.position = stampObject.position;
            if (bc.rootTransform != null)
            {
                g.transform.parent = bc.rootTransform;
                g.isStatic = bc.rootTransform.gameObject.isStatic;
            }

            var rotation = dummy.transform.GetChild(0).rotation;
            var offset = dummy.transform.GetChild(0).localPosition;

            g.transform.GetChild(0).rotation = rotation;
            g.transform.GetChild(0).localPosition = offset;

            var spawnable = g.GetComponent<SpawnableObject>();
            var spawnableParameters = spawnable.GetSpawnable();
            var obstacleIndex = GetIndexFromPrefabList(bc.blocScriptable.obstaclesPrefabs, g);
            var obsRectBounds = dummyBounds.size;

            if (obsRectBounds == Vector3.zero)
                obsRectBounds = (Vector2)spawnableParameters.Size;
            var spawnableType = spawnableParameters.GetType();
            if (typeof(ObstacleSpawnable) == spawnableType)
            {
                (spawnable as Obstacle).SetObstacle(new Vector2Int((int)g.transform.localPosition.x, (int)g.transform.localPosition.y), obsRectBounds, rotation, offset, obstacleIndex);
            }
            else if (typeof(CollectibleSpawnable) == spawnableType)
            {
                (spawnable as Collectible).SetCollectible(new Vector2Int((int)g.transform.localPosition.x, (int)g.transform.localPosition.y), rotation, obsRectBounds, obstacleIndex);
            }
        }
    }

    Bounds dummyBounds;
    void RotateStamp(Vector2 delta)
    {
        var spn = stamp.transform.GetChild(0);
        var spnBody = spn.GetChild(0);
        spnBody.Rotate(Vector3.back , delta.y);

        if (spn.GetComponent<SpawnableObject>().GetType() == typeof(Obstacle))
        {
            dummyBounds = spnBody.gameObject.GetBoxColliderFixedBounds();
            Vector2 dummyOffset = stamp.transform.position - dummyBounds.min;
            spnBody.localPosition += (Vector3)dummyOffset;
        }
    }

    void LoadBlocParameters(Bloc selectedBloc)
    {
        if (selectedBloc.blocYRange != null)
            blocYRange = (Vector2)selectedBloc.blocYRange;
        if (selectedBloc.globalOffsetRange != null)
            gOffset = (Vector4)selectedBloc.globalOffsetRange;
        if (selectedBloc.globalRotationOffsetRange != null)
            rotOff = (Vector2)selectedBloc.globalRotationOffsetRange;
    }
    void LoadSpawnablesFromBloc(Bloc selectedBloc)
    {
        if (bc.rootTransform == null)
        {
            Debug.LogError("Can't spawn bloc without a root Transform");
            return;
        }
        // destroy all root objects
        while (bc.rootTransform.childCount > 0)
        {
            Undo.DestroyObjectImmediate(bc.rootTransform.GetChild(0).gameObject);
        }
        foreach (Spawnable spawnable in selectedBloc.spawnlablesParams)
        {
            var prefab = bc.blocScriptable.obstaclesPrefabs[spawnable.SpawnablePrefabIndex];
            var g = PrefabUtility.InstantiatePrefab(prefab, bc.rootTransform) as GameObject;
            Undo.RegisterCreatedObjectUndo(g, "ReStamp");
            var gT = g.transform;
            gT.localPosition = (Vector2)spawnable.BlocPosition;
            var bodyT = gT.GetChild(0);
            bodyT.rotation = spawnable.BodyRotation;

            var spawnableObj = gT.GetComponentInChildren<SpawnableObject>();
            Type spawnableType = spawnableObj.GetSpawnable().GetType();
            if (spawnableType == typeof(ObstacleSpawnable))
            {
                bodyT.localPosition = (spawnable as ObstacleSpawnable).BodyOffset;
                (spawnableObj as Obstacle).obstacleParameters = (ObstacleSpawnable)spawnable;
            }
            else if (spawnableType == typeof(ObstacleSpawnable))
            {
                (spawnableObj as Collectible).collectibleParameters = (CollectibleSpawnable)spawnable;
                // implement ?..
            }
        }
    }

    int GetIndexFromPrefabList(in List<GameObject> prefabList, in GameObject chosenObject)
    {
        var chosenPrefabParent = PrefabUtility.GetCorrespondingObjectFromSource(chosenObject);
        GameObject correspondingP = prefabList.FirstOrDefault(w => w == chosenPrefabParent);
        if (!correspondingP) throw new System.Exception("Couldn't find prefab correspondance, this is not a prefab");
        var indexFound = prefabList.IndexOf(correspondingP);
        if (indexFound < 0) throw new System.Exception("Couldn't find prefab in prefab list");
        else return indexFound;
    }

    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseMove) SceneView.RepaintAll();

        var isErasing = Event.current.control;
        var controlId = GUIUtility.GetControlID(FocusType.Passive);
        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = ray.origin;

        // Handling raycast
        int creatorMask = bc.layerMask << 0;
        RaycastHit2D hit2d = editorPS.Raycast(ray.origin, ray.direction, Mathf.Infinity, creatorMask);

        if (hit2d.collider)
        {
            //worldCursor = hit2d.point;
            var snappedMousePos = new Vector3(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y));
            var visibleMousePos = snappedMousePos + new Vector3(.5f, .5f);
            Handles.DrawWireDisc(visibleMousePos, -ray.direction, .5f); // white disc is always visible
            OverlapCapsule(visibleMousePos, 1, bc.layerMask);
            if (isErasing)
                DrawEraser(snappedMousePos);
            else
                DrawStamp(snappedMousePos);
        }

        switch (Event.current.type)
        {
            case EventType.ScrollWheel:
                if (Event.current.shift)
                {
                    RotateStamp(Event.current.delta.normalized * 11.25f);
                    Event.current.Use();
                }
                break;
            case EventType.MouseDown:
                //If not using the default orbit mode...
                if (Event.current.button == 0 && !Event.current.alt)
                {
                    if (isErasing)
                        PerformErase();
                    else
                        PerformStamp();
                    
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
                break;
        }
    }

    private void OverlapCapsule(Vector2 center, float brushRadius, LayerMask layerMask)
    {
        overlaps.Clear();
        overlappedGameObjects.Clear();
        if (bc.collisionTest == BlocCreator.CollisionTest.ColliderBounds)
        {
            foreach (var c in Physics2D.OverlapCapsuleAll(center, Vector2.one, CapsuleDirection2D.Vertical, 0f))
            {
                if (c.transform.parent == bc.rootTransform)
                {
                    overlaps.Add(c.bounds);
                    overlappedGameObjects.Add(c.gameObject);
                }
            }
        }
        if (bc.collisionTest == BlocCreator.CollisionTest.RendererBounds)
        {
            var capsule = new Bounds(center, new Vector3(brushRadius, brushRadius, brushRadius));
            for (var i = 0; i < bc.rootTransform.childCount; i++)
            {
                var child = bc.rootTransform.GetChild(i);
                var bounds = child.gameObject.GetRendererBounds();
                if (capsule.Intersects(bounds))
                {
                    overlaps.Add(bounds);
                    overlappedGameObjects.Add(child.gameObject);
                }
            }
        }
    }

    void DrawStamp(Vector3 center)
    {
        stamp.transform.parent = bc.transform;

        stamp.transform.position = center;

        stamp.transform.rotation = Quaternion.identity;

        var dummy = stamp.transform.GetChild(0).gameObject;
        dummy.SetActive(true);// .gameObject.SetActive(true);

        var bounds = stamp.GetRendererBounds();
        var childVolume = bounds.size.x * bounds.size.y * bounds.size.z;
        foreach (var b in overlaps)
        {
            if (b.Intersects(bounds))
            {
                var overlapVolume = b.size.x * b.size.y * b.size.z;
                var intersection = Intersection(b, bounds);
                var intersectionVolume = intersection.size.x * intersection.size.y * intersection.size.z;
                var maxIntersection = Mathf.Max(intersectionVolume / overlapVolume, intersectionVolume / childVolume);
                if (maxIntersection > bc.maxIntersectionVolume)
                {
                    dummy.SetActive(false);
                }
            }
        }
    }

    Bounds Intersection(Bounds A, Bounds B)
    {
        var min = new Vector3(Mathf.Max(A.min.x, B.min.x), Mathf.Max(A.min.y, B.min.y), Mathf.Max(A.min.z, B.min.z));
        var max = new Vector3(Mathf.Min(A.max.x, B.max.x), Mathf.Min(A.max.y, B.max.y), Mathf.Min(A.max.z, B.max.z));
        return new Bounds(Vector3.Lerp(min, max, 0.5f), max - min);
    }

    void DrawEraser(Vector3 center)
    {
        erase.Clear();
        stamp.transform.GetChild(0).gameObject.SetActive(false);

        stamp.transform.position = center;
        stamp.transform.rotation = Quaternion.identity;

        for (var i = 0; i < overlaps.Count; i++)
        {
            var h = overlaps[i];
            Handles.color = Color.red;
            Handles.DrawWireDisc(h.center, Vector3.back, h.extents.magnitude);
            erase.Add(overlappedGameObjects[i]);
        }
    }

    public override bool RequiresConstantRepaint() { return true; }
}
