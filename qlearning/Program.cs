using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace qlearning
{
    class Program
    {
        /***** değiştirilebilir alan  ****/
        public static List<char> line = new List<char> { 'X', '-', '-', '-', '-', '-', '-', '-', 'P', '-', '-', '-', '-', '-', '-', '-', 'G' };
        public static List<float> Qmatris = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static List<int> Rmatris = new List<int> { -100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 };
        public static float alpha = 0.5f;
        public static int iteration = 5;
        public static int pointGoal = 5;
        public static int speedMS = 5;
        /*********************************/

        public static int initialState = 8;
        public static int actionState = 0;      
        public static int pointCurrent = 0;
        public static float lucky = 0.3f;
        public static string txtPerIterationQmatris="";
        public static List<List<string>> listOfResultIteration = new List<List<string>>();
        static void Main(string[] args)
        {
            Random rnd = new Random((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));

            for (int i = 0; i < iteration;i++)
            {
                List<string> resultIteration = new List<string>();
                //displayLine(line);
                bool result = true;
                int say = 0;
                initialState = 8;
                pointCurrent = 0;
                Console.Clear();
                lucky += 0.2f;
                Console.Write("{0}. oyun başlıyor.",i+1);
                Thread.Sleep(3000);
                while (result)
                {
                    say++;
                    float num = (float)rnd.NextDouble();

                    if (num > lucky)
                    {
                        actionState = chooseActionRandom();
                        Console.Write("rnd-> {0}'dan {1}'a\n", initialState, actionState);
                        if (initialState > actionState) result = shiftLEFT();
                        else result = shiftRIGHT();
                    }
                    else
                    {
                        actionState = chooseActionMax();
                        Console.Write("max-> {0}'dan {1}'a\n", initialState, actionState);
                        if (initialState > actionState) result = shiftLEFT();
                        else result = shiftRIGHT();
                    }

                    qlearning();

                    if (pointCurrent != pointGoal) result = true;
                    if (initialState == 0) { pointCurrent--; initialState = 8; }
                    if (initialState == line.Count - 1) { pointCurrent++; initialState = 8; }

                    if (pointCurrent == pointGoal || pointCurrent == pointGoal * -1) result = false;

                    Console.Write("{0}. puan \n", pointCurrent);

                    displayLine(line);

                    displayQmatris();

                    Thread.Sleep(speedMS);
                    Console.Clear();
                }//while

                //Console.Write("{0}", line.Count);  
                displayQmatris();
                resultIteration.Add(say.ToString());
                resultIteration.Add(txtPerIterationQmatris);

                listOfResultIteration.Add(resultIteration);
   
                Console.WriteLine();               
            }
            Console.Clear();

            lucky = (lucky + 1) / 2;

            //iteration sonu
            displaySkorBoard();
            Console.ReadLine();
        }

        private static void displaySkorBoard()
        {
            int j=0;
            foreach (List<string> i in listOfResultIteration)
            {
                j++;
                List<string> resultIteration = i;
                Console.Write("{0}.oyun {1} adımda sonlandı.\n Durum matrisi: {2}\n", j, resultIteration[0], resultIteration[1]);
            }

        }

        private static void qlearning()
        {
            if(actionState==line.Count-1)
                 Qmatris[initialState] = alpha * (Rmatris[actionState] + Qmatris[actionState - 1]);
            else if(actionState==0)
                 Qmatris[initialState] = alpha * (Rmatris[actionState] + Qmatris[actionState + 1]);
            else if (Qmatris[actionState - 1] < 0 && Qmatris[actionState + 1]==0)
                Qmatris[initialState] = alpha * (Rmatris[actionState] + Qmatris[actionState - 1]); // - olarakda q matrisi dolsun diye
            else 
                Qmatris[initialState] = alpha * (Rmatris[actionState] + Math.Max(Qmatris[actionState - 1], Qmatris[actionState + 1]));
           
        }

        private static int chooseActionMax()
        {
            if (Qmatris[initialState + 1] == Qmatris[initialState - 1])
            {
                Random rnd = new Random((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
                float num = (float)rnd.NextDouble();
                if (num > 0.5) actionState = initialState + 1;
                else actionState = initialState - 1;
            }
            else if (Qmatris[initialState + 1] > Qmatris[initialState - 1])
                actionState = initialState + 1;
            else
                actionState = initialState - 1;
            return actionState;
        }

        private static int chooseActionRandom()
        {
            Random rnd = new Random((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
            float num = (float)rnd.NextDouble();
            if (num > 0.5)
                actionState = initialState + 1;
            else
                actionState = initialState - 1;

            return actionState;
        }

       
        private static bool shiftLEFT()
        {
            if (initialState - 1 >=0)
            {
                for (int i = 0; i < line.Count; ++i)
                {
                    if (i == initialState - 1) line[i] = 'P';
                    else if (i == 0) line[i] = 'X';
                    else if (i == line.Count - 1) line[i] = 'G';
                    else line[i] = '-';
                }
                initialState--;          
            }
            if (initialState == 0) return false;
            return true;
        }
        private static bool shiftRIGHT()
        {
            if (initialState + 1 < line.Count)
            {
                for (int i = 0; i < line.Count; ++i)
                {
                    if (i == initialState + 1) line[i] = 'P';
                    else if (i == 0) line[i] = 'X';
                    else if (i == line.Count - 1) line[i] = 'G';
                    else line[i] = '-';
                }
                initialState++;
            }
            if (initialState == line.Count) return false;
            return true;
        }

        private static void displayLine(List<char> line)
        {       
            //Console.SetCursorPosition(0, 0);
            foreach (char i in line) 
                Console.Write("{0}", i.ToString());         
            Console.WriteLine();
            
        }
        private static void displayQmatris()
        {
            txtPerIterationQmatris = "";
            foreach (float i in Qmatris)
            {
                Console.Write("{0} ", i.ToString());
                txtPerIterationQmatris += " " + String.Format("{0:0.0000}", i.ToString());
            }
            Console.WriteLine();
        }

    }
}
