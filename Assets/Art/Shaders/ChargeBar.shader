Shader "Unlit/Healthbar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Health ("Health", range(0,1)) = 1
        _FullHealthColor("Full Health Color", color) = (0,1,0,1)
        _LowHealthColor("Low Health Color", color) = (1,0,0,1)
        _LerpThreshold("Lerp threshold", float) = 0.2
        _BGColor("BG Color", color) = (0,0,0,1)
        _Curve("Curve degree",float) = 8
        _BounceSpeed("Bounce speed", float) = 1
        _BounceSize("Bounce size", float) = 1
        _BounceThreshold("Bounce threshold", range(0,1)) = 0.3
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Zwrite off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Health;
            float4 _FullHealthColor;
            float4 _LowHealthColor;
            float4 _BGColor;
            float _Curve;
            float _BounceSpeed;
            float _BounceSize;
            float _BounceThreshold;
            float _LerpThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.y += abs(cos(_Time.y*_BounceSpeed))/_BounceSize * (_Health < _BounceThreshold);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 coords = i.uv;
                coords.x *= _Curve;
                 
                float2 pointOnLineSeg = float2(clamp(coords.x, 0.5, _Curve-0.5),0.5);
                float sdf = distance(coords,pointOnLineSeg) *2 - 1;
                clip(-sdf); //if input < 0, discard the pixel

                float border = sdf + 0.1;
                float pd = fwidth(border);
                //float borderMask = step(0,-border);
                float borderMask = 1 - saturate(border/pd);

                float healthbarMask = _Health > i.uv.x;     ///determines if the current x coordinate is smaller than health (1 = true, 0 = false)
                float4 healthbarCol = tex2D(_MainTex, float2(_Health,i.uv.y));  //sample the colour from the texture
                float lerpAmount = clamp(_Health*_LerpThreshold,0,1);
                float4 finalColour = lerp(_LowHealthColor, _FullHealthColor, lerpAmount);    //to lerp between two different colours supplied
                //float healthbarMaskSteps = _Health > floor(i.uv.x * 8) / 8;   //for fixed steps of health
                finalColour = lerp(_BGColor, finalColour, healthbarMask);   //lerp between the BG or health colour by the health mask (which will be 0 or 1)
                return float4(finalColour.xyz * borderMask, 1);
            }
            ENDCG
        }
    }
}