Shader "Unlit/TestObjectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vertBase
            #pragma fragment fragBase2
            #pragma target 3.0
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityStandardCoreForward.cginc"
            #include "UnityCG.cginc"
            #include "UnityStandardCore.cginc"
            #include "UnityShaderVariables.cginc"
            #include "UnityStandardConfig.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "AutoLight.cginc"

            half4 fragBase2 (VertexOutputForwardBase i) : SV_Target
            {
                FRAGMENT_SETUP(s)
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                UnityLight mainLight = MainLight ();
                UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
                
                half occlusion = 1;
                half3 gi = ShadeSHPerPixel(s.normalWorld, 0, s.posWorld);
                half4 col = half4(gi, 1);
                // UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);
                // half4 col = half4(gi.light.color, 1);
                return col;
            }
            ENDCG
        }
    }
}
