using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Domain;

namespace TestProject
{
    public class BattleUnitTests
    {
        [TestClass]
        public class UnitUnitTests
        {
            [TestMethod]
            public void BattleAllowsEnqueueAndDequeue()
            {
                var battle = new Battle();

                battle.OnClientConnected(4);
                battle.OnClientDisconnected(4);

                Assert.IsTrue(battle.State == Battle.BattleState.Waiting);
            }
            
            [TestMethod]
            public void BattleRunsWhenTwoPlayersArrived()
            {
                var battle = new Battle();

                battle.OnClientConnected(4);
                battle.OnClientConnected(44);

                Assert.IsTrue(battle.State == Battle.BattleState.Running);
            }
            
            [TestMethod]
            public void BattleStopsWhenPlayerLeave()
            {
                var battle = new Battle();

                battle.OnClientConnected(4);
                battle.OnClientConnected(44);
                battle.OnClientDisconnected(44);

                Assert.IsTrue(battle.State == Battle.BattleState.Complete);
            }
        }
    }
}