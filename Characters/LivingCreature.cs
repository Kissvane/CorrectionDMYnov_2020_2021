using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class LivingCreature : Character, IPoisonable
    {
        public int PoisonLevel { get; set; }
        public bool poisoned = false;

        public void IncreasePoisonLevel(int damages)
        {
            if (alive)
            {
                MyLog(Name + " est empoisonné.");
                PoisonLevel += damages;
                if (!poisoned)
                {
                    poisoned = true;
                    Task.Run(() => Poison(), StopToken);
                }
            }
        }

        protected async Task Poison()
        {
            while (PoisonLevel > 0 && alive)
            {
                await Task.Delay(RoundToInt(5000/Program.TimeAccelerationFactor), StopToken);
                TakePoisonDamage();
            }
        }

        public void TakePoisonDamage()
        {
            if (alive)
            {
                MyLog(Name + " subis " + PoisonLevel + " points de dégats.");
                CurrentLife -= PoisonLevel;
                Death();
            }
        }

        public LivingCreature(string name, int attack = 100, int defense = 100, float attackSpeed = 2f, int damages = 50, int maximumLife = 100, float powerSpeed = 0.5f, bool holyDamages = false) : base(name, attack, defense, attackSpeed, damages, maximumLife, powerSpeed, holyDamages)
        {
            PoisonLevel = 0;
        }

        public override void Reset()
        {
            base.Reset();
            PoisonLevel = 0;
            poisoned = false;
        }

    }
}
