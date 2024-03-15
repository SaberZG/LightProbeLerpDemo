using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

public delegate void SaveLightProbeDelegate(string name);
public delegate void LerpLightProbesDelegate(LightProbes probeA, LightProbes probeB, LightProbes probeC, float lerp);
public class EventsHandler
{
    public static SaveLightProbeDelegate SaveLightProbeEvent;
    public static LerpLightProbesDelegate LerpLightProbesEvent;

    public static void CallSaveLightProbeEvent(string name)
    {
        if (SaveLightProbeEvent != null)
        {
            SaveLightProbeEvent.Invoke(name);
        }
    }

    public static void CallLerpLightProbes(LightProbes probeA, LightProbes probeB, LightProbes probeC, float lerp)
    {
        if (LerpLightProbesEvent != null)
        {
            LerpLightProbesEvent.Invoke(probeA, probeB, probeC, lerp);
        }
    }
}
