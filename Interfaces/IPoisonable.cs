using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    interface IPoisonable
    {
        int PoisonLevel { get; set; }
        void IncreasePoisonLevel(int damages);
        void TakePoisonDamage();
    }
}
