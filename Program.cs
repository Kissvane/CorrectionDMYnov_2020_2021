using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CorrectionDMYnov
{
    class Program
    {
        public static WinStats[] Wincounter = new WinStats[12];
        public static int wantedBattleNumber = 1000000;
        public static int BattleStarted = 0;
        public static int BattleEnded = 0;
        public static CancellationTokenSource cancellation = new CancellationTokenSource();
        public static float TimeAccelerationFactor = 1f;
        static bool timedOut = false;

        
        static async Task Main(string[] args)
        {
            Task battle = BattleRoyale();
            await battle;

            #region statistics about the battle
            //total point per battle 66
            /*InitWincounter();

            Task.Run(() => Cancel());

            Thread watcher = new Thread(Watcher2);
            watcher.Priority = ThreadPriority.Highest;
            watcher.Start();

            Task[] battles = new Task[wantedBattleNumber];
            for (int i = 0; i < wantedBattleNumber; i++)
            {
                Task t = BattleRoyale(false);
                battles[i] = t;
            }

            

            await Task.WhenAll(battles);

            watcher.Abort();
            ShowStats();*/
            #endregion
        }

#region statistics functions
        private static void Cancel()
        {
            Console.WriteLine("Press the any key to cancel current battles...");
            while (!Console.KeyAvailable)
            {

            }
            
            Console.WriteLine("\nkey pressed : battles cancelled.\n");
            cancellation.Cancel();
            timedOut = true;
        }

        static void Watcher2()
        {
            Console.WriteLine("Start watcher");
            while (BattleEnded < wantedBattleNumber && !timedOut)
            {
                Thread.Sleep(2000);
                Console.Clear();
                Console.WriteLine("Battles : "+BattleStarted+"/"+BattleEnded+"/"+wantedBattleNumber);
            }
            Console.WriteLine("End watcher");
            ShowStats();
        }

        static void InitWincounter()
        {
            Wincounter[0].ClassName = "Berseker";
            Wincounter[1].ClassName = "Guerrier";
            Wincounter[2].ClassName = "Zombie";
            Wincounter[3].ClassName = "Paladin";
            Wincounter[4].ClassName = "Robot";
            Wincounter[5].ClassName = "Vampire";
            Wincounter[6].ClassName = "Pretre";
            Wincounter[7].ClassName = "Magicien";
            Wincounter[8].ClassName = "Illusioniste";
            Wincounter[9].ClassName = "Alchimiste";
            Wincounter[10].ClassName = "Assassin";
            Wincounter[11].ClassName = "Necromancien";
        }

        static void ShowStats()
        {
            Wincounter = Wincounter.OrderByDescending(c => c.points).ToArray();
            Console.WriteLine("Nombre de combats terminés : "+BattleEnded);
            foreach (WinStats stat in Wincounter)
            {
                Console.WriteLine(stat.ClassName+" a gagné "+ stat.points + " points.");
            }
        }

#endregion

        static async Task BattleRoyale(bool verbose = true)
        {
            Console.ForegroundColor = ConsoleColor.White;

            FightManager fightManager = new FightManager();

            await Task.Run(() => fightManager.StartCombat(verbose), cancellation.Token);
        }
    }
}
