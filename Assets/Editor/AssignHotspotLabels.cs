#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

// Place this file inside an "Editor" folder anywhere under Assets/
// (e.g. Assets/Editor/AssignHotspotLabels.cs) so it only compiles for the
// Unity Editor and never ships in a build.
//
// Run via: Tools > Addressables > Assign Hotspot ID Labels
//
// This replaces whatever labels your hotspot images currently have (e.g.
// the old per-year-folder labels like "2020-A") with a single label per
// image of the form "HS_<hotspotID>", derived the same way HotspotManager
// parses IDs at runtime (leading digits of the filename).
//
// After running this:
//   1. Open Window > Asset Management > Addressables > Groups
//   2. Select the group containing your hotspot images
//   3. In its Bundled Asset Group Schema, set Bundle Mode to
//      "Pack Together By Label" (it should already be set from before -
//      it will now split by hotspot ID instead of by year)
//   4. Build > New Build > Default Build Script
public static class AssignHotspotLabels
{
    [MenuItem("Tools/Addressables/Assign Hotspot ID Labels")]
    public static void AssignLabels()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("[AssignHotspotLabels] Addressable Asset Settings not found. " +
                "Make sure Addressables is initialized (Window > Asset Management > Addressables > Groups).");
            return;
        }

        int labeled = 0;
        int skipped = 0;

        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null)
                continue;

            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();
            group.GatherAllAssets(entries, true, true, true);

            foreach (AddressableAssetEntry entry in entries)
            {
                string assetPath = entry.AssetPath;
                if (string.IsNullOrEmpty(assetPath) || !assetPath.Contains("Hotspot_Images"))
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                int imageId = -1;

                for (int i = 0; i < fileName.Length; i++)
                {
                    if (!char.IsDigit(fileName[i]))
                    {
                        int.TryParse(fileName[..i], out imageId);
                        break;
                    }
                }

                // Handle the case where the whole filename is digits (no suffix).
                if (imageId < 0 && fileName.Length > 0 && fileName.All(char.IsDigit))
                    int.TryParse(fileName, out imageId);

                if (imageId < 0)
                {
                    Debug.LogWarning($"[AssignHotspotLabels] Could not parse hotspot ID from '{fileName}' ({assetPath}), skipping.");
                    skipped++;
                    continue;
                }

                // Remove any existing labels (e.g. old per-year labels) so
                // "Pack Together By Label" only splits by hotspot ID.
                foreach (string existingLabel in entry.labels.ToArray())
                    entry.SetLabel(existingLabel, false, true);

                string newLabel = $"HS_{imageId}";
                if (!settings.GetLabels().Contains(newLabel))
                    settings.AddLabel(newLabel);

                entry.SetLabel(newLabel, true, true);
                labeled++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[AssignHotspotLabels] Done. Labeled {labeled} entries, skipped {skipped} (couldn't parse ID).");
    }
}
#endif