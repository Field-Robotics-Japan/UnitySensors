Shader "UnitySensors/DepthBufferLidar"
{
    // TODO: rewrite it in shader graph to support URP/HDRP
    Properties
    {
        _F ("_F", float) = 0.0
        _Y_MIN ("_Y_MIN", float) = 0.0
        _Y_MAX ("_Y_MAX", float) = 1.0
        _Y_COEF ("_Y_COEF", float) = 1.0
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
            float _F;
            float _Y_MIN;
            float _Y_MAX;
            float _Y_COEF;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                clip(i.uv.y - _Y_MIN);
                clip(_Y_MAX - i.uv.y);

                float2 uv_local = float2(i.uv.x, (i.uv.y - _Y_MIN) * _Y_COEF);
                float2 ndc = uv_local * 2 - 1;
                float4 viewDir = mul(unity_CameraInvProjection, float4(ndc, 1, 1));

                float depth01 = Linear01Depth(tex2D(_CameraDepthTexture, uv_local).r);
                float3 viewPos = (viewDir.xyz / viewDir.w) * depth01;
                
                float distance = length(viewPos) / _F;
                return float4(distance, distance, distance, 1);
            }
            ENDCG
        }
    }
}