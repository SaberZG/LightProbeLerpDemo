using System;
using UnityEngine;
using UnityEngine.Rendering;
using SHTools;

public class LightProbeUtil
{
    public static void LerpLightProbes(LightProbes probeA, LightProbes probeB, LightProbes probeC, float lerp)
    {
        var executeStartTime = Time.realtimeSinceStartup;
        var coeffA = probeA.bakedProbes;
        var coeffB = probeB.bakedProbes;

        if (coeffA.Length != coeffB.Length)
        {
            Debug.LogError("两个光照探针的数量不相等");
            return;
        }

        SphericalHarmonicsL2[] lerpedCoeff = new SphericalHarmonicsL2[coeffA.Length];
        for (int i = 0; i < coeffA.Length; i++)
        {
            lerpedCoeff[i] = SHUtility.Lerp(coeffA[i], coeffB[i], lerp);
        }

        probeC.bakedProbes = lerpedCoeff;
        LightmapSettings.lightProbes = probeC;
        var executeEndTime = Time.realtimeSinceStartup;
        Debug.LogFormat("Execute Lerp Time：{0}", executeEndTime - executeStartTime);
    }
}
