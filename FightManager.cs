using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    public class FightManager
    {
        public WinStats[] Wincounter = new WinStats[12];
        public List<Character> OriginalCharacterList = new List<Character>();
        public List<Character> CharactersList = new List<Character>();
        public List<Character> Bodies = new List<Character>();
        DateTime StartTime;
        List<string> FightDescriptions = new List<string>();
        List<ConsoleColor> Colors = new List<ConsoleColor>();
        private static Object _lock = new Object();
        bool Verbose = false;
        int actionIndex = 0;
        int DeadCharacterNumber = 0;

        public FightManager()
        {
            OriginalCharacterList = new List<Character>
            {
                new Berseker("Guts"),
                new Guerrier("Musashi"),
                new Zombie("Rageux"),
                new Paladin("Uther"),
                new Robot("R2D2"),
                new Vampire("Dracula"),
                new Pretre("Anduin"),
                new Magicien("Gandalf"),
                new Illusionniste("Majax"),
                new Alchimiste("Edouard"),
                new Assassin("Fitz"),
                new Necromancien("Skelos")
            };

            foreach (Character character in OriginalCharacterList)
            {
                character.SetFightManager(this);
            }
        }

        public async Task StartCombat(bool verbose = true)
        {
            Verbose = verbose;
            DeadCharacterNumber = 0;
            Bodies.Clear();
            CharactersList.AddRange(OriginalCharacterList);

            lock (_lock)
            {
                Program.BattleStarted++;
            }

            if (Verbose)
            {
                Thread watcher = new Thread(UpdateUIThread);
                watcher.Priority = ThreadPriority.Highest;
                watcher.Start();
            }

            StartTime = DateTime.Now;
            MyLog("Le combat commence a "+StartTime);
            List<Task> Behaviours = new List<Task>();
            //faire en sorte que les personnages ne soient pas blessé avant le début du combat
            foreach (Character personnage in CharactersList)
            {
                personnage.Reset();
                personnage.IsDead += DeathReaction;
                Task behaviour = Task.Run(() => personnage.CharacterRoutines(), Program.cancellation.Token);
                Behaviours.Add(behaviour);
            }

            foreach (Character character in CharactersList)
            {
                IPreparator preparator = character as IPreparator;
                if (preparator != null)
                {
                    preparator.Preparation();
                }
            }

            await Task.WhenAll(Behaviours);

            if (CharactersList.Count > 0)
            {
                MyLog(CharactersList[0].Name + " remporte la victoire "+ DateTime.Now.TimeOfDay.ToString());
                int indexof = OriginalCharacterList.IndexOf(CharactersList[0]);
                if (indexof != -1)
                {
                    lock (_lock)
                    {
                        CalculateWinPoint(indexof);
                        SendResults();
                    }
                }
            }
            else
            {
                MyLog("Tout le monde est mort, il n'y a pas de vainqueur ... " + DateTime.Now.TimeOfDay.ToString());
            }

            UpdateUi();
        }

        void DeathReaction(Object sender, DeathEventArgs e)
        {
            Character dead = (Character)sender;
            MyLog("La foule acclame la mort de "+dead.Name +" "+DateTime.Now.TimeOfDay.ToString());
            int indexof = OriginalCharacterList.IndexOf(dead);
            if (indexof != -1)
            {
                lock (_lock)
                {
                    CalculateWinPoint(indexof);
                }
            }
        }

        void CalculateWinPoint(int index)
        {
            Wincounter[index].points += DeadCharacterNumber;
            DeadCharacterNumber++;
        }

        void SendResults()
        {
            for (int i = 0; i < Wincounter.Length; i++)
            {
                Program.Wincounter[i].points += Wincounter[i].points;
            }
            Program.BattleEnded++;
        }

        public void MyLog(string text, ConsoleColor color)
        {
            if (Verbose)
            {
                lock (_lock)
                {
                    FightDescriptions.Add(text);
                    Colors.Add(color);
                }
            }
        }

        public void MyLog(string text)
        {
            if (Verbose)
            {
                lock (_lock)
                {
                    FightDescriptions.Add(text);
                    Colors.Add(ConsoleColor.White);
                }
            }
        }

        public void UpdateUIThread(object callback)
        {
            while (CharactersList.Count > 1)
            {
                UpdateUi();
                Thread.Sleep(200);
            }
        }

        public void UpdateUi()
        {
            //Console.Clear();

            for (int i = actionIndex; i < FightDescriptions.Count; i++)
            {
                actionIndex++;
                Console.ForegroundColor = Colors[i];
                Console.WriteLine(FightDescriptions[i]);
                Console.ForegroundColor = ConsoleColor.White;
            }

            //show UI HERE
        }

        
    }
}
