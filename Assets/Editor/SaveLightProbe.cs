using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class SaveLightProbe
{
    [InitializeOnLoadMethod][DidReloadScripts]
    private static void OnScriptsLoaded()
    {
        if (EventsHandler.SaveLightProbeEvent == null)
        {
            EventsHandler.SaveLightProbeEvent += SaveLightProbeToAssets;
        }

        if (EventsHandler.LerpLightProbesEvent == null)
        {
            EventsHandler.LerpLightProbesEvent += LightProbeUtil.LerpLightProbes;
        }
    }
    public static void SaveLightProbeToAssets(string name = "")
    {
        string savePath = string.Format("Assets/{0}.asset", name);
        LightProbes lp = LightmapSettings.lightProbes;
        if (lp != null)
        {
            AssetDatabase.CreateAsset(GameObject.Instantiate(lp), savePath);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
