using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.entities
{
    internal class Humidity
    {
        public string ConsumedAt { get; set; }
        public float HumidityValue { get; set; }

        public override string ToString()
        {
            return $"Data: {this.ConsumedAt} - Umidade: {this.HumidityValue}";
        }
    }
}
