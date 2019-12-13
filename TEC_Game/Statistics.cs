using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TEC_Game
{
    class Statistics
    {
        private int win, lose;
        public Statistics()
        {
            this.win = 0;
            this.lose = 0; //to make sure value is not empty in case of exception
            try
            {
                string Path = Environment.CurrentDirectory + "\\Stats.txt";
                StreamReader reader = new StreamReader(Path);
                string wins_str = reader.ReadLine();
                string loses_str = reader.ReadLine();
                int wins = Int32.Parse(wins_str);
                int loses = Int32.Parse(loses_str);
                this.win = wins;
                this.lose = loses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void UpdateWin()
        {
            this.win++;
        }
        public void UpdateLose()
        {
            this.lose++;
        }
        public void SetWin(int winNew)
        {
            this.win = winNew;
        }
        public void SetLose(int loseNew)
        {
            this.lose = loseNew;
        }
        public int GetWin()
        {
            return this.win;
        }
        public int GetLose()
        {
            return this.lose;
        }
    }
}
