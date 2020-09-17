using JunoEngine.Systems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using FirstPersonGameEngine.Systems;

namespace FirstPersonGameEngine
{
    public partial class MainForm : Form
    {
        private readonly Unit Player;
        private readonly Game mainGame;

        private const char playerChar = 'p';

        private bool bobbingUp = true;

        private int screenWidth
        {
            get
            {
                var resolutionDivider = Options.resolutionDivider;

                return this.Width / resolutionDivider;
            }
        }

        private int screenHeight
        {
            get
            {
                var resolutionDivider = Options.resolutionDivider;

                return this.Height / resolutionDivider;
            }
        }

        private double timerA, timerB;

        private double timeSinceLastFrame;

        private bool dontDraw;

        private Color[,] viewport;

        private Thread worldThread;

        public MainForm()
        {
            InitializeComponent();

            // Create the game space
            Player = new Unit(Vector3.zero, 180, playerChar);
            mainGame = new Game(Player);
            Player.position = mainGame.GetUnitPosistion(Player);

            display.Paint += FrontImage_Paint;

            ResizeBegin += (object sender, EventArgs e) => dontDraw = true;
            ResizeEnd += (object sender, EventArgs e) => dontDraw = false;

            // Input
            foreach (var key in Enum.GetValues(typeof(Keys))) Input.ChangeState((Keys)key, false); // Add all keys to the keytable. (This will allow each keypress to be detected on key press instead of on second keypress.)
            // Update the keytable when a key is pressed
            KeyDown += (object sender, KeyEventArgs e) => Input.ChangeState(e.KeyCode, true);
            KeyUp += (object sender, KeyEventArgs e) => Input.ChangeState(e.KeyCode, false);

            // Create the game timer
            var gameTime = new System.Windows.Forms.Timer
            {
                Interval = 10
            };
            gameTime.Tick += GameTime_Tick;

            // Initialize the frame rate counters
            timerA = DateTime.Now.TimeOfDay.TotalSeconds;
            timerB = DateTime.Now.TimeOfDay.TotalSeconds;

            // Start the game
            gameTime.Start();
        }

        /// <summary>
        /// Creates a thread to draw the world
        /// </summary>
        private void DrawWorldThread()
        {
            if (worldThread != null && worldThread.IsAlive == true && worldThread.ThreadState == ThreadState.Running) return;

            // Start a thread for drawing the world
            worldThread = new Thread(new ThreadStart(UpdateViewportData));
            worldThread.IsBackground = false;
            worldThread.Priority = ThreadPriority.Highest;

            worldThread.Start();
        }

        /// <summary>
        /// Update tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTime_Tick(object sender, EventArgs e)
        {
            // Get the frame rate
            timerB = DateTime.Now.TimeOfDay.TotalSeconds;
            timeSinceLastFrame = timerA - timerB;
            timerA = timerB;

            Debug.RecieveFrameTime(timeSinceLastFrame);

            // Check all units for any change.
            // (This would not be optimal for larger games. Instead just draw the world every update.)
            foreach (var unit in mainGame.units)
            {
                // If there was a change, update the screen
                if (unit.IsStill == false)
                {
                    unit.SetStill();

                    DrawWorldThread();
                }
            }

            // Get the player input
            HandleMovement((float)timeSinceLastFrame);
            HandleControlInputs();

            // Update the viewPort
            display.Invalidate();
        }

        private void HandleMovement(float elapsedTimeSinceLastFrame)
        {
            float moveSpeed = 2;
            float rotSpeed = 0.8f;
            float bobAmount = 2.5f;
            float bobSpeed = .5f;

            float runMultiplier = 2;

            // Walk button
            if (Input.KeyDown(Keys.Space) == false)
            {
                rotSpeed *= runMultiplier;
                moveSpeed *= runMultiplier;

                bobAmount *= runMultiplier;
                bobSpeed *= runMultiplier;
            }

            // Controls

            // Handle rotation
            Player.angle -= Input.horizontal * (rotSpeed * elapsedTimeSinceLastFrame);

            Player.angle = Player.angle % (3.14f * 2);

            // Move player
            float newPlayerX = -((float)Math.Sin(Player.angle) * (Input.vertical * moveSpeed) * elapsedTimeSinceLastFrame);
            float newPlayerY = -((float)Math.Cos(Player.angle) * (Input.vertical * moveSpeed) * elapsedTimeSinceLastFrame);

            // Collision check
            var _char = mainGame.mapData[((int)(Player.position.y + newPlayerY) * mainGame.mapWidth + (int)(Player.position.x + newPlayerX))];
            if (char.IsDigit(_char) == false)
            {
                Player.position.x += newPlayerX;
                Player.position.y += newPlayerY;
            }

            // Head bob
            if (Player.IsStill == false)
            {
                if (bobbingUp && Player.position.z < bobAmount)
                {
                    Player.position.z += bobSpeed;
                }
                else
                {
                    bobbingUp = false;
                    if (Player.position.z > -bobAmount)
                    {
                        Player.position.z -= bobSpeed;
                    }
                    else
                    {
                        bobbingUp = true;
                    }
                }
            }
            else
            {
                if (Player.position.z > 0)
                {
                    Player.position.z -= bobSpeed;
                }
                else
                if (Player.position.z < 0)
                {
                    Player.position.z += bobSpeed;
                }

                bobbingUp = false;
            }
        }

        /// <summary>
        /// Extra button actions
        /// </summary>
        private void HandleControlInputs()
        {
            if (Input.KeyDown(Keys.F3))
            {
                Debug.Displaying = !Debug.Displaying;
            }

            if (Input.KeyDown(Keys.F2))
            {
                Options.floorReflections = !Options.floorReflections;
                DrawWorldThread();
            }

            if (Input.KeyDown(Keys.F5))
            {
                Options.depthOfView++;
                DrawWorldThread();
            }
            else if (Input.KeyDown(Keys.F4))
            {
                Options.depthOfView--;
                DrawWorldThread();
            }

            if (Input.KeyDown(Keys.F6))
            {
                Options.floorReflection -= 0.1f;
                DrawWorldThread();
            }
            else if (Input.KeyDown(Keys.F7))
            {
                Options.floorReflection += 0.1f;
                DrawWorldThread();
            }
            else if (Input.KeyDown(Keys.F8))
            {
                Options.motionBlur = Options.motionBlur == 3 ? 1 : 3;
                DrawWorldThread();
            }

            if (Input.KeyDown(Keys.Escape))
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Updates the current display data
        /// </summary>
        private void UpdateViewportData()
        {
            if (dontDraw) return;

            // Get the options from the Options manager
            var fov = Options.fov;
            var depthOfView = Options.depthOfView;

            // Get the colors from the screen
            viewport = new Color[screenWidth, screenHeight];

            Color wallColor = Color.Empty;
            float distToLastWall = 0;

            // Draw screen
            for (int x = 0; x < screenWidth; x++)
            {
                // Foreach column, calculate the projected ra angle into world space
                float rayAngle = (Player.angle - fov / 2.0f) + ((float)x / (float)screenWidth) * fov;
                float distanceToWall = 0;

                bool boundary = false;

                // Unit vector for ray in player space
                float eyeX = (float)Math.Sin(rayAngle);
                float eyeY = (float)Math.Cos(rayAngle);

                while (distanceToWall < depthOfView)
                {
                    distanceToWall += (distanceToWall * 0.05f) + 0.01f;

                    int testX = (int)(Player.position.x + eyeX * distanceToWall);
                    int testY = (int)(Player.position.y + eyeY * distanceToWall);

                    // Test if ray is out of bounds
                    if (testX < 0 || testX >= mainGame.mapWidth || testY < 0 || testY >= mainGame.mapHeight)
                    {
                        // Set distance to maximum depth
                        distanceToWall = depthOfView;

                        break;
                    }
                    else
                    {
                        var character = mainGame.mapData[testY * mainGame.mapWidth + testX];

                        if (char.IsDigit(character))
                        {
                            wallColor = mainGame.GetWallColor(character);

                            var pairs = new List<Pair<float, float>>();

                            for (int boundaryX = 0; boundaryX < 2; boundaryX++)
                            {
                                for (int boundaryY = 0; boundaryY < 2; boundaryY++)
                                {
                                    float vectorY = (float)testY + boundaryY - Player.position.y;
                                    float vectorX = (float)testX + boundaryX - Player.position.x;

                                    float dist = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
                                    float dot = (eyeX * vectorX / dist) + (eyeY * vectorY / dist);

                                    pairs.Add(new Pair<float, float>(dist, dot));
                                }
                            }

                            // Sort the pairs from closest to furthest.
                            pairs.Sort((a, b) =>
                            {
                                return (a.First < b.First) ? -1 : 1;
                            });

                            float bound = 0.008f;
                            if (Math.Acos(pairs[0].Second) < bound) boundary = true;
                            else
                            if (Math.Acos(pairs[1].Second) < bound) boundary = true;
                            else
                            if (Math.Acos(pairs[2].Second) < bound) boundary = true;

                            break;
                        }
                    }
                }

                // Calculate the distance to ceiling and floor
                int ceiling = (int)((screenHeight / 2f) - screenHeight / distanceToWall);
                int floor = screenHeight - ceiling;

                if (distanceToWall > depthOfView) distanceToWall = depthOfView;

                float wallBrightness = (depthOfView - distanceToWall) / depthOfView;

                for (int y = 0; y < screenHeight; y++)
                {
                    byte brightnessValue = 0;
                    var nY = y;
                    nY += (int)MathF.Round(Player.position.z);

                    if (nY <= ceiling)
                    {
                        // Ceiling
                        // Shade the ceiling
                        float shading = 1 - ((nY + screenHeight / 2.0f) / (screenHeight / 2.0f)) / 2;

                        brightnessValue = (byte)(255 * (shading / 1.5f));
                    }
                    else
                    {
                        if (nY > ceiling && nY < floor)
                        {
                            // Wall
                            distToLastWall += 0.25f;

                            if (boundary)
                            {
                                wallBrightness -= 0.5f;

                                if (wallBrightness < 0) wallBrightness = 0;
                            }
                            else
                            {
                                brightnessValue = (byte)(255 * wallBrightness);

                                var brightness = Color.FromArgb(brightnessValue, brightnessValue, brightnessValue, brightnessValue);
                                viewport[x, y] = Blend(wallColor, brightness, .8);

                                continue;
                            }
                        }
                        else
                        {
                            // Floor
                            if (Options.floorReflections)
                            {
                                // Shade the floor
                                float shading = ((nY - screenHeight / 2.0f) / (screenHeight / 2.0f)) / 2f;

                                if (distToLastWall > .01f)
                                {
                                    var floorRelectDOV = depthOfView - 5;

                                    var newShading = shading + (((floorRelectDOV - distanceToWall) / floorRelectDOV) / 4f);

                                    if (newShading > shading) shading = newShading;
                                }

                                brightnessValue = (byte)(255 * shading);

                                distToLastWall -= Options.floorReflection;

                                if (distToLastWall < 0) distToLastWall = 0;
                            }
                            else
                            {
                                brightnessValue = 0;
                            }
                        }
                    }

                    if (brightnessValue > 255) brightnessValue = 255;
                    if (brightnessValue < 0) brightnessValue = 0;

                    if (y < viewport.GetLength(1) && y >= 0)
                    {
                        float b = brightnessValue / 255f;

                        viewport[x, y] = Color.FromArgb(brightnessValue, (int)(wallColor.R * b),
                            (int)(wallColor.G * b), (int)(wallColor.B * b));
                    }
                }
            }
        }

        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        private void FrontImage_Paint(object sender, PaintEventArgs e)
        {
            if (dontDraw) return;
            if (viewport == null) return; // Dont attempt to draw if there is no pallete to draw from.

            var graphics = e.Graphics;

            graphics.Clear(Color.Empty);

            var renderScale = Options.resolutionDivider;
            var motionBlur = (Player.IsStill ? 1 : Options.motionBlur);

            // Draw screen
            for (int x = 0; x < screenWidth / motionBlur; x++)
            {
                for (int y = 0; y < screenHeight / motionBlur; y++)
                {
                    // Check if the screen has been resized and recalculate the pallete if so.
                    if (x * motionBlur >= viewport.GetLength(0) || y * motionBlur >= viewport.GetLength(1))
                    {
                        DrawWorldThread();
                        continue;
                    }

                    // Get the color from the pallete
                    var color = viewport[x * motionBlur, y * motionBlur];

                    // Dont draw if color is black/empty
                    if (color.IsEmpty || color == Color.Black || color.GetBrightness() < 0.05f) continue;

                    var rectPos = new Rectangle((x * motionBlur) * renderScale, (y * motionBlur) * renderScale, motionBlur * renderScale, motionBlur * renderScale);
                    graphics.FillRectangle(new SolidBrush(color), rectPos);
                }
            }

            if (Debug.Displaying)
                graphics.DrawString($"FPS={Debug.frameRate}\n" +
                    $"AFPS={Debug.avgFrameRate}\n" +
                    $"LFPS={Debug.lowestFrameRate}\n" +
                    $"HFPS={Debug.highestFrameRate}\n" +
                    $"X:{(int)Player.position.x} Y:{(int)Player.position.y} Z:{(int)Player.position.z}\n" +
                    $"ROT:{(int)((360 / 2) / (3.14 / Player.angle))}", SystemFonts.DefaultFont, Brushes.LightSalmon, new RectangleF(10, 10, screenWidth, screenHeight));
        }
    }
}