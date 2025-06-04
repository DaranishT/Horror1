Shader "Custom/HelmetVisor_Toggleable" {
    Properties {
        _MainTex ("HUD Texture", 2D) = "white" {}
        _Distortion ("Distortion Strength", Range(-0.3, 0.3)) = 0.15
        _EdgeDarkness ("Edge Darkness", Range(0, 1)) = 0.5
        [Toggle]_EnableDistortion ("Enable Distortion", Float) = 1 // New toggle property
    }
    SubShader {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent+100" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Distortion;
            float _EdgeDarkness;
            float _EnableDistortion; // Added toggle

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;
                
                // Apply distortion only if enabled
                if (_EnableDistortion > 0.5) {
                    uv = uv * 2.0 - 1.0; // Remap UVs to [-1, 1]
                    float distance = length(uv);
                    uv *= 1.0 + (_Distortion * distance * distance); // Barrel distortion
                    uv = uv * 0.5 + 0.5; // Remap back to [0, 1]
                }

                // Edge vignette (always applied)
                float2 vignetteUV = i.uv * 2.0 - 1.0;
                float vignetteDistance = length(vignetteUV);
                float vignette = 1.0 - (_EdgeDarkness * smoothstep(0.7, 1.0, vignetteDistance));

                // Sample texture
                fixed4 col = tex2D(_MainTex, uv);
                col.rgb *= vignette;
                return col;
            }
            ENDCG
        }
    }
}