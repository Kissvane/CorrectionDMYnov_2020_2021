using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    interface IPoisoner
    {
        float PoisonDamagePercent { get; set; }
        float DamagePercent { get; set; }
    }
}
