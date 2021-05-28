using System;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Server.Repository;

namespace Server.Domain
{
    public class Match
    {
        public static int InvalidPeerId = -1;

        public int firstPeerId = InvalidPeerId;
        public int secondPeerId = InvalidPeerId;
    }

    public class Battle : Aggregate<Guid>
    {
        public enum BattleState
        {
            Waiting, Running, Complete
        }
        
        private GameWorld _world;
        public BattleState State { get; private set; } = BattleState.Waiting;
        public Match peers = new ();

        public delegate void BattleFinishedDelegate(Guid id, GameWorld.GameResult result);
        public event BattleFinishedDelegate BattleFinished;

        public Battle() : base(new BattleCreated())
        { }

        private void OnBattleFinished(Guid id, GameWorld.GameResult result)
        {
            ReceiveEvent(new BattleFinishedEvent(Id, GetNextVersion(), DateTime.Now, result));
        }

        public void Tick()
        {
            if(State != BattleState.Running) return;
            
            _world.Tick();
        }

        protected override void ApplyEvent(IAggregateEvent<Guid> e)
        {
            if (e is BattleCreated)
            {
                _world = new GameWorld();
            }
            else if (e is ClientShotEvent cse)
            {
                if (State == BattleState.Running)
                {
                    _world.ClientTryShoot(cse.peerId);
                }
            }
            else if (e is ClientConnectedEvent clientConnectedEvent)
            {
                if (peers.firstPeerId == Match.InvalidPeerId)
                {
                    peers.firstPeerId = clientConnectedEvent.peerId;
                }
                else
                {
                    peers.secondPeerId = clientConnectedEvent.peerId;
                    
                    _world.BattleFinished += OnBattleFinished;
                    
                    _world.Start(peers);
                    ReceiveEvent(new BattleStartedEvent(Id, GetNextVersion(), DateTime.Now));
                }
            }
            else if (e is BattleStartedEvent)
            {
                State = BattleState.Running;
            
                Console.WriteLine($"Starting battle for peers {peers.firstPeerId.ToString()}, {peers.secondPeerId.ToString()}");
            }
            else if (e is BattleFinishedEvent battleFinishedEvent)
            {
                State = BattleState.Complete;
                BattleFinished?.Invoke(Id, battleFinishedEvent.Result);

                _world.BattleFinished -= OnBattleFinished;
                
                Flush();
            }
            else if (e is ClientDisconnectedEvent clientDisconnectedEvent)
            {
                if (State == BattleState.Waiting)
                {
                    peers.firstPeerId = Match.InvalidPeerId;
                }
                else if (State == BattleState.Running)
                {
                    _world.PlayerDisconnected(clientDisconnectedEvent.peerId);
                }
            }
        }
        
        public void OnClientConnected(int peer)
        {
            ReceiveEvent(new ClientConnectedEvent(Id, GetNextVersion(), DateTime.Now, peer));
        }

        public void OnClientDisconnected(int peer)
        {
            ReceiveEvent(new ClientDisconnectedEvent(Id, GetNextVersion(), DateTime.Now, peer));
        }

        public void OnClientMessage(int peer)
        {
            ReceiveEvent(new ClientShotEvent(Id, GetNextVersion(), DateTime.Now, peer));
        }

        private void Flush()
        {
            RepositoryUtility.Flush(this);
            _world.Flush();
        }
    }


}