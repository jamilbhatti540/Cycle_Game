using UnityEngine;

public class EnvironmentZoneSwitch : MonoBehaviour
{
    public Material skyboxMaterial;

    public bool enableFog = true;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.01f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        RenderSettings.skybox = skyboxMaterial;

        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        DynamicGI.UpdateEnvironment();
    }
}