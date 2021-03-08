using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{

    #region events args
    public class DeathEventArgs : EventArgs
    {
        public string Name;

        public DeathEventArgs(string name)
        {
            Name = name;
        }
    }

    public class WoundEventArgs : EventArgs
    {
        public string Name;
        public int Wound;
        public bool Critical;

        public WoundEventArgs(string name, int wound, bool critical)
        {
            Name = name;
            Wound = wound;
            Critical = critical;
        }
    }

    public struct WinStats
    {
        public string ClassName;
        public int points;
    }
    #endregion

    public class Character
    {
        #region variables

        public string Name;
        protected int Attack;
        protected int Defense;
        public float AttackSpeed;
        protected int Damages;
        public int MaximumLife;
        public int CurrentLife;
        protected float PowerSpeed;
        protected int AttackDelay = 0;
        protected int PowerDelay = 0;
        public List<int> DamageDelays = new List<int>();
        public Random random;
        protected FightManager fightManager;
        protected int consoleColor;
        public CancellationTokenSource StopTokenSource;
        public CancellationToken StopToken;
        public bool HolyDamages = false;
        public bool alive = true;
        protected Object _lock = new Object();

        #endregion

        #region events

        #region Death
        public event EventHandler<DeathEventArgs> IsDead;

        protected virtual void OnCharacterDead(DeathEventArgs e)
        {
            if (IsDead != null)
            {
                IsDead.Invoke(this, e);
            }
        }
        #endregion

        #region Hurt
        public event EventHandler<WoundEventArgs> IsWounded;

        protected virtual void OnCharacterWounded(WoundEventArgs e)
        {
            if (IsWounded != null)
            {
                IsWounded.Invoke(this, e);
            }
        }
        #endregion

        #endregion

        #region Initialization
        public Character(string name, int attack = 100, int defense = 100, float attackSpeed = 2f, int damages = 50, int maximumLife = 100, float powerSpeed = 0.5f, bool holyDamages = false)
        {
            Name = name;
            Attack = attack;
            Defense = defense;
            AttackSpeed = attackSpeed;
            Damages = damages;
            MaximumLife = maximumLife;
            PowerSpeed = powerSpeed;
            random = new Random(NameToInt() + (int)DateTime.Now.Ticks);
            CalculateAttackDelay();
            CalculatePowerDelay();
            consoleColor = random.Next(1, 15);
            HolyDamages = holyDamages;
            StopTokenSource = new CancellationTokenSource();
            StopToken = StopTokenSource.Token;
        }

        public void SetFightManager(FightManager fightManager)
        {
            this.fightManager = fightManager;
        }

        public virtual void CalculateAttackDelay()
        {
            AttackDelay = (int)(1000 / AttackSpeed - RollDice());
            //MyLog(Name + "delai de d'attaque de " + AttackDelay);
        }

        public void CalculatePowerDelay()
        {
            PowerDelay = (int)(1000 / PowerSpeed - RollDice());
            //MyLog(Name + "delai de d'utilisation du pouvoir de " + AttackDelay);
        }
        #endregion

        #region Tools
        public virtual void Reset()
        {
            CurrentLife = MaximumLife;
            DamageDelays.Clear();
            IsDead = null;
            IsWounded = null;
        }

        public virtual int RollDice()
        {
            return random.Next(1, 101);
        }

        public int NameToInt()
        {
            int result = 0;
            int index = 0;
            foreach (char c in Name)
            {
                index++;
                result += c * index;
            }
            return result;
        }

        public void MyLog(string text)
        {
            text += " " + DateTime.Now.TimeOfDay.ToString();
            fightManager.MyLog(text, (ConsoleColor)consoleColor);
        }

        public int RoundToInt(float value)
        {
            return (int)Math.Round((double)(value));
        }

        public int CountTrueCharacters()
        {
            List<Character> TrueCharacters = new List<Character>();
            TrueCharacters.AddRange(fightManager.CharactersList);

            //enlever les illusions avant de déterminer si on peut se camoufler ou pas.
            TrueCharacters.RemoveAll(c =>
            {
                Illusion i = c as Illusion;
                return i != null;
            });

            return TrueCharacters.Count;
        }

        public void TestVictory()
        {
            if (alive && CountTrueCharacters() == 1)
            {
                //stop attacks and powers
                StopTokenSource.Cancel();
            }
        }

        /*void string[] CharacterUI()
        {

        }   */
        #endregion

        #region Routines

        public async Task CharacterRoutines()
        {
            Task attacks = Task.Run(() => AttackRoutine(), StopToken);
            Task powers = Task.Run(() => PowerRoutine(), StopToken);

            List<Task> tasks = new List<Task> { attacks, powers };

            try
            {
                await Task.WhenAll(tasks);
            }
            //recuperer l'exception du à l'interruption des delay
            catch { }
            finally
            {
                StopTokenSource.Dispose();
            }
        }

        public async Task AttackRoutine()
        {
            while (alive && CountTrueCharacters() > 1)
            {
                //wait normal attack delay
                await Task.Delay(RoundToInt((AttackDelay-RollDice())/Program.TimeAccelerationFactor),StopToken);
                //wait for attack delay causing by received damages
                while (DamageDelays.Count > 0 && alive && CountTrueCharacters() > 1)
                {
                    await Task.Delay(RoundToInt(DamageDelays[0]/ Program.TimeAccelerationFactor), StopToken);
                    DamageDelays.RemoveAt(0);
                }

                if (alive)
                {
                    SelectTargetAndAttack();
                    TestVictory();
                }
            }

        }

        public async Task PowerRoutine()
        {
            while (alive && CountTrueCharacters() > 1)
            {
                await Task.Delay(RoundToInt((PowerDelay-RollDice())/Program.TimeAccelerationFactor), StopToken);
                if (alive)
                {
                    UsePower();
                    TestVictory();
                }
            }
        }

        #endregion

        #region Defense

        public virtual void TakeDamages(int _damages)
        {
            lock (_lock)
            {
                MyLog(Name + " subis " + _damages + " points de dégats.");
                if (alive)
                {
                    CurrentLife -= _damages;
                    WoundEventArgs args = new WoundEventArgs(Name, _damages, _damages > CurrentLife);
                    OnCharacterWounded(args);
                    DamageDelays.Add(_damages);

                    if (CurrentLife <= 0 && alive)
                    {
                        Death();
                    }
                }
            }
        }

        public virtual void Death()
        {
            alive = false;
            StopTokenSource.Cancel();
            MyLog(Name + " est mort. " + CurrentLife);
            DeathEventArgs args = new DeathEventArgs(Name);
            OnCharacterDead(args);
            IsDead = null;
            IsWounded = null;
            fightManager.CharactersList.Remove(this);
            fightManager.Bodies.Add(this);
        }

        public virtual bool Defend(int _attackValue, int _damage, Character _attacker)
        {
            if (_attacker.alive)
            {
                //On calcule la marge d'attaque
                //en soustrayant le jet de defense du personnage qui defend au jet d'attaque reçu
                int AttaqueMargin = _attackValue - (Defense + RollDice());
                //Si la marge d'attaque est supérieure à 0
                if (AttaqueMargin > 0)
                {
                    IPoisoner poisoner = _attacker as IPoisoner;
                    IPoisonable poisonable = this as IPoisonable;

                    //MyLog(Name + " se defend mais encaisse quand même le coup.");
                    //on calcule les dégâts finaux
                    int finalDamages = RoundToInt(AttaqueMargin * _damage / 100f);
                    if (poisoner != null && poisonable != null)
                    {
                        TakeDamages(RoundToInt(finalDamages * poisoner.DamagePercent));
                        poisonable.IncreasePoisonLevel(RoundToInt(finalDamages * poisoner.PoisonDamagePercent));
                    }
                    else
                    {
                        TakeDamages(finalDamages);
                    }
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

        #endregion

        #region Attack and Power

        public virtual void UsePower()
        {
            MyLog(Name + " has a useless power");
        }

        protected virtual bool MakeAnAttack(Character target)
        {
            MyLog(Name + " attaque " + target.Name + ".");
            return !target.Defend(Attack + RollDice(), Damages, this);
        }

        public virtual Character SelectTarget()
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
                if (currentCharacter.alive)
                {
                    //on l'ajoute à la liste des cible valide
                    validTarget.Add(currentCharacter);
                }
            }

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

        //selectionner une cible valide
        public virtual void SelectTargetAndAttack()
        {

            //on prend un personnage au hasard dans la liste des cibles valides et on le designe comme la cible de l'attaque 
            Character target = SelectTarget();

            if (target != null)
            {
                MakeAnAttack(target);
            }
            else
            {
                MyLog(Name + " n'a pas trouvé de cible valide");
            }
        }

        #endregion
    }
}
