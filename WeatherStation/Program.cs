using System.Data;
using System.Text;
using System.Net;
using MQTTnet.Server;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Connecting;
using MQTTnet.Client;

namespace WeatherStation
{
    internal static class Program
    {
        static IMqttClient? client;
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

            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId($"Est_Met_{DateTime.Now}")
                .WithTcpServer("broker.hivemq.com", 1883)
                .Build();

            client = factory.CreateMqttClient();
            client.UseConnectedHandler(OnConnectedHandler);

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from MQTT Brokers.");
            });

            await client.ConnectAsync(clientOptions, CancellationToken.None);

            var serverOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1883)
                .WithClientId(ClientId)
                .Build();

            WeatherService.AddHumidity(52.13f);
            WeatherService.AddHumidity(55.13f);
            WeatherService.AddTemperature(28);
            WeatherService.AddTemperature(29);
            WeatherService.AddTemperature(35);
            WeatherService.AddTemperature(45);
        }

        private static void OnConnectedHandler(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("MQTTClient::OnConnectedHandler() - MQTT Connected");
            if (!client.IsConnected)
            {
                Console.WriteLine("MQTTClient::OnConnectedHandler() - MQTT Connected handler received without being connected.");
                return;
            }
            else
            {
                SubscribeToTopics();
            }
        }

        private async static void SubscribeToTopics()
        {
            client.UseApplicationMessageReceivedHandler(ConsumeTopicMessage);

            await client.SubscribeAsync(
                new MqttTopicFilter
                {
                    Topic = TemperatureTopic
                },
                new MqttTopicFilter
                {
                    Topic = HumidityTopic
                }
            );
        }

        private static void ConsumeTopicMessage(MqttApplicationMessageReceivedEventArgs message)
        {
            string topic = message.ApplicationMessage.Topic;

            var payloadText = Encoding.UTF8.GetString(
                    message?.ApplicationMessage?.Payload ?? Array.Empty<byte>());

            var floatPayload = float.Parse(payloadText.Replace(".", ","));

            if (topic == HumidityTopic)
            {
                WeatherService.AddHumidity(floatPayload);
            }
            else if (topic == TemperatureTopic)
            {
                WeatherService.AddTemperature(floatPayload);
            }

            Console.WriteLine($"Received msg: {payloadText}, Topic: {topic}");
        }
    }
}