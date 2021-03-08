using System;

namespace CorrectionDMYnov
{
    class Necromancien : Undead, IPoisoner, IStealthy
    {
        bool somebodyIsDead = false;
        public float PoisonDamagePercent { get; set; }
        public float DamagePercent { get; set; }
        readonly int baseAttack = 0;
        readonly int baseDefense = 0;
        readonly int baseDamages = 0;
        readonly int baseMaximumLife = 0;

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

        public void DeathReaction(Object sender, DeathEventArgs e)
        {
            somebodyIsDead = true;
            ((Character)sender).IsDead -= DeathReaction;
            PowerBoost();
            CantHideAnymore();
        }

        #endregion

        public Necromancien(string name) : base(name, 0, 10, 1f, 0, 275, 5f)
        {
            PoisonDamagePercent = 0.5f;
            PoisonDamagePercent = 0.5f;
            somebodyIsDead = false;
            baseAttack = Attack;
            baseDefense = Defense;
            baseDamages = Damages;
            baseMaximumLife = MaximumLife;
        }

        public override void Reset()
        {
            Hidden = false;
            somebodyIsDead = false;
            Attack = baseAttack;
            Defense = baseDefense;
            Damages = baseDamages;
            MaximumLife = baseMaximumLife;
            base.Reset();
        }

        public override void Death()
        {
            base.Death();
            StopWaitingDeath();
        }

        public override void UsePower()
        {
            if (!somebodyIsDead)
            {
                Hide();
            }
        }

        public override int RollDice()
        {
            return random.Next(1, 150);
        }

        protected override bool MakeAnAttack(Character target)
        {
            bool attackresult = base.MakeAnAttack(target);
            CantHideAnymore();
            return attackresult;
        }

        public override void TakeDamages(int _damages)
        {
            base.TakeDamages(_damages);
            CantHideAnymore();
        }

        void PowerBoost()
        {
            Attack += 5;
            Defense += 5;
            Damages += 5;
            MaximumLife += 50;
            CurrentLife += 50;
            MyLog(Name + " mange une âme et se renforce.");
        }

    }
}
