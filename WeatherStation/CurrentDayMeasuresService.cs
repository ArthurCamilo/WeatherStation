using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.entities;

namespace WeatherStation
{
    internal class CurrentDayMeasuresService
    {

        private static float GetAverageDayHumidities(string date)
        {
            var TodaysHumiditiesDt = DatabaseConnection.GetHumiditiesByDate(date);
            var humidities = DataTypeConverter.ToHumidityList(TodaysHumiditiesDt);
            return humidities.Count == 0 ? 0f : humidities?.Average(a => a.HumidityValue) ?? 0f;
        }

        private static float GetAverageDayTemperatures(string date)
        {
            var TodaysTemperaturesDt = DatabaseConnection.GetTemperaturesByDate(date);
            var temperatures = DataTypeConverter.ToTemperatureList(TodaysTemperaturesDt);
            return temperatures.Count == 0 ? 0f : temperatures?.Average(a => a.TemperatureValue) ?? 0f;
        }

        public static void UpdateCurrentDayMeasureHumidity(float humidity)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            var todaysMeasurement = DatabaseConnection.GetCurrentMeasures(today);
            var avgHumidities = GetAverageDayHumidities(today);
            var avgTemperatures = GetAverageDayTemperatures(today);
            if (todaysMeasurement.Rows.Count != 0)
            {
                DatabaseConnection.UpdateDayMeasuresHumidity(humidity, avgHumidities, avgTemperatures, today);
            }
            else
            {
                DatabaseConnection.AddCurrentDayHumidityMeasures(humidity, avgHumidities, avgTemperatures, today);
            }
        }

        public static void UpdateCurrentDayMeasureTemperature(float temperature)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            var todaysMeasurement = DatabaseConnection.GetCurrentMeasures(today);
            var avgHumidities = GetAverageDayHumidities(today);
            var avgTemperatures = GetAverageDayTemperatures(today);
            if (todaysMeasurement.Rows.Count != 0)
            {
                DatabaseConnection.UpdateDayMeasuresTemperature(temperature, avgHumidities, avgTemperatures, today);
            }
            else
            {
                DatabaseConnection.AddCurrentDayTemperatureMeasures(temperature, avgHumidities, avgTemperatures, today);
            }
        }

    }
}
