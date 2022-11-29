// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShaderFog"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _FogColor ("Fog Color (RGB)", Color) = (0.5, 0.5, 0.5, 1.0)
        _FogStart ("Fog Start", Float) = 0.0
        _FogEnd ("Fog End", Float) = 10.0

        _bwBlend("BW", range(0,1)) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _MainTex;
            uniform float _bwBlend;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Scale;

           v2f vert(appdata v)
           {
               v2f o;
               o.uv = v.uv;
               o.vertex = UnityObjectToClipPos(v.vertex);
               return o;
           }

            fixed4 frag(v2f i) : SV_TARGET
            {
                //get depth from depth texture
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                //linear depth between camera and far clipping plane
                depth = Linear01Depth(depth);

                return depth;
            }

            //float4 frag(v2f i) : COLOR
            //{
            //    float4 c = tex2D(_MainTex, i.uv);
            //    float lum = c.r * .3 + c.g * .59 + c.b * .11;
            //    float3 bw = float3(lum, lum, lum);
            //
            //    float4 result = c;
            //    float depth = tex2D(_CameraDepthTexture,i.uv).r;
            //    depth = Linear01Depth((depth));
            //    depth = depth * _ProjectionParams.z;
            //    result.rgb = float3(depth,depth,depth); //lerp(c.rgb, bw, depth);
            //    return result;
            //}

            //Tags { "RenderType"="Opaque" }
            //Fog { Mode off }
            //
            //CGPROGRAM
            //#pragma surface surf Lambert vertex:vert //finalcolor:fcolor
            //
            //sampler2D _MainTex;
            //sampler2D _CameraDepthTexture;
            //fixed4 _FogColor;
            //float _FogStart;
            //float _FogEnd;
            //
            //struct Input {
            //	float2 uv_MainTex;
            //	float fogVar;
            //};
            //
            //void vert(inout appdata_full v, out Input data) {
            //	data.uv_MainTex = v.texcoord.xy;
            //	float zpos = UnityObjectToClipPos(v.vertex).z;
            //	data.fogVar = saturate(1.0 - (_FogEnd - zpos) / (_FogEnd - _FogStart));
            //}
            //
            //void surf(Input IN, inout SurfaceOutput o) {
            //	half4 c = tex2D(_MainTex, IN.uv_MainTex);
            //	o.Albedo = c.rgb;
            //	o.Alpha = c.a;
            //}

            //void fcolor(Input IN, SurfaceOutput o, inout fixed4 color) {
            //	fixed3 fogColor = _FogColor.rgb;
            //	float a = tex2D(_CameraDepthTexture, IN.uv_MainTex);
            //	color.rgb = lerp(color.rgb, fogColor, IN.fogVar);
            //}
            ENDCG
        }
    }
}