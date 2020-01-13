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

            Player = new Unit(Vector3.zero, 0, playerChar);
            mainGame = new Game(Player);

            Player.position = mainGame.GetUnitPosistion(Player);

            display.Paint += FrontImage_Paint;

            ResizeBegin += (object sender, EventArgs e) => dontDraw = true;
            ResizeEnd += (object sender, EventArgs e) => dontDraw = false;

            foreach (var key in Enum.GetValues(typeof(Keys)))
            {
                Input.ChangeState((Keys)key, false);
            }

            // Input
            KeyDown += (object sender, KeyEventArgs e) => Input.ChangeState(e.KeyCode, true);
            KeyUp += (object sender, KeyEventArgs e) => Input.ChangeState(e.KeyCode, false);

            // Create a threading example for the game timer
            var gameTime = new System.Windows.Forms.Timer
            {
                Interval = 10
            };
            gameTime.Start();
            gameTime.Tick += GameTime_Tick;

            timerA = DateTime.Now.TimeOfDay.TotalSeconds;
            timerB = DateTime.Now.TimeOfDay.TotalSeconds;

            DrawWorldThread();

            display.Invalidate();
        }

        private void DrawWorldThread()
        {
            if (worldThread != null && worldThread.IsAlive == true && worldThread.ThreadState == ThreadState.Running) return;

            // Start a thread for drawing the world
            worldThread = new Thread(new ThreadStart(UpdateViewportData));
            worldThread.IsBackground = false;
            worldThread.Priority = ThreadPriority.Highest;

            worldThread.Start();
        }

        private void GameTime_Tick(object sender, EventArgs e)
        {
            timerB = DateTime.Now.TimeOfDay.TotalSeconds;
            timeSinceLastFrame = timerA - timerB;
            timerA = timerB;

            foreach (var unit in mainGame.units)
            {
                if (unit.IsStill == false)
                {
                    unit.SetStill();

                    DrawWorldThread();
                }
            }

            HandleMovement((float)timeSinceLastFrame);
            HandleControlInputs();

            // Update the viewPort
            display.Invalidate();
        }

        private void HandleMovement(float elapsedTimeSinceLastFrame)
        {
            float moveSpeed = 2;
            float rotSpeed = 0.8f;
            float bobAmount = 15;
            float bobSpeed = 2;

            float runMultiplier = 2;

            // Walk button
            if (Input.KeyDown(Keys.Space) == false)
            {
                rotSpeed *= runMultiplier;
                moveSpeed *= runMultiplier;
            }

            // Controls

            // Handle rotation
            Player.angle -= Input.horizontal * (rotSpeed * elapsedTimeSinceLastFrame);

            Player.angle = Player.angle % (3.14f * 2);

            // Move player
            float newPlayerX = -((float)Math.Sin(Player.angle) * (Input.vertical * moveSpeed) * elapsedTimeSinceLastFrame);
            float newPlayerY = -((float)Math.Cos(Player.angle) * (Input.vertical * moveSpeed) * elapsedTimeSinceLastFrame);

            var _char = mainGame.mapData[((int)(Player.position.y + newPlayerY) * mainGame.mapWidth + (int)(Player.position.x + newPlayerX))];
            // Collision check
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
                    Player.position.z -= 1;
                }
                else
                if (Player.position.z < 0)
                {
                    Player.position.z += 1;
                }

                bobbingUp = false;
            }
        }

        private void HandleControlInputs()
        {
            if (Input.KeyDown(Keys.F3))
            {
                Debug.Displaying = !Debug.Displaying;
            }
        }

        private void UpdateViewportData()
        {
            if (dontDraw) return;

            var fov = Options.fov;
            var depthOfView = Options.depthOfView;

            Task<Color[,]> colors = Task.Run(() =>
            {
                var value = new Color[screenWidth, screenHeight];

                float distToLastWall = 0;

                // Draw screen
                for (int x = 0; x < screenWidth; x++)
                {
                    // Foreach column, calculate the projected ra angle into world space
                    float rayAngle = (Player.angle - fov / 2.0f) + ((float)x / (float)screenWidth) * fov;
                    float distanceToWall = 0;

                    Color wallColor = Color.Empty;
                    bool boundary = false;

                    // Unit vector for ray in player space
                    float eyeX = (float)Math.Sin(rayAngle);
                    float eyeY = (float)Math.Cos(rayAngle);

                    while (distanceToWall < depthOfView)
                    {
                        distanceToWall += .05f;

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

                                if (wallBrightness > .1f)
                                {
                                    distToLastWall += 0.25f;
                                }

                                if (boundary)
                                {
                                    wallBrightness -= 0.5f;

                                    if (wallBrightness < 0) wallBrightness = 0;

                                    distToLastWall += 0.25f;
                                }
                                else
                                {
                                    brightnessValue = (byte)(255 * wallBrightness);

                                    var brightness = Color.FromArgb(brightnessValue, brightnessValue, brightnessValue, brightnessValue);
                                    value[x, y] = Blend(wallColor, brightness, .8);

                                    continue;
                                }
                            }
                            else
                            {
                                // Floor
                                // Shade the floor
                                float shading = ((nY - screenHeight / 2.0f) / (screenHeight / 2.0f)) / 2f;

                                if (distToLastWall > .1f)
                                {
                                    var floorRelectDOV = depthOfView - 5;

                                    var newShading = shading + (((floorRelectDOV - distanceToWall) / floorRelectDOV) / 4f);

                                    if (newShading > shading) shading = newShading;
                                }

                                brightnessValue = (byte)(255 * shading);

                                distToLastWall -= .5f;

                                if (distToLastWall < 0) distToLastWall = 0;
                            }
                        }

                        if (brightnessValue > 255) brightnessValue = 255;
                        if (brightnessValue < 0) brightnessValue = 0;

                        if (y < value.GetLength(1) && y >= 0)
                        {
                            value[x, y] = Color.FromArgb(brightnessValue, brightnessValue, brightnessValue, brightnessValue); ;
                        }
                    }
                }

                return value;
            });

            viewport = colors.Result;
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

            var resolutionDivider = Options.resolutionDivider;

            if (viewport == null)
            {
                return;
            }

            var graphics = e.Graphics;

            graphics.Clear(Color.Empty);

            var bufferAmount = (Player.IsStill ? 1 : 3);

            // Draw screen
            for (int x = 0; x < screenWidth / bufferAmount; x++)
            {
                for (int y = 0; y < screenHeight / bufferAmount; y++)
                {
                    if (x * bufferAmount >= viewport.GetLength(0) || y * bufferAmount >= viewport.GetLength(1))
                    {
                        DrawWorldThread();
                        continue;
                    }

                    var color = viewport[x * bufferAmount, y * bufferAmount];

                    if (color.IsEmpty || color == Color.Black || color.GetBrightness() < 0.05f) continue;

                    var rectPos = new Rectangle((x * bufferAmount) * resolutionDivider, (y * bufferAmount) * resolutionDivider, bufferAmount * resolutionDivider, bufferAmount * resolutionDivider);
                    graphics.FillRectangle(new SolidBrush(color), rectPos);
                }
            }

            if (Debug.Displaying)
                graphics.DrawString($"FPS={1.0f / timeSinceLastFrame}\n" +
                    $"X:{Player.position.x} Y:{Player.position.y}  ROT:{(360 / 2) / (3.14 / Player.angle)}", SystemFonts.DefaultFont, Brushes.LightSalmon, new RectangleF(10, 10, screenWidth, screenHeight));
        }
    }
}