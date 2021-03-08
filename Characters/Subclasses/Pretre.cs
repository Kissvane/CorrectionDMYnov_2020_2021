using System;
using System.Collections.Generic;

namespace CorrectionDMYnov
{
    class Pretre : Character
    {
        readonly int LifeDecile = 0;

        //Holy Damages
        public Pretre(string name) : base(name, 100, 125, 1.5f, 90, 150, 1f, true)
        {
            LifeDecile = RoundToInt(MaximumLife / 10f);
        }

        public override void UsePower()
        {
            CurrentLife += LifeDecile;
            CurrentLife = Math.Min(CurrentLife, MaximumLife);
            MyLog(Name + " se soigne de " + LifeDecile + " points de vie.");
        }

        //selectionner une cible valide
        public override void SelectTargetAndAttack()
        {
            //on cree une liste dans laquelle on stockera les cibles valides (= undead)
            List<Character> validTarget = new List<Character>();
            if (alive)
            {
                //on parcours la liste des personnages vivants
                for (int i = 0; i < fightManager.CharactersList.Count; i++)
                {
                    Character currentCharacter = fightManager.CharactersList[i];
                    IStealthy stealthGuy = currentCharacter as IStealthy;
                    //on passe au personnage suivant quand on se trouve soi même
                    if (currentCharacter == this || (stealthGuy != null && stealthGuy.Hidden))
                    {
                        continue;
                    }

                    Undead undead = fightManager.CharactersList[i] as Undead;
                    //si le personnage testé n'est pas celui qui attaque et qu'il est vivant et qu'il n'est pas caché
                    if (undead != null && currentCharacter.alive)
                    {
                        //on l'ajoute à la liste des cible valide
                        validTarget.Add(currentCharacter);
                    }
                }

                if (validTarget.Count > 0)
                {
                    //on prend un personngae au hasard dans la liste des cibles valides et on le designe comme la cible de l'attaque 
                    Character target = validTarget[random.Next(0, validTarget.Count)];
                    MakeAnAttack(target);
                }
                else
                {
                    base.SelectTargetAndAttack();
                }
            }
        }
    }
}
