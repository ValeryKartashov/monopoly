using System;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonopolyClient
{
    public partial class Game : Form
    {
        private static TcpClient Client = new TcpClient();
        private static NetworkStream Stream;
        private bool RedConnected, BlueConnected;
        private static int Dice, CurrentPosition, CurrentPlayerId, RedDotsNameSplitter, BlueDotsNameSplitter;
        private readonly Player[] Players = new Player[2];
        private readonly Property[] Properties = new Property[40];
        private readonly PictureBox[] Tile;
        private class Property
        {
            public bool Buyable, Owned;
            public string Color, Name;
            public int Price, Rent;
        }
        private class Player
        {
            public int Balance = 1500, NumberOfPropertiesOwned, Jail, Position;
            public bool InJail, Loser;
            public readonly int[] PropertiesOwned = new int[40];
        }
        private class ReceivedMessage
        {
            public bool InJail, Loser;
            public int EndPosition, Balance, Jail;
            public readonly int[] PropertiesOwned = new int[40];
        }
        public Game()
        {
            InitializeComponent();
            if (Gamemodes.Multiplayer)
                try
                {
                    var connection = new Connection();
                    connection.ShowDialog();
                    if (connection.DialogResult is DialogResult.Cancel)
                    {
                        var mainMenu = new MainMenu();
                        mainMenu.ShowDialog();
                        Disconnect();
                    }
                    currentPlayersTurn_textbox.Text = "Ожидание подключения второго игрока...";
                    throwDiceBtn.Enabled = false;
                    buyBtn.Enabled = false;
                    endTurnBtn.Enabled = false;
                    try
                    {
                        Client = new TcpClient();
                        Client.Connect(ConnectionOptions.IP, ConnectionOptions.Port);
                        Stream = Client.GetStream();
                    }
                    catch
                    {
                        MessageBox.Show("Подключиться к серверу не удалось." 
                                        + Environment.NewLine 
                                        + "Сервер выключен");
                        Disconnect();
                    }
                    var receiveThread = new Thread(ReceiveMessage);
                    receiveThread.Start();
                    Stream.Write(
                        Encoding.Unicode.GetBytes("Кто-то присоединился к серверу"), 
                        0, 
                        Encoding.Unicode.GetBytes("Кто-то присоединился к серверу").Length);
                    var colorChoosing = new ColorChoosing();
                    colorChoosing.ShowDialog();
                    if (colorChoosing.DialogResult is DialogResult.Cancel)
                    {
                        var mainMenu = new MainMenu();
                        mainMenu.ShowDialog();
                        Disconnect();
                    }
                    Stream.Write(
                        Encoding.Unicode.GetBytes(ConnectionOptions.PlayerName), 
                        0, 
                        Encoding.Unicode.GetBytes(ConnectionOptions.PlayerName).Length);
                    switch (ConnectionOptions.PlayerName)
                    {
                        case "Red":
                            RedConnected = true;
                            CurrentPlayerId = 0;
                            break;
                        case "Blue":
                            BlueConnected = true;
                            CurrentPlayerId = 1;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            #region Создание поля и игроков
            Tile = new[]
            {
                tile0, tile1, tile2, tile3, tile4, tile5, tile6, tile7, tile8, tile9, tile10,
                tile11, tile12, tile13, tile14, tile15, tile16, tile17, tile18, tile19, tile20,
                tile21, tile22, tile23, tile24, tile25, tile26, tile27, tile28, tile29, tile30,
                tile31, tile32, tile33, tile34, tile35, tile36, tile37, tile38, tile39
            };
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
            Players[0] = new Player();
            Players[1] = new Player();
            #endregion
            UpdatePlayersStatusBoxes();
            buyBtn.Enabled = false;
        }
        private void CreateTile(string tileName, bool tileBuyable, string tileColor, int tilePrice, int tilePosition)
        {
            var property = new Property
            {
                Name = tileName,
                Color = tileColor,
                Buyable = tileBuyable,
                Price = tilePrice
            };
            Properties[tilePosition] = property;
        }
        private string PropertiesToString(int[] propertyList)
        {
            var tempString = "";
            for (var i = 0; i < 40; i++)
                if (propertyList[i] != 0)
                    tempString = tempString + Properties[propertyList[i]].Name + ", " + Properties[propertyList[i]].Color + "\n";
            return tempString;
        }
        private void UpdatePlayersStatusBoxes()
        {
            redPlayerStatusBox_richtextbox.Text = "Красный игрок" + "\n" 
                + "Баланс: " + Players[0].Balance + "\n" 
                + PropertiesToString(Players[0].PropertiesOwned);
            bluePlayerStatusBox_richtextbox.Text = "Синий игрок" + "\n"
                + "Баланс: " + Players[1].Balance + "\n"
                + PropertiesToString(Players[1].PropertiesOwned);
        }
        private void ChangeBalance(Player player, int cashChange)
        {
            player.Balance += cashChange;
            UpdatePlayersStatusBoxes();
        }
        private void InJail(int currentPlayer)
        {
            Players[currentPlayer].Jail += 1;
            buyBtn.Enabled = false;
            throwDiceBtn.Enabled = false;
            switch (CurrentPlayerId)
            {
                case 0:
                    currentPlayersTurn_textbox.Text = 
                        "Красный игрок, вы в тюрьме.\r\nНа следующий ход вы сможете ходить. ";
                    break;
                case 1:
                    currentPlayersTurn_textbox.Text = 
                        "Синий игрок, вы в тюрьме.\r\nНа следующий ход вы сможете ходить. ";
                    break;
            }
            if (Players[currentPlayer].Jail != 3) return;
            Players[currentPlayer].InJail = false;
            Players[currentPlayer].Jail = 0;
            throwDiceBtn.Enabled = true;
            switch (CurrentPlayerId)
            {
                case 0:
                    currentPlayersTurn_textbox.Text = 
                        "Красный игрок, вы освобождены. ";
                    break;
                case 1:
                    currentPlayersTurn_textbox.Text = 
                        "Синий игрок, вы освобождены. ";
                    break;
            }
        }
        private int GetRent(int dice)
        {
            switch (Properties[CurrentPosition].Color)
            {
                case "Null":
                    Properties[CurrentPosition].Rent = 0;
                    break;
                case "Station":
                    Properties[CurrentPosition].Rent = dice * 20;
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
        private void DrawCircle(int position, int playerId)
        {
            int x = Tile[position].Location.X, y = Tile[position].Location.Y;
            switch (playerId)
            {
                case 0:
                {
var redMarker = new PictureBox
{
Size = new Size(30, 30),
Name = "redMarker" + RedDotsNameSplitter,
BackgroundImage = redDot_picturebox.BackgroundImage,
BackColor = Color.Transparent,
Left = x,
Top = y
};
Controls.Add(redMarker);
redMarker.BringToFront();
RedDotsNameSplitter++;
                    break;
                }
                case 1:
                {
                    var blueMarker = new PictureBox
                    {
                        Size = new Size(30, 30),
                        Name = "blueMarker" + BlueDotsNameSplitter,
                        BackgroundImage = blueDot_picturebox.BackgroundImage,
                        BackColor = Color.Transparent,
                        Left = x,
                        Top = y
                    };
                    Controls.Add(blueMarker);
                    blueMarker.BringToFront();
                    BlueDotsNameSplitter++;
                    break;
                }
            }
        }
        private void ReceiveMessage()
        {
            while (true)
                try
                {
var data = new byte[256];
var builder = new StringBuilder();
do
{
var bytes = Stream.Read(data, 0, data.Length);
builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
} 
while (Stream.DataAvailable);
var message = builder.ToString();
switch (message)
                    {
                        case "Оба подключились":
                        {
                            switch (ConnectionOptions.PlayerName)
                            {
                                case "Red":
                                    currentPlayersTurn_textbox.Text = "Чтобы начать, бросьте кубик";
                                    throwDiceBtn.Enabled = true;
                                    buyBtn.Enabled = false;
                                    endTurnBtn.Enabled = true;
                                    break;
                                case "Blue":
                                    currentPlayersTurn_textbox.Text = "Сейчас ходит красный игрок, ожидайте";
                                    break;
                            }
                            break;
                        }                        
                        case "Red присоединился к серверу":
                        {
                            RedConnected = true;
                            ConnectionOptions.NameRedIsTaken = true;
                            if (!BlueConnected) continue;
                            Stream.Write(Encoding.Unicode.GetBytes("Оба подключились"), 0, Encoding.Unicode.GetBytes("Оба подключились").Length);
                            break;
                        }
                        case "Blue присоединился к серверу":
                        {
                            BlueConnected = true;
                            ConnectionOptions.NameBlueIsTaken = true;
                            if (!RedConnected) continue;
                            Stream.Write(Encoding.Unicode.GetBytes("Оба подключились"), 0, Encoding.Unicode.GetBytes("Оба подключились").Length);
                            break;
                        }
                        case "Красный занят":
                        {
                            ConnectionOptions.NameRedIsTaken = true;
                            break;
                        }
                        case "Синий занят":
                        {
                            ConnectionOptions.NameBlueIsTaken = true;
                            break;
                        }
                    }
                    if (message.Contains("Результаты хода"))
                    {
                        var tempMessage = message;
                        var subString = string.Empty;
                        switch (CurrentPlayerId)
                        {
                            case 0:
                                subString = "Результаты хода синего игрока: ";
                                break;
                            case 1:
                                subString = "Результаты хода красного игрока: ";
                                break;
                        }
                        tempMessage = tempMessage.Replace(subString, "");
                        currentPlayersTurn_textbox.Invoke((MethodInvoker)delegate
                        {
                            currentPlayersTurn_textbox.Text = "Ваш ход";
                        });
                        throwDiceBtn.Enabled = true;
                        buyBtn.Enabled = false;
                        endTurnBtn.Enabled = true;
                        var receivedMessage = new ReceivedMessage();
                        var stringPosition = tempMessage.Split('~')[1];
                        receivedMessage.EndPosition = Convert.ToInt32(stringPosition);
                        var stringBalance = tempMessage.Split('~')[2];
                        receivedMessage.Balance = Convert.ToInt32(stringBalance);
                        var stringInJail = tempMessage.Split('~')[3];
                        switch (stringInJail)
                        {
                            case "TRUE":
                                receivedMessage.InJail = true;
                                break;
                            case "FALSE":
                                receivedMessage.InJail = false;
                                break;
                        }
                        var stringJail = tempMessage.Split('~')[4];
                        receivedMessage.Jail = Convert.ToInt32(stringJail);
                        var stringPropertiesOwned = tempMessage.Split('~')[5];
                        if (stringPropertiesOwned != "NULL")
                        {
                            var tempArrayOfPropertiesOwned = stringPropertiesOwned.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x)).ToArray();
                            for (var k = 0; k < tempArrayOfPropertiesOwned.Length; k++) receivedMessage.PropertiesOwned[k] = tempArrayOfPropertiesOwned[k];
                        }
                        var stringLoser = tempMessage.Split('~')[6];
                        switch (stringLoser)
                        {
                            case "TRUE":
                                receivedMessage.Loser = true;
                                break;
                            case "FALSE":
                                receivedMessage.Loser = false;
                                break;
                        }
                        if (Players[CurrentPlayerId].InJail)
                        {
                            CurrentPosition = 10;
                            MoveIcon(CurrentPosition);
                            Players[CurrentPlayerId].Position = CurrentPosition;
                            InJail(CurrentPlayerId);
                        }
                        if (Players[CurrentPlayerId].Loser || Players[CurrentPlayerId].Balance <= 0) Lose();
                        var count = 0;
                        for (var u = 0; u < 2; u++)
                        {
                            if (Players[u].Loser) count++;
                            if (Players[CurrentPlayerId].Loser || count < 1) continue;
                            currentPlayersTurn_textbox.Text = "Вы победили!";
                            switch (CurrentPlayerId)
                            {
                                case 0:
                                    if (MessageBox.Show("Красный игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                                    break;
                                case 1:
                                    if (MessageBox.Show("Синий игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                                    break;
                            }
                        }
                        switch (CurrentPlayerId)
                        {
                            case 0:
                                CurrentPlayerId = 1;
                                MoveIcon(receivedMessage.EndPosition);
                                Players[CurrentPlayerId].Position = receivedMessage.EndPosition;
                                Players[CurrentPlayerId].Balance = receivedMessage.Balance;
                                Players[CurrentPlayerId].InJail = receivedMessage.InJail;
                                Players[CurrentPlayerId].Jail = receivedMessage.Jail;
                                if (Players[CurrentPlayerId].InJail) InJail(CurrentPlayerId);
                                var i = 0;
                                foreach (var item in receivedMessage.PropertiesOwned)
                                {
                                    Players[CurrentPlayerId].PropertiesOwned[i] = item;
                                    i++;
                                }
                                foreach (var item in Players[CurrentPlayerId].PropertiesOwned)
                                    if (item != 0)
                                    {
                                        Properties[item].Owned = true;
                                        Players[CurrentPlayerId].NumberOfPropertiesOwned++;
                                        currentPlayersTurn_textbox.Invoke((MethodInvoker)delegate
                                        {
                                            DrawCircle(item, 1);
                                        });
                                    }
                                Players[CurrentPlayerId].Loser = receivedMessage.Loser;
                                if (Players[CurrentPlayerId].Loser || Players[CurrentPlayerId].Balance <= 0) Lose();
                                CurrentPlayerId = 0;
                                UpdatePlayersStatusBoxes();
                                break;
                            case 1:
                                CurrentPlayerId = 0;
                                MoveIcon(receivedMessage.EndPosition);
                                Players[CurrentPlayerId].Position = receivedMessage.EndPosition;
                                Players[CurrentPlayerId].Balance = receivedMessage.Balance;
                                Players[CurrentPlayerId].InJail = receivedMessage.InJail;
                                Players[CurrentPlayerId].Jail = receivedMessage.Jail;
                                if (Players[CurrentPlayerId].InJail) InJail(CurrentPlayerId);
                                var k = 0;
                                foreach (var item in receivedMessage.PropertiesOwned)
                                {
                                    Players[CurrentPlayerId].PropertiesOwned[k] = item;
                                    k++;
                                }
                                foreach (var item in Players[CurrentPlayerId].PropertiesOwned)
                                    if (item != 0)
                                    {
                                        Properties[item].Owned = true;
                                        Players[CurrentPlayerId].NumberOfPropertiesOwned++;
                                        currentPlayersTurn_textbox.Invoke((MethodInvoker)delegate
                                        {
                                            DrawCircle(item, 0);
                                        });
                                    }
                                Players[CurrentPlayerId].Loser = receivedMessage.Loser;
                                if (Players[CurrentPlayerId].Loser || Players[CurrentPlayerId].Balance <= 0) Lose();
                                CurrentPlayerId = 1;
                                UpdatePlayersStatusBoxes();
                                break;
                        }
                    }
                    if (message.Contains("Рента красному"))
                    {
                        var sumOfRentString = message.Replace("Рента красному игроку: ", "");
                        var sumOfRent = Convert.ToInt32(sumOfRentString);
                        ChangeBalance(Players[1], -sumOfRent);
                        ChangeBalance(Players[0], sumOfRent);
                        MessageBox.Show("Синий игрок заплатил красному игроку сумму аренды, равную " + sumOfRent);
                    }
                    else if (message.Contains("Рента синему"))
                    {
                        var sumOfRentString = message.Replace("Рента синему игроку: ", "");
                        var sumOfRent = Convert.ToInt32(sumOfRentString);
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
        private void Lose()
        {
            Players[CurrentPlayerId].Loser = true;
            throwDiceBtn.Enabled = false;
            buyBtn.Enabled = false;
            endTurnBtn.Enabled = false;
            switch (CurrentPlayerId)
            {
                case 0 when Players[0].Loser:
                    currentPlayersTurn_textbox.Text = "Красный игрок проиграл!";
                    break;
                case 1 when Players[1].Loser:
                    currentPlayersTurn_textbox.Text = "Синий игрок проиграл!";
                    break;
            }
        }
        private static void Disconnect()
        {
            Stream?.Close();
            Client?.Close();
            Environment.Exit(0);
        }
        private void MoveIcon(int position)
        {
            int x, y;
            switch (CurrentPlayerId)
            {
                case 0:
                    x = Tile[position].Location.X;
                    y = Tile[position].Location.Y;
                    redPawnIcon.Location = new Point(x, y);
                    break;
                case 1:
                    x = Tile[position].Location.X;
                    y = Tile[position].Location.Y;
                    bluePawnIcon.Location = new Point(x, y);
                    break;
            }
        }
        private async Task<int> MoveTileByTile(int from, int to)
        {
            if (to < 40)
            {
                for (var i = from; i <= to; i++)
                {
                    await Task.Delay(150);
                    MoveIcon(i);
                }
            }
            else
            {
                for (var i = from; i <= 39; i++)
                {
                    await Task.Delay(150);
                    MoveIcon(i);
                }
                for (var i = 0; i <= to - 40; i++)
                {
                    await Task.Delay(150);
                    MoveIcon(i);
                }
            }
            return 1;
        }
        private void ThrowDiceBtn_Click(object sender, EventArgs e)
        {
            switch (CurrentPlayerId)
            {
                case 0:
                    currentPlayersTurn_textbox.Text = "Ходит красный игрок. ";
                    break;
                case 1:
                    currentPlayersTurn_textbox.Text = "Ходит синий игрок. ";
                    break;
            }
            bool visitedJailExploration = false, visitedTaxTile = false, visitedGo = false, visitedFreeParking = false, goingToJail = false;
            buyBtn.Enabled = true;
            UpdatePlayersStatusBoxes();
            var rand = new Random();
            var firstDice = rand.Next(1, 7);
            var secondDice = rand.Next(1, 7);
            Dice = firstDice + secondDice;
            whatIsOnDices_textbox.Text = "На кубиках выпало: " + firstDice + " и " + secondDice + ". Итого: " + Dice + ". ";
            throwDiceBtn.Enabled = false;
            var positionBeforeDicing = Players[CurrentPlayerId].Position;
            CurrentPosition = Players[CurrentPlayerId].Position + Dice;
            var positionAfterDicing = Players[CurrentPlayerId].Position + Dice;
            if (RollToJail_checkbox.Checked)
            {
                CurrentPosition = 30;
                positionBeforeDicing = Players[CurrentPlayerId].Position;
                positionAfterDicing = 30;
                goingToJail = true;
            }
            if (Players[CurrentPlayerId].InJail) InJail(CurrentPlayerId);
            switch (CurrentPosition)
            {
                case 0:
                    buyBtn.Enabled = false;
                    visitedGo = true;
                    break;
                case 10 when Players[CurrentPlayerId].InJail is false:
                    buyBtn.Enabled = false;
                    visitedJailExploration = true;
                    break;
                case 20:
                    buyBtn.Enabled = false;
                    visitedFreeParking = true;
                    break;
                case 30:
                    CurrentPosition = 10;
                    Players[CurrentPlayerId].InJail = true;
                    InJail(CurrentPlayerId);
                    goingToJail = true;
                    break;
            }
            if (CurrentPosition >= 40)
            {
                ChangeBalance(Players[CurrentPlayerId], 200);
                Players[CurrentPlayerId].Position = CurrentPosition - 40;
                CurrentPosition = Players[CurrentPlayerId].Position;
            }
            if (Properties[CurrentPosition].Color is "White")
            {
                ChangeBalance(Players[CurrentPlayerId], -200);
                buyBtn.Enabled = false;
                visitedTaxTile = true;
            }
            Players[CurrentPlayerId].Position = CurrentPosition;
            switch (goingToJail)
            {
                case true:
                    MoveIcon(10);
                    break;
                case false:
                    _ = MoveTileByTile(positionBeforeDicing, positionAfterDicing);
                    break;
            }
            currentPositionInfo_richtextbox.Text = "Позиция " + CurrentPosition;
            currentPositionInfo_richtextbox.AppendText("\r\n" + Properties[CurrentPosition].Name);
            currentPositionInfo_richtextbox.AppendText("\r\n" + "Цена " + Properties[CurrentPosition].Price);
            currentPositionInfo_richtextbox.AppendText("\r\n" + "Тип " + Properties[CurrentPosition].Color);
            if (visitedJailExploration) currentPositionInfo_richtextbox.AppendText("\r\n" + "Вы посетили тюрьму с экскурсией. ");
            if (visitedTaxTile) currentPositionInfo_richtextbox.AppendText("\r\n" + "Вы заплатили налог!");
            if (visitedGo) currentPositionInfo_richtextbox.AppendText("\r\n" + "Получите +200 за прохождение круга. ");
            if (visitedFreeParking) currentPositionInfo_richtextbox.AppendText("\r\n" + "Минутка отдыха...");
            if (goingToJail)
            {
                currentPositionInfo_richtextbox.AppendText("\r\n" + "Вы в тюрьме");
                switch (CurrentPlayerId)
                {
                    case 0:
                        currentPlayersTurn_textbox.Text = "Красный игрок, вы в тюрьме. \r\nВы попали в тюрьму. Вы пропустите этот и следующий ход. ";
                        break;
                    case 1:
                        currentPlayersTurn_textbox.Text = "Синий игрок, вы в тюрьме. \r\nВы попали в тюрьму. Вы пропустите этот и следующий ход. ";
                        break;
                }
            }
            currentPositionInfo_richtextbox.ScrollToCaret();
            if (Players[CurrentPlayerId].Loser || Players[CurrentPlayerId].Balance <= 0) Lose();
            var count = 0;
            for (var i = 0; i < 2; i++)
            {
                if (Players[i].Loser) count++;
                if (Players[CurrentPlayerId].Loser || count < 1) continue;
                currentPlayersTurn_textbox.Text = "Вы победили! Поздравляем!";
                switch (CurrentPlayerId)
                {
                    case 0:
                        if (MessageBox.Show("Красный игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                        break;
                    case 1:
                        if (MessageBox.Show("Синий игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                        break;
                }
            }
            if (Players[CurrentPlayerId].PropertiesOwned[CurrentPosition] == CurrentPosition || !Properties[CurrentPosition].Owned) return;
            buyBtn.Enabled = false;
            switch (CurrentPlayerId)
            {
                case 0:
                    ChangeBalance(Players[0], -GetRent(Dice));
                    ChangeBalance(Players[1], GetRent(Dice));
                    if (Gamemodes.Multiplayer)
                    {
                        var rentMessage = "Рента синему игроку: " + GetRent(Dice);
                        MessageBox.Show("Красный игрок заплатил синему игроку сумму аренды, равную " + GetRent(Dice));
                        Stream.Write(Encoding.Unicode.GetBytes(rentMessage), 0, Encoding.Unicode.GetBytes(rentMessage).Length);
                    }
                    break;
                case 1:
                    ChangeBalance(Players[1], -GetRent(Dice));
                    ChangeBalance(Players[0], GetRent(Dice));
                    if (Gamemodes.Multiplayer)
                    {
                        var rentMessage = "Рента красному игроку: " + GetRent(Dice);
                        MessageBox.Show("Синий игрок заплатил красному игроку сумму аренды, равную " + GetRent(Dice));
                        Stream.Write(Encoding.Unicode.GetBytes(rentMessage), 0, Encoding.Unicode.GetBytes(rentMessage).Length);
                    }
                    break;
            }
            switch (CurrentPlayerId)
            {
                case 0:
                    currentPlayersTurn_textbox.Text = "Красный игрок, вы попали на клетку другого игрока и заплатили ";
                    break;
                case 1:
                    currentPlayersTurn_textbox.Text = "Синий игрок, вы попали на клетку другого игрока и заплатили ";
                    break;
            }
            if (CurrentPosition is 5 || CurrentPosition is 15 || CurrentPosition is 25 || CurrentPosition is 35) currentPlayersTurn_textbox.Text += Dice * 20;
            else currentPlayersTurn_textbox.Text += Properties[CurrentPosition].Rent;
        }
        private void BuyBtn_Click(object sender, EventArgs e)
        {
            if (Properties[CurrentPosition].Buyable && Properties[CurrentPosition].Owned is false)
                if (Players[CurrentPlayerId].Balance >= Properties[CurrentPosition].Price)
                {
                    ChangeBalance(Players[CurrentPlayerId], -Properties[CurrentPosition].Price);
                    Players[CurrentPlayerId].PropertiesOwned[Players[CurrentPlayerId].NumberOfPropertiesOwned] = CurrentPosition;
                    Properties[CurrentPosition].Owned = true;
                    Players[CurrentPlayerId].NumberOfPropertiesOwned++;
                    UpdatePlayersStatusBoxes();
                    buyBtn.Enabled = false;
                    DrawCircle(CurrentPosition, CurrentPlayerId);
                }
                else currentPlayersTurn_textbox.Text = "У вас недостаточно денег";
            else currentPlayersTurn_textbox.Text = "Данное действие выполнить невозможно";
            if (Players[CurrentPlayerId].Balance <= 0) Lose();
        }
        private void QuitGameBtn_Click(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show("Завершить работу?", "Выход", MessageBoxButtons.YesNo);
            switch (dialog)
            {
                case DialogResult.Yes:
                    if (Gamemodes.Multiplayer) Stream.Write(
                        Encoding.Unicode.GetBytes(ConnectionOptions.PlayerName + " покидает сервер"), 
                        0, 
                        Encoding.Unicode.GetBytes(ConnectionOptions.PlayerName + " покидает сервер").Length);
                    Disconnect();
                    Application.Exit();
                    break;
                case DialogResult.No:
                    break;
            }
        }
        private void EndTurnBtn_Click(object sender, EventArgs e)
        {
            if (Gamemodes.Multiplayer)
            {
                if (Players[CurrentPlayerId].Loser || Players[CurrentPlayerId].Balance <= 0) Lose();
                var count = 0;
                for (var i = 0; i < 2; i++)
                {
                    if (Players[i].Loser) count++;
                    if (Players[CurrentPlayerId].Loser || count < 1) continue;
                    currentPlayersTurn_textbox.Text = "Вы победили!";
                    switch (CurrentPlayerId)
                    {
                        case 0:
                            if (MessageBox.Show("Красный игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                            break;
                        case 1:
                            if (MessageBox.Show("Синий игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                            break;
                    }
                }
                currentPositionInfo_richtextbox.Text = string.Empty;
                var turnLogString = string.Empty;
                switch (CurrentPlayerId)
                {
                    case 0:
                        turnLogString = "Результаты хода красного игрока: ";
                        break;
                    case 1:
                        turnLogString = "Результаты хода синего игрока: ";
                        break;
                }
                turnLogString += CurrentPlayerId.ToString() + '~' 
                    + Players[CurrentPlayerId].Position + '~' + 
                    Players[CurrentPlayerId].Balance + '~' + 
                    Players[CurrentPlayerId].InJail + '~' + 
                    Players[CurrentPlayerId].Jail + '~';
                foreach (var item in Players[CurrentPlayerId].PropertiesOwned)
                    if (item != 0)
                    {
                        turnLogString += item;
                        turnLogString += ' ';
                    }
                if (turnLogString.Last() is '~') turnLogString += "NULL";
                turnLogString += '~' + Players[CurrentPlayerId].Loser.ToString();
                if (CurrentPlayerId is 0)
                {
                    currentPlayersTurn_textbox.Text = "Сейчас ходит синий игрок, ожидайте. ";
                    Stream.Write(Encoding.Unicode.GetBytes(turnLogString), 0, Encoding.Unicode.GetBytes(turnLogString).Length);
                }
                else
                {
                    currentPlayersTurn_textbox.Text = "Сейчас ходит красный игрок, ожидайте. ";
                    Stream.Write(Encoding.Unicode.GetBytes(turnLogString), 0, Encoding.Unicode.GetBytes(turnLogString).Length);
                }
                throwDiceBtn.Enabled = false;
                buyBtn.Enabled = false;
                endTurnBtn.Enabled = false;
            }
            if (Gamemodes.Singleplayer)
            {
                CurrentPlayerId = CurrentPlayerId is 0 ? 1 : 0;
                switch (CurrentPlayerId)
                {
                    case 0:
                        currentPlayersTurn_textbox.Text = "Ходит красный игрок. ";
                        break;
                    case 1:
                        currentPlayersTurn_textbox.Text = "Ходит синий игрок. ";
                        break;
                }
                throwDiceBtn.Enabled = true;
                buyBtn.Enabled = false;
                if (Players[CurrentPlayerId].InJail)
                {
                    CurrentPosition = 10;
                    MoveIcon(CurrentPosition);
                    Players[CurrentPlayerId].Position = CurrentPosition;
                    InJail(CurrentPlayerId);
                }
                if (Players[CurrentPlayerId].Loser || Players[CurrentPlayerId].Balance <= 0) Lose();
                var count = 0;
                for (var i = 0; i < 2; i++)
                {
                    if (Players[i].Loser) count++;
                    if (Players[CurrentPlayerId].Loser || count < 1) continue;
                    currentPlayersTurn_textbox.Text = "Вы победили!";
                    switch (CurrentPlayerId)
                    {
                        case 0:
                            if (MessageBox.Show("Красный игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                            break;
                        case 1:
                            if (MessageBox.Show("Синий игрок победил!", "Сообщение", MessageBoxButtons.OK) is DialogResult.OK) Application.Exit();
                            break;
                    }
                }
                currentPositionInfo_richtextbox.Text = string.Empty;
            }
        }
    }
}