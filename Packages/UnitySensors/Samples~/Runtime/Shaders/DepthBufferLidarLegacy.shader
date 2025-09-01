Shader "UnitySensors/DepthBufferLidar"
{
    // TODO: rewrite it in shader graph to support URP/HDRP
    Properties
    {
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
            float _Y_MIN;
            float _Y_MAX;
            float _Y_COEF;

            #include "DepthBufferLidar.cginc"

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
                float overlapAlpha;
                GetOverlapAlpha_float(i.uv, overlapAlpha);
                clip(overlapAlpha - 0.5);

                float2 sceneDepthUV;
                GetSceneDepthUV_float(i.uv, sceneDepthUV);

                float depth01 = Linear01Depth(tex2D(_CameraDepthTexture, sceneDepthUV));
                
                float distance;
                Depth2Distance_float(depth01, sceneDepthUV, distance);
                return float4(distance.xxx, 1);
            }
            ENDCG
        }
    }
}