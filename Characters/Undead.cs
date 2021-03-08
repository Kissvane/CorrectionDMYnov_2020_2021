using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Undead : Character
    {
        public Undead(string name, int attack = 100, int defense = 100, float attackSpeed = 2f, int damages = 50, int maximumLife = 100, float powerSpeed = 0.5f) : base(name, attack, defense, attackSpeed, damages, maximumLife, powerSpeed) { }

        public override bool Defend(int _attackValue, int _damage, Character _attacker)
        {
            if (_attacker.alive)
            {
                //On calcule la marge d'attaque
                //en soustrayant le jet de defense du personnage qui defend au jet d'attaque reçu
                int AttaqueMargin = _attackValue - (Defense + RollDice());
                //Si la marge d'attaque est supérieure à 0
                if (AttaqueMargin > 0)
                {
                    //MyLog(Name + " se defend mais encaisse quand même le coup.");
                    //on calcule les dégâts finaux
                    int finalDamages = (int)(AttaqueMargin * _damage / 100f);
                    if (_attacker.HolyDamages)
                    {
                        finalDamages *= 2;
                    }
                    TakeDamages(finalDamages);
                    return false;
                }
                else
                {
                    //annoncer dans la console que le personnage a reussi sa defense
                    MyLog(Name + " réussi sa défense.");
                    return true;
                }
            }
            return false;
        }
    }
}
