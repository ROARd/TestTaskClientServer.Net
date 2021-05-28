using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Server.Domain;

namespace Server
{
    public class ServerMain
    {
        public static ServiceProvider ServiceProvider;
        public static double CycleInterval = 500;
        private static readonly TimeSpan _cycleInterval = TimeSpan.FromMilliseconds(CycleInterval);
        private static readonly Stopwatch sw = new ();
        
        static void Main(string[] args)
        {
            var battleOverseer = new BattleOverseer();
            var netCommunication = new NetCommunication();
            
            netCommunication.Init();
            netCommunication.ClientArrived += battleOverseer.OnClientConnected;
            netCommunication.ClientDisconnected += battleOverseer.OnClientDisconnected;
            netCommunication.ClientMessage += battleOverseer.OnClientMessage;
            
            ServiceProvider = new ServiceCollection()
                .AddTextFileStoreForType<Unit>()
                .AddTextFileStoreForType<Battle>()
                .AddTextFileStoreForType<GameWorld>()
                .AddSingleton(netCommunication)
                .BuildServiceProvider();

            while (true)
            {
                sw.Restart();
                
                netCommunication.Do();
                battleOverseer.Tick();
                
                sw.Stop();
                var cycles = sw.Elapsed.Ticks / (float)_cycleInterval.Ticks;
                var sleepInterval = _cycleInterval - _cycleInterval.Multiply(cycles - Math.Floor(cycles));
                Thread.Sleep(sleepInterval);
            }
        }
    }
}
