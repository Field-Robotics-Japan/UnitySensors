Shader "UnitySensors/FisheyeCamera"
{

    Properties
    {
        [NoScaleOffset] _MainTex ("Cubemap", Cube) = "" { }
        _Angle ("Angle", Range(90, 360)) = 180
    }

    Subshader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _MainTex;
            float _Angle;
            float4x4 _WorldTransform;

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }
            
            // Reference: https://github.com/prefrontalcortex/DomeTools/blob/main/package/Runtime/Effects/DomemasterInclude.cginc
            float map(float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }

            float3 UVToDirection(float2 uv, float targetAngleRad)
            {
                float2 uv2 = uv * 2 - 1;
                uv2.y *= -1;
                float phi = atan2(uv2.y, uv2.x);
                float len = length(uv2);
                float theta = len * 0.5;
                theta = map(theta, 1, 0, targetAngleRad, 0);
                float3 dir = float3(sin(theta) * cos(phi), cos(theta), sin(theta) * sin(phi));
                
                return dir;
            }

            // Reference: https://github.com/prefrontalcortex/DomeTools/blob/main/package/Runtime/Effects/CubeToDome2.shadergraph
            fixed4 frag(v2f i) : COLOR
            {
                float3 dir = UVToDirection(i.uv, radians(_Angle));
                float3 worldDir = mul(_WorldTransform, dir);
                
                float3 fisheyeMask = distance(0.5, i.uv) <= 0.5;
                float3 color = texCUBE(_MainTex, worldDir);
                
                color *= fisheyeMask;

                return float4(color, 1);
            }

            ENDCG
        }
    }
    Fallback Off
}
