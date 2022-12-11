using MQTTnet.Client;
using MQTTnet;
using System.Text;
using MQTTnet.Server;
using MQTTnet.Packets;
using System.Windows.Forms;

namespace WeatherStation
{
    internal static class Program
    {
        static IMqttClient? client;
        //public static IMqttServer? server;
        public static readonly string TemperatureTopic = "Est_Met/Temperatura";
        public static readonly string HumidityTopic = "Est_Met/Umidade";
        static readonly string ClientId = "Est_Met";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            DatabaseConnection.SetupDatabase();
            ConnectToMQTT();

            Application.Run(new Form1());
        }

        private static async void ConnectToMQTT()
        {
            var factory = new MqttFactory();

            //var serverOptions = new MqttServerOptionsBuilder()
            //    .WithDefaultEndpointPort(1883)
            //    .WithClientId(ClientId)
            //    .Build();

            var wifiUser = "VIVO-B251";
            var wifiPassword = "1615006004";

            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(ClientId)
                .WithTcpServer("test.mosquitto.org", 1883)
                .WithCredentials(wifiUser, wifiPassword)
                .WithCleanSession()
                .Build();

            //server = factory.CreateMqttServer();
            //await server.StartAsync(serverOptions);

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(clientOptions)
                .Build();

            //client = factory.CreateMqttClient();

            //_ = client.ConnectAsync(clientOptions, CancellationToken.None);

            client = new MqttFactory().CreateManagedMqttClient();

            await client.StartAsync(managedOptions);

            SubscribeToTopics();
        }

        private async static void SubscribeToTopics()
        {
            client.ApplicationMessageReceivedAsync += ConsumeTopicMessage;

            var listToSubscribe = new List<MqttTopicFilter> {
                new MqttTopicFilterBuilder().WithTopic(HumidityTopic).Build(),
                new MqttTopicFilterBuilder().WithTopic(TemperatureTopic).Build()
            };

            await client.SubscribeAsync(listToSubscribe);
        }

        private static Task ConsumeTopicMessage(MqttApplicationMessageReceivedEventArgs message)
        {
            string topic = message.ApplicationMessage.Topic;

            var payloadText = Encoding.UTF8.GetString(
                    message?.ApplicationMessage?.Payload ?? Array.Empty<byte>());

            var floatPayload = float.Parse(payloadText);

            if (topic == HumidityTopic)
            {
                DatabaseConnection.AddHumidity(floatPayload);
                CurrentDayMeasuresService.UpdateCurrentDayMeasureHumidity(floatPayload);
            }
            else if (topic == TemperatureTopic)
            {
                DatabaseConnection.AddTemperature(floatPayload);
                CurrentDayMeasuresService.UpdateCurrentDayMeasureTemperature(floatPayload);
            }

            Console.WriteLine($"Received msg: {payloadText}, Topic: {topic}");
            return new Task(delegate { Console.WriteLine("Received msg"); });
        }

    }
}