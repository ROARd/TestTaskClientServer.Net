using System;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager client = new NetManager(listener);
            client.Start();
            var server = client.Connect("localhost", 9050, "SomeConnectionKey");
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                Console.WriteLine($"Server: {dataReader.GetString()}");
                dataReader.Recycle();
            };

            listener.PeerDisconnectedEvent += (peer, info) =>
            {
                Console.WriteLine($"Disconnected");
            };

            Task.Run(() =>
            {
                while (true)
                {
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put("1");
                    server.Send(writer, DeliveryMethod.ReliableOrdered);
                    
                    Thread.Sleep(1000);
                }
                
                /*
                 string input = string.Empty;
                 while (true)
                {
                    input = Console.ReadLine();
                    if (String.IsNullOrEmpty(input) == false)
                    {
                        NetDataWriter writer = new NetDataWriter();
                        writer.Put($"{input}");

                        server.Send(writer, DeliveryMethod.ReliableOrdered);
                    }
                    
                    Thread.Sleep(15);
                }*/
            });

            while (true)
            {
                client.PollEvents();
                Thread.Sleep(15);
            }
        }
    }
}