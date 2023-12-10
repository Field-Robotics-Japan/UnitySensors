using Unity.Collections;

namespace UnitySensors.Data.PointCloud
{
    public interface IPointInterface
    {
        public void CopyTo(NativeArray<byte> dst);
    }
}