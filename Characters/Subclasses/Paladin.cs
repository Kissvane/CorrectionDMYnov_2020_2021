using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Paladin : LivingCreature
    {
        //holy damages
        public Paladin(string name) : base(name, 60, 145, 1.6f, 40, 250, 0.5f, true) { }

        public override void UsePower()
        {
            for (int i = 0; i < DamageDelays.Count; i++)
            {
                DamageDelays[i] = 0;
            }
        }
    }
}
