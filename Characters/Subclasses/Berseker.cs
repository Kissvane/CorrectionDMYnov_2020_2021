namespace CorrectionDMYnov
{
    class Berseker : LivingCreature
    {
        int AttackAndDamageBonus = 0;
        float AttackSpeedBonus = 0;
        readonly int lifeDecile = 0;
        readonly float BaseAttackSpeed = 0f;
        readonly int BaseAttack = 0;
        readonly int BaseDamages = 0;

        public Berseker(string name) : base(name, 50, 50, 1.1f, 20, 400, 1)
        {
            lifeDecile = RoundToInt(MaximumLife / 10f);
            BaseAttackSpeed = AttackSpeed;
            BaseAttack = Attack;
            BaseDamages = Damages;
        }

        public override void Reset()
        {
            Attack = BaseAttack;
            Damages = BaseDamages;
            AttackSpeed = BaseAttackSpeed;
            base.Reset();
        }

        public override void UsePower()
        {
            AttackAndDamageBonus = RoundToInt((MaximumLife - CurrentLife) / 2f);
            float temp = MaximumLife;
            int SpeedBoost = 0;
            while (temp > CurrentLife)
            {
                temp -= lifeDecile;
                SpeedBoost++;
            }
            AttackSpeedBonus = SpeedBoost * 0.3f;
            AttackSpeed = BaseAttackSpeed + AttackSpeedBonus;
            Attack = BaseAttack + AttackAndDamageBonus;
            Damages = BaseDamages + AttackAndDamageBonus;
            MyLog(Name + " damages go to " + Damages);
        }

        public override void CalculateAttackDelay()
        {
            AttackDelay = RoundToInt(1000 / (AttackSpeed + AttackSpeedBonus) - RollDice());
        }

        public override void TakeDamages(int _damages)
        {
            lock (_lock)
            {
                MyLog(Name + " subis " + _damages + " points de dégats.");

                if (alive)
                {
                    CurrentLife -= _damages;
                    //bersker passive competence
                    if (_damages > CurrentLife)
                    {
                        DamageDelays.Add(_damages);
                    }

                    if (CurrentLife <= 0)
                    {
                        Death();
                    }
                }
            }
        }
    }
}
