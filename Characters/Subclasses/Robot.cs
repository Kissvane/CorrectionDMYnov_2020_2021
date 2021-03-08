using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Robot : Character
    {
        int baseAttack = 0;

        public Robot(string name) : base(name, 25, 100, 1.2f, 50, 275, 0.5f)
        {
            baseAttack = Attack;
        }

        public override void Reset()
        {
            Attack = baseAttack;
            base.Reset();
        }

        public override void UsePower()
        {
            Attack = RoundToInt(Attack * 1.5f);
            MyLog(Name+" Attack go to "+Attack+" "+CurrentLife);
        }

        public override int RollDice()
        {
            return 50;
        }
    }
}
