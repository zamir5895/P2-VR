﻿Shader "URP/UI/Prerendered"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "12.1" // 2021.3+
        }

        Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
        Pass
        {
            Tags {"RenderType"="Transparent"}

            Blend One OneMinusSrcAlpha, Zero OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ ALPHA_SQUARED
            #pragma multi_compile _ EXPENSIVE
            #pragma multi_compile _ OVERLAP_MASK

            #define WITH_CLIP 0
            #define ALPHA_TO_MASK 0
            #define ALPHA_BLEND 1
            #include "UIPrerendered.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "MotionVectors"
            Tags {
                "Queue" = "Opaque"
                "LightMode" = "MotionVectors"
                "RenderType"="Opaque"
                "RenderPipeline" = "UniversalPipeline"
            }

            ZWrite On

            HLSLPROGRAM
            #pragma vertex mv_vert
            #pragma fragment mv_frag
            #pragma multi_compile _ ALPHA_SQUARED
            #pragma multi_compile _ EXPENSIVE

            #define WITH_CLIP 1
            #define ALPHA_TO_MASK 0
            #define MOTION_VECTORS 1
            #include "UIPrerendered.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "XRMotionVectors"
            Tags {
                "Queue" = "Opaque"
                "LightMode" = "XRMotionVectors"
                "RenderType"="Opaque"
                "RenderPipeline" = "UniversalPipeline"
            }

            ZWrite On

            HLSLPROGRAM
            #pragma vertex mv_vert
            #pragma fragment mv_frag
            #pragma multi_compile _ ALPHA_SQUARED
            #pragma multi_compile _ EXPENSIVE

            #define WITH_CLIP 1
            #define ALPHA_TO_MASK 0
            #define MOTION_VECTORS 1
            #include "UIPrerendered.hlsl"
            ENDHLSL
        }
    }

    FallBack "UI/Prerendered"
}
