Shader "UnitySensors/Panoramic"
{

	Properties
    {
        [NoScaleOffset] _MainTex("Cubemap", Cube) = "" {}
        _Rotation("Rotation", Vector) = (0, 0, 0, 0)
	}

    Subshader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            #define PI    3.141592653589793
            #define TAU   6.283185307179587

            float3      _Rotation;
            samplerCUBE _MainTex;

            struct v2f {
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

            float3 EquirectangularProjection(float2 In)
            {
                float a = In.x * TAU;
                float b = In.y * PI;

                return float3(
                    -sin(a) * sin(b),
                    -cos(b),
                    -cos(a) * sin(b)
                );
            }

            float3x3 EulerToRotMatrix(float3 In)
            {
                // Inspired from
                // https://github.com/whdlgp/Equirectangular_rotate/blob/master/equirectangular_rotate.py
                float3 s = sin(In);
                float3 c = cos(In);

                float3x3 x = float3x3(
                    float3(1,   0,    0),
                    float3(0, c.x, -s.x),
                    float3(0, s.x,  c.x)
                );

                float3x3 y = float3x3(
                    float3( c.y, 0, s.y),
                    float3(   0, 1,   0),
                    float3(-s.y, 0, c.y)
                );

                float3x3 z = float3x3(
                    float3(c.z, -s.z, 0),
                    float3(s.z,  c.z, 0),
                    float3(  0,    0, 1)
                );

                return mul(mul(x,y),z);
            }

            fixed4 frag(v2f i) : COLOR
            {
                float3 unit = EquirectangularProjection(i.uv);
                float3x3 rotMatrix = EulerToRotMatrix(mul(_Rotation,PI/180.0));

                unit = mul(rotMatrix, unit);

                return texCUBE(_MainTex, unit);
            }

        ENDCG
        }
    }
    Fallback Off
}