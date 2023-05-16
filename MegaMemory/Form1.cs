using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Numerics;
using System.IO;
using System.Drawing.Text;
using System.Diagnostics;

namespace MegaMemory
{
    public partial class Form1 : Form
    {
        private List<Card> MasterStackA; // default card stack
        private List<Card> MasterStackB;
        private List<Card> TempStackA; // temp stack for randomization
        private List<Card> TempStackB; // temp stack for randomization
        private List<Card> DealingStackA; // dealing stack 1
        private List<Card> DealingStackB; // dealing stack 2

        private int GameState; // what the game is doing right now
        private int NextGameState; // what the game will be doing when counter has reached 0
        private int OldGameState; // gamestate saved and reinstated after exiting pause
        private int Counter; // general purpose countdown

        private bool ExitingScoresMenuFromGame = false;

        public const int WAIT_SUPER_SHORT = 30;
        public const int WAIT_SHORT = 60;
        public const int WAIT_LONG = 120;

        public const int PLAYER_TYPE_HUMAN = 0;
        public const int AI_EASY = 1;
        public const int AI_AVERAGE = 2;
        public const int AI_HARD = 3;

        public const int GAMESTATE_NONE = 000; // all possible game states
        public const int GAMESTATE_WAITING = 100;
        public const int BEGIN_GAME_ENDED = 120;
        public const int SWAP_PLAYERS = 150;
        public const int BEGIN_PLAYER_TURNING_CARDS = 200;
        public const int PLAYER_TURNING_CARDS = 210;
        public const int PLAYER_MADE_PAIR = 220;

        private double[] IntelligenceTable = new double[4] { 0.0, 0.3, 0.7, 1.0 }; // percentage chances that a computer player will recall a pair or match

        public int BoardSize;
        private Board[] Boards = new Board[] { new Board(3, 0, 4, 5, "small"), new Board(2, 0, 6, 5, "medium"), new Board(1, 0, 8, 5, "large"), new Board(0, 0, 10, 5, "Giagantic") }; // board sizes

        public int WIDTH; // for positioning
        public int HEIGHT;
        public int MIDX;
        public int MIDY;

        private TexturePack GamePack; // TexturePack containing all graphics
        private SoundBank SoundBank; // where we store sounds to play

        private Sprite Stage; // root of display hierarchy

        private Window MainWindow; // windows.. but not microsoft ;)
        private Window ExitDialogWindow;
        private Window NewGameWindow;
        private Window GmeWindow;
        private Window PausedWindow;
        private Window ScoresWindow;
        private Window GameRulesWindow;
        private Window SelectBoardWindow;

        private Window ActiveWindow; // which window is active
        private Sprite ActiveWidget; // widget that was most recently clicked

        private TextField InfoText; // displays ingame info (whose turn it is etc)

        private CheckBox ToggleAudioCheckBox; // this is special because it gets attached and detached to different windows

        private List<TextField> GameRulesPhases; // list of TextFields used to display rules
        private int GameRulesPhase; // which page of text is currently being shown

        private Sprite CardGroup; // all cards get added to this sprite, which in turn is added to the game window

        private Sprite StarGroup; // some star sprites to decorate the screen when a player wins

        private Image FlashA; // effects to overlay when a pair is made
        private Image FlashB;

        private List<Card> TableCards; // list of cards available for a computer player to choose from

        public Random RNG; // random number generator

        private PrivateFontCollection MyFonts; // for utilizing a truetype font that is not installed on the system
        private FontFamily MyFontFamily;

        private Font medFont; // fonts for printing
        private Font largeFont;
        private Font inputFont;

        private SolidBrush TextfieldWhiteBrush; // white colored brush
        private SolidBrush TextfieldShadowBrush; // black colored brush
        private StringFormat TextfieldStringFormat; // for aligning TextFields

        public Rectangle OnpaintDestRect; // objects used when drawing Sprites
        private ImageAttributes OnpaintImageAttrs;
        public ColorMatrix OnpaintColorMatrix;

        private WMPLib.WindowsMediaPlayer MediaPlayer; // for playing music

        private HighScores HighScores; // manages high scores

        private List<string> RandomNames1; // names that will be used for computer players
        private List<string> RandomNames2;

        private string[] PlayerTypes = new string[] { "human", "easy", "average", "hard" }; // the types of player available

        private Player PlayerOne; // player one
        private Image PlayerOneImage;

        private Player PlayerTwo; // player two
        private Image PlayerTwoImage;

        private Player ActivePlayer; // the player who is currently playing
        private Player OtherPlayer;

        private String Path; // the folder where Mega Memory lives

        private static Timer GameTimer; // a timer for our enterFrame event

        public Form1()
        {
            InitializeComponent();

            KeyPreview = true;

            MouseDown += mouseDown;
            MouseUp += mouseUp;
        }


        /// <summary>
        /// Save current player names and cleanup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnpaintImageAttrs.Dispose();
            OnpaintImageAttrs.Dispose();
            GameTimer.Stop();
            GameTimer.Dispose();

            TextfieldShadowBrush.Dispose();
            TextfieldWhiteBrush.Dispose();

            MyFontFamily.Dispose();
            MyFonts.Dispose();

            medFont.Dispose();
            largeFont.Dispose();
            inputFont.Dispose();

            FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + @"\assets\playernames.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine(PlayerOne.Name);
            writer.WriteLine(PlayerTwo.Name);
            writer.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Path = Directory.GetCurrentDirectory(); // cache this for loading files from folders


            //
            // Avoid flickering when repainting form
            //

            DoubleBuffered = true;

            //
            // We will need some randomness, even if it isn't the best implementation
            //

            RNG = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            //
            // Create positioning properties
            //

            WIDTH = ClientRectangle.Width;
            HEIGHT = ClientRectangle.Height;
            MIDX = WIDTH / 2;
            MIDY = HEIGHT / 2;

            //
            // Create stuff for drawing text
            //

            MyFonts = new PrivateFontCollection();
            MyFonts.AddFontFile(Path + @"\assets\fonts\BlankSpace.ttf"); // load custom font
            MyFontFamily = MyFonts.Families[0];

            medFont = new Font(MyFontFamily, 45, FontStyle.Regular, GraphicsUnit.Pixel);
            largeFont = new Font(MyFontFamily, 75, FontStyle.Regular, GraphicsUnit.Pixel);
            inputFont = new Font(MyFontFamily, 60, FontStyle.Regular, GraphicsUnit.Pixel);

            TextfieldWhiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)); // color of text
            TextfieldShadowBrush = new SolidBrush(Color.FromArgb(63, 0, 0, 0)); // color of text shadow
            TextfieldStringFormat = new StringFormat(); // for alignment

            playerOneNameTextBox.Font = inputFont;
            playerTwoNameTextBox.Font = inputFont;

            //
            // Create players
            //

            PlayerOne = new Player();
            PlayerTwo = new Player();

            TableCards = new List<Card>(); // list that computer players can overturn

            //
            // Create list of random names that will be assigned to computer players
            //

            RandomNames1 = new List<string>();
            foreach (string name in File.ReadAllLines(Path + @"\assets\randomnames1.txt"))
            {
                RandomNames1.Add(name);
            }

            RandomNames2 = new List<string>();
            foreach (string name in File.ReadAllLines(Path + @"\assets\randomnames2.txt"))
            {
                RandomNames2.Add(name);
            }

            //
            // Get name of last human players
            //

            StreamReader readingFile = new StreamReader(Path + @"\assets\playernames.txt");
            PlayerOne.Name = readingFile.ReadLine();
            PlayerTwo.Name = readingFile.ReadLine();
            readingFile.Close();

            Debug.WriteLine(PlayerOne.Name);
            Debug.WriteLine(PlayerTwo.Name);

            //
            // All of our images are stored in a Texture Atlas (https://en.wikipedia.org/wiki/Texture_atlas)
            //

            GamePack = new TexturePack("game");

            //
            // Create a SoundBank and populate it with SoundPlayers
            //

            SoundBank = new SoundBank();
            SoundBank.loadSound("deck_shuffle");
            SoundBank.loadSound("button_click");
            SoundBank.loadSound("card_flip");
            SoundBank.loadSound("card_deal");
            SoundBank.loadSound("notify");
            SoundBank.loadSound("highscore");
            SoundBank.loadSound("madepair");

            //
            // Create drawing stuff for redrawing Sprites
            //

            OnpaintDestRect = new Rectangle();
            OnpaintImageAttrs = new ImageAttributes();
            OnpaintColorMatrix = new ColorMatrix();

            //
            // Create card decks
            //

            z_CreateDeck();


            //
            // Create HighScores
            //

            HighScores = new HighScores();

            //
            // Create display hierarchy
            //

            Stage = new Sprite(); // all other game imagery will be attached to this at one time or another

            //
            // Add a background image which will always be visible
            //

            Image background = new Image(GamePack.GetTextureRegion("gui/table.png"));
            background.SetScale(11);
            Stage.AddChild(background);

            //
            // toggle audio on/off checkbox
            // NOTE: this widget is shared amongst all Windows
            //

            ToggleAudioCheckBox = new CheckBox(GamePack.GetTextureRegion("gui/audio_true.png"), GamePack.GetTextureRegion("gui/audio_false.png"), true);
            ToggleAudioCheckBox.OnClicked += toggleAudio;
            ToggleAudioCheckBox.SetAnchorPoint(0.5f, 0.5f);
            ToggleAudioCheckBox.SetPosition(45, 45);

            Image image;
            Button button;
            TextField textfield;
            ToggleButton toggleButton;
            TextureRegion region;

            //
            // Create main window
            //

            MainWindow = new Window();

            // Mega Memory logo
            image = new Image(GamePack.GetTextureRegion("gui/logo.png"));
            image.SetAnchorPoint(0.5f, 0.5f);
            image.SetPosition(MIDX, 170);
            MainWindow.AddChild(image);

            // goto new game window
            button = new Button(GamePack.GetTextureRegion("gui/play_u.png"), GamePack.GetTextureRegion("gui/play_d.png"));
            button.OnButtonUp += openNewGameWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(376, 390);
            MainWindow.AddChild(button);

            // goto scores window button
            button = new Button(GamePack.GetTextureRegion("gui/scores_u.png"), GamePack.GetTextureRegion("gui/scores_d.png"));
            button.OnButtonUp += openScoresWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(906, 390);
            MainWindow.AddChild(button);

            // goto rules window button
            button = new Button(GamePack.GetTextureRegion("gui/rules_u.png"), GamePack.GetTextureRegion("gui/rules_d.png"));
            button.OnButtonUp += openRulesWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(376, 580);
            MainWindow.AddChild(button);

            // exit game button
            button = new Button(GamePack.GetTextureRegion("gui/exit_u.png"), GamePack.GetTextureRegion("gui/exit_d.png"));
            button.OnButtonUp += checkExitGame;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(906, 580);
            MainWindow.AddChild(button);

            Stage.AddChildAt(MainWindow, 1000); // this is the first window so make it active and force form to repaint
            MainWindow.AddChild(ToggleAudioCheckBox);
            ActiveWindow = MainWindow;
            Invalidate();

            //
            // Create exit game confirmation request window
            //

            ExitDialogWindow = new Window();

            // Background
            image = new Image(GamePack.GetTextureRegion("gui/message_window.png"));
            image.SetAnchorPoint(0.5f, 0.5f);
            image.SetPosition(MIDX, MIDY);
            ExitDialogWindow.AddChild(image);

            // Message
            textfield = new TextField(medFont, "Do you really\r\nwant to exit?");
            textfield.SetPosition(MIDX, 230);
            textfield.SetAlignment(StringAlignment.Center);
            ExitDialogWindow.AddChild(textfield);

            // Confirm exit game
            button = new Button(GamePack.GetTextureRegion("gui/yes_u.png"), GamePack.GetTextureRegion("gui/yes_d.png"));
            button.OnButtonUp += exitGame;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(480, 452);
            ExitDialogWindow.AddChild(button);

            // Cancel exit game
            button = new Button(GamePack.GetTextureRegion("gui/no_u.png"), GamePack.GetTextureRegion("gui/no_d.png"));
            button.OnButtonUp += cancelExitGame;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(795, 452);
            ExitDialogWindow.AddChild(button);

            //
            // Create new game window
            //

            NewGameWindow = new Window();

            // Player one pane
            image = new Image(GamePack.GetTextureRegion("gui/player_panel.png"));
            image.SetAnchorPoint(0.5f, 0.5f);
            image.SetPosition(MIDX, 148);
            NewGameWindow.AddChild(image);

            // Player one title
            textfield = new TextField(medFont, "Player One");
            textfield.SetPosition(MIDX, 50);
            textfield.SetAlignment(StringAlignment.Center);
            NewGameWindow.AddChild(textfield);

            // Player one name TextBox background
            image = new Image(GamePack.GetTextureRegion("gui/textbox.png"));
            image.SetAnchorPoint(0.5f, 0.5f);
            image.SetPosition(304, 182);
            NewGameWindow.AddChild(image);

            // Player one type display
            PlayerOneImage = new Image(GamePack.GetTextureRegion("gui/human.png"));
            PlayerOneImage.SetAnchorPoint(0.5f, 0.5f);
            PlayerOneImage.SetPosition(777, 180);
            NewGameWindow.AddChild(PlayerOneImage);

            // Set player one to previous type
            button = new Button(GamePack.GetTextureRegion("gui/sub_u.png"), GamePack.GetTextureRegion("gui/sub_d.png"));
            button.OnButtonUp += z_prevPlayerOneType;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(1061, 180);
            NewGameWindow.AddChild(button);

            // Set player one to next type
            button = new Button(GamePack.GetTextureRegion("gui/add_u.png"), GamePack.GetTextureRegion("gui/add_d.png"));
            button.OnButtonUp += z_nextPlayerOneType;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(1166, 180);
            NewGameWindow.AddChild(button);

            // Player two pane
            image = new Image(GamePack.GetTextureRegion("gui/player_panel.png"));
            image.SetAnchorPoint(0.5f, 0.5f);
            image.SetPosition(MIDX, 424);
            NewGameWindow.AddChild(image);

            // Player two title
            textfield = new TextField(medFont, "Player Two");
            textfield.SetPosition(MIDX, 325);
            textfield.SetAlignment(StringAlignment.Center);
            NewGameWindow.AddChild(textfield);

            // Player two name TextBox background
            image = new Image(GamePack.GetTextureRegion("gui/textbox.png"));
            image.SetAnchorPoint(0.5f, 0.5f);
            image.SetPosition(304, 462);
            NewGameWindow.AddChild(image);

            // Player two type display
            PlayerTwoImage = new Image(GamePack.GetTextureRegion("gui/human.png"));
            PlayerTwoImage.SetAnchorPoint(0.5f, 0.5f);
            PlayerTwoImage.SetPosition(777, 460);
            NewGameWindow.AddChild(PlayerTwoImage);

            // Set player two to previous type
            button = new Button(GamePack.GetTextureRegion("gui/sub_u.png"), GamePack.GetTextureRegion("gui/sub_d.png"));
            button.OnButtonUp += z_prevPlayerTwoType;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(1061, 460);
            NewGameWindow.AddChild(button);

            // Set player two to next type
            button = new Button(GamePack.GetTextureRegion("gui/add_u.png"), GamePack.GetTextureRegion("gui/add_d.png"));
            button.OnButtonUp += z_nextPlayerTwoType;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(1166, 460);
            NewGameWindow.AddChild(button);

            // Exit new game window back to main window
            button = new Button(GamePack.GetTextureRegion("gui/cancel_u.png"), GamePack.GetTextureRegion("gui/cancel_d.png"));
            button.OnButtonUp += closeNewGameWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(MIDX - 200, 640);
            NewGameWindow.AddChild(button);

            // Confirm settings and start new game
            button = new Button(GamePack.GetTextureRegion("gui/next_u.png"), GamePack.GetTextureRegion("gui/next_d.png"));
            button.OnButtonUp += z_openBoardSelectWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(MIDX + 128, 640);
            NewGameWindow.AddChild(button);

            //
            // Create game rules window
            //

            GameRulesWindow = new Window();

            // show prev page
            button = new Button(GamePack.GetTextureRegion("gui/prev_u.png"), GamePack.GetTextureRegion("gui/prev_d.png"));
            button.OnButtonUp += gotoPrevRulesPage;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(304, 620);
            GameRulesWindow.AddChild(button);

            // show next page
            button = new Button(GamePack.GetTextureRegion("gui/next_u.png"), GamePack.GetTextureRegion("gui/next_d.png"));
            button.OnButtonUp += gotoNextRulesPage;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(MIDX, 620);
            GameRulesWindow.AddChild(button);

            // exit rules window back to main window
            button = new Button(GamePack.GetTextureRegion("gui/done_u.png"), GamePack.GetTextureRegion("gui/done_d.png"));
            button.OnButtonUp += closeRulesWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(980, 620);
            GameRulesWindow.AddChild(button);

            // we will cycle through a list of TextFields explain how the game is played
            GameRulesPhases = new List<TextField>();

            textfield = new TextField(medFont, "\r\n\r\nHow To Play Mega Memory\r\n\r\nThe objective of the game is very\r\nsimple.. accumulate the most pairs\r\nby the time all cards are turned over.");
            textfield.SetPosition(MIDX, 30);
            textfield.SetAlignment(StringAlignment.Center);
            GameRulesPhases.Add(textfield);

            textfield = new TextField(medFont, "\r\n\r\nThe match begins with all cards laid face-down\r\non the table and the players take turns at\r\noverturning two cards at a time in the hope\r\nof making a matching pair.\r\n\r\nThe active player is chosen at random.");
            textfield.SetPosition(MIDX, 30);
            textfield.SetAlignment(StringAlignment.Center);
            GameRulesPhases.Add(textfield);

            textfield = new TextField(medFont, "\r\n\r\n\r\nThe active player overturns two cards\r\nand if the cards are the same then they\r\nare removed from the table, the player\r\nis awarded a point, and gets another turn.");
            textfield.SetPosition(MIDX, 30);
            textfield.SetAlignment(StringAlignment.Center);
            GameRulesPhases.Add(textfield);

            textfield = new TextField(medFont, "\r\n\r\nIf the cards were not the same they are\r\nturned back over and it becomes the\r\nother players turn.\r\n\r\nThe match proceeds in this manner until all cards\r\nhave been overturned and made into pairs.");
            textfield.SetPosition(MIDX, 30);
            textfield.SetAlignment(StringAlignment.Center);
            GameRulesPhases.Add(textfield);

            textfield = new TextField(medFont, "\r\n\r\n\r\nThe player with the most pairs wins!!!\r\n\r\nTip: Try to remember which cards\r\nyour opponent overturns");
            textfield.SetPosition(MIDX, 30);
            textfield.SetAlignment(StringAlignment.Center);
            GameRulesPhases.Add(textfield);

            //
            // Create scores display window
            //

            ScoresWindow = new Window();

            textfield = new TextField(largeFont, "Best Scores");
            textfield.SetTextColor(Color.FromArgb(255, 255, 255, 63)); // a yellowish color
            textfield.SetPosition(MIDX, 30);
            textfield.SetAlignment(StringAlignment.Center);
            ScoresWindow.AddChild(textfield);

            // exit scores window back to main window
            button = new Button(GamePack.GetTextureRegion("gui/okay_u.png"), GamePack.GetTextureRegion("gui/okay_d.png"));
            button.OnButtonUp += closeScoresWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(MIDX, 620);
            ScoresWindow.AddChild(button);

            HighScores.sortScores();
            String[] strings = HighScores.getStrings(); // get HighScores formatted into columnar strings

            // left place column
            textfield = new TextField(medFont, strings[0]);
            textfield.SetPosition(70, 190);
            textfield.SetAlignment(StringAlignment.Near);
            ScoresWindow.AddChild(textfield);

            // left names column
            textfield = new TextField(medFont, strings[1]);
            textfield.SetPosition(200, 190);
            textfield.SetAlignment(StringAlignment.Near);
            ScoresWindow.AddChild(textfield);

            // lect scores column
            textfield = new TextField(medFont, strings[2]);
            textfield.SetPosition(MIDX - 100, 190);
            textfield.SetAlignment(StringAlignment.Far);
            ScoresWindow.AddChild(textfield);

            // right place column
            textfield = new TextField(medFont, strings[3]);
            textfield.SetPosition(MIDX + 70, 190);
            textfield.SetAlignment(StringAlignment.Near);
            ScoresWindow.AddChild(textfield);

            // right names column
            textfield = new TextField(medFont, strings[4]);
            textfield.SetPosition(MIDX + 200, 190);
            textfield.SetAlignment(StringAlignment.Near);
            ScoresWindow.AddChild(textfield);

            // right scores column
            textfield = new TextField(medFont, strings[5]);
            textfield.SetPosition(WIDTH - 70, 190);
            textfield.SetAlignment(StringAlignment.Far);
            ScoresWindow.AddChild(textfield);

            //
            // Create game window
            //

            GmeWindow = new Window();

            // player two name/score
            textfield = new TextField(medFont, PlayerTwo.Name);
            PlayerTwo.HUD = textfield;
            textfield.SetPosition(WIDTH - 100, 16);
            textfield.SetAlignment(StringAlignment.Far);
            GmeWindow.AddChild(textfield);

            // player one name/score
            textfield = new TextField(medFont, PlayerOne.Name);
            PlayerOne.HUD = textfield;
            textfield.SetPosition(100, 16);
            textfield.SetAlignment(StringAlignment.Near);
            GmeWindow.AddChild(textfield);

            // info text
            InfoText = new TextField(largeFont, "Info text line");
            InfoText.SetPosition(MIDX, MIDY - 128);
            InfoText.SetAlignment(StringAlignment.Center);

            // forfiet game button
            button = new Button(GamePack.GetTextureRegion("gui/pause_u.png"), GamePack.GetTextureRegion("gui/pause_d.png"));
            button.OnButtonUp += showForfeitMessage;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(WIDTH - 45, 45);
            GmeWindow.AddChild(button);

            // create flash cards used for special effects when a pair is made
            region = GamePack.GetTextureRegion("gui/flasher.png");
            FlashA = new Image(region);
            FlashA.SetAnchorPoint(0.5f, 0.5f);
            FlashA.SetVisible(false);
            GmeWindow.AddChild(FlashA);

            FlashB = new Image(region);
            FlashB.SetAnchorPoint(0.5f, 0.5f);
            FlashB.SetVisible(false);
            GmeWindow.AddChild(FlashB);

            // create sprite to act as container for all cards that are placed on the table
            CardGroup = new Sprite();
            GmeWindow.AddChild(CardGroup);

            // create a bunch of stars to show at the end of game
            region = GamePack.GetTextureRegion("gui/star.png");
            StarGroup = new Sprite();
            for (int i = 0; i < 8; i++)
            {
                image = new Image(region);
                image.SetAnchorPoint(0.5f, 0.5f);
                StarGroup.AddChild(image);
            }

            //
            // Create board selection window
            //

            SelectBoardWindow = new Window();

            // player two name/score
            textfield = new TextField(medFont, "Select Board Size");
            textfield.SetPosition(MIDX, 48);
            textfield.SetAlignment(StringAlignment.Center);
            SelectBoardWindow.AddChild(textfield);

            List<ToggleButton> toggleGroup = new List<ToggleButton>();

            toggleButton = new ToggleButton(GamePack.GetTextureRegion("gui/small_true.png"), GamePack.GetTextureRegion("gui/small_false.png"), toggleGroup, false);
            toggleButton.OnToggled += z_setBoardSize1;
            toggleButton.SetAnchorPoint(0.5f, 0.5f);
            toggleButton.SetPosition(356, 240);
            SelectBoardWindow.AddChild(toggleButton);

            toggleButton = new ToggleButton(GamePack.GetTextureRegion("gui/medium_true.png"), GamePack.GetTextureRegion("gui/medium_false.png"), toggleGroup, true);
            toggleButton.OnToggled += z_setBoardSize2;
            toggleButton.SetAnchorPoint(0.5f, 0.5f);
            toggleButton.SetPosition(926, 240);
            SelectBoardWindow.AddChild(toggleButton);

            BoardSize = 2; // medium by default

            toggleButton = new ToggleButton(GamePack.GetTextureRegion("gui/large_true.png"), GamePack.GetTextureRegion("gui/large_false.png"), toggleGroup, false);
            toggleButton.OnToggled += z_setBoardSize3;
            toggleButton.SetAnchorPoint(0.5f, 0.5f);
            toggleButton.SetPosition(356, 430);
            SelectBoardWindow.AddChild(toggleButton);

            toggleButton = new ToggleButton(GamePack.GetTextureRegion("gui/gigantic_true.png"), GamePack.GetTextureRegion("gui/gigantic_false.png"), toggleGroup, false);
            toggleButton.OnToggled += z_setBoardSize4;
            toggleButton.SetAnchorPoint(0.5f, 0.5f);
            toggleButton.SetPosition(926, 430);
            SelectBoardWindow.AddChild(toggleButton);

            // Confirm board selection and start new game
            button = new Button(GamePack.GetTextureRegion("gui/okay_u.png"), GamePack.GetTextureRegion("gui/okay_d.png"));
            button.OnButtonUp += z_beginNewGame;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(MIDX + 128, 640);
            SelectBoardWindow.AddChild(button);

            // Exit back to main window
            button = new Button(GamePack.GetTextureRegion("gui/cancel_u.png"), GamePack.GetTextureRegion("gui/cancel_d.png"));
            button.OnButtonUp += closeBoardSelectWindow;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(MIDX - 200, 640);
            SelectBoardWindow.AddChild(button);

            //
            // Create forfiet game confirmation request window
            //

            PausedWindow = new Window();

            // Background
            image = new Image(GamePack.GetTextureRegion("gui/message_window.png"));
            image.SetAnchorPoint(0.5f, 0.5f);
            image.SetPosition(MIDX, MIDY);
            PausedWindow.AddChild(image);

            // Message
            textfield = new TextField(medFont, "Game paused\r\nResume or Quit?");
            textfield.SetPosition(MIDX, 230);
            textfield.SetAlignment(StringAlignment.Center);
            PausedWindow.AddChild(textfield);

            // Confirm exit game
            button = new Button(GamePack.GetTextureRegion("gui/quit_u.png"), GamePack.GetTextureRegion("gui/quit_d.png"));
            button.OnButtonUp += forfeitGame;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(830, 452);
            PausedWindow.AddChild(button);

            // Cancel exit game
            button = new Button(GamePack.GetTextureRegion("gui/resume_u.png"), GamePack.GetTextureRegion("gui/resume_d.png"));
            button.OnButtonUp += cancelForfeit;
            button.SetAnchorPoint(0.5f, 0.5f);
            button.SetPosition(510, 452);
            PausedWindow.AddChild(button);





            GameState = GAMESTATE_NONE; // valid states are "none", "menu", "game", and "scores"
            NextGameState = GAMESTATE_NONE;
            Counter = 0;


            //
            // Try to get a windows form to repaint itsself 60 times a second.. it's not a pretty sight.
            //

            GameTimer = new System.Windows.Forms.Timer();
            GameTimer.Interval = 1000 / 60;
            GameTimer.Tick += new System.EventHandler(z_enterFrame);
            GameTimer.Start();

            //
            // SoundPlayer can only play .wav files and can only play one at a time (stops all other sounds when playing a new sound) so we need to use the windows media control to enable music to play alongside sound effects
            //

            MediaPlayer = new WMPLib.WindowsMediaPlayer();
            MediaPlayer.settings.setMode("loop", true);
            MediaPlayer.settings.autoStart = true;
            MediaPlayer.settings.volume = 50;
            MediaPlayer.URL = Path + @"\assets\sounds\music_menu.wav";
        }



        /// <summary>
        /// Toggle audio playback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void toggleAudio(object sender, EventArgs eventArgs)
        {
            CheckBox c = (CheckBox)sender; // cast sender to the correct type
            if (!c.IsChecked())
            {
                MediaPlayer.controls.pause(); // pause music playback
            }
            else
            {
                MediaPlayer.controls.play(); // resume music playback
            }
            SoundBank.enable(c.IsChecked());
        }

        public void playButtonSound(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");

        }

        //
        // Manage MouseDown events
        //

        private void mouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // we only care about left clicks
            {
                if (ActiveWindow != null) // only proceed if there is an active window
                {
                    if (ActiveWindow.IsEnabled())
                    {
                        void checkClick(Sprite sprite)
                        {
                            string type = sprite.GetType().ToString();
                            if (type != "MegaMemory.Sprite") // skip non clickables
                            {
                                if (sprite.ContainsPoint(e.Location)) // was widget clicked?
                                {
                                    switch (type) // should really get the string, split by . delimiter and then make it caps to get a better name. this is however faster
                                    {
                                        case "MegaMemory.Button":
                                            Button b = (Button)sprite; // cast to correct type
                                            b.SetImageState(true); // toggle visual state
                                            ActiveWidget = b; // this is the active widget
                                            b.ButtonDown(); // execute code
                                            Invalidate(); // force all graphics to be redrawn
                                            break;

                                        case "MegaMemory.CheckBox":
                                            CheckBox cb = (CheckBox)sprite;
                                            cb.ToggleCheckState();
                                            cb.CheckboxClicked();
                                            Invalidate();
                                            break;

                                        case "MegaMemory.ToggleButton":
                                            ToggleButton toggleButton = (ToggleButton)sprite;
                                            toggleButton.ToggleCheckState();
                                            toggleButton.TogglebuttonToggled();
                                            Invalidate();
                                            break;

                                        case "MegaMemory.Card":
                                            Card c = (Card)sprite;
                                            ActiveWidget = c;
                                            Debug.WriteLine("mousedown over card");
                                            break;

                                        default:
                                            break;
                                    }
                                    return;
                                }
                            }
                            foreach (Sprite child in sprite.Children)
                            {
                                checkClick(child); // recurse
                            }
                        }

                        checkClick(ActiveWindow); // find and process clicked widget (if any)

                    }
                }
            }
        }

        //
        // Manage MouseUp events
        //

        private void mouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // only process left clicks
            {
                if (ActiveWidget != null) // only check inside active window
                {
                    if (ActiveWidget.ContainsPoint(e.Location)) // mouse released inside widget?
                    {
                        switch (ActiveWidget.GetType().ToString()) // get widgets type as a string 
                        {
                            case "MegaMemory.Button":
                                Button b = (Button)ActiveWidget; // cast to correct type
                                b.SetImageState(false); // set unclicked image state
                                b.ButtonUp(); // execute code
                                Invalidate(); // force all graphics to be redrawn
                                ActiveWidget = null;
                                break;

                            case "MegaMemory.Card":
                                Card c = (Card)ActiveWidget;
                                switch (GameState)
                                {
                                    case PLAYER_TURNING_CARDS: // we are only iterested if a player (in this case a human player) is overturning cards
                                        if (ActivePlayer.PlayerType == PLAYER_TYPE_HUMAN)
                                        {
                                            if (ActivePlayer.CardB == null)
                                            {
                                                if (ActivePlayer.CardA != null)
                                                {
                                                    if (!c.FaceUp) // don;t allow the player to click the same card twice and make a pair from it!
                                                    {
                                                        SoundBank.playSound("card_flip");
                                                        ActivePlayer.CardB = c; // set card
                                                        c.SetFaceUp(true);

                                                        if (ActivePlayer.CardA.FaceValue == ActivePlayer.CardB.FaceValue) // check if cards match
                                                        {
                                                            SoundBank.playSound("madepair");
                                                            PlayerOne.forget(ActivePlayer.CardA); // remove card from player memories
                                                            PlayerOne.forget(ActivePlayer.CardB);
                                                            PlayerTwo.forget(ActivePlayer.CardA);
                                                            PlayerTwo.forget(ActivePlayer.CardB);

                                                            ActivePlayer.Score++; // add to score
                                                            ActivePlayer.HUD.SetText(ActivePlayer.Name + " " + ActivePlayer.Score); // update player HUD

                                                            FlashA.SetPosition(ActivePlayer.CardA.GetX(), ActivePlayer.CardA.GetY());
                                                            FlashB.SetPosition(ActivePlayer.CardB.GetX(), ActivePlayer.CardB.GetY());
                                                            FlashA.SetVisible(true);
                                                            FlashB.SetVisible(true);

                                                            InfoText.SetText(ActivePlayer.Name + " made a pair\r\nand gets another turn"); // display info
                                                            InfoText.SetVisible(true);

                                                            GameState = GAMESTATE_WAITING; // set pause before next phase begins
                                                            NextGameState = PLAYER_MADE_PAIR;
                                                            Counter = WAIT_SHORT;
                                                            Invalidate();
                                                        }
                                                        else
                                                        {
                                                            GameState = GAMESTATE_WAITING;
                                                            NextGameState = SWAP_PLAYERS;
                                                            Counter = WAIT_SHORT;
                                                            Invalidate();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    SoundBank.playSound("card_flip");
                                                    ActivePlayer.CardA = c;
                                                    c.SetFaceUp(true);
                                                    Invalidate();
                                                }
                                            }
                                        }
                                        break;

                                    //
                                    // nothing happens when a card is clicked and the gameState is not one of the above
                                    //

                                    default:
                                        break;
                                }
                                break; // end switch (gameState)

                            default:
                                break;
                        }
                    }
                    else
                    {
                        //
                        // mouse released outside of all widgets but there was a widget clicked previously.. cleanup by returning that widget to its unclicked state
                        //
                        switch (ActiveWidget.GetType().ToString())
                        {
                            case "MegaMemory.Button":
                                Button b = (Button)ActiveWidget;
                                b.SetImageState(false);
                                Invalidate();
                                break;

                            //case "MegaMemory.Card":
                            //    Card c = (Card)activeWidget;
                            //    c.setScale(1);
                            //    Invalidate();
                            //    break;

                            default:
                                break;
                        }
                        ActiveWidget = null;
                    }
                }
            }
        }

        /// <summary>
        /// Repaint the entire form
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //            base.OnPaint(e); // is this even required? seems to work just fine without it

            Graphics graphics = e.Graphics;

            TextField textfield;
            TextureRegion textureRegion;
            Rectangle source;

            GraphicsUnit units = GraphicsUnit.Pixel;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias; // make text nice and smooth

            //
            // Recursivly draw a Sprite hierarchy
            //

            void drawSprite(Sprite sprite)
            {
                if (sprite.IsVisible() && sprite.HasImage) // Sprite is visible and has an image attached to it
                {
                    float px = sprite.GetX();
                    float py = sprite.GetY();

                    float ax = sprite.GetAnchorPositionX();
                    float ay = sprite.GetAnchorPositionY();

                    OnpaintColorMatrix.Matrix33 = sprite.GetAlpha(); // set matrix alpha (opacity of Sprite)
                    OnpaintImageAttrs.SetColorMatrix(OnpaintColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    switch (sprite.GetType().ToString()) // choose what to do based on the Sprites type
                    {
                        case "MegaMemory.TextField":
                            textfield = (TextField)sprite;
                            TextfieldStringFormat.Alignment = textfield.Alignment; // setup alignment
                            graphics.DrawString(textfield.Text, textfield.Font, TextfieldShadowBrush, px + 6, py + 6, TextfieldStringFormat); // draw shadow first
                            graphics.DrawString(textfield.Text, textfield.Font, textfield._SolidBrush, px, py, TextfieldStringFormat);
                            break;

                        default:
                            //
                            // All other types are drawn using their Textureregion
                            //
                            textureRegion = sprite.GetTextureRegion();
                            source = textureRegion.Region; // source Rectangle

                            OnpaintDestRect.X = (int)(px - ax); // set coordinates for dest Rectangle
                            OnpaintDestRect.Y = (int)(py - ay);
                            OnpaintDestRect.Width = (int)(sprite.GetWidth() * sprite.GtScaleX());
                            OnpaintDestRect.Height = (int)(sprite.GetHeight() * sprite.GetScaleY());

                            graphics.DrawImage(textureRegion.Texture, OnpaintDestRect, source.Left, source.Top, source.Width, source.Height, units, OnpaintImageAttrs); // draw it!!
                            break;
                    }
                    //logCat.AppendText("redrew a " + sprite.GetType().ToString() + "\n");
                }
                else // Sprite has no image.. just draw it's children
                {
                    foreach (Sprite child in sprite.Children)
                    {
                        drawSprite(child); // recurse
                    }
                }
            }
            drawSprite(Stage); // draw all sprites attached to the stage
        }

        /// <summary>
        /// Game logic loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void z_enterFrame(object sender, EventArgs e)
        {

            switch (GameState) // determine which mode the game is in
            {
                //
                // counting down to zero whereon a new game state will be set
                //
                case GAMESTATE_WAITING:
                    Counter--;
                    if (Counter == 0)
                    {
                        GameState = NextGameState; // set new state
                        NextGameState = GAMESTATE_NONE;
                    }
                    break;

                //
                // game has ended
                //
                case BEGIN_GAME_ENDED:
                    GameState = GAMESTATE_NONE;

                    HighScores.addScore(new Score(ActivePlayer.Name, ActivePlayer.Score)); // add both players scores
                    HighScores.addScore(new Score(OtherPlayer.Name, OtherPlayer.Score));
                    HighScores.sortScores(); // sort top 10
                    HighScores.saveScores(); // save top 10
                    String[] strings = HighScores.getStrings(); // get formatted highscores
                    for (int i = 0; i <= 5; i++) // set TextFields inscores window with formatted highscores
                    {
                        TextField textField = (TextField)ScoresWindow.GetChildAt(i +2);
                        textField.SetText(strings[i]);
                    }
                    StarGroup.RemoveFromParent();

                    MediaPlayer.URL = Path + @"\assets\sounds\music_scores.wav";
                    ExitingScoresMenuFromGame = true;

                    ActiveWindow = null;
                    GmeWindow.RemoveFromParent();
                    Stage.AddChild(ScoresWindow);
                    ScoresWindow.AddChild(ToggleAudioCheckBox);
                    ActiveWindow = ScoresWindow;
                    Invalidate();
                    break;

                //
                // start player is revealing cards state
                //
                case BEGIN_PLAYER_TURNING_CARDS:
                    if (CardGroup.Children.Count == 0) // if card group has no children.. all pairs have been made so begin end game state
                    {
                        SoundBank.playSound("highscore"); // determine winner
                        if (ActivePlayer.Score == OtherPlayer.Score)
                        {
                            InfoText.SetText("Game drawn, no winner");
                        }
                        else if (ActivePlayer.Score > OtherPlayer.Score)
                        {
                            InfoText.SetText(ActivePlayer.Name + " is the winner");
                        }
                        else
                        {
                            InfoText.SetText(OtherPlayer.Name + " is the winner");
                        }
                        randomizeStars(); // randomize the star pattern
                        GmeWindow.AddChild(StarGroup); // add stars to window
                        GmeWindow.AddChild(InfoText);
                        InfoText.SetVisible(true);
                        GameState = GAMESTATE_WAITING;
                        NextGameState = BEGIN_GAME_ENDED;
                        Counter = WAIT_LONG;
                        Invalidate();
                    }
                    else
                    {
                        ActivePlayer.CardA = null;
                        ActivePlayer.CardB = null;

                        InfoText.SetVisible(false);
                        TableCards.Clear(); // create list of cards that computer players choose from
                        foreach (Sprite sprite in CardGroup.Children)
                        {
                            TableCards.Add((Card)sprite);

                            // the following code enabled the computer players to only choose cards they had not previously chosen.
                            // the result was that the computer players became too hard to beat so I removed it :)
                            //Card card = (Card)sprite;
                            //if (!activePlayer.memory.Contains(card))
                            //{
                            //    tableCards.Add(card);
                            //}
                        }
                        GameState = PLAYER_TURNING_CARDS;
                    }
                    Invalidate();
                    break;

                //
                // player is revealing cards
                //
                case PLAYER_TURNING_CARDS:
                    if (ActivePlayer.PlayerType == PLAYER_TYPE_HUMAN)
                    {
                        // add human stuff here?
                    }
                    else
                    {
                        SoundBank.playSound("card_flip");

                        Card cardA, cardB;

                        if (ActivePlayer.recallPair(RNG.NextDouble()))
                        {
                            cardA = ActivePlayer.CardA;
                            cardB = ActivePlayer.CardB;
                            Debug.WriteLine(ActivePlayer.Name + " remembered a pair: " + cardA.FaceValue + " and " + cardB.FaceValue);
                        }
                        else
                        {
                            cardA = TableCards[RNG.Next(0, TableCards.Count - 1)];
                            TableCards.Remove(cardA); // remove so it cannot be drawn again

                            if (ActivePlayer.recallMatch(cardA, RNG.NextDouble()))
                            {
                                cardB = ActivePlayer.CardB;
                                Debug.WriteLine(ActivePlayer.Name + " remembered a match: " + cardA.FaceValue + " and " + cardB.FaceValue);
                            }
                            else
                            {
                                cardB = TableCards[RNG.Next(0, TableCards.Count - 1)];
                            }
                            Debug.WriteLine(ActivePlayer.Name + " revealed :" + cardA.FaceValue + " and " + cardB.FaceValue);
                        }

                        ActivePlayer.CardA = cardA;
                        ActivePlayer.CardB = cardB;

                        cardA.SetFaceUp(true);
                        cardB.SetFaceUp(true);

                        PlayerOne.remember(cardA); // all players remember what cards were overturned
                        PlayerOne.remember(cardB);

                        PlayerTwo.remember(cardA);
                        PlayerTwo.remember(cardB);

                        if (cardA.FaceValue == cardB.FaceValue)
                        {
                            SoundBank.playSound("madepair");
                            Debug.WriteLine(ActivePlayer.Name + " made a pair");
                            PlayerOne.forget(cardA);
                            PlayerOne.forget(cardB);
                            PlayerTwo.forget(cardA);
                            PlayerTwo.forget(cardB);
                            ActivePlayer.Score++;
                            ActivePlayer.HUD.SetText(ActivePlayer.Name + " " + ActivePlayer.Score);

                            FlashA.SetPosition(ActivePlayer.CardA.GetX(), ActivePlayer.CardA.GetY());
                            FlashB.SetPosition(ActivePlayer.CardB.GetX(), ActivePlayer.CardB.GetY());
                            FlashA.SetVisible(true);
                            FlashB.SetVisible(true);

                            InfoText.SetText(ActivePlayer.Name + " made a pair\r\nand gets another turn");
                            InfoText.SetVisible(true);
                            GameState = GAMESTATE_WAITING;
                            NextGameState = PLAYER_MADE_PAIR;
                            Counter = WAIT_SHORT;
                            Invalidate();
                        }
                        else
                        {
                            GameState = GAMESTATE_WAITING;
                            NextGameState = SWAP_PLAYERS;
                            Counter = WAIT_SHORT;
                            Invalidate();
                        }
                    }
                    break;

                case PLAYER_MADE_PAIR:
                    CardGroup.RemoveChild(ActivePlayer.CardA);
                    CardGroup.RemoveChild(ActivePlayer.CardB);
                    GameState = BEGIN_PLAYER_TURNING_CARDS;

                    FlashA.SetVisible(false);
                    FlashB.SetVisible(false);

                    Invalidate();
                    break;

                case SWAP_PLAYERS:
                    InfoText.SetText(OtherPlayer.Name + "'s turn");
                    InfoText.SetVisible(true);


                    ActivePlayer.CardA.SetFaceUp(false);
                    ActivePlayer.CardB.SetFaceUp(false);
                    Player temp = ActivePlayer;
                    ActivePlayer = OtherPlayer;
                    OtherPlayer = temp;
                    GameState = GAMESTATE_WAITING;
                    NextGameState = BEGIN_PLAYER_TURNING_CARDS;
                    Counter = WAIT_SUPER_SHORT;

                    Invalidate();
                    break;

                 //
                 // all other game modes do nothing
                 //
                default:
                    break;
            }
        }

        //
        // main window widget methods
        //

        public void openNewGameWindow(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            MainWindow.RemoveFromParent();

            z_setPlayerOneType(0);
            z_setPlayerTwoType(0);

            Stage.AddChild(NewGameWindow);
            ActiveWindow = NewGameWindow;
            Invalidate();
        }

        public void openRulesWindow(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ActiveWindow = null;
            MainWindow.RemoveFromParent();

            GameRulesPhase = 0;
            GameRulesWindow.AddChild(GameRulesPhases[0]);

            Stage.AddChild(GameRulesWindow);
            GameRulesWindow.AddChild(ToggleAudioCheckBox);
            ActiveWindow = GameRulesWindow;
            Invalidate();
        }

        public void exitNewGame(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");

        }


        public void openScoresWindow(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ActiveWindow = null;
            MainWindow.RemoveFromParent();

            Stage.AddChild(ScoresWindow);
            ScoresWindow.AddChild(ToggleAudioCheckBox);
            ActiveWindow = ScoresWindow;
            Invalidate();
        }

        //
        // Exit game confirmation request
        //

        public void checkExitGame(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("notify");
            Stage.AddChild(ExitDialogWindow);
            ActiveWindow = ExitDialogWindow;
            Invalidate();
        }

        public void cancelExitGame(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ExitDialogWindow.RemoveFromParent();
            ActiveWindow = MainWindow;
            Invalidate();
        }

        public void exitGame(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            Application.Exit();
        }

        //
        // Forfiet game confirmation request
        //

        public void showForfeitMessage(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("notify");
            OldGameState = GameState;
            GameState = GAMESTATE_NONE;

            Stage.AddChild(PausedWindow);
            ActiveWindow = PausedWindow;
            Invalidate();
        }

        public void cancelForfeit(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ActiveWindow = null;
            PausedWindow.RemoveFromParent();

            GameState = OldGameState;

            ActiveWindow = GmeWindow;
            Invalidate();
        }

        public void forfeitGame(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ActiveWindow = null;
            GmeWindow.RemoveFromParent();
            PausedWindow.RemoveFromParent();

            GameState = GAMESTATE_NONE;

            MainWindow.AddChild(ToggleAudioCheckBox);
            Stage.AddChild(MainWindow);
            ActiveWindow = MainWindow;
            Invalidate();

            MediaPlayer.URL = Path + @"\assets\sounds\music_menu.wav";
        }

        //
        // Exit game rules window back to main window
        //

        public void closeNewGameWindow(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ActiveWindow = null;
            NewGameWindow.RemoveFromParent();

            playerOneNameTextBox.Enabled = false;
            playerOneNameTextBox.Visible = false;

            playerTwoNameTextBox.Enabled = false;
            playerTwoNameTextBox.Visible = false;

            Stage.AddChild(MainWindow);
            MainWindow.AddChild(ToggleAudioCheckBox);
            ActiveWindow = MainWindow;
            Invalidate();
        }

        public void closeBoardSelectWindow(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ActiveWindow = null;
            SelectBoardWindow.RemoveFromParent();

            Stage.AddChild(MainWindow);
            MainWindow.AddChild(ToggleAudioCheckBox);
            ActiveWindow = MainWindow;
            Invalidate();
        }

        //
        // Exit game rules window back to main window
        //

        public void closeScoresWindow(object sender, EventArgs eventArgs)
        {
            if (ExitingScoresMenuFromGame)
            {
                MediaPlayer.URL = Path + @"\assets\sounds\music_menu.wav";
                ExitingScoresMenuFromGame = false;
            }

            SoundBank.playSound("button_click");
            ActiveWindow = null;
            ScoresWindow.RemoveFromParent();

            Stage.AddChild(MainWindow);
            MainWindow.AddChild(ToggleAudioCheckBox);
            ActiveWindow = MainWindow;
            Invalidate();
        }

        //
        // Exit game rules window back to main window
        //

        public void closeRulesWindow(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            ActiveWindow = null;
            GameRulesWindow.RemoveFromParent();
            GameRulesWindow.RemoveChildAt(10000); // remove last added page

            Stage.AddChild(MainWindow);
            MainWindow.AddChild(ToggleAudioCheckBox);
            ActiveWindow = MainWindow;
            Invalidate();
        }

        //
        // Show next page
        //

        public void gotoNextRulesPage(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            int phase = GameRulesPhase;
            if (phase < GameRulesPhases.Count - 1) // don't go past end of list
            {
                GameRulesPhases[phase].RemoveFromParent();
                phase++;
                GameRulesWindow.AddChild(GameRulesPhases[phase]); // add next page
                GameRulesPhase = phase;
                Invalidate();
            }
        }

        //
        // Show prev page
        //
        public void gotoPrevRulesPage(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            int phase = GameRulesPhase;
            if (phase > 0) // don't go past start of list
            {
                GameRulesPhases[phase].RemoveFromParent();
                phase--;
                GameRulesWindow.AddChild(GameRulesPhases[phase]);
                GameRulesPhase = phase;
                Invalidate();
            }
        }


        /// <summary>
        /// Set player one to be previous player type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void z_prevPlayerOneType(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            z_setPlayerOneType(-1);
        }

        /// <summary>
        /// Set player one to be next player type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void z_nextPlayerOneType(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            z_setPlayerOneType(1);
        }

        public void z_openBoardSelectWindow(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            NewGameWindow.RemoveFromParent();

            playerOneNameTextBox.Enabled = false;
            playerOneNameTextBox.Visible = false;

            playerTwoNameTextBox.Enabled = false;
            playerTwoNameTextBox.Visible = false;

            Stage.AddChild(SelectBoardWindow);
            ActiveWindow = SelectBoardWindow;
            Invalidate();
        }


            public void z_beginNewGame(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("deck_shuffle");
            ActiveWindow = null;
            SelectBoardWindow.RemoveFromParent();

            GmeWindow.AddChild(ToggleAudioCheckBox);

            shuffleDeck();

            PlayerOne.reset();
            PlayerOne.HUD.SetText(PlayerOne.Name + " 0");

            PlayerTwo.reset();
            PlayerTwo.HUD.SetText(PlayerTwo.Name + " 0");

            Board board = Boards[BoardSize - 1];

            List<Vector2> rows = new List<Vector2>(); // make x, y pairs for card positions on table

            for (int r = board.Y; r < board.Y + board.Height; r++)
            {
                for (int c = board.X; c < board.X + board.Width; c++)
                {
                    rows.Add(new Vector2(c, r)); // save position
                }
            }

            int x, y;
            for (int i = 0; i < (board.Width * board.Height) / 2; i++)
            {
                Card cardA = DealingStackA[0]; // get next card
                DealingStackA.Remove(cardA); // remove (stop repeated selection)
                Vector2 gridPositionA = rows[RNG.Next(0, rows.Count - 1)]; // get a random x, y grid position on the table
                rows.Remove(gridPositionA); // remove (again to stop repeated selection)
                cardA.GridPosition = gridPositionA; // save position in card
                x = 90 + (int)(gridPositionA.X * 124); // calculate actual x and y position
                y = 156 + (int)(gridPositionA.Y * 124);
                cardA.SetPosition(x, y); // set position
                CardGroup.AddChild(cardA); // add to game window

                Card cardB = DealingStackB[0]; // repeat with matching card from stack b
                DealingStackB.Remove(cardB);
                Vector2 gridPositionB = rows[RNG.Next(0, rows.Count - 1)];
                rows.Remove(gridPositionB);
                cardB.GridPosition = gridPositionB;
                x = 90 + (int)(gridPositionB.X * 124);
                y = 156 + (int)(gridPositionB.Y * 124);
                cardB.SetPosition(x, y);
                CardGroup.AddChild(cardB);
            }



            //// randomly select who goes first
            if (RNG.NextDouble() < 0.5f)
            {
                ActivePlayer = PlayerOne;
                OtherPlayer = PlayerTwo;
            }
            else
            {
                ActivePlayer = PlayerTwo;
                OtherPlayer = PlayerOne;
            }

            GmeWindow.AddChild(InfoText);
            InfoText.SetVisible(true);
            InfoText.SetText(ActivePlayer.Name + "'s turn");
            InfoText.SetTextColor(Color.FromArgb(255, 254, 240, 5));

            GameState = GAMESTATE_WAITING;
            NextGameState = BEGIN_PLAYER_TURNING_CARDS;
            Counter = WAIT_SUPER_SHORT;

            //if (playerTwo.hasNoMorePairs())
            //{
            //    infoText.setText("jxghfkjashfjkhsfsdkaskf");
            //    gameState = GAMESTATE_WAITING;
            //    nextGameState = COMPUTER_REQUESTING_CARD;
            //    counter = WAIT_SHORT;
            //    infoText.setText("");
            //    Invalidate();
            //}
            //else
            //{
            //    gameState = GAMESTATE_WAITING;
            //    nextGameState = COMPUTER_MAKING_PAIRS;
            //    counter = WAIT_SHORT;
            //    infoText.setText(playerTwo.name + " is making pairs");
            //    Invalidate();
            //}
            ////            infoText.setText("Select pairs from hand");




            MediaPlayer.URL = Path + @"\assets\sounds\music_game.wav"; // start ingame music playing

            Stage.AddChild(GmeWindow); // add game window
            ActiveWindow = GmeWindow; // make it active
            Invalidate(); // redraw!
        }

        /// <summary>
        /// Create card stacks
        /// </summary>
        public void z_CreateDeck()
        {
            MasterStackA = new List<Card>(); // unmangled stacks
            MasterStackB = new List<Card>();
            TempStackA = new List<Card>(); // temporary stacks for randomization
            TempStackB = new List<Card>();
            DealingStackA = new List<Card>(); // randomized stacks
            DealingStackB = new List<Card>();

            TextureRegion backRegion = GamePack.GetTextureRegion("back/0.png"); // all cards will have this back image

            for (int v = 0; v < 53; v++) // there are 53 unique cards (only 25 pairs are used per game)
            {
                MasterStackA.Add(new Card(v, GamePack.GetTextureRegion("face/" + v + ".png"), backRegion)); // create new card and add to master stacks
                MasterStackB.Add(new Card(v, GamePack.GetTextureRegion("face/" + v + ".png"), backRegion));
            }
        }

        /// <summary>
        /// Randomize dealing stacks
        /// </summary>
        public void shuffleDeck()
        {
            TempStackA.Clear(); // empty temp stack
            foreach (Card card in MasterStackA) // copy cards from master stack to temp stack
            {
                card.RemoveFromParent(); // remove incase it is still attached to game window
                card.SetFaceUp(false); // set card face down
                TempStackA.Add(card); // add to temp stack
            }

            TempStackB.Clear(); // repeat for stack b (matching pair stack)
            foreach (Card card in MasterStackB)
            {
                card.RemoveFromParent();
                card.SetFaceUp(false);
                TempStackB.Add(card);
            }

            DealingStackA.Clear(); // empty dealing stacks
            DealingStackB.Clear();
            for (int i = 0; i < 53; i++) // add cards into dealing deck in random order
            {
                int n = RNG.Next(0, TempStackA.Count - 1);
                Card cardA = TempStackA[n]; // get random card from master stacks
                Card cardB = TempStackB[n];
                TempStackA.Remove(cardA); // remove card from temp stacks (prevent multiple selection/addition)
                TempStackB.Remove(cardB);
                DealingStackA.Add(cardA); // add to dealing stacks
                DealingStackB.Add(cardB);
            }
        }

        public void z_setBoardSize1(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            BoardSize = 1;
        }

        public void z_setBoardSize2(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            BoardSize = 2;
        }

        public void z_setBoardSize3(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            BoardSize = 3;
        }

        public void z_setBoardSize4(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            BoardSize = 4;
        }

        /// <summary>
        /// Cycle player one type
        /// </summary>
        /// <param name="n"></param>
        private void z_setPlayerOneType(int n)
        {
            PlayerOne.PlayerType = (PlayerOne.PlayerType + n) & 3; // cycle between 0 and 3
            PlayerOneImage.SetTextureRegion(GamePack.GetTextureRegion("gui/" + PlayerTypes[PlayerOne.PlayerType] + ".png")); // set graphic to display correct type
            if (PlayerOne.PlayerType == PLAYER_TYPE_HUMAN) // human player
            {
                playerOneNameTextBox.Text = PlayerOne.Name;
                playerOneNameTextBox.ReadOnly = false;
                playerOneNameTextBox.Enabled = true;
            }
            else // computer player
            {
                PlayerOne.Intelligence = IntelligenceTable[PlayerOne.PlayerType]; // set intelligence
                playerOneNameTextBox.Text = RandomNames1[RNG.Next(0, RandomNames1.Count)]; // select a random name
                PlayerOne.Name = playerOneNameTextBox.Text;
                playerOneNameTextBox.ReadOnly = true;
                playerOneNameTextBox.Enabled = false;
            }
            playerOneNameTextBox.Visible = true;
        }

        /// <summary>
        /// Set player one to be previous player type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void z_prevPlayerTwoType(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            z_setPlayerTwoType(-1);
        }

        /// <summary>
        /// Set player one to be next player type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void z_nextPlayerTwoType(object sender, EventArgs eventArgs)
        {
            SoundBank.playSound("button_click");
            z_setPlayerTwoType(1);
        }

        /// <summary>
        /// Cycle player two type
        /// </summary>
        /// <param name="n"></param>
        private void z_setPlayerTwoType(int n)
        {
            PlayerTwo.PlayerType = (PlayerTwo.PlayerType + n) & 3; // cycle between 0 and 3
            PlayerTwoImage.SetTextureRegion(GamePack.GetTextureRegion("gui/" + PlayerTypes[PlayerTwo.PlayerType] + ".png")); // set graphic to display correct type
            if (PlayerTwo.PlayerType == PLAYER_TYPE_HUMAN) // human player
            {
                playerTwoNameTextBox.Text = PlayerTwo.Name;
                playerTwoNameTextBox.ReadOnly = false;
                playerTwoNameTextBox.Enabled = true;
            }
            else // computer player
            {
                PlayerTwo.Intelligence = IntelligenceTable[PlayerOne.PlayerType]; // set intelligence
                playerTwoNameTextBox.Text = RandomNames2[RNG.Next(0, RandomNames2.Count)];
                PlayerTwo.Name = playerTwoNameTextBox.Text;
                playerTwoNameTextBox.ReadOnly = true;
                playerTwoNameTextBox.Enabled = false;
            }
            playerTwoNameTextBox.Visible = true;
        }

        /// <summary>
        /// Set layer one name, and suppress system beep on ENTER key pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setPlayerOneName(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                if (playerOneNameTextBox.Text.ToLower() == PlayerTwo.Name)
                {
                        playerOneNameTextBox.Text += "1";
                }
                PlayerOne.Name = playerOneNameTextBox.Text;
            }
        }

        /// <summary>
        /// Set player two name, and suppress system beep on ENTER key pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setPlayerTwoName(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                if (playerTwoNameTextBox.Text.ToLower() == PlayerOne.Name)
                {
                    playerTwoNameTextBox.Text += "2";
                }
                PlayerTwo.Name = playerTwoNameTextBox.Text;
            }
        }

        public void randomizeStars()
        {
            int x1 = 184;
            int y1 = 288;
            for (int c = 0; c < 8; c++)
            {
                Sprite sprite = StarGroup.GetChildAt(c);
                int x2 = x1 + RNG.Next(-32, 32);
                int y2 = y1 + RNG.Next(-64, 128);

                sprite.SetPosition(x2, y2);
                x1 += 128;
            }
        }



        /// <summary>
        /// Set player name when textbox loses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playerTwoNameTextBox_Leave(object sender, EventArgs e)
        {
            if (playerTwoNameTextBox.Text.ToLower() == PlayerOne.Name)
            {
                playerTwoNameTextBox.Text += "1";
            }
            PlayerTwo.Name = playerTwoNameTextBox.Text;
        }

        /// <summary>
        /// Set player name when textbox loses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playerOneNameTextBox_Leave(object sender, EventArgs e)
        {
            if (playerOneNameTextBox.Text.ToLower() == PlayerTwo.Name)
            {
                playerOneNameTextBox.Text += "1";
            }
            PlayerOne.Name = playerOneNameTextBox.Text;
        }
    }
}
