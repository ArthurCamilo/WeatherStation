using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.entities;

namespace WeatherStation
{
    public class DatabaseConnection
    {
        private static SQLiteConnection sqliteConnection;

        public static void SetupDatabase()
        {
            //CreateDatabase();
            CreateTemperatureTable();
            CreateHumidityTable();
            CreateDayMeasuresTable();
            //AddDefaultCurrentMeasurement();
        }

        private static SQLiteConnection DbConnection()
        {
            sqliteConnection = new SQLiteConnection("Data Source=C:\\Users\\Arthur Camilo\\Documents\\UDESC\\Redes\\database.sqlite; Version=3;");
            sqliteConnection.Open();
            return sqliteConnection;
        }

        private static void CreateDatabase()
        {
            try
            {
                SQLiteConnection.CreateFile(@"C:\Users\Arthur Camilo\Documents\UDESC\Redes\database.sqlite");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CreateTemperatureTable()
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Temperatures(ConsumedDate INT, TemperatureValue float)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CreateHumidityTable()
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Humidities(ConsumedDate INT, HumidityValue float)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CreateDayMeasuresTable()
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS DayMeasures(Date VARCHAR(255), Humidity float, Temperature float, AvgHumidity float, AvgTemperature float)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetTemperatures()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "SELECT DateTime(ConsumedDate, 'unixepoch') as ConsumedDate, TemperatureValue FROM Temperatures";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetHumidities()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "SELECT DateTime(ConsumedDate, 'unixepoch') as ConsumedDate, HumidityValue FROM Humidities";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetHumiditiesByDate(string date)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT DateTime(ConsumedDate, 'unixepoch') as ConsumedDate, HumidityValue FROM Humidities where Date(consumedDate, 'unixepoch') = '{date}'";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetTemperaturesByDate(string date)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT Date(consumedDate, 'unixepoch'), TemperatureValue FROM Temperatures where Date(consumedDate, 'unixepoch') = '{date}'";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AddTemperature(float temperature)
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Temperatures(ConsumedDate, TemperatureValue) values (@consumedDate, @temperature)";
                    cmd.Parameters.AddWithValue("@consumedDate", DateTimeOffset.Now.ToUnixTimeSeconds());
                    cmd.Parameters.AddWithValue("@temperature", temperature);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AddHumidity(float humidity)
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Humidities(ConsumedDate, HumidityValue) values (@consumedDate, @humidity)";
                    cmd.Parameters.AddWithValue("@consumedDate", DateTimeOffset.Now.ToUnixTimeSeconds());
                    cmd.Parameters.AddWithValue("@humidity", humidity);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AddCurrentDayTemperatureMeasures(float temperature, float avgHumidity, float avgTemperature, string date)
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO DayMeasures(date, temperature, humidity, avgHumidity, avgTemperature) values (@date, @temperature, 0, @avgHumidity, @avgTemperature)";
                    cmd.Parameters.AddWithValue("@temperature", temperature);
                    cmd.Parameters.AddWithValue("@avgHumidity", avgHumidity);
                    cmd.Parameters.AddWithValue("@avgTemperature", avgTemperature);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AddCurrentDayHumidityMeasures(float humidity, float avgHumidity, float avgTemperature, string date)
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO DayMeasures(date, humidity, temperature, avgHumidity, avgTemperature) values (@date, @humidity, 0, @avgHumidity, @avgTemperature)";
                    cmd.Parameters.AddWithValue("@humidity", humidity);
                    cmd.Parameters.AddWithValue("@avgHumidity", avgHumidity);
                    cmd.Parameters.AddWithValue("@avgTemperature", avgTemperature);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteTemperature(string consumedDate)
        {
            try
            {
                using (var cmd = new SQLiteCommand(DbConnection()))
                {
                    cmd.CommandText = $"DELETE FROM Temperatures Where Datetime(consumedDate, 'unixepoch')='{consumedDate}'";
                    cmd.Parameters.AddWithValue("@consumedDate", consumedDate);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteHumidity(string consumedDate)
        {
            try
            {
                using (var cmd = new SQLiteCommand(DbConnection()))
                {
                    cmd.CommandText = $"DELETE FROM Humidities Where Datetime(consumedDate, 'unixepoch')='{consumedDate}'";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateDayMeasuresTemperature(float temperature, float avgHumidity, float avgTemperature, string date)
        {
            try
            {
                using (var cmd = new SQLiteCommand(DbConnection()))
                {
                    cmd.CommandText = "UPDATE DayMeasures SET temperature=@temperature, avgHumidity=@avgHumidity, avgTemperature=@avgTemperature WHERE date=@date";
                    cmd.Parameters.AddWithValue("@temperature", temperature);
                    cmd.Parameters.AddWithValue("@avgHumidity", avgHumidity);
                    cmd.Parameters.AddWithValue("@avgTemperature", avgTemperature);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.ExecuteNonQuery();
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateDayMeasuresHumidity(float humidity, float avgHumidity, float avgTemperature, string date)
        {
            try
            {
                using (var cmd = new SQLiteCommand(DbConnection()))
                {
                    cmd.CommandText = "UPDATE DayMeasures SET humidity=@humidity, avgHumidity=@avgHumidity, avgTemperature=@avgTemperature WHERE date=@date";
                    cmd.Parameters.AddWithValue("@humidity", humidity);
                    cmd.Parameters.AddWithValue("@avgHumidity", avgHumidity);
                    cmd.Parameters.AddWithValue("@avgTemperature", avgTemperature);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.ExecuteNonQuery();
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCurrentMeasures(string date)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT date, Humidity, Temperature, AvgHumidity, AvgTemperature FROM DayMeasures Where date='{date}'";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
