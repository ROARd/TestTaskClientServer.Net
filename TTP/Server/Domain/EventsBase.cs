using System;
using JKang.EventSourcing.Events;

namespace Server.Event
{
    public abstract class GameAggregateEvent : AggregateEvent<Guid>
    {
        public int Tick;
        public GameAggregateEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int tick) : base(aggregateId, aggregateVersion, timestamp)
        {
            Tick = tick;
        }
    }
    
    public abstract class GameAggregateCreatedEvent : AggregateCreatedEvent<Guid>
    {
        public int Tick;
        public GameAggregateCreatedEvent(Guid aggregateId, DateTime timestamp, int tick) : base(aggregateId, timestamp)
        {
            Tick = tick;
        }
    }
    
    public abstract class ClientEvent : AggregateEvent<Guid>
    {
        public int peerId;

        protected ClientEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int peerId) : base(aggregateId, aggregateVersion, timestamp)
        {
            this.peerId = peerId;
        }
    }
}