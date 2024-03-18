/*
* @aAuthor: SaberGodLY
* @Description: 测试用类
* @Date: 2024年03月15日
*/
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
    public bool BackToProbe1 = false;
    public bool BackToProbe2 = false;
    public LightProbes probe1;
    public LightProbes probe2;
    private LightProbes probe3;
    [Range(0f, 1f)]
    public float LerpVal = 0.5f;
    private bool lerpAdd = true;
    public string SaveName = "LightProbes1";

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
}
