using System.Collections.Generic;
using System.Linq;

namespace CorrectionDMYnov
{
    class Illusionniste : LivingCreature, I_IllusionMaker
    {
        public List<Illusion> illusions { get; set; }
        int baseAttackValue = 0;

        public Illusionniste(string name) : base(name, 75, 75, 1f, 50, 100, 0.5f)
        {
            illusions = new List<Illusion>();
            baseAttackValue = Attack;
        }

        public override void UsePower()
        {
            CreateIllusion();
        }

        public override void Reset()
        {
            if (illusions.Count > 0)
            {
                DestroyRemainingIllusions();
            }
            Attack = baseAttackValue;
            base.Reset();
        }

        public override int RollDice()
        {
            return random.Next(1, 200);
        }

        public override void Death()
        {
            base.Death();
            DestroyRemainingIllusions();
        }

        public override Character SelectTarget()
        {

            //on cree une liste dans laquelle on stockera les cibles valides
            List<Character> validTarget = new List<Character>();
            for (int i = 0; i < fightManager.CharactersList.Count; i++)
            {
                Character currentCharacter = fightManager.CharactersList[i];
                IStealthy stealthGuy = currentCharacter as IStealthy;

                if (currentCharacter == this || (stealthGuy != null && stealthGuy.Hidden))
                {
                    continue;
                }
                //si le personnage testé n'est pas celui qui attaque et qu'il est vivant et qu'il n'est pas caché
                if (currentCharacter != this && currentCharacter.alive)
                {
                    //on l'ajoute à la liste des cible valide
                    validTarget.Add(currentCharacter);
                }
            }

            //enlever toutes les illusions de ce personnage de la liste des cibles valides
            validTarget.RemoveAll(c => illusions.Contains(c));

            if (validTarget.Count > 0)
            {
                //on prend un personngae au hasard dans la liste des cibles valides et on le designe comme la cible de l'attaque 
                Character target = validTarget[random.Next(0, validTarget.Count)];
                return target;
            }
            else
            {
                MyLog(Name + " n'a pas trouvé de cible valide");
                return null;
            }
        }

        void DestroyRemainingIllusions()
        {
            foreach (Illusion illusion in illusions)
            {
                fightManager.CharactersList.Remove(illusion);
                //break link to allow garbage collection
                illusion.origin = null;
            }
            illusions.Clear();
            MyLog("Les illusions restantes de " + Name + " disparaissent.");
        }

        public void CreateIllusion()
        {
            Illusion illusion = new Illusion(Name + "_Illusion", this);
            illusion.SetFightManager(fightManager);
            illusions.Add(illusion);
            fightManager.CharactersList.Add(illusion);
            MyLog(Name + " crée une illusion.");
            UpdateAttackWithIllusions();
        }

        public void IllusionDestroyed(Illusion illusion)
        {
            fightManager.CharactersList.Remove(illusion);
            illusions.Remove(illusion);
            if (alive)
            {
                UpdateAttackWithIllusions();
            }
        }

        public void UpdateAttackWithIllusions()
        {
            Attack = baseAttackValue + 10 * illusions.Count;
            MyLog("l'attaque de " + Name + " est maintenant de " + Attack);
        }
    }
}
