using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Domain;

namespace TestProject
{
    [TestClass]
    public class UnitUnitTests
    {
        [TestMethod]
        public void ShotStartsCooldown()
        {
            var unit = new Unit(new GameWorld());
            
            unit.Shoot();

            Assert.IsFalse(unit.CanShoot);
        }
        
        [TestMethod]
        public void TickReducesCooldown()
        {
            var unit = new Unit(new GameWorld());
            
            unit.Shoot();
            var maxCooldown = unit.Cooldown;
            unit.Tick();
            
            Assert.IsTrue(maxCooldown > unit.Cooldown);
        }
        
        [TestMethod]
        public void CanShotAfterCooldown()
        {
            var unit = new Unit(new GameWorld());
            
            unit.Shoot();
            while (unit.Cooldown > 0)
            {
                unit.Tick();
            }
            Assert.IsTrue(unit.CanShoot);
        }
        
        [TestMethod]
        public void ShotConsumesClip()
        {
            var unit = new Unit(new GameWorld());
            
            var initialClip = unit.Clip;
            unit.Shoot();

            Assert.IsTrue(unit.Clip == initialClip - 1);
        }
        
        [TestMethod]
        public void CannotShootOverClipSize()
        {
            var unit = new Unit(new GameWorld());

            var initialClip = unit.Clip;
            
            for (int i = 0; i < initialClip + 1; i++)
            {
                unit.Shoot();
                while (unit.Cooldown > 0)
                {
                    unit.Tick();
                }
            }
            unit.Shoot();
            
            Assert.IsTrue(unit.Clip == 0);
            Assert.IsFalse(unit.CanShoot);
        }
        
        [TestMethod]
        public void CanReceiveDamage()
        {
            var unit = new Unit(new GameWorld());

            var initialHp = unit.Hp;
            
            unit.ReceiveDamage();
            
            Assert.IsTrue(unit.Hp == initialHp - 1);
        }
        
        [TestMethod]
        public void DieIfDamagedOverHp()
        {
            var unit = new Unit(new GameWorld());

            var initialHp = unit.Hp;
            
            for (int i = 0; i < initialHp; i++)
            {
                unit.ReceiveDamage();
            }
            
            Assert.IsTrue(unit.IsDead);
        }
    }
}