using System;
using System.Collections.Generic;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Server.Repository;

namespace Server.Domain
{
    public class GameWorld : Aggregate<Guid>
    {
        public enum GameResult
        {
            None, FirstPlayerWon, SecondPlayerWon, Draw
        }

        private static readonly Random Random = new ();
        private Match _peers;
        
        private List<PlayerController> _controllers = new ();
        private Dictionary<Guid, Unit> _units = new ();
        
        private double _timeElapsed;
        
        public int CurrentTick { get; private set; }
        public GameResult Result { get; private set; } = GameResult.None;
        
        public event Battle.BattleFinishedDelegate BattleFinished;

        public GameWorld() : base(new GameWorldCreated(0))
        { }

        public void MulticastEvent(IAggregateEvent<Guid> e)
        {
            foreach (var playerController in _controllers)
            {
                playerController.Message(e.ToString());
            }
        }
        
        public void Tick()
        {
            if (_peers == null) return;
            
            ApplyEvent(new GameWorldTickEvent(Id, GetNextVersion(), DateTime.Now, CurrentTick));
        }

        private PlayerController GetControllerByPeer(int peer)
        {
            return _controllers.Find(c => c.peerId == peer);    // Rude fast solution
        }

        private Unit GetOpposeUnit(int peerId)
        {
            return _controllers.Find(c => c.peerId != peerId).ControlledUnit;
        }
        
        public Unit GetPossessedUnit(int peerId)
        {
            return _controllers.Find(c => c.peerId == peerId).ControlledUnit;
        }

        protected override void ApplyEvent(IAggregateEvent<Guid> e)
        {
            if (e is GameWorldCreated)
            {
            }
            else if (e is GameWorldStartedEvent gameWorldStartEvent)
            {
                _peers = gameWorldStartEvent.Peers;
            
                PlayerController controller = null;
                Unit unit = null;

                controller = new PlayerController(_peers.firstPeerId, this);
                _controllers.Add(controller);
                unit = new Unit(this);
                unit.UnitDied += UnitDied;
                _units.Add(unit.Id, unit);
                controller.Possess(unit);
                
                controller = new PlayerController(_peers.secondPeerId, this);
                _controllers.Add(controller);
                unit = new Unit(this);
                unit.UnitDied += UnitDied;
                _units.Add(unit.Id, unit);
                controller.Possess(unit);

                MulticastEvent(e);
            }
            else if(e is GameWorldNotifiedUnitDead unitDiedEvent)
            {
                var unit = _units[unitDiedEvent.DeadGuid];
                ReceiveEvent(new GameWorldFinishedEvent(Id, GetNextVersion(), DateTime.Now,
                    unit.Owner.peerId == _peers.firstPeerId ? GameResult.SecondPlayerWon : GameResult.FirstPlayerWon, CurrentTick));
            }
            else if(e is ClientShotEvent clientShotEvent)
            {
                var attackingUnit = GetControllerByPeer(clientShotEvent.peerId).ControlledUnit;
                var opposeUnit = GetOpposeUnit(attackingUnit.Owner.peerId); // hack due to aiming absence
                
                if (attackingUnit.CanShoot)
                {
                    attackingUnit.Shoot();
                    
                    if (Random.Next(0, 100) * 0.01f <= GameWorldConfig.accuracy)
                    {
                        opposeUnit.ReceiveDamage();
                    }
                }
            }
            else if(e is GameWorldTickEvent)
            {
                CurrentTick += 1;
                _timeElapsed += ServerMain.CycleInterval;
                if (_timeElapsed > GameWorldConfig.maxBattleTime)
                {
                    ReceiveEvent(new GameWorldFinishedEvent(Id, GetNextVersion(), DateTime.Now, GameResult.Draw, CurrentTick));
                }
                else
                {
                    foreach (var pair in _units)
                    {
                        if (pair.Value.IsDead == false)
                        {
                            pair.Value.Tick();
                        }
                    }
                }
            }
            else if(e is GameWorldFinishedEvent gameWorldFinishedEvent)
            {
                Result = gameWorldFinishedEvent.Result;
                MulticastEvent(e);

                BattleFinished?.Invoke(Id, Result);
            }
        }

        private void UnitDied(Guid unitId)
        {
            ReceiveEvent(new GameWorldNotifiedUnitDead(Id, GetNextVersion(), DateTime.Now, unitId, CurrentTick));
        }

        public void ClientTryShoot(int peerId)
        {
            ReceiveEvent(new ClientShotEvent(Id, GetNextVersion(), DateTime.Now, peerId));
        }

        public void Start(Match peers)
        {
            ReceiveEvent(new GameWorldStartedEvent(Id, GetNextVersion(), DateTime.Now, peers, CurrentTick));
        }

        public void PlayerDisconnected(int peerId)
        {
            ReceiveEvent(new GameWorldFinishedEvent(Id, GetNextVersion(), DateTime.Now,
                peerId == _peers.firstPeerId ? GameResult.SecondPlayerWon : GameResult.FirstPlayerWon, CurrentTick));
        }

        public void Flush()
        {
            RepositoryUtility.Flush(this);
            foreach (var pair in _units)
            {
                RepositoryUtility.Flush(pair.Value);
            }
        }
    }
}