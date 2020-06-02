using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    SerializedProperty scriptableProp;

    private void OnEnable()
    {
        stamp = new GameObject("Stamp");
        stamp.hideFlags = HideFlags.HideAndDontSave;
        bc = target as BlocCreator;
        if (bc.SelectedPrefab != null)
            CreateNewStamp();

        var scriptObject = bc.gameObject;
        editorPS = PhysicsSceneExtensions2D.GetPhysicsScene2D(scriptObject.scene);
        scriptableProp = serializedObject.FindProperty("blocsScriptable");
        if (bc.currentBlocSelection != null)
        {
            blocYRange = bc.currentBlocSelection.blocYRange;
            gOffset = bc.currentBlocSelection.globalOffsetRange;
            rotOff = bc.currentBlocSelection.globalRotationOffsetRange;
        }

        EditorUtility.SetDirty(bc.blocsScriptable);
        GetSavedBlocsNames();
        RefreshBlocName();
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

    //string blocName = "Enter Bloc Name"; // bloc name to store
    int selectedName = -1;
    Bloc.BlocArea blocArea = Bloc.BlocArea.COUNTRY; // bloc area to store

    Vector2 blocYRange;
    Vector4 gOffset;
    Vector2 rotOff;
    int selectedEggshellId = 0;

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

        if (scriptableProp != null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(scriptableProp);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(bc.blocsScriptable);

                GetSavedBlocsNames();
                RefreshBlocName();

                //AssetDatabase.Refresh();
            }
        }

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

        // EGGSHGELL PARAMETER
        var selectedSpawnable = bc.SelectedPrefab.GetComponent<SpawnableObject>();
        if (selectedSpawnable is Collectible)
        {
            if (selectedSpawnable.name.Contains("eggshell"))
            {
                if (selectedEggshellId < 0) selectedEggshellId = 0;
                selectedEggshellId = EditorGUILayout.IntField("Eggshell ID", selectedEggshellId);
                GUILayout.Space(6);
            }
            else
                selectedEggshellId = -1;
        }

        // ADDITIONAL BLOC PARAMETERS
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("MISC PARAMETERS:", GUILayout.MaxWidth(250));
        if (GUILayout.Button("Reset Misc", GUILayout.MaxWidth(100)))
        {
            blocYRange = Vector2Int.zero;
            gOffset = Vector4.zero;
            rotOff = Vector2.zero;
        }
        EditorGUILayout.EndHorizontal();

        // Bloc Y Range parameter
        GUILayout.Space(6);
        MinMaxIntSliderGUI("Bloc Y Range", ref blocYRange.x, ref blocYRange.y, -9, 9);
        GUILayout.Space(3);
        // Global offset parameters
        MinMaxIntSliderGUI("Global Obs X Offset", ref gOffset.x, ref gOffset.y, -6, 6);
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
        bc.blocName = EditorGUILayout.TextField("Bloc Name : ", bc.blocName);
        if (EditorGUI.EndChangeCheck())
        {
            if (bc.blocName != "")
            {
                if (bc.blocNames.Contains(bc.blocName))
                {
                    selectedName = bc.blocNames.IndexOf(bc.blocName);
                }
            }
        }
        EditorGUI.BeginChangeCheck();
        selectedName = EditorGUILayout.Popup("Available Blocs Selection : ", selectedName, bc.blocNames.ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            bc.blocName = bc.blocNames[selectedName];
        }

        blocArea = (Bloc.BlocArea)EditorGUILayout.EnumPopup("Bloc Area : ", blocArea);
        GUILayout.Space(8);
        var scriptableStoredBlocs = bc.blocsScriptable.storedBlocs;
        bc.currentBlocSelection = scriptableStoredBlocs.FirstOrDefault(b => b.blocName == bc.blocName); // is there a block with that name saved already ?

        EditorGUI.BeginDisabledGroup(bc.rootTransform.childCount == 0 || (stamp.transform.parent == bc.rootTransform && bc.rootTransform.childCount == 1));
        if (GUILayout.Button("Wipe all obstacles"))
        {
            DestroyAllRootSpawnables(); // destroy all root objects
        }
        GUILayout.Space(6);
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(bc.currentBlocSelection == null);
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Load existing Bloc"))
        {
            // load bloc parameters
            LoadBlocParameters(bc.currentBlocSelection);
            LoadSpawnablesFromBloc(bc.currentBlocSelection);
        }
        GUILayout.Space(6);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete existing bloc"))
        {
            DestroyAllRootSpawnables(); // destroy all root objects
            scriptableStoredBlocs.Remove(bc.currentBlocSelection);
            RefreshBlocName();
            //AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(8);
        EditorGUI.BeginDisabledGroup(bc.blocName == "Enter Bloc Name" || bc.blocName == "");
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save Bloc to Scriptable"))
        {
            var spawnableObjects = bc.rootTransform.GetComponentsInChildren<SpawnableObject>().ToList();
            spawnableObjects.Remove(spawnableObjects.FirstOrDefault(s => s.transform.parent.name.Contains("Stamp")));
            Spawnable[] spawnables = spawnableObjects.Select(o => o.GetSpawnable()).ToArray();
            int blocLength = 0;
            for(int i = 0; i < spawnableObjects.Count; ++i)
            {
                var spawnableObject = spawnableObjects[i];
                var spawnable = spawnables[i];
                var obsRightBoundX = spawnable.BlocPosition.x;
                // compute bloc length to possibly add
                if (spawnable is ObstacleSpawnable)
                    obsRightBoundX += (spawnable as ObstacleSpawnable).BoundsSize.x;
                else
                    obsRightBoundX += spawnableObject.Size.x;
                if (obsRightBoundX > blocLength) blocLength = Mathf.CeilToInt(obsRightBoundX);
            }
            Bloc newBloc = new Bloc(blocArea, bc.rootTransform.childCount, blocLength, bc.blocName, spawnables);

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

            if (bc.currentBlocSelection != null)
            {
                scriptableStoredBlocs[scriptableStoredBlocs.IndexOf(bc.currentBlocSelection)] = newBloc;
            }
            else
            {
                bc.blocsScriptable.storedBlocs.Add(newBloc);
                bc.blocNames.Add(newBloc.blocName); // add bloc name to list of names
                selectedName = bc.blocNames.IndexOf(bc.blocName);  // set pop field as equal to the new bloc name
            }
            //AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
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
        bc.blocNames = bc.blocsScriptable.storedBlocs.Select(w => w.blocName).ToList();
    }
    void RefreshBlocName()
    {
        if (bc.blocNames.Count == 0)
        {
            bc.blocName = "Enter Bloc Name";
        }
        else
        {
            bc.blocName = bc.blocNames[0];
            selectedName = 0;
        }
    }

    GameObject dummy = null;
    SpawnableObject dummySpawnable = null;
    Transform spnBodyT;
    SpriteRenderer spnBodyRenderer;
    bool canPlaceStamp = false;
    void CreateNewStamp()
    {
        while (stamp.transform.childCount > 0)
            DestroyImmediate(stamp.transform.GetChild(0).gameObject);

        dummy = PrefabUtility.InstantiatePrefab(bc.SelectedPrefab as GameObject) as GameObject;
        dummySpawnable = dummy.GetComponent<SpawnableObject>();
        spnBodyT = dummy.transform.GetChild(0);
        spnBodyRenderer = spnBodyT.GetComponent<SpriteRenderer>();

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
        //var dummy = stamp.transform.GetChild(0);
        if (dummy.activeSelf && canPlaceStamp)
        {
            var stampObject = dummy;
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(stampObject.gameObject);
            var g = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(g, "Stamp");

            g.transform.position = stampObject.transform.position;
            if (bc.rootTransform != null)
            {
                g.transform.parent = bc.rootTransform;
                g.isStatic = bc.rootTransform.gameObject.isStatic;
            }

            var rotation = dummy.transform.GetChild(0).rotation.eulerAngles.z;
            var offset = dummy.transform.GetChild(0).localPosition;

            g.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, rotation);
            g.transform.GetChild(0).localPosition = offset;

            var spawnable = g.GetComponent<SpawnableObject>();
            var spawnableParameters = spawnable.GetSpawnable();
            var obstacleIndex = GetIndexFromPrefabList(bc.blocsScriptable.obstaclesPrefabs, g);

            var obsRectBounds = dummyBounds.size;
            if (obsRectBounds == Vector3.zero)
                obsRectBounds = spawnable.Size;
            var spawnableType = spawnableParameters.GetType();
            if (typeof(ObstacleSpawnable) == spawnableType)
            {
                (spawnable as Obstacle).SetObstacle(g.transform.localPosition, obstacleIndex, offset, obsRectBounds, rotation);
            }
            else if (typeof(CollectibleSpawnable) == spawnableType)
            {
                (spawnable as Collectible).SetCollectible(g.transform.localPosition, obstacleIndex, selectedEggshellId);
            }
        }
    }

    Bounds dummyBounds;
    void RotateStamp(Vector2 delta)
    {
        if (dummy.GetComponent<SpawnableObject>().GetType() == typeof(Obstacle))
        {
            spnBodyT.Rotate(Vector3.back, delta.y);
            dummyBounds = spnBodyT.gameObject.GetBoxColliderFixedBounds();
            Vector2 dummyOffset = stamp.transform.position - dummyBounds.min;
            spnBodyT.localPosition += (Vector3)dummyOffset;
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
        if (!(bc.rootTransform.childCount == 0 || (stamp.transform.parent == bc.rootTransform && bc.rootTransform.childCount == 1)))
            DestroyAllRootSpawnables(); // destroy all root objects
        foreach (Spawnable spawnable in selectedBloc.spawnlablesParams)
        {
            var prefab = bc.blocsScriptable.obstaclesPrefabs[spawnable.SpawnablePrefabIndex];
            var g = PrefabUtility.InstantiatePrefab(prefab, bc.rootTransform) as GameObject;
            Undo.RegisterCreatedObjectUndo(g, "ReStamp");
            var gT = g.transform;
            gT.localPosition = spawnable.BlocPosition;
            var bodyT = gT.GetChild(0);
            if (spawnable is ObstacleSpawnable)
                bodyT.localPosition = (spawnable as ObstacleSpawnable).BodyOffset;

            var spawnableObj = gT.GetComponentInChildren<SpawnableObject>();
            //Type spawnableType = spawnableObj.GetSpawnable().GetType();
            
            if (spawnableObj is Obstacle)
            {
                bodyT.rotation = Quaternion.Euler(0, 0, (spawnable as ObstacleSpawnable).BodyRotation);
                (spawnableObj as Obstacle).obstacleParameters = (ObstacleSpawnable)spawnable;
            }
            else if (spawnableObj is Collectible)
            {
                (spawnableObj as Collectible).collectibleParameters = (CollectibleSpawnable)spawnable;
            }
        }
    }
    void DestroyAllRootSpawnables()
    {
        GameObject[] toDestroy = new GameObject[bc.rootTransform.childCount];
        int c = 0;
        foreach(Transform childT in bc.rootTransform)
        {
            if (childT.gameObject != stamp)
            {
                toDestroy[c] = childT.gameObject;
                ++c;
            }
        }

        foreach (GameObject spn in toDestroy)
        {
            if (spn != null)
                Undo.DestroyObjectImmediate(spn);
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
        if (Event.current.type == EventType.MouseMove)
        {
            SceneView.RepaintAll();
            Physics2D.autoSyncTransforms = true;
        }
        else
            Physics2D.autoSyncTransforms = false;

        var isErasing = Event.current.control;
        var controlId = GUIUtility.GetControlID(FocusType.Passive);
        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = ray.origin;

        // Handling raycast
        int creatorMask = bc.layerMask;
        RaycastHit2D hit2d = editorPS.Raycast(ray.origin, ray.direction, Mathf.Infinity, creatorMask);

        if (hit2d.collider)
        {
            Vector3 snappedMousePos = Vector3.zero;
            var discS = new Vector3(.5f, .5f); // default disc size 
            if (dummySpawnable.Size.x < 1 && dummySpawnable.GetSpawnable() is CollectibleSpawnable) //specific to small collectibles(less than 1 of size)
            {
                snappedMousePos = new Vector3(Mathf.Floor(mousePosition.x / dummySpawnable.Size.x), Mathf.Floor(mousePosition.y / dummySpawnable.Size.x)) * dummySpawnable.Size.x;
                discS = dummySpawnable.Size / 2;
            }
            else
            {
                snappedMousePos = new Vector3(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y));
            }
            var visibleMousePos = snappedMousePos + discS;

            Handles.DrawWireDisc(visibleMousePos, -ray.direction, discS.x); // white disc is always visible

            OverlapCircle(snappedMousePos, discS.x * 2, LayerMask.GetMask("Obstacle"));
            if (isErasing)
                DrawEraser(snappedMousePos);
            else
                DrawStamp(snappedMousePos);

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
        else
        {
            if (dummy)
                dummy.SetActive(false);
        }
    }

    private void OverlapCircle(Vector2 center, float brushRadius, LayerMask obsLayerMask)
    {
        overlaps.Clear();
        overlappedGameObjects.Clear();

        var capsule = new Bounds(center, new Vector3(brushRadius, brushRadius, brushRadius)); // capsule is the circle
        capsule = spnBodyT.gameObject.GetBoxColliderFixedBounds(); // capsule is the stamp bounds
        for (var i = 0; i < bc.rootTransform.childCount; ++i)
        {
            var child = bc.rootTransform.GetChild(i);
            if (child.gameObject == stamp)
                continue;
            var bounds = child.GetChild(0).gameObject.GetBoxColliderFixedBounds();
            if (capsule.Intersects(bounds))
            {
                overlaps.Add(bounds);
                overlappedGameObjects.Add(child.gameObject);
            }
        }
    }

    void DrawStamp(Vector2 center)
    {
        stamp.transform.parent = bc.rootTransform;
        stamp.transform.position = center;
        dummy.SetActive(true);
        canPlaceStamp = true;
        spnBodyRenderer.color = Color.white;

        if (overlaps.Count > 0)
        {
            Collider2D[] colliderCastC = new Collider2D[10];
            ContactFilter2D contactFilter2D = new ContactFilter2D(); contactFilter2D.SetLayerMask(LayerMask.GetMask("Obstacle")); contactFilter2D.useTriggers = true;
            var coOverlaps = (spnBodyT.GetComponent<Collider2D>()).OverlapCollider(contactFilter2D, colliderCastC);
            if (coOverlaps > 0)
            {
                spnBodyRenderer.color = Color.red;
                canPlaceStamp = false;
                //dummy.SetActive(false);
            }
        }
        /*foreach (var b in overlaps)
        {
            if (b.Intersects(bounds))
            {
                var overlapVolume = b.size.x * b.size.y;
                var intersection = Intersection(b, bounds);
                var intersectionVolume = intersection.size.x * intersection.size.y;
                var maxIntersection = Mathf.Max(intersectionVolume / overlapVolume, intersectionVolume / childVolume);
                //if (maxIntersection > bc.maxIntersectionVolume)
                //{
                    //dummy.SetActive(false);
                //}
            }
        }*/
    }
    /*
    Bounds Intersection(Bounds A, Bounds B)
    {
        var min = new Vector3(Mathf.Max(A.min.x, B.min.x), Mathf.Max(A.min.y, B.min.y), Mathf.Max(A.min.z, B.min.z));
        var max = new Vector3(Mathf.Min(A.max.x, B.max.x), Mathf.Min(A.max.y, B.max.y), Mathf.Min(A.max.z, B.max.z));
        return new Bounds(Vector3.Lerp(min, max, 0.5f), max - min);
    }
    */
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
