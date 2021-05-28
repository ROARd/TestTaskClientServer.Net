using System;
using System.Collections.Generic;
using Server.Domain;

namespace Server
{
    public class BattleOverseer
    {
        private Dictionary<Guid, Battle> _activeBattles = new ();
        private Dictionary<int, Battle> _battleByPeer = new ();

        private Battle _formingBattle;
        
        public event NetCommunication.NetCommunicationDelegate DisconnectPeer;

        private Battle CreateNewBattle()
        {
            var battle = new Battle();
            _activeBattles.Add(battle.Id, battle);

            battle.BattleFinished += OnBattleFinished;

            return battle;
        }

        private void OnBattleFinished(Guid id, GameWorld.GameResult result)
        {
            var battle = _activeBattles[id];
            _activeBattles.Remove(id);
            battle.BattleFinished -= OnBattleFinished;
            
            _battleByPeer.Remove(battle.peers.firstPeerId);
            _battleByPeer.Remove(battle.peers.secondPeerId);
            
            Console.WriteLine($"Battle finished for peers {battle.peers.firstPeerId.ToString()}, {battle.peers.secondPeerId.ToString()} with {result}");
        }
        
        private Battle TryGetBattle(int peer)
        {
            if(_battleByPeer.TryGetValue(peer, out var battle))
            {
                return battle;
            }
            return null;
        }

        public void Tick()
        {
            foreach (var activeBattle in _activeBattles)
            {
                activeBattle.Value.Tick();
            }
        }
        
        public void OnClientConnected(int peer)
        {
            if (_formingBattle == null || _formingBattle.State != Battle.BattleState.Waiting)
            {
                _formingBattle = CreateNewBattle();
            }
            
            _battleByPeer.Add(peer, _formingBattle);
            _formingBattle.OnClientConnected(peer);
        }

        public void OnClientDisconnected(int peer)
        {
            TryGetBattle(peer)?.OnClientDisconnected(peer);
            _battleByPeer.Remove(peer);
        }

        public void OnClientMessage(int peer)
        {
            TryGetBattle(peer)?.OnClientMessage(peer);
        }
    }
}