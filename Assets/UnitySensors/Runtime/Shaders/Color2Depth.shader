Shader "UnitySensors/Color2Depth"
{
    Properties
    {
        _F("_F", float) = 0.0
        _Y_MIN("_Y_MIN", float) = 0.0
        _Y_MAX("_Y_MAX", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture;
            float4 _CameraDepthTexture_ST;
            float _F;
            float _Y_MIN;
            float _Y_MAX;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 viewDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _CameraDepthTexture);
                o.viewDir = mul(unity_CameraInvProjection, float4 (o.uv * 2.0 - 1.0, 1.0, 1.0));
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                clip(i.uv.y - _Y_MIN);
                clip(_Y_MAX - i.uv.y);
                float depth01 = Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r);
                float3 viewPos = (i.viewDir.xyz / i.viewDir.w) * depth01;
                float distance = 1.0f - length(viewPos) / _F;
                return float4(distance, distance, distance, 1.0f);
            }
            ENDCG
        }
    }
}