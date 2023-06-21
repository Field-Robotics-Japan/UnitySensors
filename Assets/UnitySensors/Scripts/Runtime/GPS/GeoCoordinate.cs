using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// branch : gps

namespace FRJ.Sensor
{
    /// <summary>
    /// 平面直角座標系と緯度経度の相互変換
    /// 参照：国土地理院の「測量計算サイト」
    /// http://vldb.gsi.go.jp/sokuchi/surveycalc/main.html
    /// </summary>
    public class GeoCoordinate
    {
        const double daa = 6378137;             //長半径
        const double dF  = 298.257222101d;      //逆扁平率
        const double dM0 = 0.9999;              //平面直角座標系のY軸上における縮尺係数(UTM座標系の場合→0.9996)

        private double _lon0;
        private double _lat0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="lat0">原点:緯度[度]</param>
        /// <param name="lon0">原点:経度[度]</param>
        public GeoCoordinate(double lat0, double lon0)
        {
            this._lat0 = lat0;
            this._lon0 = lon0;
        }

        /// <summary>
        /// 地理座標系（緯度・経度） -> 平面直角座標
        /// </summary>
        /// <param name="Lat">緯度[度]</param>
        /// <param name="Lon">経度[度]</param>
        /// <returns>X、Y</returns>
        public (double X, double Z) LatLon2XZ(double Lat, double Lon)
        {
            double dn = 1d / (2 * dF - 1);

            //ラジアン単位に
            Lat = Deg2Rad(Lat);
            Lon = Deg2Rad(Lon);
            double Lat0 = Deg2Rad(this._lat0);
            double Lon0 = Deg2Rad(this._lon0);

            double dt = Math.Sinh(Atanh(Math.Sin(Lat)) - (2 * Math.Sqrt(dn)) / (1 + dn) * Atanh(2 * Math.Sqrt(dn) / (1 + dn) * Math.Sin(Lat)));
            double dtb = Math.Sqrt(1 + Math.Pow(dt, 2));
            double dLmc = Math.Cos(Lon - Lon0);
            double dLms = Math.Sin(Lon - Lon0);
            double dXi = Math.Atan(dt / dLmc);
            double dEt = Atanh(dLms / dtb);

            //α1→0～α5→4
            double[] dal = new double[6];
            dal[0] = 0;
            dal[1] = 1d / 2d * dn - 2d / 3d * Math.Pow(dn, 2) + 5d / 16d * Math.Pow(dn, 3) + 41d / 180d * Math.Pow(dn, 4) - 127d / 288d * Math.Pow(dn, 5);
            dal[2] = 13d / 48d * Math.Pow(dn, 2) - 3d / 5d * Math.Pow(dn, 3) + 557d / 1440d * Math.Pow(dn, 4) + 281d / 630d * Math.Pow(dn, 5);
            dal[3] = 61d / 240d * Math.Pow(dn, 3) - 103d / 140d * Math.Pow(dn, 4) + 15061d / 26880d * Math.Pow(dn, 5);
            dal[4] = 49561d / 161280d * Math.Pow(dn, 4) - 179d / 168d * Math.Pow(dn, 5);
            dal[5] = 34729d / 80640d * Math.Pow(dn, 5);
            double dSg = 0; double dTu = 0;
            for (int j = 1; j <= 5; j++)
            {
                dSg += 2 * j * dal[j] * Math.Cos(2 * j * dXi) * Math.Cosh(2 * j * dEt);
                dTu += 2 * j * dal[j] * Math.Sin(2 * j * dXi) * Math.Sinh(2 * j * dEt);
            }
            dSg = dSg + 1;

            //A0-A5
            double[] dA = new double[6];
            dA[0] = 1 + Math.Pow(dn, 2) / 4 + Math.Pow(dn, 4) / 64;
            dA[1] = -3d / 2d * (dn - Math.Pow(dn, 3) / 8 - Math.Pow(dn, 5) / 64);
            dA[2] = 15d / 16d * (Math.Pow(dn, 2) - Math.Pow(dn, 4) / 4);
            dA[3] = -35d / 48d * (Math.Pow(dn, 3) - 5d / 16d * Math.Pow(dn, 5));
            dA[4] = 315d / 512d * Math.Pow(dn, 4);
            dA[5] = -693d / 1280d * Math.Pow(dn, 5);
            double dAb = dM0 * daa / (1 + dn) * dA[0];
            double dSb = 0;
            for (int j = 1; j <= 5; j++)
            {
                dSb += dA[j] * Math.Sin(2 * j * Lat0);
            }
            dSb = dM0 * daa / (1 + dn) * (dA[0] * Lat0 + dSb);

            double X = 0; double Z = 0;
            for (int j = 1; j <= 5; j++)
            {
                Z += dal[j] * Math.Sin(2 * j * dXi) * Math.Cosh(2 * j * dEt);
                X += dal[j] * Math.Cos(2 * j * dXi) * Math.Sinh(2 * j * dEt);
            }
            
            return (
                dAb * (dEt + X),
                dAb * (dXi + Z) - dSb
            );
        }

        /// <summary> 
        /// 平面直角座標系 -> 地理座標系（緯度・経度）
        /// </summary>
        /// <param name="X">X座標(東西方向、m)</param>
        /// <param name="Z">Y座標(南北方向、m)</param>
        /// <returns>Lon：緯度[度], Lat：経度[度]</returns>
        public (double Lat, double Lon) XZ2LatLon(double X, double Z)
        {
            double dn = 1d / (2 * dF - 1);

            //ラジアン単位に
            double Lon0 = Deg2Rad(this._lon0);
            double Lat0 = Deg2Rad(this._lat0);

            //Sφ0、A
            double[] dA = new double[6];
            dA[0] = 1 + Math.Pow(dn, 2) / 4 + Math.Pow(dn, 4) / 64;
            dA[1] = -3d / 2d * (dn - Math.Pow(dn, 3) / 8 - Math.Pow(dn, 5) / 64);
            dA[2] = 15d / 16d * (Math.Pow(dn, 2) - Math.Pow(dn, 4) / 4);
            dA[3] = -35d / 48d * (Math.Pow(dn, 3) - 5d / 16d * Math.Pow(dn, 5));
            dA[4] = 315d / 512d * Math.Pow(dn, 4);
            dA[5] = -693d / 1280d * Math.Pow(dn, 5);
            double dAb = dM0 * daa / (1 + dn) * dA[0];
            double dSb = 0;
            for (int j = 1; j <= 5; j++)
            {
                dSb += dA[j] * Math.Sin(2 * j * Lat0);
            }
            dSb = dM0 * daa / (1 + dn) * (dA[0] * Lat0 + dSb);

            //ξ・η
            double dXi = (Z + dSb) / dAb;
            double dEt = X / dAb;

            //β
            double[] dBt = new double[6];
            dBt[1] = 1d / 2d * dn - 2d / 3d * Math.Pow(dn, 2) + 37d / 96d * Math.Pow(dn, 3) - 1d / 360d * Math.Pow(dn, 4) - 81d / 512d * Math.Pow(dn, 5);
            dBt[2] = 1d / 48d * Math.Pow(dn, 2) + 1d / 15d * Math.Pow(dn, 3) - 437d / 1440d * Math.Pow(dn, 4) + 46d / 105d * Math.Pow(dn, 5);
            dBt[3] = 17d / 480d * Math.Pow(dn, 3) - 37d / 840d * Math.Pow(dn, 4) - 209d / 4480d * Math.Pow(dn, 5);
            dBt[4] = 4397d / 161280d * Math.Pow(dn, 4) - 11d / 504d * Math.Pow(dn, 5);
            dBt[5] = 4583d / 161280d * Math.Pow(dn, 5);

            //ξ’・η'・σ'・τ'・χ
            double dXi2 = 0;
            double dEt2 = 0;
            double dSg2 = 0;
            double dTu2 = 0;
            for (int j = 1; j <= 5; j++)
            {
                dXi2 += dBt[j] * Math.Sin(2 * j * dXi) * Math.Cosh(2 * j * dEt);
                dEt2 += dBt[j] * Math.Cos(2 * j * dXi) * Math.Sinh(2 * j * dEt);
                dSg2 += dBt[j] * Math.Cos(2 * j * dXi) * Math.Cosh(2 * j * dEt);
                dTu2 += dBt[j] * Math.Sin(2 * j * dXi) * Math.Sinh(2 * j * dEt);
            }
            dXi2 = dXi - dXi2;
            dEt2 = dEt - dEt2;
            dSg2 = 1 - dSg2;
            double dCi = Math.Asin(Math.Sin(dXi2) / Math.Cosh(dEt2));

            //δ
            double[] dDt = new double[7];
            dDt[1] = 2 * dn - 2d / 3d * Math.Pow(dn, 2) - 2 * Math.Pow(dn, 3) + 116d / 45d * Math.Pow(dn, 4) + 26d / 45d * Math.Pow(dn, 5) - 2854d / 675d * Math.Pow(dn, 6);
            dDt[2] = 7d / 3d * Math.Pow(dn, 2) - 8d / 5d * Math.Pow(dn, 3) - 227d / 45d * Math.Pow(dn, 4) + 2704d / 315d * Math.Pow(dn, 5) + 2323d / 945d * Math.Pow(dn, 6);
            dDt[3] = 56d / 15d * Math.Pow(dn, 3) - 136d / 35d * Math.Pow(dn, 4) - 1262d / 105d * Math.Pow(dn, 5) + 73814d / 2835d * Math.Pow(dn, 6);
            dDt[4] = 4279d / 630d * Math.Pow(dn, 4) - 332d / 35d * Math.Pow(dn, 5) - 399572d / 14175d * Math.Pow(dn, 6);
            dDt[5] = 4174d / 315d * Math.Pow(dn, 5) - 144838d / 6237d * Math.Pow(dn, 6);
            dDt[6] = 601676d / 22275d * Math.Pow(dn, 6);

            //ラジアン単位の緯度経度
            double Lat = dCi;
            double Lon = Lon0 + Math.Atan(Math.Sinh(dEt2) / Math.Cos(dXi2));
            for (int j = 1; j <= 6; j++)
            {
                Lat += dDt[j] * Math.Sin(2 * j * dCi);
            }

            //度単位に
            return (
                Rad2Deg(Lat),
                Rad2Deg(Lon)
            );
        }

        //双曲線正接関数の逆関数
        private static double Atanh(double x) => (1d / 2d * Math.Log((1 + x) / (1 - x), Math.E));
        private static double Deg2Rad(double Deg) => (Math.PI * Deg / 180d);
        private static double Rad2Deg(double Rad) => (180d * Rad / Math.PI);
    }
}
