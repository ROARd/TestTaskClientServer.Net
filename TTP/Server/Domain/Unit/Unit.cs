using System;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;

namespace Server.Domain
{
    public class Unit : Aggregate<Guid>
    {
        public PlayerController Owner;
        
        private GameWorld World;

        public double Cooldown { get; private set; }
        public int Clip { get; private set; } = GameWorldConfig.clipSize;
        public int Hp { get; private set; } = GameWorldConfig.health;

        public bool IsDead => Hp <= 0;
        public bool CanShoot => Cooldown <= 0 && Clip > 0;

        public delegate void UnitDelegate(Guid id);

        public event UnitDelegate UnitDied;

        public Unit(GameWorld world) : base(new UnitCreated(world, world.CurrentTick))
        { }
        
        public void Tick()
        {
            ReceiveEvent(new UnitTickEvent(Id, GetNextVersion(), DateTime.Now, World.CurrentTick));
        }

        public void Shoot()
        {
            ReceiveEvent(new UnitShootEvent(Id, GetNextVersion(), DateTime.Now, World.CurrentTick));
        }
        
        public void ReceiveDamage()
        {
            ReceiveEvent(new UnitDamagedEvent(Id, GetNextVersion(), DateTime.Now, World.CurrentTick));
        }

        protected override void ApplyEvent(IAggregateEvent<Guid> e)
        {
            if (e is UnitCreated unitCreated)
            {
                World = unitCreated.World;
                World.MulticastEvent(e);
            }
            else if (e is UnitShootEvent)
            {
                if (CanShoot)
                {
                    Cooldown = GameWorldConfig.shotCooldown;
                    Clip -= 1;
                    World.MulticastEvent(e);
                }
            }
            else if (e is UnitDamagedEvent)
            {
                Hp -= 1;
                if (IsDead)
                {
                    ReceiveEvent(new UnitDiedEvent(Id, GetNextVersion(), DateTime.Now, World.CurrentTick));
                }
                World.MulticastEvent(e);
            }
            else if (e is UnitDiedEvent)
            {
                World.MulticastEvent(e);
                UnitDied?.Invoke(Id);
            }
            else if (e is UnitTickEvent)
            {
                if (Cooldown > 0)
                {
                    Cooldown -= ServerMain.CycleInterval;
                }
            }
        }
    }
    
    public class PlayerController
    {
        public readonly int peerId;
        
        public Unit ControlledUnit;
        public GameWorld World;

        private NetCommunication netComm; // should not be the whole service but netChannel or something like

        public PlayerController(int peerId, GameWorld world)
        {
            this.peerId = peerId;
            World = world;
            netComm = (NetCommunication)ServerMain.ServiceProvider.GetService(typeof(NetCommunication));
        }

        public void Possess(Unit unit)
        {
            ControlledUnit = unit;
            unit.Owner = this;
        }

        public void Message(object obj)
        {
            netComm.MessageBack(this.peerId, obj);
        }
    }
}