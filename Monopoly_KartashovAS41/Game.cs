using System;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monopoly_KartashovAS41
{
    public partial class Game : Form
    {
        // 0 - красный игрок, 1 - синий игрок
        public static TcpClient Client = new TcpClient();
        public static NetworkStream Stream;
        public static string IP, TurnLogString;
        public static bool PassedGo, DiceRoll;
        public static int
            Port, Rent, CurrentPosition, PositionBeforeDicing, PositionAfterDicing,
            FirstDice, SecondDice, Dice, CurrentPlayerId = 0,
            RedDotsNameSplitter = 0, BlueDotsNameSplitter = 0; 
        public static int[] Position = new int[] { 0, 0 };
        public Property[] Properties = new Property[40];
        public Player[] Players = new Player[2];
        public PictureBox[] Tile;
        public class Property
        {
            public int Owner, Value, Rent;
            public string Colour, Name;
            public bool Buyable, Owned = false;
        }
        public class Player
        {
            public int[] PropertiesOwned = new int[40];
            public int Balance = 1500, NumberOfPropertiesOwned = 0, Jail = 0;
            public string PlayerName, PlayerImageName;
            public bool PassGo = false, inJail = false, Loser = false;
        }
        public class ReceivedMessage
        {
            public int[] PropertiesOwned = new int[40];
            public int PlayerId, Position, Balance, Jail;
            public bool InJail, Loser;
        }
        public void CreatePlayer(string playerName, int i, bool inJail)
        {
            Player temp = new Player
            {
                PlayerName = playerName,
                inJail = inJail
            };
            Players[i] = temp;
        }
        public void CreateTile(string Name, bool buyable, string colour, int value, int location)
        {
            Property temp = new Property
            {
                Name = Name,
                Colour = colour,
                Buyable = buyable,
                Value = value
            };
            Properties[location] = temp;
        }
        public void RollDice()
        {
            if (DiceRoll == true)
            {
                Random rand = new Random();
                FirstDice = rand.Next(1, 7);
                SecondDice = rand.Next(1, 7);
                Dice = FirstDice + SecondDice;
                return;
            }
        }
        public string PropertiesToString(int[] propertyList)
        {
            string tempString = "";
            for (int i = 0; i < 40; i++)
            {
                if (propertyList[i] != 0)
                    tempString = tempString + Properties[propertyList[i]].Name + ", " + Properties[propertyList[i]].Colour + "\n";
            }
            return tempString;
        }
        public void UpdatePlayersStatusBoxes()
        {
            redPlayerStatusBox_richtextbox.Text = Players[0].PlayerName + "\n" + Players[0].PlayerImageName + "\n" + Players[0].Balance + "\n" + PropertiesToString(Players[0].PropertiesOwned);
            bluePlayerStatusBox_richtextbox.Text = Players[1].PlayerName + "\n" + Players[1].PlayerImageName + "\n" + Players[1].Balance + "\n" + PropertiesToString(Players[1].PropertiesOwned);
        }
        public void ChangeBalance(Player player, int cashChange)
        {
            player.Balance += cashChange;
            UpdatePlayersStatusBoxes();
        }
        public bool IsOwned(int currentPlayer, int currentPosition)
        {
            if ((Players[currentPlayer].PropertiesOwned[currentPosition] != currentPosition) && (Properties[currentPosition].Owned == true))
                return false;
            else
                return true;
        }
        public void InJail(int currentPlayer)
        {
            Players[currentPlayer].Jail = Players[currentPlayer].Jail + 1;
            buyBtn.Enabled = false;
            throwDiceBtn.Enabled = false;
            currentPlayersTurn_textbox.Text = "Игрок " + Players[CurrentPlayerId].PlayerImageName;
            currentPlayersTurn_textbox.Text += "\r\nВы в тюрьме. На следующий ход вы сможете ходить";
            if (Players[currentPlayer].Jail == 3)
            {
                Players[currentPlayer].inJail = false;
                Players[currentPlayer].Jail = 0;
                throwDiceBtn.Enabled = true;
                currentPlayersTurn_textbox.Text = "Игрок " + Players[CurrentPlayerId].PlayerImageName;
                currentPlayersTurn_textbox.Text += "\r\nВы освобождены!";
            }
        }
        public int GetRent(int Dice)
        {
            switch (Properties[CurrentPosition].Colour)
            {
                case "Null":
                    Properties[CurrentPosition].Rent = 0;
                    break;
                case "Station":
                    Properties[CurrentPosition].Rent = Dice * 20;
                    break;
                case "White":
                    Properties[CurrentPosition].Rent = 0;
                    break;
                case "Brown":
                    Properties[CurrentPosition].Rent = 60;
                    break;
                case "Turquoise":
                    Properties[CurrentPosition].Rent = 120;
                    break;
                case "Purple":
                    Properties[CurrentPosition].Rent = 160;
                    break;
                case "Orange":
                    Properties[CurrentPosition].Rent = 200;
                    break;
                case "Red":
                    Properties[CurrentPosition].Rent = 240;
                    break;
                case "Yellow":
                    Properties[CurrentPosition].Rent = 280;
                    break;
                case "Green":
                    Properties[CurrentPosition].Rent = 320;
                    break;
                case "Blue":
                    Properties[CurrentPosition].Rent = 400;
                    break;
            }
            return Properties[CurrentPosition].Rent;
        }
        public void DrawCircle(int position, string color)
        {
            int x = Tile[position].Location.X;
            int y = Tile[position].Location.Y;
            if (color == "Red")
            {
                PictureBox redMarker = new PictureBox
                {
                    Size = new Size(30, 30),
                    Name = "redMarker" + RedDotsNameSplitter.ToString(),
                    BackgroundImage = redDot_picturebox.BackgroundImage,
                    BackColor = Color.Transparent,
                    Left = x,
                    Top = y
                };
                Controls.Add(redMarker);
                redMarker.BringToFront();
                RedDotsNameSplitter++;
            }
            if (color == "Blue")
            {
                PictureBox blueMarker = new PictureBox
                {
                    Size = new Size(30, 30),
                    Name = "blueMarker" + BlueDotsNameSplitter.ToString(),
                    BackgroundImage = blueDot_picturebox.BackgroundImage,
                    BackColor = Color.Transparent,
                    Left = x,
                    Top = y
                };
                Controls.Add(blueMarker);
                blueMarker.BringToFront();
                BlueDotsNameSplitter++;
            }
        }
        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = Stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable);
                    string message = builder.ToString();
                    if (message.Contains("Результаты хода красного игрока: ") && CurrentPlayerId == 1)
                    {
                        string tempMessage = message;
                        string subString = "Результаты хода красного игрока: ";
                        tempMessage = tempMessage.Replace(subString, "");

                        currentPlayersTurn_textbox.Invoke(
                            (MethodInvoker)delegate ()
                            { currentPlayersTurn_textbox.Text = "Ваш ход"; });
                        throwDiceBtn.Enabled = true;
                        buyBtn.Enabled = false;
                        endTurnBtn.Enabled = true;

                        ReceivedMessage receivedMessage = new ReceivedMessage();

                        string stringPlayerId = tempMessage.Split('~')[0];
                        receivedMessage.PlayerId = Convert.ToInt32(stringPlayerId);

                        string stringPosition = tempMessage.Split('~')[1];
                        receivedMessage.Position = Convert.ToInt32(stringPosition);

                        string stringBalance = tempMessage.Split('~')[2];
                        receivedMessage.Balance = Convert.ToInt32(stringBalance);

                        string stringInJail = tempMessage.Split('~')[3];
                        if (stringInJail == "FALSE")
                            receivedMessage.InJail = false;
                        if (stringInJail == "TRUE")
                            receivedMessage.InJail = true;

                        string stringJail = tempMessage.Split('~')[4];
                        receivedMessage.Jail = Convert.ToInt32(stringJail);

                        string stringPropertiesOwned = tempMessage.Split('~')[5];
                        if (stringPropertiesOwned != "NULL")
                        {
                            int[] tempArrayOfPropertiesOwned = stringPropertiesOwned.Split(' ').
                            Where(x => !string.IsNullOrWhiteSpace(x)).
                            Select(x => int.Parse(x)).ToArray();
                            for (int k = 0; k < tempArrayOfPropertiesOwned.Length; k++)
                                receivedMessage.PropertiesOwned[k] = tempArrayOfPropertiesOwned[k];
                        }

                        string stringLoser = tempMessage.Split('~')[6];
                        if (stringLoser == "FALSE")
                            receivedMessage.Loser = false;
                        if (stringLoser == "TRUE")
                            receivedMessage.Loser = true;

                        if (Players[CurrentPlayerId].inJail == true)
                        {
                            CurrentPosition = 10;
                            MoveIcon(CurrentPosition);
                            Position[CurrentPlayerId] = CurrentPosition;
                            InJail(CurrentPlayerId);
                        }
                        if (Players[CurrentPlayerId].Loser == true || Players[CurrentPlayerId].Balance <= 0)
                            Lose();
                        int count = 0;
                        for (int u = 0; u < 2; u++)
                        {
                            if (Players[u].Loser == true)
                                count++;
                            if (Players[CurrentPlayerId].Loser == false && count >= 1)
                            {
                                currentPlayersTurn_textbox.Text = "Вы победили!";
                                DialogResult dialog = MessageBox.Show("Игрок " + Players[CurrentPlayerId].PlayerImageName + " победил!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                if (dialog == DialogResult.OK)
                                    Application.Exit();
                            }
                        }

                        CurrentPlayerId = 0;
                        MoveIcon(receivedMessage.Position);
                        Position[CurrentPlayerId] = receivedMessage.Position;
                        Players[CurrentPlayerId].Balance = receivedMessage.Balance;
                        Players[CurrentPlayerId].inJail = receivedMessage.InJail;
                        Players[CurrentPlayerId].Jail = receivedMessage.Jail;
                        if (Players[CurrentPlayerId].inJail == true)
                            InJail(CurrentPlayerId);
                        int i = 0;
                        foreach (int item in receivedMessage.PropertiesOwned)
                        {
                            Players[CurrentPlayerId].PropertiesOwned[i] = item;
                            i++;
                        }
                        foreach (int item in Players[CurrentPlayerId].PropertiesOwned)
                            if (item != 0)
                            {
                                Properties[item].Owned = true;
                                Properties[item].Owner = CurrentPlayerId;
                                Players[CurrentPlayerId].NumberOfPropertiesOwned++;
                                currentPlayersTurn_textbox.Invoke((MethodInvoker)delegate () { DrawCircle(item, "Red"); });
                            }
                        Players[CurrentPlayerId].Loser = receivedMessage.Loser;
                        if (Players[CurrentPlayerId].Loser == true || Players[CurrentPlayerId].Balance <= 0)
                            Lose();
                        CurrentPlayerId = 1;
                        UpdatePlayersStatusBoxes();
                    }
                    else if (message.Contains("Результаты хода синего игрока: ") && CurrentPlayerId == 0)
                    {
                        string tempMessage = message;
                        string subString = "Результаты хода синего игрока: ";
                        tempMessage = tempMessage.Replace(subString, "");

                        currentPlayersTurn_textbox.Invoke(
                            (MethodInvoker)delegate ()
                            { currentPlayersTurn_textbox.Text = "Ваш ход"; });
                        throwDiceBtn.Enabled = true;
                        buyBtn.Enabled = false;
                        endTurnBtn.Enabled = true;

                        ReceivedMessage receivedMessage = new ReceivedMessage();

                        string stringPlayerId = tempMessage.Split('~')[0];
                        receivedMessage.PlayerId = Convert.ToInt32(stringPlayerId);

                        string stringPosition = tempMessage.Split('~')[1];
                        receivedMessage.Position = Convert.ToInt32(stringPosition);

                        string stringBalance = tempMessage.Split('~')[2];
                        receivedMessage.Balance = Convert.ToInt32(stringBalance);

                        string stringInJail = tempMessage.Split('~')[3];
                        if (stringInJail == "FALSE")
                            receivedMessage.InJail = false;
                        if (stringInJail == "TRUE")
                            receivedMessage.InJail = true;

                        string stringJail = tempMessage.Split('~')[4];
                        receivedMessage.Jail = Convert.ToInt32(stringJail);

                        string stringPropertiesOwned = tempMessage.Split('~')[5];
                        if (stringPropertiesOwned != "NULL")
                        {
                            int[] tempArrayOfPropertiesOwned = stringPropertiesOwned.Split(' ').
                            Where(x => !string.IsNullOrWhiteSpace(x)).
                            Select(x => int.Parse(x)).ToArray();
                            for (int p = 0; p < tempArrayOfPropertiesOwned.Length; p++)
                                receivedMessage.PropertiesOwned[p] = tempArrayOfPropertiesOwned[p];
                        }

                        string stringLoser = tempMessage.Split('~')[6];
                        if (stringLoser == "FALSE")
                            receivedMessage.Loser = false;
                        if (stringLoser == "TRUE")
                            receivedMessage.Loser = true;

                        if (Players[CurrentPlayerId].inJail == true)
                        {
                            CurrentPosition = 10;
                            MoveIcon(CurrentPosition);
                            Position[CurrentPlayerId] = CurrentPosition;
                            InJail(CurrentPlayerId);
                        }
                        if (Players[CurrentPlayerId].Loser == true || Players[CurrentPlayerId].Balance <= 0)
                            Lose();
                        int count = 0;
                        for (int u = 0; u < 2; u++)
                        {
                            if (Players[u].Loser == true)
                                count++;
                            if (Players[CurrentPlayerId].Loser == false && count >= 1)
                            {
                                currentPlayersTurn_textbox.Text = "Вы победили!";
                                DialogResult dialog = MessageBox.Show("Игрок " + Players[CurrentPlayerId].PlayerImageName + " победил!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                if (dialog == DialogResult.OK)
                                    Application.Exit();
                            }
                        }


                        CurrentPlayerId = 1;
                        //_ = MoveTileByTile(Position[CurrentPlayerId], receivedMessage.Position);
                        MoveIcon(receivedMessage.Position);
                        Position[CurrentPlayerId] = receivedMessage.Position;
                        Players[CurrentPlayerId].Balance = receivedMessage.Balance;
                        Players[CurrentPlayerId].inJail = receivedMessage.InJail;
                        if (Players[CurrentPlayerId].inJail == true)
                            InJail(CurrentPlayerId);
                        Players[CurrentPlayerId].Jail = receivedMessage.Jail;
                        int i = 0;
                        foreach (int item in receivedMessage.PropertiesOwned)
                        {
                            Players[CurrentPlayerId].PropertiesOwned[i] = item;
                            i++;
                        }
                        foreach (int item in Players[CurrentPlayerId].PropertiesOwned)
                            if (item != 0)
                            {
                                Properties[item].Owned = true;
                                Properties[item].Owner = CurrentPlayerId;
                                Players[CurrentPlayerId].NumberOfPropertiesOwned++;
                                currentPlayersTurn_textbox.Invoke((MethodInvoker)delegate () { DrawCircle(item, "Blue"); });
                            }
                        Players[CurrentPlayerId].Loser = receivedMessage.Loser;
                        if (Players[CurrentPlayerId].Loser == true || Players[CurrentPlayerId].Balance <= 0)
                            Lose();
                        CurrentPlayerId = 0;
                        UpdatePlayersStatusBoxes();
                    }
                    if (message.Contains("Рента красному игроку: "))
                    {
                        string sumOfRentString = message.Replace("Рента красному игроку: ", "");
                        int sumOfRent = Convert.ToInt32(sumOfRentString);
                        ChangeBalance(Players[1], -sumOfRent);
                        ChangeBalance(Players[0], sumOfRent);
                        MessageBox.Show("Синий игрок заплатил красному игроку сумму аренды, равную " + sumOfRent);
                    }
                    else if (message.Contains("Рента синему игроку: "))
                    {
                        string sumOfRentString = message.Replace("Рента синему игроку: ", "");
                        int sumOfRent = Convert.ToInt32(sumOfRentString);
                        ChangeBalance(Players[0], -sumOfRent);
                        ChangeBalance(Players[1], sumOfRent);
                        MessageBox.Show("Красный игрок заплатил синему игроку сумму аренды, равную " + sumOfRent);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Подключение прервано! " + ex.Message);
                    Disconnect();
                }
            }
        }
        public void Lose()
        {
            Players[CurrentPlayerId].Loser = true;
            throwDiceBtn.Enabled = false;
            buyBtn.Enabled = false;
            endTurnBtn.Enabled = false;
            if (CurrentPlayerId == 0 && Players[0].Loser == true)
            {
                currentPlayersTurn_textbox.Text = "Красный игрок проиграл!";

            }
            else if (CurrentPlayerId == 1 && Players[1].Loser == true)
            {
                currentPlayersTurn_textbox.Text = "Синий игрок проиграл!";
            }
        }
        static void Disconnect()
        {
            if (Stream != null)
                Stream.Close();//отключение потока
            if (Client != null)
                Client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
        public Game()
        {
            InitializeComponent();
            if (Gamemodes.Multiplayer)
            {
                try
                {
                    IP = ConnectionOptions.IP;
                    Port = ConnectionOptions.Port;
                    Client = new TcpClient();
                    Client.Connect(IP, Port);
                    Stream = Client.GetStream();
                    string namemessage = ConnectionOptions.PlayerName;
                    byte[] data = Encoding.Unicode.GetBytes(namemessage);
                    Stream.Write(data, 0, data.Length);
                    if (ConnectionOptions.PlayerName == "Blue")
                    {
                        currentPositionInfo_richtextbox.Text = String.Empty;
                        throwDiceBtn.Enabled = false;
                        buyBtn.Enabled = false;
                        endTurnBtn.Enabled = false;
                        CurrentPlayerId = 1;
                    }
                    Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                    receiveThread.Start(); //старт потока
                    
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
            Tile = new PictureBox[] { 
                tile0, tile1, tile2, tile3, tile4, tile5, tile6, tile7, tile8, tile9, tile10,
                tile11, tile12, tile13, tile14, tile15, tile16, tile17 , tile18, tile19, tile20,
                tile21, tile22, tile23, tile24, tile25, tile26, tile27, tile28, tile29, tile30,
                tile31, tile32, tile33, tile34, tile35, tile36, tile37, tile38, tile39 };

            currentPlayersTurn_textbox.Text = "Чтобы начать, бросьте кубик";
            if (Gamemodes.Multiplayer)
                if (ConnectionOptions.PlayerName == "Blue")
                    currentPlayersTurn_textbox.Text = "Сейчас ходит красный игрок, ожидайте";
            CreateTile("GO", false, "Null", 0, 0);
            CreateTile("Mediter-Ranean Avenue", true, "Brown", 60, 1);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 2);
            CreateTile("Baltic Avenue", true, "Brown", 60, 3);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 4);
            CreateTile("Reaing Railroad", true, "Station", 200, 5);
            CreateTile("Oriental Avenue", true, "Turquoise", 100, 6);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 7);
            CreateTile("Vermont Avenue", true, "Turquoise", 100, 8);
            CreateTile("Connecticut Avenue", true, "Turquoise", 120, 9);
            CreateTile("Jail", false, "Null", 0, 10);

            CreateTile("St. Charles Place", true, "Purple", 140, 11);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 12);
            CreateTile("States Avenue", true, "Purple", 140, 13);
            CreateTile("Bones Lane", true, "Purple", 160, 14);
            CreateTile("Pennsylvania Railroad", true, "Station", 200, 15);
            CreateTile("St. James Place", true, "Orange", 180, 16);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 17);
            CreateTile("Tennessee Avenue", true, "Orange", 180, 18);
            CreateTile("New York Avenue", true, "Orange", 200, 19);
            CreateTile("Free Parking", false, "Null", 0, 20);

            CreateTile("Kentucky Avenue", true, "Red", 220, 21);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 22);
            CreateTile("Indiana Avenue", true, "Red", 220, 23);
            CreateTile("Illinois Avenue", true, "Red", 240, 24);
            CreateTile("B.& O. Railroad", true, "Station", 200, 25);
            CreateTile("Atlantic Avenue", true, "Yellow", 260, 26);
            CreateTile("Ventnor Avenue", true, "Yellow", 260, 27);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 28);
            CreateTile("Marvin Gardens", true, "Yellow", 280, 29);
            CreateTile("Go To Jail", false, "Null", 0, 30);

            CreateTile("Pacific Avenue", true, "Green", 300, 31);
            CreateTile("North Carolina Avenue", true, "Green", 300, 32);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 33);
            CreateTile("Pennsylvania Avenue", true, "Green", 320, 34);
            CreateTile("Short Line", true, "Station", 200, 35);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 36);
            CreateTile("Park Place", true, "Blue", 350, 37);
            CreateTile("Income Tax: Pay 200M", false, "White", 0, 38);
            CreateTile("Boardwalk", true, "Blue", 400, 39);

            CreatePlayer("Red", 0, false);
            CreatePlayer("Blue", 1, false);
            Players[0].PlayerImageName = "redPawn";
            Players[1].PlayerImageName = "bluePawn";
            UpdatePlayersStatusBoxes();
            buyBtn.Enabled = false;
        }
        public void MoveIcon(int position)
        {
            int x = Tile[position].Location.X, y = Tile[position].Location.Y;
            if (CurrentPlayerId == 0)
            {
                x = Tile[position].Location.X + 5;
                y = Tile[position].Location.Y;
                redPawnIcon.Location = new Point(x, y);
            }
            else if (CurrentPlayerId == 1)
            {
                x = Tile[position].Location.X;
                y = Tile[position].Location.Y;
                bluePawnIcon.Location = new Point(x, y);
            }
            /*switch (CurrentPlayerId)
            {
                case 0:
                    x = Tile[position].Location.X + 5;
                    y = Tile[position].Location.Y;
                    redPawnIcon.Location = new Point(x, y);
                    break;
                case 1:
                    x = Tile[position].Location.X;
                    y = Tile[position].Location.Y;
                    bluePawnIcon.Location = new Point(x, y);
                    break;
                default:
                    MessageBox.Show("Произошла ошибка");
                    break;
            }*/
        }
        async Task<int> MoveTileByTile(int start, int end)
        {
            if (end < 40)
            {
                for (int i = start; i <= end; i++)
                {
                    await Task.Delay(150);
                    MoveIcon(i);
                }
            }
            else if (end >= 40)
            {
                for (int i = start; i <= 39; i++)
                {
                    await Task.Delay(150);
                    MoveIcon(i);
                }
                for (int i = 0; i <= end - 40; i++)
                {
                    await Task.Delay(150);
                    MoveIcon(i);
                }
            }
            return 42;
        }
        private void ThrowDiceBtn_Click(object sender, EventArgs e)
        {
            bool visitedJailExploration = false;
            bool visitedTaxTile = false;
            bool visitedGo = false;
            bool visitedFreeParking = false;
            bool goingToJail = false;
            buyBtn.Enabled = true;
            UpdatePlayersStatusBoxes();
            DiceRoll = true;
            RollDice();
            whatIsOnDices_textbox.Text = "На кубиках выпало: " + FirstDice + " и " + SecondDice + ". Итого: " + Dice;
            DiceRoll = false;
            throwDiceBtn.Enabled = false;
            var positionBeforeDicing = Position[CurrentPlayerId];
            CurrentPosition = Position[CurrentPlayerId] + Dice;
            var positionAfterDicing = Position[CurrentPlayerId] + Dice;
            if (RollToJail_checkbox.Checked == true)
            {
                CurrentPosition = 30;
                positionBeforeDicing = Position[CurrentPlayerId];
                positionAfterDicing = 30;
                goingToJail = true;
            }
            // ?
            if (Players[CurrentPlayerId].inJail == true)
                InJail(CurrentPlayerId);
            if (CurrentPosition == 0)
            {
                buyBtn.Enabled = false;
                visitedGo = true;
            }
            if (CurrentPosition == 20)
            {
                buyBtn.Enabled = false;
                visitedFreeParking = true;
            }
            if ((CurrentPosition == 10) && (Players[CurrentPlayerId].inJail == false))
            {
                buyBtn.Enabled = false;
                visitedJailExploration = true;
            }
            if (CurrentPosition >= 40)
            {
                ChangeBalance(Players[CurrentPlayerId], 200);
                Position[CurrentPlayerId] = CurrentPosition - 40;
                CurrentPosition = Position[CurrentPlayerId];
            }
            if (Properties[CurrentPosition].Colour == "White")
            {
                ChangeBalance(Players[CurrentPlayerId], -200);
                buyBtn.Enabled = false;
                visitedTaxTile = true;
            }
            if (CurrentPosition == 30)
            {
                CurrentPosition = 10;
                Players[CurrentPlayerId].inJail = true;
                InJail(CurrentPlayerId);
                goingToJail = true;
            }
            Position[CurrentPlayerId] = CurrentPosition;
            if (goingToJail == false)
                _ = MoveTileByTile(positionBeforeDicing, positionAfterDicing);
            if (goingToJail == true)
                MoveIcon(10);
            currentPositionInfo_richtextbox.Text = "Позиция " + CurrentPosition;
            currentPositionInfo_richtextbox.AppendText("\r\n" + Properties[CurrentPosition].Name);
            currentPositionInfo_richtextbox.AppendText("\r\n" + "Цена " + Properties[CurrentPosition].Value);
            currentPositionInfo_richtextbox.AppendText("\r\n" + "Тип " + Properties[CurrentPosition].Colour);
            currentPositionInfo_richtextbox.ScrollToCaret();
            if (visitedJailExploration)
            {
                currentPositionInfo_richtextbox.Text = "Позиция " + CurrentPosition;
                currentPositionInfo_richtextbox.AppendText("\r\n" + Properties[CurrentPosition].Name);
                currentPositionInfo_richtextbox.AppendText("\r\n" + "Вы посетили тюрьму с экскурсией. Удачи!");
                currentPositionInfo_richtextbox.ScrollToCaret();
            }
            if (visitedTaxTile)
            {
                currentPositionInfo_richtextbox.Text = "Позиция " + CurrentPosition;
                currentPositionInfo_richtextbox.AppendText("\r\n" + Properties[CurrentPosition].Name);
                currentPositionInfo_richtextbox.AppendText("\r\n" + "Вы заплатили налог!");
                currentPositionInfo_richtextbox.ScrollToCaret();
            }
            if (visitedGo)
            {
                currentPositionInfo_richtextbox.Text = "Позиция " + CurrentPosition;
                currentPositionInfo_richtextbox.AppendText("\r\n" + Properties[CurrentPosition].Name);
                currentPositionInfo_richtextbox.AppendText("\r\n" + "Вы на старте, получите 200М. Удачи на новом круге!");
                currentPositionInfo_richtextbox.ScrollToCaret();
            }
            if (visitedFreeParking)
            {
                currentPositionInfo_richtextbox.Text = "Позиция " + CurrentPosition;
                currentPositionInfo_richtextbox.AppendText("\r\n" + Properties[CurrentPosition].Name);
                currentPositionInfo_richtextbox.AppendText("\r\n" + "Хоть минутка отдыха...");
                currentPositionInfo_richtextbox.ScrollToCaret();
            }
            if (goingToJail)
            {
                currentPositionInfo_richtextbox.Text = "Позиция " + CurrentPosition;
                currentPositionInfo_richtextbox.AppendText("\r\n" + Properties[CurrentPosition].Name);
                currentPositionInfo_richtextbox.AppendText("\r\n" + "Вы в тюрьме");
                currentPositionInfo_richtextbox.ScrollToCaret();
                currentPlayersTurn_textbox.Text = "Игрок " + Players[CurrentPlayerId].PlayerImageName;
                currentPlayersTurn_textbox.Text += "\r\nВы попали в тюрьму. Вы пропустите этот и следующий ход";
            }
            if (Players[CurrentPlayerId].Loser == true || Players[CurrentPlayerId].Balance <= 0)
                Lose();
            int count = 0;
            for (int i = 0; i < 2; i++)
            {
                if (Players[i].Loser == true)
                    count++;
                if (Players[CurrentPlayerId].Loser == false && count >= 1)
                {
                    currentPlayersTurn_textbox.Text = "Вы победили! Поздравляем!";
                    DialogResult dialog = MessageBox.Show("Игрок " + Players[CurrentPlayerId].PlayerImageName + " победил!", "ИГРА ОКОНЧЕНА", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    if (dialog == DialogResult.OK)
                        Application.Exit();
                }
            }
            if (goingToJail == false)
                currentPlayersTurn_textbox.Text = "Игрок " + Players[CurrentPlayerId].PlayerImageName;
            if (IsOwned(CurrentPlayerId, CurrentPosition) == false)
            {
                if (CurrentPlayerId == 0)
                {
                    ChangeBalance(Players[0], -GetRent(Dice));
                    ChangeBalance(Players[1], GetRent(Dice));
                    if (Gamemodes.Multiplayer)
                    {
                        string rentMessage = "Рента синему игроку: " + GetRent(Dice).ToString();
                        MessageBox.Show("Красный игрок заплатил синему игроку сумму аренды, равную " + GetRent(Dice));
                        Stream.Write(Encoding.Unicode.GetBytes(rentMessage), 0, Encoding.Unicode.GetBytes(rentMessage).Length);
                    }
                }
                else
                {
                    ChangeBalance(Players[1], -GetRent(Dice));
                    ChangeBalance(Players[0], GetRent(Dice));
                    if (Gamemodes.Multiplayer)
                    {
                        string rentMessage = "Рента красному игроку: " + GetRent(Dice).ToString();
                        MessageBox.Show("Синий игрок заплатил красному игроку сумму аренды, равную " + GetRent(Dice));
                        Stream.Write(Encoding.Unicode.GetBytes(rentMessage), 0, Encoding.Unicode.GetBytes(rentMessage).Length);
                    }
                }
                buyBtn.Enabled = false;
                currentPlayersTurn_textbox.Text = "Игрок " + Players[CurrentPlayerId].PlayerImageName;
                currentPlayersTurn_textbox.Text += "\r\nВы попали на клетку другого игрока и заплатили " + Properties[CurrentPosition].Rent + " M";
                if (CurrentPosition == 5 || CurrentPosition == 15 || CurrentPosition == 25 || CurrentPosition == 35)
                {
                    currentPlayersTurn_textbox.Text = "Игрок " + Players[CurrentPlayerId].PlayerImageName;
                    currentPlayersTurn_textbox.Text += "\r\nВы попали на клетку другого игрока и заплатили " + Dice * 25 + " M";
                }
            }
        }
        private void BuyBtn_Click(object sender, EventArgs e)
        {
            if ((Properties[CurrentPosition].Buyable == true) && (Properties[CurrentPosition].Owned == false))
                if (Players[CurrentPlayerId].Balance >= Properties[CurrentPosition].Value)
                {
                    ChangeBalance(Players[CurrentPlayerId], -Properties[CurrentPosition].Value);
                    Players[CurrentPlayerId].PropertiesOwned[Players[CurrentPlayerId].NumberOfPropertiesOwned] = CurrentPosition;
                    Properties[CurrentPosition].Owned = true;
                    Properties[CurrentPosition].Owner = CurrentPlayerId;
                    Players[CurrentPlayerId].NumberOfPropertiesOwned++;
                    UpdatePlayersStatusBoxes();
                    buyBtn.Enabled = false;
                    if (Players[CurrentPlayerId].PlayerImageName == "redPawn")
                        DrawCircle(CurrentPosition, "Red");
                    if (Players[CurrentPlayerId].PlayerImageName == "bluePawn")
                        DrawCircle(CurrentPosition, "Blue");
                }
                else
                currentPlayersTurn_textbox.Text = "У вас недостаточно денег";
            else
                currentPlayersTurn_textbox.Text = "Данное действие выполнить невозможно";
            if (Players[CurrentPlayerId].Balance <= 0)
                Lose();
        }
        private void QuitGameBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Завершить работу?", "Выход", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
                Application.Exit();
            else if (dialog == DialogResult.No)
                return;
        }
        private void CreateTurnLogString()
        {
            if (CurrentPlayerId == 0)
                TurnLogString = "Результаты хода красного игрока: ";
            if (CurrentPlayerId == 1)
                TurnLogString = "Результаты хода синего игрока: ";
            TurnLogString += 
                CurrentPlayerId.ToString() + '~' + 
                Position[CurrentPlayerId].ToString() + '~' + 
                Players[CurrentPlayerId].Balance.ToString()+ '~' +
                Players[CurrentPlayerId].inJail.ToString() + '~' + 
                Players[CurrentPlayerId].Jail.ToString() + '~';
            foreach (int item in Players[CurrentPlayerId].PropertiesOwned)
                if (item != 0)
                {
                    TurnLogString += item;
                    TurnLogString += ' ';
                }
            if (TurnLogString.Last() == '~')
                TurnLogString += "NULL";
            TurnLogString += '~' + Players[CurrentPlayerId].Loser.ToString();
        }
        private void EndTurnBtn_Click(object sender, EventArgs e)
        {
            if (Gamemodes.Multiplayer)
            {
                /*
                if (Players[CurrentPlayerId].inJail == true)
                {
                    CurrentPosition = 10;
                    MoveIcon(CurrentPosition);
                    Position[CurrentPlayerId] = CurrentPosition;
                    InJail(CurrentPlayerId);
                }*/
                if (Players[CurrentPlayerId].Loser == true || Players[CurrentPlayerId].Balance <= 0)
                    Lose();
                int count = 0;
                for (int i = 0; i < 2; i++)
                {
                    if (Players[i].Loser == true)
                        count++;
                    if (Players[CurrentPlayerId].Loser == false && count >= 1)
                    {
                        currentPlayersTurn_textbox.Text = "Вы победили!";
                        DialogResult dialog = MessageBox.Show("Игрок " + Players[CurrentPlayerId].PlayerImageName + " победил!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        if (dialog == DialogResult.OK)
                            Application.Exit();
                    }
                }
                currentPositionInfo_richtextbox.Text = String.Empty;
                CreateTurnLogString();
                
                if (CurrentPlayerId == 0)
                {
                    currentPlayersTurn_textbox.Text = "Сейчас ходит синий игрок, ожидайте";
                    Stream.Write(Encoding.Unicode.GetBytes(TurnLogString), 0, Encoding.Unicode.GetBytes(TurnLogString).Length);
                }
                else
                {
                    currentPlayersTurn_textbox.Text = "Сейчас ходит красный игрок, ожидайте";
                    Stream.Write(Encoding.Unicode.GetBytes(TurnLogString), 0, Encoding.Unicode.GetBytes(TurnLogString).Length);
                }
                throwDiceBtn.Enabled = false;
                buyBtn.Enabled = false;
                endTurnBtn.Enabled = false;
                
            }
            if (Gamemodes.Singleplayer)
            {
                if (CurrentPlayerId == 0)
                    CurrentPlayerId = 1;
                else
                    CurrentPlayerId = 0;
                currentPlayersTurn_textbox.Text = "Игрок " + Players[CurrentPlayerId].PlayerImageName;
                throwDiceBtn.Enabled = true;
                buyBtn.Enabled = false;
                if (Players[CurrentPlayerId].inJail == true)
                {
                    CurrentPosition = 10;
                    MoveIcon(CurrentPosition);
                    Position[CurrentPlayerId] = CurrentPosition;
                    InJail(CurrentPlayerId);
                }
                if (Players[CurrentPlayerId].Loser == true || Players[CurrentPlayerId].Balance <= 0)
                    Lose();
                int count = 0;
                for (int i = 0; i < 2; i++)
                {
                    if (Players[i].Loser == true)
                        count++;
                    if (Players[CurrentPlayerId].Loser == false && count >= 1)
                    {
                        currentPlayersTurn_textbox.Text = "Вы победили!";
                        DialogResult dialog = MessageBox.Show("Игрок " + Players[CurrentPlayerId].PlayerImageName + " победил!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        if (dialog == DialogResult.OK)
                            Application.Exit();
                    }
                }
                currentPositionInfo_richtextbox.Text = String.Empty;
            }
        }
    }
}