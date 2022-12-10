using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.entities
{
    internal class DayMeasure
    {
        public string Date { get; set; }
        public float Humidity { get; set; }
        public float Temperature { get; set; }
        public float AvgHumidity { get; set; }
        public float AvgTemperature { get; set; }
    }
}
