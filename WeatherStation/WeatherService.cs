using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.entities;

namespace WeatherStation
{
    internal class WeatherService
    {

        public static void AddTemperature(float temperature)
        {
            DatabaseConnection.AddTemperature(temperature);
            CurrentDayMeasuresService.UpdateCurrentDayMeasureTemperature(temperature);
        }

        public static void AddHumidity(float humidity)
        {
            DatabaseConnection.AddHumidity(humidity);
            CurrentDayMeasuresService.UpdateCurrentDayMeasureHumidity(humidity);
        }
    }
}
