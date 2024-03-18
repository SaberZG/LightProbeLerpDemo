/*
* @aAuthor: SaberGodLY
* @Description: 线性内插光照探针相关方法合计
* @Date: 2024年03月14日
*/
using UnityEngine;
using UnityEngine.Rendering;
using SHTools;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct Job_LerpSphericalharmonicsL2 : IJob
{
    [NoAlias][NativeDisableParallelForRestriction][ReadOnly]public NativeArray<SphericalHarmonicsL2> coeffA;
    [NoAlias][NativeDisableParallelForRestriction][ReadOnly]public NativeArray<SphericalHarmonicsL2> coeffB;
    [NoAlias][NativeDisableParallelForRestriction][WriteOnly]public NativeArray<SphericalHarmonicsL2> coeffC;
    [NoAlias]public float lerp;
    public void Execute()
    {
        for (int i = 0; i < coeffA.Length; i++)
        {
            coeffC[i] = SHUtility.Lerp(coeffA[i], coeffB[i], lerp);
        }
    }
}

public class LightProbeUtil
{
    // 测试函数
    public static void LerpLightProbes(LightProbes probeA, LightProbes probeB, LightProbes probeC, float lerp)
    {
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
    }
}
