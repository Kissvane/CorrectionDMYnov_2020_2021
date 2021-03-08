using System;

namespace CorrectionDMYnov
{
    class Assassin : LivingCreature, IPoisoner, IStealthy
    {
        public float PoisonDamagePercent { get; set; }
        public float DamagePercent { get; set; }

        #region stealth interface

        public bool Hidden { get; set; }

        public void CantHideAnymore()
        {
            if (Hidden && CountTrueCharacters() < 5)
            {
                Hidden = false;
                MyLog(Name + " ne peut plus se cacher.");
            }
        }

        public void Hide()
        {
            if (!Hidden && CountTrueCharacters() >= 5)
            {
                Hidden = true;
                MyLog(Name + " passe en mode furtif.");
            }
        }

        public void Preparation()
        {
            WaitForDeath();
        }

        public void WaitForDeath()
        {
            if (fightManager != null)
            {
                foreach (Character enemy in fightManager.CharactersList)
                {
                    if (enemy != this)
                    {
                        enemy.IsDead += DeathReaction;
                    }
                }
            }
        }

        public void DeathReaction(object sender, DeathEventArgs e)
        {
            ((Character)sender).IsDead -= DeathReaction;
            CantHideAnymore();
        }

        public void StopWaitingDeath()
        {
            foreach (Character enemy in fightManager.CharactersList)
            {
                if (enemy != this)
                {
                    enemy.IsDead -= DeathReaction;
                }
            }
        }

        #endregion

        public Assassin(string name) : base(name, 150, 100, 1, 100, 185, 0.5f)
        {
            PoisonDamagePercent = 0.1f;
            DamagePercent = 1f;
        }

        public override void Reset()
        {
            base.Reset();
            Hidden = false;
        }

        public override void UsePower()
        {
            Hide();
        }

        public override void Death()
        {
            base.Death();
            StopWaitingDeath();
        }

        public override void TakeDamages(int _damages)
        {
            base.TakeDamages(_damages);
            CantHideAnymore();
        }

        protected override bool MakeAnAttack(Character target)
        {
            target.IsWounded += Execution;
            bool result = base.MakeAnAttack(target);
            target.IsWounded -= Execution;
            CantHideAnymore();
            return result;
        }

        void Execution(Object sender, WoundEventArgs e)
        {
            if (e.Critical)
            {
                ((Character)sender).Death();
            }
        }
    }
}
