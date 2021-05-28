using System;
using Server.Event;

namespace Server.Domain
{
    public class UnitCreated : GameAggregateCreatedEvent
    {
        public readonly GameWorld World;

        public UnitCreated(GameWorld gameWorld, int tick) : base(Guid.NewGuid(), DateTime.Now, tick)
        {
            World = gameWorld;
        }
        
        public override string ToString()
        {
            return $"{nameof(UnitCreated)} id:{AggregateId.ToString()}, tick:{Tick.ToString()}";
        }
    }
    public class UnitShootEvent : GameAggregateEvent
    {
        public UnitShootEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        { }

        public override string ToString()
        {
            return $"{nameof(UnitShootEvent)} id:{AggregateId.ToString()}, tick:{Tick.ToString()}";
        }
    }
    
    public class UnitDamagedEvent : GameAggregateEvent
    {
        public UnitDamagedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        { }
        public override string ToString()
        {
            return $"{nameof(UnitDamagedEvent)} id:{AggregateId.ToString()}, tick:{Tick.ToString()}";
        }
    }
    
    public class UnitDiedEvent : GameAggregateEvent
    {
        public UnitDiedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        {
        }
        
        public override string ToString()
        {
            return $"{nameof(UnitDiedEvent)} id:{AggregateId.ToString()}, tick:{Tick.ToString()}";
        }
    }
    
    public class UnitTickEvent : GameAggregateEvent
    {
        public UnitTickEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        {
        }
        
        public override string ToString()
        {
            return $"{nameof(UnitTickEvent)} id:{AggregateId.ToString()}, tick:{Tick.ToString()}";
        }
    }
}