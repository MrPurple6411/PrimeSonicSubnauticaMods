namespace CyclopsEnhancedSonar;

using System;
using System.Collections;
using Common;
using UnityEngine;

// Code adapted from the original CyclopsNearFieldSonar mod by frigidpenguin
internal class CySonarComponent : MonoBehaviour
{
    public void SetMapState(bool state)
    {
        if (state)
            script?.EnableMap();
        else
            script?.DisableMap();
    }

    private const float scale = 6.699605f;
    private const float fadeRadius = 1.503953f;
    private const float shipScale = 0.2f;
    private readonly Vector3 position = new(-0.9762846f, 2, -10.6917f);
    private readonly Vector3 shipPosition = new(0, 0, 0);

    private Vector3 originalLocation;
    private Vector3 compassLocation = new(0, 0, -1);
    private Transform compass;
    private Transform sonarMap;
    private GameObject projector;
    private bool frontMapActive;

    private MiniWorld script;
    private MiniWorld scriptFront;
    private GameObject ship;
    private SubRoot subRoot;

    public void Awake()
    {
        subRoot = GetComponentInParent<SubRoot>();
    }

    public IEnumerator Start()
    {
        compass = this.gameObject.transform.Find("Compass");
        sonarMap = this.gameObject.transform.Find("SonarMap_Small");
        originalLocation = sonarMap.localPosition;

        projector = sonarMap.Find("submarine_hologram_projector").gameObject;

        var holder = new GameObject("NearFieldSonar");
        holder.transform.SetParent(sonarMap, false);
        holder.transform.localScale = Vector3.one * 0.1f;

        var task = CraftData.GetPrefabForTechTypeAsync(TechType.Seaglide);
        yield return task;

        var template = task.GetResult();
        GameObject prefab = template.GetComponent<VehicleInterface_MapController>().interfacePrefab;
        var hologram = GameObject.Instantiate(prefab);
        hologram.transform.SetParent(holder.transform, false);

        script = hologram.GetComponentInChildren<MiniWorld>();
        script.fadeRadius = fadeRadius;
        script.fadeSharpness /= 16f;
        script.hologramHolder.transform.localScale = Vector3.one * scale;
        script.hologramHolder.transform.localPosition = position * (1 / scale);
        script.active = true;
        script.EnableMap();

        ship = GameObject.Instantiate(this.gameObject.transform.Find("HolographicDisplay/HolographicDisplayVisuals/CyclopsMini_Mid").gameObject);
        ship.gameObject.name = "CyclopsMini_Mid";
        ship.transform.SetParent(sonarMap, false);
        ship.transform.localPosition = shipPosition;
        ship.transform.localScale = Vector3.one * shipScale;

        MeshRenderer[] cyclopsMeshRenderers = ship.transform.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in cyclopsMeshRenderers)
        {
            if (meshRenderer.gameObject.name.StartsWith("cyclops_room_"))
            {
                meshRenderer.enabled = false;
            }
        }

        MeshRenderer oldShipRenderer = sonarMap.Find("CyclopsMini").GetComponent<MeshRenderer>();
        Material shipMaterial = oldShipRenderer.material;

        foreach (MeshRenderer meshRenderer in cyclopsMeshRenderers)
            meshRenderer.sharedMaterial = shipMaterial;

        oldShipRenderer.enabled = false;
        sonarMap.Find("Base").GetComponent<MeshRenderer>().enabled = false;
    }

    public void Update()
    {
        if (sonarMap == null || !Player.main.IsPiloting() || Player.main.currentSub != subRoot)
            return;

        if (GameInput.GetButtonDown(GameInput.Button.AltTool))
        {
            frontMapActive = !frontMapActive;
            if(frontMapActive)
            {
                sonarMap.SetParent(compass);
                sonarMap.localPosition = compassLocation;
                projector.SetActive(false);
            }
            else
            {
                sonarMap.SetParent(this.gameObject.transform);
                sonarMap.localPosition = originalLocation;
                projector.SetActive(true);
            }
            
            return;
        }

        if (!frontMapActive)
            return;
    }
}
