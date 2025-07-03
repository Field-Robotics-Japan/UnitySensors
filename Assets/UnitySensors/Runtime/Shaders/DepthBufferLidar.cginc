// float _Y_MIN;
// float _Y_MAX;
// float _Y_COEF;

void GetSceneDepthUV_float(float2 uv, out float2 sceneDepthUV)
{
    sceneDepthUV = float2(uv.x, (uv.y - _Y_MIN) * _Y_COEF);
}
void GetOverlapAlpha_float(float2 uv, out float overlapAlpha)
{
    overlapAlpha = (uv.y >= _Y_MIN) * (uv.y <= _Y_MAX);
}
void Depth2Distance_float(float depth, float2 sceneDepthUV, out float distance)
{
    float2 ndc = sceneDepthUV * 2 - 1;
    float4 viewDir = mul(unity_CameraInvProjection, float4(ndc, 1, 1));
    float3 viewPos = viewDir.xyz / viewDir.w * depth;
    distance = length(viewPos);
}