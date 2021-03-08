using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Illusion : Character
    {
        public I_IllusionMaker origin;

        public Illusion(string name, I_IllusionMaker _origin) : base(name)
        {
            origin = _origin;
        }

        public override void TakeDamages(int _damages)
        {
            Death();
        }

        public override void Death()
        {
            MyLog("Une illusion de " + Name + " est détruite.");
            origin.IllusionDestroyed(this);
            alive = false;
            origin = null;
            this.fightManager = null;
        }
    }
}
