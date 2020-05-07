using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

public class Velodyne_UDP_Send : MonoBehaviour
{
    // 
    public static string IP = "192.168.2.101";
    public static int port = 2368;
    public static int numDataBLocks = 12;

    //
    private static int[] laserIdxs = { 0, 2, 4, 6, 8, 10, 12, 14, 1, 3, 5, 7, 9, 11, 13, 15 };
    private static int[] laserIdxs1 = { 0,8 ,1,9, 2,10, 3,11, 4,12, 5,13, 6,14,  7, 15};

    // connection
    private static IPAddress broadcast;
    private static IPEndPoint ep;
    private static Socket s;

    //gos
    private static Lidar lidarGO;

    //functions
    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    public static float getAzimuth(byte[] az)
    {
        //Console.WriteLine("Hex: {0:X}", ByteArrayToString(az));
        ushort azInt = System.BitConverter.ToUInt16(az, 0);
        float azimuth = (float)azInt / 100.0f;
        //Console.Write("azimuth : {0} -->", azimuth);
        return azimuth;
    }

    public static byte[] makeAzimuthBytes(float az)
    {
        ushort azimuth = (ushort)(az * 100.0f);
        //Console.Write("azimuth : {0} -->", azimuth);
        byte[] azimuthArr = System.BitConverter.GetBytes(azimuth);
        //Console.WriteLine("Hex: {0:X}", ByteArrayToString(azimuthArr));
        return azimuthArr;
    }

    public static byte[] makeDistanceBytes(float dist)
    {
        ushort distance = (ushort)(dist / 0.002f);
        //Console.Write("distance : {0} ", distance);
        byte[] distArr = System.BitConverter.GetBytes(distance);
        //Console.WriteLine("Hex: {0:X}", ByteArrayToString(distArr));
        return distArr;
    }

    public static float getDistance(byte[] distance)
    {
        //Console.WriteLine("Hex: {0:X}", ByteArrayToString(distance));
        ushort distInt = System.BitConverter.ToUInt16(distance, 0);
        float dist = (float)distInt * 0.002f;
        //Console.Write(" {0} |", dist);
        return dist;
    }

    public static byte[] Serialize(float[] distanceData, float[] azimuth, int azimutStart, int numLayers, int numIncrements)
    {
        byte[] result = new byte[1206];
        byte[] azimuthArr;
        byte[] distanceArr;

        int dbIdx = 0;
        int azIdx = azimutStart;
        int distIdx;

        for (int db = 0; db < 12; db++)
        {
            if (azIdx >= numIncrements)
            {
                azIdx = 0;
            }
            //Debug.Log("azIdx " + azIdx + " dbIdx " + db + "\n");

            distIdx = azIdx * numLayers;

            // write a data block
            dbIdx = db * 100;
            result[dbIdx + 0] = 0xff;
            result[dbIdx + 1] = 0xee;
            azimuthArr = makeAzimuthBytes(azimuth[azIdx]);
            Buffer.BlockCopy(azimuthArr, 0, result, dbIdx + 2, 2);
            //Debug.Log("db "+db +" azimut " + azimuth[azIdx]);

            // write channel data, first firing
            for (int c1 = 0; c1 < 16; c1++)
            {
                distanceArr = makeDistanceBytes(distanceData[distIdx + laserIdxs1[c1]]);
                //Debug.Log("dist1[ " + (distIdx + c1) + "] " + distanceData[distIdx + c1]+ " for idx "+ laserIdxs1[c1] + " mapped "+ distanceData[distIdx + laserIdxs1[c1]]);

                Buffer.BlockCopy(distanceArr, 0, result, dbIdx + 4 + c1 * 3, 2);
                result[dbIdx + 4 + c1 * 3 + 2] = 0xff;
            }
            // write channel data, 2nd firing
            for (int c2 = 16; c2 < 32; c2++)
            {
                distanceArr = makeDistanceBytes(distanceData[distIdx + laserIdxs1[c2 -16]]);
                //Debug.Log("dist2[ " + (distIdx + c2)+"] " + distanceData[distIdx + c2]);

                Buffer.BlockCopy(distanceArr, 0, result, dbIdx + 4 + c2 * 3, 2);
                result[dbIdx + 4 + c2 * 3 + 2] = 0x12;
            }

            //update idxs
            azIdx = azIdx + 2;
        }
        result[1200] = 0x00;
        result[1201] = 0x00;
        result[1202] = 0x00;
        result[1203] = 0x00;
        result[1204] = 0x37;
        result[1205] = 0x22;
        return result;
    }

    public static int Deserialize(byte[] data)
    {
        using (MemoryStream inputStream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(inputStream))
        {
            byte[] flagArr, azimuthArr, distanceArr;
            byte reflectivity;
            int idx;

            float distance = 0;
            float azimuth = 0;
            float[] distanceValues = new float[16];


            //----channel blocks---
            for (int db = 0; db < 12; db++)
            {
                flagArr = reader.ReadBytes(2);
                azimuthArr = reader.ReadBytes(2);
                azimuth = getAzimuth(azimuthArr);

                for (int c = 0; c < 16; c++)
                {
                    distanceArr = reader.ReadBytes(2);
                    idx = laserIdxs[c];
                    distance = getDistance(distanceArr);
                    distanceValues[idx] = distance;
                    reflectivity = reader.ReadByte();
                }
                for (int c = 16; c < 32; c++)
                {
                    distanceArr = reader.ReadBytes(2);
                    idx = laserIdxs[c-16];
                    distance = getDistance(distanceArr);
                    distanceValues[idx] = distance;
                    reflectivity = reader.ReadByte();
                }
                Console.Write("res azimuth : {0} -->", azimuth);
                for (int i = 0; i < 16; i++)
                {
                    Console.Write(" {0} | ", distanceValues[i]);
                }
                Console.WriteLine("");
            }

            //----get time---
            byte[] timestamp = reader.ReadBytes(4);
            Array.Reverse(timestamp);
            float time = System.BitConverter.ToSingle(timestamp, 0);
            //Console.WriteLine("time : {0}", time);

            //----get factory---
            byte[] factory = reader.ReadBytes(2);
        }
        return 0;
    }

    public void Start()
    {
        broadcast = IPAddress.Parse(IP);
        ep = new IPEndPoint(broadcast, 2368);
        s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        lidarGO = gameObject.GetComponent<Lidar>();
    }

    public void FixedUpdate()
    {
        Boolean cont = true;
        int idx = 0;
        int azIncrPerMsg = 2 * numDataBLocks;
        while (cont)
        {          
            //Debug.Log("start with IDx "+idx+" at "+Time.time);
            byte[] dummy1 = Serialize(lidarGO.distances, lidarGO.azimuts, idx, lidarGO.numberOfLayers, lidarGO.numberOfIncrements);
            s.SendTo(dummy1, ep);
            idx = idx + azIncrPerMsg;
            if (idx > (lidarGO.numberOfIncrements-1))
            {
                idx = idx - lidarGO.numberOfIncrements;
                cont = false;
            }

        }
    }
}

