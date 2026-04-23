using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance
{
    /// <summary>
    /// 经纬度地址计算
    /// </summary>
    public class GaodeLocation
    {
        public static string key = "4ef5f447b2cd947918fc40f1954f36c0";//API-Key

        /// <summary>
        /// 根据起始经纬度和终点经纬度查询两者之间的距离
        /// </summary>
        /// <param name="startLonLat">起始位置</param>
        /// <param name="endLonLat">终点位置</param>
        /// <returns>两者之间的距离</returns>
        public static double getDistance(string startLonLat, string endLonLat)
        {
            //如果调用的接口次数用完了，则自己计算，自己计算的正确度不高
            try
            {
                var res = CalculateDistance(Convert.ToDouble(startLonLat.Split(',')[0]), Convert.ToDouble(startLonLat.Split(',')[1]), Convert.ToDouble(endLonLat.Split(',')[0]), Convert.ToDouble(endLonLat.Split(',')[1]));
                if (res <= 2000)
                {
                    string queryResult = WebHelperRequest.HttpWebRequest($"https://restapi.amap.com/v3/distance?origins={startLonLat}&destination={endLonLat}&output=json&key={key}&type=0");//type=0直线距离
                    JObject jsonObj = JObject.Parse(queryResult);
                    if (jsonObj["results"] == null || jsonObj["results"][0]["distance"] == null)
                        return res;
                    var distance = (jsonObj["results"][0]["distance"]).ToDouble();
                    return distance;
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 测试计算两地之间的距离
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // 地球半径（单位：km）
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c * 1000; // 转换为米
            return distance;
        }
        public static double ToRadians(double angle)
        {
            return Math.PI * angle / 180;
        }

    }
}
