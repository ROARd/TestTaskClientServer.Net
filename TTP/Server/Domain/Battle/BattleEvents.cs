using System;
using JKang.EventSourcing.Events;
using Server.Event;

namespace Server.Domain
{
    public class ClientShotEvent : ClientEvent
    {
        public ClientShotEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int peerId) : base(aggregateId, aggregateVersion, timestamp, peerId)
        {
        }
    }
    
    public class ClientDisconnectedEvent : ClientEvent
    {
        public ClientDisconnectedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int peerId) : base(aggregateId, aggregateVersion, timestamp, peerId)
        {
        }
    }
    
    public class ClientConnectedEvent : ClientEvent
    {
        public ClientConnectedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, int peerId) : base(aggregateId, aggregateVersion, timestamp, peerId)
        {
        }
    }
    
    public class BattleCreated : AggregateCreatedEvent<Guid>
    {
        public BattleCreated() : base(Guid.NewGuid(), DateTime.Now)
        {
        }
    }

    public class BattleStartedEvent : AggregateEvent<Guid>
    {
        public BattleStartedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp) : base(aggregateId, aggregateVersion, timestamp)
        {
        }
    }
    
    public class BattleFinishedEvent : AggregateEvent<Guid>
    {
        public GameWorld.GameResult Result;
        public BattleFinishedEvent(Guid aggregateId, int aggregateVersion, DateTime timestamp, GameWorld.GameResult result) : base(aggregateId, aggregateVersion, timestamp)
        {
            Result = result;
        }
    }
}