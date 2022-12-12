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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateData();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (5 * 1000); // 5 sec
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
                label4.Text = $"Temperatura Atual: {Math.Round(currentMeasure.Temperature, 2)}º";
                label5.Text = $"Umidade Atual: {Math.Round(currentMeasure.Humidity, 2)}%";
                label6.Text = $"Média de temperatura do dia: {Math.Round(currentMeasure.AvgTemperature, 2)}º";
                label8.Text = $"Média de umidade do dia: {Math.Round(currentMeasure.AvgHumidity, 2)}%";
            }
            else
            {
                label4.Text = $"Temperatura Atual: Carregando...";
                label5.Text = $"Umidade Atual: Carregando...";
                label6.Text = $"Média de temperatura do dia: Carregando...";
                label8.Text = $"Média de umidade do dia: Carregando...";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.SelectedItems.Count; i++)
            {
                var item = listBox2.SelectedItems[i].ToString();
                var dateToDelete = item.Substring(6, 19);
                DatabaseConnection.DeleteHumidity(dateToDelete);
            }
            updateData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                var item = listBox1.SelectedItems[i].ToString();
                var dateToDelete = item.Substring(6, 19);
                DatabaseConnection.DeleteTemperature(dateToDelete);
            }
            updateData();
        }

    }
}