#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DesertDaylight.Editor
{
    // Editor-only: creates a native profile using the project's installed URP version.
    public static class DesertDaylightPreset
    {
        [MenuItem("Tools/Desert Daylight/Create URP Volume Profile")]
        public static void CreateProfile()
        {
            if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset))
            {
                EditorUtility.DisplayDialog("URP required",
                    "Select a Universal Render Pipeline asset in Project Settings before creating this preset.", "OK");
                return;
            }

            const string folder = "Assets/DesertDaylight/Profiles";
            if (!AssetDatabase.IsValidFolder("Assets/DesertDaylight"))
                AssetDatabase.CreateFolder("Assets", "DesertDaylight");
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets/DesertDaylight", "Profiles");

            string path = AssetDatabase.GenerateUniqueAssetPath(folder + "/DesertDaylight_URP.asset");
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            profile.name = "DesertDaylight_URP";
            bool assetCreated = false;
            try
            {
                AssetDatabase.CreateAsset(profile, path);
                assetCreated = true;

                var tone = profile.Add<Tonemapping>(false);
                tone.mode.Override(TonemappingMode.ACES);

                // Restrained warmth and exposure for bright peach sand and sandstone.
                var color = profile.Add<ColorAdjustments>(false);
                color.postExposure.Override(-0.25f);
                color.contrast.Override(10f);
                color.saturation.Override(-6f);
                color.hueShift.Override(0f);
                color.colorFilter.Override(Color.white);

                var balance = profile.Add<WhiteBalance>(false);
                balance.temperature.Override(4f);
                balance.tint.Override(0f);

                var bloom = profile.Add<Bloom>(false);
                bloom.intensity.Override(0.08f);
                bloom.threshold.Override(1.3f);
                bloom.scatter.Override(0.5f);
                bloom.tint.Override(Color.white);
                bloom.highQualityFiltering.Override(false);
                bloom.dirtIntensity.Override(0f);

                var vignette = profile.Add<Vignette>(false);
                vignette.intensity.Override(0.08f);
                vignette.smoothness.Override(0.45f);
                vignette.color.Override(Color.black);
                vignette.center.Override(new Vector2(0.5f, 0.5f));
                vignette.rounded.Override(false);

                // Explicit zero overrides prevent these effects leaking in from
                // lower-priority sample volumes or the project's default profile.
                profile.Add<DepthOfField>(false).mode.Override(DepthOfFieldMode.Off);
                profile.Add<MotionBlur>(false).intensity.Override(0f);
                profile.Add<FilmGrain>(false).intensity.Override(0f);
                profile.Add<ChromaticAberration>(false).intensity.Override(0f);
                profile.Add<LensDistortion>(false).intensity.Override(0f);

                foreach (var component in profile.components)
                {
                    component.name = component.GetType().Name;
                    component.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    AssetDatabase.AddObjectToAsset(component, profile);
                    EditorUtility.SetDirty(component);
                }
                EditorUtility.SetDirty(profile);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = profile;
                EditorGUIUtility.PingObject(profile);
                Debug.Log("Created " + path + ". Drag this asset into your Global Volume's Profile field. " +
                    "Enable Post Processing on the gameplay camera. See README for setup.", profile);
            }
            catch (Exception exception)
            {
                // Only the new, uniquely named asset is removed if creation fails.
                if (assetCreated)
                    AssetDatabase.DeleteAsset(path);
                if (profile != null)
                    UnityEngine.Object.DestroyImmediate(profile);
                Debug.LogException(exception);
            }
        }
    }
}
#endif
