using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWEB2010Project1
{
    class Program
    {
        static void Main(string[] args)
        {
            Coach userCoach;
            List<DraftClass> draftPlayerList;
            bool shouldContinue;
            bool removePlayer;
            bool pickAgain = true;
            StartProgram();
            do
            {
                shouldContinue = true;
                userCoach = new Coach();
                draftPlayerList = new List<DraftClass>();
                CreateDraftPlayerList(ref draftPlayerList);
                while (shouldContinue)
                {
                    CreateTable(draftPlayerList, userCoach);
                    removePlayer = ShouldReleasePlayer(userCoach, draftPlayerList);
                    if (!removePlayer)
                    {
                        break;
                    }
                    
                    GetCoachesPick(draftPlayerList, userCoach);
                    if (userCoach.PlayersChoosen.Count >= 5)
                    {
                        break;
                    }
                    shouldContinue = DraftAgain(userCoach.SalaryCapRemaining);
                }
                CreateTable(draftPlayerList, userCoach);
                CheckCostEffective(userCoach);
                pickAgain = DraftAgain();
            } while (pickAgain);
            EndProgram();
            Console.ReadKey(true);
        }
        static void EndProgram() // ends program, thanks user
        {
            Console.Clear();
            int hour = DateTime.Now.Hour;
            Console.WriteLine("Hopefully you gained a valuable insight on the upcoming draft");
            Console.Write("Thank you using this program, have a good ");
            Console.Write(hour < 16 ? "day" : "night");
        }
        static void StartProgram() // sets window size, and introduces program
        {
            Console.SetWindowPosition(0, 0);
            Console.WindowWidth = 185;
            Console.BufferWidth = 185;
            Console.WindowHeight = 48;
            Console.WriteLine("Welcome to our NFL draft simulation program");
            Console.WriteLine("This program assumes a salary cap of $95 million");
            Console.WriteLine("Press any key to get started");
            Console.ReadKey(true);
        }
        static bool DraftAgain() // asks user if they want to run through draft process again
        {
            bool pickAgain;
            string tempAnswer;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteCenteredText("ENJOY YOUR DRAFT CHOICES", Console.BufferWidth);
            Console.ResetColor();
            Console.WriteLine("Would you like to run through the draft again? (Y/N)");
            tempAnswer = Console.ReadLine();
            pickAgain = ValidBool(tempAnswer);
            return pickAgain;
        }
        static bool ShouldReleasePlayer(Coach userCoach, List<DraftClass> draftPlayerList)
        {
            bool removePlayer = true;
            string tempAnswer;
            if (userCoach.SalaryCapRemaining < draftPlayerList.Min(x => x.Salary)) // checks that coach has enough money to draft the cheapeast player
            {
                Console.WriteLine("You do not have enough money to draft another player");
                Console.WriteLine("Do you want to release a player?");
                Console.WriteLine("Enter 'Y' to release a player, or 'N' to quit the draft");
                tempAnswer = Console.ReadLine();
                removePlayer = ValidBool(tempAnswer);
                if (removePlayer)
                {
                    ReleasePlayer(userCoach); // displays list for coach to release a player, removes selected player from list
                }
            }
            return removePlayer;
        }
        static void WriteCoachStats(Coach userCoach) //Used at end of CreateTable method
        {
            int playersDrafted = userCoach.PlayersChoosen.Count;
            userCoach.PlayersChoosen.Sort();
            userCoach.PlayersChoosen.Reverse(); // sort and reverse to order coach selections by salardy descending
            if (playersDrafted > 0)
            {
                Console.WriteLine("Current players and positions drafted");
                for (int i = 0; i < playersDrafted; i++)
                {
                    WriteCenteredText(userCoach.PlayersChoosen[i].Position + ":" + userCoach.PlayersChoosen[i].Name);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Selected players salary: {0:C} | Salary Remaining: {1:C}", userCoach.PlayersChoosen.Sum(x => x.Salary), userCoach.SalaryCapRemaining);
            Console.WriteLine();
        }
        static bool ValidBool(string tempAnswer) //Used by other methods to check if user entered a 'Y' or 'N', Y = true
        {
            bool shouldContinue = false;
            tempAnswer = tempAnswer.ToLower();
            while (tempAnswer != "y" && tempAnswer != "n")
            {
                Console.WriteLine("Please enter either 'Y' or 'N' to continue");
                tempAnswer = Console.ReadLine().ToLower();
            }
            if (tempAnswer == "y")
            {
                shouldContinue = true;
            }
            return shouldContinue;
        }
        static bool DraftAgain(int salaryRemaining) //Asks and returns value if user wants to draft another player
        {
            bool shouldContinue = false;
            string tempAnswer;
            Console.WriteLine("You have {0:C} remaining.", salaryRemaining);
            Console.WriteLine("Do you want to make another pick? (Y/N)");
            tempAnswer = Console.ReadLine();
            shouldContinue = ValidBool(tempAnswer);
            return shouldContinue;
        }
        static void ReleasePlayer(Coach userCoach) //Allows the coach to release a player
        {
            string tempAnswer;
            int playerNumber;
            int longestName = userCoach.PlayersChoosen.Max(x => x.Name.Length);
            Console.WriteLine("Select the player from the list to release");
            for (int i = 0; i < userCoach.PlayersChoosen.Count; i++)
            {
                Console.WriteLine("{0}: {1} Salary: {2}", i+1, userCoach.PlayersChoosen[i].Name.PadRight(longestName), userCoach.PlayersChoosen[i].Salary.ToString("C"));
            }
            tempAnswer = Console.ReadLine();
            while (!Int32.TryParse(tempAnswer, out playerNumber) || playerNumber > userCoach.PlayersChoosen.Count || playerNumber < 1)
            {
                Console.WriteLine("Select a number from the list above to release a player");
                tempAnswer = Console.ReadLine();
            }
            --playerNumber;
            userCoach.RemovePlayer(playerNumber);
        }
        static void CheckCostEffective(Coach userCoach) //Checks if coaches selected players salary is under limit, and has drafted 3 of any top 3 players
        {
            const int COST_EFFECTIVE_THRESHOLD = 65000000;
            List<DraftClass> highValueDrafts;
            if (userCoach.PlayersChoosen.Count(x => x.DraftPosition <= 3) >= 3) //checks if coach drafted three high value targets
            {
                highValueDrafts = new List<DraftClass>();
                foreach (var player in userCoach.PlayersChoosen) //Creates new list of drafted players
                {
                    if (player.DraftPosition <= 3)
                    {
                        highValueDrafts.Add(player);
                    }
                }
                highValueDrafts.Sort(); //sorts the list by ascending salary
                if (highValueDrafts.Take(3).Sum(x => x.Salary) < COST_EFFECTIVE_THRESHOLD) // checks the first three (lowest) values to be cost effective
                {
                    Console.WriteLine("Cost Effective!!");
                    Console.WriteLine();
                }
            }
        }
        static string GetDraftPosition(string[] allPositions) //Displays all distinct positions, and returns user position choice for GetCoachesPick
        {
            string tempAnswer;
            int positionPick;
            Console.WriteLine("To make a draft pick, Select a number below for the position you want to draft");
            for (int i = 0; i < allPositions.Length; i++)
            {
                Console.WriteLine("{0}: {1}", i + 1, allPositions[i]);
            }
            tempAnswer = Console.ReadLine();
            while (!Int32.TryParse(tempAnswer, out positionPick) || positionPick > allPositions.Length || positionPick < 1)
            {
                Console.WriteLine("Please make a selection from the numbers above");
                tempAnswer = Console.ReadLine();
            }
            Console.WriteLine();
            --positionPick;
            return allPositions[positionPick];
        }
        static void GetCoachesPick(List<DraftClass> draftPlayerList, Coach userCoach) //Adds users pick to coaches list of players choosen
        {
            string tempAnswer;
            int playerPick;
            string positionPick;
            bool pickAgain;
            List<DraftClass> playersLeftInPosition;
            var positions = draftPlayerList.Select(x => x.Position).Distinct(); //Captures distinct player positions
            string[] allPositions = positions.ToArray();
            do
            {
                pickAgain = false;
                playersLeftInPosition = new List<DraftClass>();
                positionPick = GetDraftPosition(allPositions); //Gets coaches choice of specific position
                Console.WriteLine("The available players from the {0} position are:", positionPick);
                for (int i = 0; i < draftPlayerList.Count; i++)
                {
                    if (draftPlayerList[i].Position == positionPick && draftPlayerList[i].IsDrafted == false) // selects only players who are in selected positin and have not yet been drafted
                    {
                        playersLeftInPosition.Add(draftPlayerList[i]); // this list is used to add a specific player to coach list
                        Console.WriteLine("{0}: {1}", playersLeftInPosition.Count, draftPlayerList[i].Name);
                    }
                }
                Console.WriteLine("Select the number next to the player you want to draft");
                tempAnswer = Console.ReadLine();
                while (!Int32.TryParse(tempAnswer, out playerPick) || playerPick > playersLeftInPosition.Count || playerPick < 1)
                {
                    Console.WriteLine("Enter the number next to the player you want to draft");
                    tempAnswer = Console.ReadLine();
                }
                Console.WriteLine();
                --playerPick;
                if (userCoach.SalaryCapRemaining - playersLeftInPosition[playerPick].Salary < 0) //checks that coach has enough money to afford player
                {
                    pickAgain = true;
                    Console.WriteLine("Your selection, is too expensive.");
                    Console.WriteLine("Press any key to select another player");
                    Console.ReadKey(true);
                    Console.Clear();
                    CreateTableHead();
                    CreateTable(draftPlayerList, userCoach);
                    WriteCoachStats(userCoach);
                }
            } while (pickAgain);
            playersLeftInPosition[playerPick].IsDrafted = true; //changes selected player to being drafted
            userCoach.AddPlayers(playersLeftInPosition[playerPick]); //adds selected player to coaches list of players
        }
        static void CreateTableHead() // Creates headings for CreateTable method
        {
            const int columnWidth = 180 / 6;
            string[] headings = { "Position", "The Best", "2nd Best", "3rd Best", "4th Best", "5th Best" };
            for (int i = 0; i < headings.Length; i++)
            {
                WriteCenteredText(headings[i], columnWidth);
            }
            Console.WriteLine();
            WriteSolidLine();
        }
        static void CreateTable(List<DraftClass> draftPlayers, Coach userCoach)
        {
            const int PLAYERS_PER_POSITION = 5;
            const int columnWidth = 180 / 6;
            //string currentPosition = draftPlayers[0].Position;
            Console.Clear();
            CreateTableHead();
            for (int i = 0; i < draftPlayers.Count; i++)
            {
                if (i % 5 == 0)
                {
                    
                    WriteCenteredText(draftPlayers[i].Position, columnWidth);
                }
                ColorDraftChoice(draftPlayers[i].IsDrafted);
                WriteCenteredText(draftPlayers[i].Name, columnWidth);
                RepositionCursor(columnWidth * (i % PLAYERS_PER_POSITION + 1), 1);
                WriteCenteredText(draftPlayers[i].CollegeName, columnWidth);
                RepositionCursor(columnWidth * (i % PLAYERS_PER_POSITION + 1), 1);
                WriteCenteredText(draftPlayers[i].Salary.ToString("C"), columnWidth);
                if (i % 5 != 4)
                {
                    RepositionCursor(columnWidth * (i % PLAYERS_PER_POSITION + 2), -2);
                }
                else
                {
                    Console.ResetColor();
                    Console.WriteLine();
                    WriteSolidLine();
                }
            }
            WriteCoachStats(userCoach);
        }
        static void ColorDraftChoice(bool isDrafted) //Used in CreateTable to color player blocks based on IsDrafted property
        {
            if (!isDrafted)
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            Console.ForegroundColor = ConsoleColor.Black;
        }
        static void RepositionCursor(int columnWidth, int rowDirection) //Used in CreateTable to change cursor lines
        {
            Console.CursorTop = Console.CursorTop + rowDirection;
            Console.CursorLeft = columnWidth;
        }
        static void WriteSolidLine() //Create one solid line to represent a row change in CreateTable method
        {
            Console.WriteLine(" " + new String('_', Console.BufferWidth - 2));
        }
        static void WriteCenteredText(string text, int columnWidth = 30) //Centers text parameter, default 6 cols at 180 buffer size = colWidth : 30
        {
            int halfTextLength = text.Length / 2;
            int padLeft = columnWidth / 2 + halfTextLength;
            Console.Write(text.PadLeft(padLeft).PadRight(columnWidth));
        }
        static void CreateDraftPlayerList(ref List<DraftClass> draftPlayerList) //used to instantiate a list of all draft players
        {
            DraftClass draftPlayer;
            for (int i = 0; i < DraftClass.DraftClass2018.GetLength(0); i++)
            {
                draftPlayer = new DraftClass(i)
                {
                    Position = DraftClass.DraftClass2018[i, 0],
                    Name = DraftClass.DraftClass2018[i, 1],
                    CollegeName = DraftClass.DraftClass2018[i, 2],
                    Salary = Int32.Parse(DraftClass.DraftClass2018[i, 3])
                };
                draftPlayerList.Add(draftPlayer);
            }
        }
    }
    class DraftClass : IComparable
    {
        public static readonly string[] arrayIdentifier =
        {
            "Position", "Name", "Institution", "Salary"
        };
        public static readonly string[,] DraftClass2018 =
        {
            {"Quarterback", "Mason Rudolph", "Oklahoma State", "26400100" },
            {"Quarterback", "Lamar Jackson", "Louisville", "20300100" },
            {"Quarterback", "Josh Rosen", "UCLA", "17420300" },
            {"Quarterback", "Sam Darnold", "Southern California", "13100145" },
            {"Quarterback", "Baker Mayfield", "Oklahoma", "10300000" },
            {"Running Back", "Saquon Barkley", "Penn State", "24500100" },
            {"Running Back", "Derrius Guice", "LSU", "19890200" },
            {"Running Back", "Bryce Love", "Stanford", "18700800" },
            {"Running Back", "Ronald Jones II", "Southern California", "15000000" },
            {"Running Back", "Damien Harris", "Alabama", "11600400" },
            {"Wide-Receiver", "Courtland Sutton", "Southern Methodist", "23400000" },
            {"Wide-Receiver", "James Washington", "Oklahoma State", "21900300" },
            {"Wide-Receiver", "Marcell Ateman", "Oklahoma State", "19300230" },
            {"Wide-Receiver", "Anthony Miller", "Memphis", "13400230" },
            {"Wide-Receiver", "Calvin Ridley", "Alabama", "10000000" },
            {"Defensive Lineman", "Maurice Hurst", "Michigan", "26200300" },
            {"Defensive Lineman", "Vita Vea", "Washington", "22000000" },
            {"Defensive Lineman", "Taven Bryan", "Florida", "16000000" },
            {"Defensive Lineman", "Da'Ron Payne", "Alabama", "18000000" },
            {"Defensive Lineman", "Harrison Phillips", "Stanford", "13000000" },
            {"Defensive-Back", "Joshua Jackson", "Iowa", "24000000" },
            {"Defensive-Back", "Derwin James", "Florida", "22500249" },
            {"Defensive-Back", "Denzel Ward", "Ohio State", "20000100" },
            {"Defensive-Back", "Minkah Fitzpatrick", "Alabama", "16000200" },
            {"Defensive-Back", "Isaiah Oliver", "Colorado", "11899999" },
            {"Tight End", "Mark Andrews", "Oklahoma", "27800900" },
            {"Tight End", "Dallas Goedert", "So. Dakota State", "21000800" },
            {"Tight End", "Jaylen Samuels", "NC State", "17499233" },
            {"Tight End", "Mike Gesicki", "Penn State", "27900200" },
            {"Tight End", "Troy Fumagalli", "Wisconsin", "14900333" },
            {"Line-backer", "Roquan Smith", "Georgia", "22900300" },
            {"Line-backer", "Tremaine Edmunds", "Virginia Tech", "19000590" },
            {"Line-backer", "Kendall Joseph", "Clemson", "18000222" },
            {"Line-backer", "Dorian O'Daniel", "Clemson", "12999999" },
            {"Line-backer", "Malik Jefferson", "Texas", "10000100" },
            {"Offensive Tackle", "Orlando Brown", "Oklahoma", "23000000" },
            {"Offensive Tackle", "Kolton Miller", "UCLA", "20000000" },
            {"Offensive Tackle", "Chukwuma Okorafor", "Western Michigan", "19400000" },
            {"Offensive Tackle", "Connor Williams", "Texas", "16200700" },
            {"Offensive Tackle", "Mike McGlinchey", "Notre Dame", "15900000" }
        };
        public DraftClass(int position)
        {
            IsDrafted = false;
            DraftPosition = position % 5 + 1;
        }
        public string Position { get; set; }
        public string Name { get; set; }
        public string CollegeName { get; set; }
        public int Salary { get; set; }
        public bool IsDrafted { get; set; }
        public int DraftPosition { get; set; }

        public int CompareTo(object obj)
        {
            int retValue;
            DraftClass temp = (DraftClass)obj;
            if (this.Salary > temp.Salary)
            {
                retValue = 1;
            }
            else if (this.Salary < temp.Salary)
            {
                retValue = -1;
            }
            else
            {
                retValue = 0;
            }
            return retValue;
        }
    }
    class Coach
    {
        public Coach()
        {
            SalaryCapRemaining = 95000000;
        }
        public int SalaryCapRemaining { get; set; }
        private List<DraftClass> playersChoosen = new List<DraftClass>();

        public List<DraftClass> PlayersChoosen
        {
            get { return playersChoosen; }
        }

        public void AddPlayers(DraftClass value)
        {
            playersChoosen.Add(value);
            SalaryCapRemaining -= value.Salary;
        }
        public void RemovePlayer(int value)
        {
            SalaryCapRemaining += playersChoosen[value].Salary;
            playersChoosen[value].IsDrafted = false;
            playersChoosen.RemoveAt(value);
        }
    }
}
