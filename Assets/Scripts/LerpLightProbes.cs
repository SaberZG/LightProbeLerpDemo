/*
* @aAuthor: SaberGodLY
* @Date: 2024年03月15日
*/
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class LerpLightProbes : MonoBehaviour
{
    public bool TriggerLerpOptimized = false;
    public LightProbes probe1;
    public LightProbes probe2;
    [Range(0f, 1f)]
    public float LerpVal = 0.5f;
    private bool lerpAdd = true;
    
    public bool TriggerInitNativeArray = false;
    private NativeArray<SphericalHarmonicsL2> coeffA;
    private NativeArray<SphericalHarmonicsL2> coeffB;
    private NativeArray<SphericalHarmonicsL2> coeffC;

    private JobHandle jobHandle;
    private bool isJobRuning = false;
    
    void Update()
    {
        if (TriggerInitNativeArray)
        {
            InitNativeArray();
            TriggerInitNativeArray = false;
        }
        if (TriggerLerpOptimized)
        {
            if (!coeffA.IsCreated)
            {
                InitNativeArray();
            }
            LerpVal += Time.deltaTime * (lerpAdd ? 1f : -1f);
            if (LerpVal > 1f || LerpVal < 0f)
            {
                LerpVal = Mathf.Clamp(LerpVal, 0.0f, 1.0f);
                lerpAdd = !lerpAdd;
            }
            TryStartCoroutine();
        }
    }
    
    public void InitNativeArray()
    {
        if (coeffA.IsCreated) coeffA.Dispose();
        if (coeffB.IsCreated) coeffB.Dispose();
        if (coeffC.IsCreated) coeffC.Dispose();

        if (probe1 == null || probe2 == null) return;
        var _coeffA = probe1.bakedProbes;
        var _coeffB = probe2.bakedProbes;
        
        if (_coeffA.Length != _coeffB.Length)
        {
            Debug.LogError("两个光照探针的数量不相等");
            return;
        }

        int count = _coeffA.Length;
        coeffA = new NativeArray<SphericalHarmonicsL2>(count, Allocator.Persistent);
        coeffB = new NativeArray<SphericalHarmonicsL2>(count, Allocator.Persistent);
        coeffC = new NativeArray<SphericalHarmonicsL2>(count, Allocator.Persistent);
        for (int i = 0; i < _coeffA.Length; i++)
        {
            coeffA[i] = _coeffA[i];
            coeffB[i] = _coeffB[i];
        }
    }
    
    private void TryStartCoroutine()
    {
        if (isJobRuning) return;

        isJobRuning = true;
        StartCoroutine("CoroutineLerpLightProbe");
    }

    private IEnumerator CoroutineLerpLightProbe()
    {
        Job_LerpSphericalharmonicsL2 job = new Job_LerpSphericalharmonicsL2();
        job.coeffA = coeffA;
        job.coeffB = coeffB;
        job.coeffC = coeffC;
        job.lerp = LerpVal;
        jobHandle = job.Schedule();
        yield return new WaitUntil(() => jobHandle.IsCompleted);
        
        jobHandle.Complete();
        LightmapSettings.lightProbes.bakedProbes = coeffC.ToArray();
        
        isJobRuning = false;
    }
    
    private void OnDestroy()
    {
        TriggerLerpOptimized = false;
        jobHandle.Complete();

        if (coeffA.IsCreated) coeffA.Dispose();
        if (coeffB.IsCreated) coeffB.Dispose();
        if (coeffC.IsCreated) coeffC.Dispose();
    }
}
