using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.entities
{
    internal class Temperature
    {
        public string ConsumedAt { get; set; }
        public float TemperatureValue { get; set; }
        public override string ToString()
        {
            return $"Data: {this.ConsumedAt} - Temperatura: {this.TemperatureValue}";
        }
    }
}
