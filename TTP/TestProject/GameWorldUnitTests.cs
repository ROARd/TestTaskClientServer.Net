using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Server.Domain;

namespace TestProject
{
    [TestClass]
    public class GameWorldUnitTests
    {
        [TestMethod]
        public void ClientCannotShotOverCooldown()
        {
            var gameWorld = new GameWorld();
            gameWorld.Start(new Match(){firstPeerId = 3, secondPeerId = 77});

            var possessedUnit = gameWorld.GetPossessedUnit(3);
            var initialClip = possessedUnit.Clip;
            
            gameWorld.ClientTryShoot(3);
            gameWorld.ClientTryShoot(3);
            
            Assert.IsTrue(possessedUnit.Clip == initialClip - 1);
        }
        
        [TestMethod]
        public void OnPlayerDisconnectOpponentWon()
        {
            var gameWorld = new GameWorld();
            gameWorld.Start(new Match{firstPeerId = 3, secondPeerId = 77});
            
            gameWorld.PlayerDisconnected(3);
            
            Assert.IsTrue(gameWorld.Result == GameWorld.GameResult.SecondPlayerWon);
        }
        
        [TestMethod]
        public void TickOverLimitGeneratesDraw()
        {
            var gameWorld = new GameWorld();
            gameWorld.Start(new Match{firstPeerId = 3, secondPeerId = 77});

            for (int i = 0; i < GameWorldConfig.maxBattleTime / ServerMain.CycleInterval + 1000; i++)
            {
                gameWorld.Tick();
            }
            
            Assert.IsTrue(gameWorld.Result == GameWorld.GameResult.Draw);
        }
    }
}