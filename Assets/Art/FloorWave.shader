Shader "Unlit/FloorWave"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        //_WaterFogColor("Water Fog Colour", color) = (0,0,0,0)
        //_WaterFogDensity("Water Fog Density", range(0,1)) = 0.5
        //_RefractStrength("Refraction Strength", range(0,1)) = 0.25
        _WaveA("Wave A(dir, steepness, wavelength)", vector) = (1,0,0.5,10)
        _WaveB("Wave B(dir, steepness, wavelength)", vector) = (1,0,0.5,10)
        _WaveC("Wave C(dir, steepness, wavelength)", vector) = (1,0,0.5,10)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members normal)
            #pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #pragma target 3.0

            //#include "LookingThroughWater.cginc"


            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            float4 _WaveA, _WaveB, _WaveC;

            UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            float3 GerstnerWave(float4 wave, float3 p, inout float3 tangent, inout float3 binormal)
            {
                float steepness = wave.z;
                float length = wave.w;
                float k = UNITY_TWO_PI / length;
                float c = sqrt(9.8 / k);
                float2 dir = normalize(wave.xy);
                float freq = k * (dot(dir, p.xz) - _Time.y * c);
                float amp = steepness / k;
                //p.x += dir.x * (amp * cos(freq));
                //p.y = sin(freq) * amp;
                //p.z += dir.y * (amp * cos(freq));
                tangent += float3(
                    -dir.x * dir.x * (steepness * sin(freq)),
                    dir.x * steepness * cos(freq),
                    -dir.x * dir.y * (steepness * sin(freq))
                );
                binormal += float3(
                    -dir.x * dir.y * (steepness * sin(freq)),
                    dir.y * steepness * cos(freq),
                    -dir.y * dir.y * (steepness * sin(freq))
                );
                return float3(
                    dir.x * (amp * cos(freq)),
                    sin(freq) * amp,
                    dir.y * (amp * cos(freq))
                );
            }


            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                float3 gridPoint = v.vertex;
                float3 tangent = float3(1, 0, 0);
                float3 binormal = float3(0, 0, 1);
                float3 p = gridPoint;
                p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
                p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
                p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
                float3 norm = normalize(cross(binormal, tangent));
                o.normal = norm;
                o.vertex.xyz = p;

                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}