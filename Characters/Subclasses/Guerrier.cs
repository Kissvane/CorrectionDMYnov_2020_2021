using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Guerrier : LivingCreature
    {
        readonly int baseAttack = 0;

        public Guerrier(string name) : base (name, 150,105, 2.2f,150,250, 0.2f)
        {
            baseAttack = Attack;
        }

        public override void Reset()
        {
            base.Reset();
            Attack = baseAttack;
        }

        public override void UsePower()
        {
            Task.Run(() => Power(), StopToken);
        }

        public async Task Power()
        {
            AttackSpeed = baseAttack + 0.5f;
            MyLog(Name+" deviens plus rapide.");
            await Task.Delay(RoundToInt(3000/Program.TimeAccelerationFactor), StopToken);
            MyLog(Name + " ralentis.");
            AttackSpeed = baseAttack;
        }
    }
}
