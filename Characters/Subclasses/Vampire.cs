using System;
using System.Collections.Generic;

namespace CorrectionDMYnov
{
    class Vampire : Undead
    {
        int ReceivedDamageSinceLastPowerUse = 0;

        public Vampire(string name) : base(name, 125, 125, 2, 50, 150, 0.2f) { }

        public override void Reset()
        {
            base.Reset();
            ReceivedDamageSinceLastPowerUse = 0;
        }

        public override void UsePower()
        {
            List<Character> characters = new List<Character>();
            characters.AddRange(fightManager.CharactersList);
            characters.Remove(this);
            Character victim = characters[random.Next(0, characters.Count)];
            MyLog(Name + " transfert sa douleur à " + victim.Name + " qui est ralenti de " + (ReceivedDamageSinceLastPowerUse / 1000f) + " secondes.");
            victim.DamageDelays.Add(ReceivedDamageSinceLastPowerUse);
            ReceivedDamageSinceLastPowerUse = 0;
        }

        public override void TakeDamages(int _damages)
        {
            base.TakeDamages(_damages);
            ReceivedDamageSinceLastPowerUse += _damages;
        }

        protected override bool MakeAnAttack(Character target)
        {
            target.IsWounded += DrinkBlood;
            bool result = base.MakeAnAttack(target);
            target.IsWounded -= DrinkBlood;
            return result;
        }

        void DrinkBlood(Object sender, WoundEventArgs e)
        {
            CurrentLife += RoundToInt(e.Wound / 2f);
            CurrentLife = Math.Min(CurrentLife, MaximumLife);
            MyLog(Name + " boit le sang de " + e.Name + " et récupère " + (int)(e.Wound / 2f) + " points de vie.");
        }


    }
}
