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
    
    // if len is outside 0..1 range, return black
    dir *= (len <= 1);
    return dir;
}


void UVToDirection_float(float2 uv, float targetAngleRad, out float3 dir)
{
    dir = UVToDirection(uv, targetAngleRad);
}