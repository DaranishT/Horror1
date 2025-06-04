Shader "URP/Custom/HelmetVisorURP"
{
    Properties
    {
        _MainTex("HUD Texture", 2D) = "white" {}
        _Distortion("Edge Distortion", Range(0, 0.5)) = 0.15
        _EdgeDarkness("Edge Darkness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Transparent" "Queue" = "Transparent+100" }
        LOD 100

        Pass
        {
            Name "HelmetVisorPass"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Distortion;
            float _EdgeDarkness;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 centeredUV = uv * 2.0 - 1.0;
                float distance = length(centeredUV);

                // Apply distortion only toward edges
                float falloff = smoothstep(0.4, 1.0, distance);
                centeredUV *= 1.0 + (_Distortion * falloff * falloff);
                uv = centeredUV * 0.5 + 0.5;

                float vignette = 1.0 - (_EdgeDarkness * smoothstep(0.7, 1.0, distance));

                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                color.rgb *= vignette;
                return color;
            }

            ENDHLSL
        }
    }

    FallBack Off
}
