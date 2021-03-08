using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    interface IStealthy : IPreparator
    {
        bool Hidden { get; set; }
        void Hide();
        void CantHideAnymore();
        void WaitForDeath();
        void StopWaitingDeath();
        int CountTrueCharacters();
        void DeathReaction(Object sender, DeathEventArgs e);
    }
}
