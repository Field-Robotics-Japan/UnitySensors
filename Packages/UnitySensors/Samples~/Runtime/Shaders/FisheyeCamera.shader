Shader "UnitySensors/FisheyeCamera"
{

    Properties
    {
        [NoScaleOffset] _MainTex ("Cubemap", Cube) = "" { }
        _Angle ("Angle", Range(90, 360)) = 180
        _Equidistant ("Equidistant Projection", Integer) = 1
        _alpha ("Alpha", Range(0, 1)) = 1
        _beta ("Beta", Float) = 1
        _fx ("Normalized Focal Length X", Float) = 1
        _fy ("Normalized Focal Length Y", Float) = 1
        _cx ("Normalized Principal Point X", Float) = 0.5
        _cy ("Normalized Principal Point Y", Float) = 0.5
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
            float _alpha;
            float _beta;
            float _fx;
            float _fy;
            float _cx;
            float _cy;
            int _Equidistant;

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
            
            // Reference: https://github.com/prefrontalcortex/DomeTools
            float3 EquidistantToDirection(float2 uv, float targetAngleRad)
            {
                float2 uv2 = uv * 2 - 1;
                float phi = atan2(uv2.y, uv2.x);
                float radius = length(uv2);
                float theta = radius * targetAngleRad * 0.5;
                float3 dir = float3(sin(theta) * cos(phi), sin(theta) * sin(phi), cos(theta));
                dir = normalize(dir);
                return dir;
            }
            
            float3 EUCMToDirection(float2 uv)
            {
                float2 uv2 = (uv - float2(_cx, _cy)) / float2(_fx, _fy);
                float r2 = dot(uv2, uv2);
                float gamma = 1 - _alpha;
                float z = (1 - _alpha * _alpha * _beta * r2) / (_alpha * sqrt(1 - (_alpha - gamma) * _beta * r2) + gamma);
                float3 dir = float3(uv2, z);
                dir = normalize(dir);
                return dir;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float3 dir;
                if (_Equidistant)
                    dir = EquidistantToDirection(i.uv, radians(_Angle));
                else
                    dir = EUCMToDirection(i.uv);
                // Apply fisheye mask based on the angle
                float angle = acos(dir.z);
                float3 fisheyeMask = angle <= radians(_Angle) * 0.5;

                float3 worldDir = mul((float3x3)_WorldTransform, dir);
                float3 color = texCUBE(_MainTex, worldDir);
                color *= fisheyeMask;
                return float4(color, 1);
            }

            ENDCG
        }
    }
    Fallback Off
}
