using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SaveLightProbe : MonoBehaviour
{
    public bool TriggerSave = false;
    public bool TriggerLerp = false;
    public bool TriggerLerpOptimized = false;
    public bool BackToProbe1 = false;
    public bool BackToProbe2 = false;
    public LightProbes probe1;
    public LightProbes probe2;
    private LightProbes probe3;
    [Range(0f, 1f)]
    public float LerpVal = 0.5f;
    private bool lerpAdd = true;
    public string SaveName = "LightProbes1";

    public bool TriggerInitNativeArray = false;
    private NativeArray<SphericalHarmonicsL2> coeffA;
    private NativeArray<SphericalHarmonicsL2> coeffB;
    private NativeArray<SphericalHarmonicsL2> coeffC;

    private JobHandle jobHandle;
    private bool isJobRuning = false;
    // Update is called once per frame
    void Update()
    {
        if (TriggerSave)
        {
            EventsHandler.CallSaveLightProbeEvent(SaveName);
            TriggerSave = false;
        }

        if (TriggerLerp)
        {
            if (probe3 == null)
            {
                probe3 = GameObject.Instantiate(probe1) as LightProbes;
            }
            EventsHandler.CallLerpLightProbes(probe1, probe2, probe3, LerpVal);

            TriggerLerpOptimized = false;
            TriggerLerp = false;
        }

        ////////////// This version performs much better and works in runtime. //////////////
        if (TriggerInitNativeArray)
        {
            InitNativeArray();
            TriggerInitNativeArray = false;
        }
        if (TriggerLerpOptimized)
        {
            if (probe3 == null)
            {
                probe3 = GameObject.Instantiate(probe1) as LightProbes;
            }

            if (!coeffA.IsCreated)
            {
                InitNativeArray();
            }
            
            TryStartCoroutine();
            TriggerLerp = false;
        }
        
        if (BackToProbe1)
        {
            LightmapSettings.lightProbes = probe1;
            BackToProbe1 = false;
        }
        
        if (BackToProbe2)
        {
            LightmapSettings.lightProbes = probe2;
            BackToProbe2 = false;
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

    private void OnDestroy()
    {
        TriggerLerpOptimized = false;
        if (jobHandle != null)
        {
            jobHandle.Complete();
        }
        
        if (coeffA.IsCreated) coeffA.Dispose();
        if (coeffB.IsCreated) coeffB.Dispose();
        if (coeffC.IsCreated) coeffC.Dispose();
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
}
