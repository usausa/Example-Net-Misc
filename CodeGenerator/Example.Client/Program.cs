namespace Example.Client
{
    using System;

    using Example.Library;
    using Example.Client.Network;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var api = new Builder()
                .UseLogger(ConsoleLogger.Default)
                .For<IHogeApi>();
            api.Add(1, 2);
            api.Sub(3, 4);

            Console.ReadLine();
        }
    }

    public class ConsoleLogger : ILogger
    {
        public static ConsoleLogger Default { get; } = new ConsoleLogger();

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}