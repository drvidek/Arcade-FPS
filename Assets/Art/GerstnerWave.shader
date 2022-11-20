Shader "Custom/GerstnerWave"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
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
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        LOD 200

        GrabPass
        {
            "_WaterBackground"
        }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert  vertex:vert alpha finalcolor:mycolor //this runs ResetAlpha on the final color pass


        #pragma target 3.0
        // make fog work
        #pragma multi_compile_fog
        //#include "LookingThroughWater.cginc"

        sampler2D _MainTex;
        uniform half4 unity_FogStart;
        uniform half4 unity_FogEnd;

        struct Input
        {
            float2 uv_MainTex;
            half fog;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4 _WaveA, _WaveB, _WaveC;

        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void ResetAlpha(Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            color.a = 1;
        }

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

       // void myvert(inout appdata_full v, out Input data)
       // {
       //     UNITY_INITIALIZE_OUTPUT(Input, data);
       //     float pos = length(UnityObjectToViewPos(v.vertex).xyz);
       //     float diff = unity_FogEnd.x - unity_FogStart.x;
       //     float invDiff = 1.0f / diff;
       //     data.fog = clamp((unity_FogEnd.x - pos) * invDiff, 0.0, 1.0);
       // }

        void vert(inout appdata_full vertexData, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            float pos = length(UnityObjectToViewPos(vertexData.vertex).xyz);
            float diff = unity_FogEnd.x - unity_FogStart.x;
            float invDiff = 1.0f / diff;
            data.fog = clamp((unity_FogEnd.x - pos) * invDiff, 0.0, 1.0);
            float3 gridPoint = vertexData.vertex.xyz;
            float3 tangent = float3(1, 0, 0);
            float3 binormal = float3(0, 0, 1);
            float3 p = gridPoint;
            p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
            p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
            p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
            float3 norm = normalize(cross(binormal, tangent));
            vertexData.normal = norm;
            vertexData.vertex.xyz = p;
        }


        void mycolor(Input IN, SurfaceOutput o, inout fixed4 color)
        {
            #ifdef UNITY_PASS_FORWARDADD
       UNITY_APPLY_FOG_COLOR(IN.fog, color, float4(0,0,0,0));
            #else
            UNITY_APPLY_FOG_COLOR(IN.fog, color, unity_FogColor);
            #endif
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
}