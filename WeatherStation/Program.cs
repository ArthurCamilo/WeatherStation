using MQTTnet.Client;
using MQTTnet;
using System.Data;
using System.Text;
using MQTTnet.Server;
using MQTTnet.Client.Options;

namespace WeatherStation
{
    internal static class Program
    {
        static IMqttClient? client;
        public static IMqttServer? server;
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

            var serverOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1883)
                .WithClientId(ClientId)
                .Build();

            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(ClientId)
                .WithTcpServer("localhost", 1883)
                .Build();

            server = factory.CreateMqttServer();
            await server.StartAsync(serverOptions);

            client = factory.CreateMqttClient();

            await client.ConnectAsync(clientOptions, CancellationToken.None);
            SubscribeToTopics();

            var messageA = new MqttApplicationMessageBuilder()
                .WithTopic(TemperatureTopic)
                .WithPayload("27.0")
                .Build();

            var messageB = new MqttApplicationMessageBuilder()
                .WithTopic(HumidityTopic)
                .WithPayload("60.5")
                .Build();

            await server.PublishAsync(messageA, messageB);
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
        }

        // this code runs when a message was received
        //void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        //{
        //    string ReceivedMessage = Encoding.UTF8.GetString(e.Message);

        //    Dispatcher.Invoke(delegate {              // we need this construction because the receiving code in the library and the UI with textbox run on different threads
        //        string consummedMessage = ReceivedMessage;
        //        Float messageToFloat = consummedMessage != null ? Float.parse(consummedMessage) : 0.0;
        //        db.AddTemperature(messageToFloat);
        //        db.UpdateCurrentMeasureTemperature(messageToFloat);
        //    });
        //}

    }
}