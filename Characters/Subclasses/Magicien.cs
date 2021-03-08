using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Magicien : LivingCreature
    {
        readonly int DamagesDecile = 0;

        public Magicien(string name) : base (name, 75, 125, 1.5f, 100, 125, 0.1f)
        {
            DamagesDecile = RoundToInt(Damages / 10f);
        }

        public override void UsePower()
        {
            //select target
            Character target = SelectTarget();
            //make super attack
            if (target != null)
            {

                int multipliedDamages = Damages * 5;
                int multipliedAttack = Attack * 5;
                int damageReduction = RoundToInt(multipliedDamages / 10f);
                MyLog(Name+" lance son sort ultime sur"+target.Name+".");
                bool successfulAttack = !target.Defend(multipliedAttack + RollDice(), multipliedDamages, this);
                if (successfulAttack)
                {
                    MakeASecondaryAttack(target, multipliedDamages, multipliedAttack, damageReduction);
                }
            }
        }

        protected override bool MakeAnAttack(Character target)
        {
            MyLog(Name + " attaque " + target.Name + ".");
            int AttackValue = Attack + RollDice();

            bool successfulAttack = !target.Defend(AttackValue, Damages, this);
            if (successfulAttack)
            {
                SelectSecondaryTargetAndAttack(AttackValue ,Damages-DamagesDecile, DamagesDecile);
            }
            return successfulAttack;
        }

        void MakeASecondaryAttack(Character target, int damages, int attackValue, int damageReduction)
        {
            MyLog("L'attaque de "+ Name + " continue vers " + target.Name + ".");
            bool successfulAttack = !target.Defend(attackValue, Damages, this);
            if (successfulAttack)
            {
                int currentDamages = damages - damageReduction;
                if (currentDamages > 0)
                {
                    SelectSecondaryTargetAndAttack(attackValue, currentDamages, damageReduction);
                }
            }
        }

        Character SelectSecondaryTarget()
        {
            //on cree une liste dans laquelle on stockera les cibles valides
            List<Character> validTarget = new List<Character>();

            for (int i = 0; i < fightManager.CharactersList.Count; i++)
            {
                Character currentCharacter = fightManager.CharactersList[i];
                //si le personnage testé n'est pas celui qui attaque et qu'il est vivant
                //les cibles cachés sont valides pour une attaque secondaire
                if (currentCharacter != this && currentCharacter.alive)
                {
                    //on l'ajoute à la liste des cible valide
                    validTarget.Add(currentCharacter);
                }
            }

            if (validTarget.Count > 0)
            {
                //on prend un personnage au hasard dans la liste des cibles valides et on le designe comme la cible de l'attaque 
                Character target = validTarget[random.Next(0, validTarget.Count)];
                return target;
            }
            else
            {
                MyLog(Name + " n'a pas trouvé de cible valide");
                return null;
            }
        }

        void SelectSecondaryTargetAndAttack(int attackValue , int damages, int damageReduction)
        {
            //on prend un personnage au hasard dans la liste des cibles valides et on le designe comme la cible de l'attaque 
            Character target = SelectSecondaryTarget();
            if (target != null)
            {
                MakeASecondaryAttack(target, damages, attackValue, damageReduction);
            }
            else
            {
                MyLog(Name + " n'a pas trouvé de cible valide");
            }
        }
    }
}
