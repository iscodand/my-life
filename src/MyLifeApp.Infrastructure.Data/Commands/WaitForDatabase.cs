using System.Net.Sockets;

namespace MyLifeApp.Infrastructure.Data.Commands
{
    // TODO => maybe this should not be here
    public class WaitForDatabase
    {
        public static void Wait()
        {
            string databaseAddress = "mssql-server";
            int databasePort = 1433;
            int timeoutInMinutes = 1;

            DateTime startTime = DateTime.Now;

            while (DateTime.Now < startTime.AddMinutes(timeoutInMinutes))
            {
                try
                {
                    Console.WriteLine("...Testing...");
                    using (TcpClient client = new())
                    {
                        client.Connect(databaseAddress, databasePort);

                        if (client.Connected)
                        {
                            Console.WriteLine("Database is ready!");
                            return;
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Database is not ready!\n...Waiting 3 seconds before try again...");
                    Thread.Sleep(3000);
                }
            }

            throw new TimeoutException("Timeout exceeded. Database Problems.");
        }
    }
}