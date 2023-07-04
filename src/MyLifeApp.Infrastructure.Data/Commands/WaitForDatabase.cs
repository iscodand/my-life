using System.Net.Sockets;

namespace MyLifeApp.Infrastructure.Data.Commands
{
    public class WaitForDatabase
    {
        public static void Wait()
        {
            string databaseAddress = "mssql-server";
            int databasePort = 1433;
            int timeoutInSeconds = 30;

            var startTime = DateTime.Now;

            while (DateTime.Now < startTime.AddSeconds(timeoutInSeconds))
            {
                Console.WriteLine("Testando");
                try
                {
                    using (var client = new TcpClient())
                    {
                        client.Connect(databaseAddress, databasePort);

                        if (client.Connected)
                        {
                            Console.WriteLine("Database is ready!");
                            return;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"...Waiting 3 seconds before try again...\n\n{ex}");
                    Thread.Sleep(3000);
                }

                Console.WriteLine("...Waiting 3 seconds before try again...");
                System.Threading.Thread.Sleep(3000);
            }

            throw new TimeoutException("Timeout exceeded. Database Problems.");
        }
    }
}