using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.entities
{
    internal class DataTypeConverter
    {
        public static List<Temperature> ToTemperatureList(DataTable dt)
        {
            return (from DataRow row in dt.Rows
                    select ConvertToTemperature(row)).ToList();
        }

        public static Temperature ConvertToTemperature(DataRow dr)
        {
            Temperature temperature = new Temperature();
            temperature.ConsumedAt = (string)dr[0];
            temperature.TemperatureValue = float.Parse(dr[1].ToString());
            return temperature;
        }

        public static List<Humidity> ToHumidityList(DataTable dt)
        {
            return (from DataRow row in dt.Rows
                    select ConvertToHumidity(row)).ToList();
        }

        public static Humidity ConvertToHumidity(DataRow dr)
        {
            Humidity humidity = new Humidity();
            humidity.ConsumedAt = (string)dr[0];
            humidity.HumidityValue = float.Parse(dr[1].ToString());
            return humidity;
        }

        public static DayMeasure ConvertToDayMeasure(DataRow dr)
        {
            DayMeasure dayMeasure = new DayMeasure();
            dayMeasure.Date = (string)dr[0];
            dayMeasure.Humidity = float.Parse(dr[1].ToString() ?? "");
            dayMeasure.Temperature = float.Parse(dr[2].ToString() ?? "");
            dayMeasure.AvgHumidity = float.Parse(dr[3].ToString() ?? "");
            dayMeasure.AvgTemperature = float.Parse(dr[4].ToString() ?? "");
            return dayMeasure;
        }
    }
}
