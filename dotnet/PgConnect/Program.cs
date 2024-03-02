using Npgsql;
using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        string userName = Environment.GetEnvironmentVariable("DB_USER");
        string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        string logFilePath = Environment.GetEnvironmentVariable("LOG_FILE_PATH");
        string intervalString = Environment.GetEnvironmentVariable("INTERVAL_MINUTES");
        int intervalMinutes = string.IsNullOrEmpty(intervalString) ? 5 : int.Parse(intervalString);

        while (true) 
        {
            try
            {
                using (var sr = new StreamReader("sdl.conf"))
                {
                    var connectionString = sr.ReadLine();
                    var csb = new NpgsqlConnectionStringBuilder(connectionString);
                    csb.Username = userName;
                    csb.Password = password;

                    using (var connection = new NpgsqlConnection(csb.ConnectionString))
                    {
                        connection.Open();
                        Console.WriteLine($"Успешное подключение к базе данных в {DateTime.Now}");

                        using (var cmd = new NpgsqlCommand("SELECT VERSION();", connection))
                        {
                            var version = cmd.ExecuteScalar();
                            Console.WriteLine($"Версия базы данных: {version}");
                            using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                                {
                                     logWriter.WriteLine($"Успешное подключение к базе данных в {DateTime.Now}");
                                     logWriter.WriteLine($"Версия базы данных: {version}");
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при подключении к базе данных в {DateTime.Now}: {ex.Message}");
                using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                                {
                                     logWriter.WriteLine($"Ошибка при подключении к базе данных в {DateTime.Now}: {ex.Message}");
                                }
            }

            Thread.Sleep(TimeSpan.FromMinutes(intervalMinutes)); 
        }
    }
}