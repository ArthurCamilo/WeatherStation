using MQTTnet;
using MQTTnet.Server;
using System.Reflection;
using System.Windows.Forms;
using WeatherStation.entities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WeatherStation
{
    public partial class Form1 : Form
    {
        // simple one-way, one-time binding 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (4 * 500); // 0.5 sec
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            updateData();
        }

        private void updateData()
        {
            var temperaturesDt = DatabaseConnection.GetTemperatures();
            List<Temperature> temperatures = DataTypeConverter.ToTemperatureList(temperaturesDt);
            listBox1.DataSource = temperatures;

            var humiditiesDt = DatabaseConnection.GetHumidities();
            List<Humidity> humitities = DataTypeConverter.ToHumidityList(humiditiesDt);
            listBox2.DataSource = humitities;

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            var currentMeasureDt = DatabaseConnection.GetCurrentMeasures(today);
            if (currentMeasureDt.Rows.Count != 0)
            {
                var currentMeasure = DataTypeConverter.ConvertToDayMeasure(currentMeasureDt.Rows[currentMeasureDt.Rows.Count - 1]);
                richTextBox1.Text = $"Temperatura Atual: {currentMeasure.Temperature};\nUmidade Atual: {currentMeasure.Humidity};\nMédia de temperatura do dia: {currentMeasure.AvgTemperature};\nMédia de umidade do dia: {currentMeasure.AvgHumidity}";
            }
            else
            {
                richTextBox1.Text = "Ainda sem dados...";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();

            var messageA = new MqttApplicationMessageBuilder()
                .WithTopic(Program.TemperatureTopic)
                .WithPayload("26,8")
                .Build();

            var messageB = new MqttApplicationMessageBuilder()
                .WithTopic(Program.HumidityTopic)
                .WithPayload("27,5")
                .Build();

            Program.server.PublishAsync(messageA, messageB);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                var item = listBox1.SelectedItems[i].ToString();
                var dateToDelete = item.Substring(6, 19);
                DatabaseConnection.DeleteHumidity(dateToDelete);
            }
            updateData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.SelectedItems.Count; i++)
            {
                var item = listBox2.SelectedItems[i].ToString();
                var dateToDelete = item.Substring(6, 19);
                DatabaseConnection.DeleteTemperature(dateToDelete);
            }
            updateData();
        }
    }
}