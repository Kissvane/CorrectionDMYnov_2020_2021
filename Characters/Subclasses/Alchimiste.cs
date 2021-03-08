using System.Collections.Generic;
using System.Linq;

namespace CorrectionDMYnov
{
    class Alchimiste : LivingCreature, IPoisoner
    {
        public float PoisonDamagePercent { get; set; }
        public float DamagePercent { get; set; }

        public Alchimiste(string name) : base(name, 50, 50, 1f, 30, 150, 0.1f, true)
        {
            PoisonDamagePercent = 1f;
            DamagePercent = 0.5f;
        }

        public override void UsePower()
        {
            List<Character> targets = new List<Character>();
            targets.AddRange(fightManager.CharactersList);
            targets.OrderByDescending(character => character.CurrentLife);
            Character target = targets[0];
            if (target.CurrentLife > CurrentLife)
            {
                int temp = target.CurrentLife;
                target.CurrentLife =  System.Math.Min(CurrentLife, target.MaximumLife);
                CurrentLife = System.Math.Min(temp, MaximumLife) ;
                MyLog(Name+" echange sa vie avec "+target.Name);
            }
        }

        //selectionner une cible valide
        public override void SelectTargetAndAttack()
        {
            //on prend un personngae au hasard dans la liste des cibles valides et on le designe comme la cible de l'attaque 
            List<Character> targets = new List<Character>();

            for (int i = 0; i < fightManager.CharactersList.Count; i++)
            {
                Character currentCharacter = fightManager.CharactersList[i];
                int random = currentCharacter.random.Next(0, 2);
                //si le personnage testé n'est pas celui qui attaque et qu'il est vivant
                //on ne peut se cacher de cette attaque
                //si ce personnage a été tiré au sort
                if (currentCharacter != this && currentCharacter.alive && random == 1)
                {
                    //on l'ajoute à la liste des cible valide
                    targets.Add(currentCharacter);
                }
            }

            if (targets.Count > 0)
            {
                int AttackValue = Attack + RollDice();
                foreach (Character target in targets)
                {
                    MakeAnAttack(target, AttackValue);
                }
            }
            else
            {
                MyLog(Name + " n'a pas trouvé de cible valide");
            }
        }

        protected bool MakeAnAttack(Character target, int AttackValue)
        {
            MyLog(Name + " attaque " + target.Name + ".");
            return !target.Defend(AttackValue, Damages, this);
        }


    }
}
