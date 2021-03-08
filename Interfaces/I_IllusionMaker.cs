using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    interface I_IllusionMaker
    {
        List<Illusion> illusions { get; set; }
        void CreateIllusion();
        void IllusionDestroyed(Illusion illusion);
    }
}
