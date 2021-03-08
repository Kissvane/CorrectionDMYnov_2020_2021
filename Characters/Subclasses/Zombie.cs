using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Zombie : Undead
    {
        public Zombie(string name) : base(name, 150, 0, 1, 20, 1500, 2f) { }

        //Zombie power
        public override void UsePower()
        {
            if (fightManager.Bodies.Count > 0 && alive)
            {
                Character eatenBody = fightManager.Bodies[random.Next(0, fightManager.Bodies.Count)];
                CurrentLife += eatenBody.MaximumLife;
                CurrentLife = Math.Min(CurrentLife, MaximumLife);
                MyLog(Name + " mange le corps de " + eatenBody.Name + " et récupère " + eatenBody.MaximumLife + " points de vie.");
                fightManager.Bodies.Remove(eatenBody);
            }
        }

        public override bool Defend(int _attackValue, int _damage, Character _attacker)
        {
            if (_attacker.alive)
            {
                //le zombie de ne se defend pas donc on utilise directement la valeur d'attaque pour calculer les dégâts
                int finalDamages = (int)(_attackValue * _damage / 100f);
                if (_attacker.HolyDamages)
                {
                    finalDamages *= 2;
                }
                TakeDamages(finalDamages);
            }
            return false;
        }

        public override void TakeDamages(int _damages)
        {
            lock (_lock)
            {
                if (alive)
                {
                    MyLog(Name + " subis " + _damages + " points de dégats.");
                    CurrentLife -= _damages;
                    //le zombie ne subit pas de délai d'attaque
                    if (CurrentLife <= 0)
                    {
                        Death();
                    }
                }
            }
        }
    }
}
