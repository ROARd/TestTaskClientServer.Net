using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Server
{
    public class NetCommunication
    {
        private EventBasedNetListener _listener;
        private NetDataWriter _writer;
        private NetManager _server;

        private readonly Dictionary<int, NetPeer> _peerById = new();
        
        public delegate void NetCommunicationDelegate(int peer);
        public event NetCommunicationDelegate ClientArrived;
        public event NetCommunicationDelegate ClientDisconnected;
        public event NetCommunicationDelegate ClientMessage;
        
        public void Init()
        {
            _listener = new EventBasedNetListener();
            _writer = new NetDataWriter();
            _server = new NetManager(_listener);
            
            _server.Start(9050);
            
            _listener.ConnectionRequestEvent += request =>
            {
                if(_server.ConnectedPeersCount < 10)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };
            
            _listener.PeerConnectedEvent += peer =>
            {
                _peerById.Add(peer.Id, peer);
                
                Console.WriteLine($"Connected: {peer.EndPoint}, id: {peer.Id}");
                
                _writer.Put($"I see ya, peer {peer.Id}");
                peer.Send(_writer, DeliveryMethod.ReliableOrdered);
                _writer.Reset();

                ClientArrived?.Invoke(peer.Id);
            };
            _listener.NetworkReceiveEvent += (peer, reader, method) =>
            {
                reader.Recycle();
                ClientMessage?.Invoke(peer.Id);
            };
            _listener.PeerDisconnectedEvent += (peer, info) =>
            {
                Console.WriteLine($"Disconnected: {peer.EndPoint}, id: {peer.Id}");
                
                _peerById.Remove(peer.Id);
                ClientDisconnected?.Invoke(peer.Id);
            };
        }
        
        public void Stop()
        {
            _server.Stop();
        }

        public void Do()
        {
            _server.PollEvents();
        }

        public void MessageBack(int peerId, object message)
        {
            var peer = _peerById[peerId];
            if(peer.ConnectionState != ConnectionState.Connected) return;

            var messageAsStr = (string) message; // must be special message class instead of str
            
            _writer.Put(messageAsStr);
            peer.Send(_writer, DeliveryMethod.ReliableOrdered);
            _writer.Reset();
        }

        public void DisconnectPeer(int peerId)
        {
            _server.DisconnectPeer(_peerById[peerId]);
        }
    }
}