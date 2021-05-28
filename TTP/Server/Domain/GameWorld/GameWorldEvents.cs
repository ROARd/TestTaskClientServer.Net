using System;
using Server.Event;

namespace Server.Domain
{
    public class GameWorldCreated : GameAggregateCreatedEvent
    {
        public GameWorldCreated(int tick) : base(Guid.NewGuid(), DateTime.Now, tick)
        {
        }
    }
    
    public class GameWorldStartedEvent : GameAggregateEvent
    {
        public readonly Match Peers;
        public GameWorldStartedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, Match peers, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        {
            Peers = peers;
        }
        
        public override string ToString()
        {
            return $"{nameof(GameWorldStartedEvent)}, tick:{Tick.ToString()}";
        }
    }
    
    public class GameWorldFinishedEvent : GameAggregateEvent
    {
        public readonly GameWorld.GameResult Result;
        public GameWorldFinishedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, GameWorld.GameResult result, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        {
            Result = result;
        }
        
        public override string ToString()
        {
            return $"{nameof(GameWorldFinishedEvent)} result:{Result}, tick:{Tick.ToString()}";
        }
    }
    
    public class GameWorldTickEvent : GameAggregateEvent
    {
        public GameWorldTickEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        {}
    }
    
    public class GameWorldNotifiedUnitDead : GameAggregateEvent
    {
        public readonly Guid DeadGuid;
        public GameWorldNotifiedUnitDead(Guid aggregateId, int aggregateVersion, DateTime timestamp, Guid deadGuid, int tick) : base(aggregateId, aggregateVersion, timestamp, tick)
        {
            DeadGuid = deadGuid;
        }
    }
}