using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BubbleTrouble
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        //GravitySprite s;
        List<Bubble> b = new List<Bubble>();
        Line timebar, livebar;

        int score = 0; bool justDied = false;
        int scoreToTimeRatio = 1, level = 1;

        Li Andrew;

        MouseState MState, OldState;
        KeyboardState KState, OldKState;

        System.Diagnostics.Stopwatch Watch = new System.Diagnostics.Stopwatch();
            
        public const float gravityScale = 1.0f;
        
        public static int WIDTH;
        public static int HEIGHT;
        double currentTime = 880;
        double timeSpeed = .5;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            WIDTH = Convert.ToInt16(parseWords(File.ReadAllLines("dimensions.txt")[0])[0]);
            HEIGHT = Convert.ToInt16(parseWords(File.ReadAllLines("dimensions.txt")[1])[0]);
            setGraphics(WIDTH, HEIGHT);

            Andrew = new Li(new Vector2(10f, HEIGHT - 80), "Ben", 9, GraphicsDevice);
            
            InitializeLevel();

            //timebar = new Line(GraphicsDevice, 10f, Color.Brown, new Vector2(10, 50), new Vector2(10 + (float)currentTime, 50));
            //livebar = new Line(GraphicsDevice, 10f, Color.Red, new Vector2(10, 10), new Vector2(10 + Andrew.lives * 20, 10));
            
            base.Initialize();
        }

        private void InitializeLevel()
        {
            currentTime = 890;

            string[] lines = new string[0];

            try
            {
                lines = File.ReadAllLines("level" + level + ".txt");
            }
            catch (FileNotFoundException e)
            {
                ExitGame();
            }

            for (int i = 0; i < lines.Length; i++)
            {
                b.Add(getBubbleFromLine(parseWords(lines[i])));
            }
        }

        /// <summary>
        /// Given an array of words, uses the data to form a bubble with the data.
        /// </summary>
        /// <param name="line">The array of words as parameters.</param>
        /// <returns>The bubble with the parameters.</returns>
        private Bubble getBubbleFromLine(string[] line)
        {
            try
            {
                Bubble x = new Bubble(new Vector2(Convert.ToInt16(line[0]), Convert.ToInt16(line[1])), "DefaultBall",
                    new Vector2((float)Convert.ToDouble(line[2]), (float)Convert.ToDouble(line[3])), 1f, Convert.ToInt16(line[4]), Color.Blue);
                Andrew.Position = new Vector2((float)Convert.ToDouble(line[5]), Andrew.Position.Y);
                return x;
            }
            catch (IndexOutOfRangeException e)
            {
                return Bubble.Default;
            }
        }

        /// <summary>
        /// Splits up a string into the words, separated by " ".
        /// </summary>
        /// <param name="s">The string to be split up.</param>
        /// <returns>An array of the words in the string.</returns>
        private string[] parseWords(string s)
        {
            List<string> words = new List<string>();
            string word = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s.ElementAt<char>(i) == ' ')
                {
                    words.Add(word);
                    word = "";
                }
                else
                {
                    word += s.ElementAt<char>(i);
                }
            }
            words.Add(word);
            word = "";
            return words.ToArray();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach(Bubble b1 in b)
                b1.LoadContent(Content);

            font = Content.Load<SpriteFont>("Font");

            Andrew.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MState = Mouse.GetState();
            KState = Keyboard.GetState();

            if (Andrew.isAlive)
            {
                Andrew.UpdateMovement(KState, OldKState, Content);
                foreach (Bubble b1 in b)
                    b1.MoveNaturallyBubble();
            }
            
            CheckGettingHit();
            UpdateTimeAndEndLevels();

            if(Andrew.shooting)
                UpdateShooting();

            if (OldKState != KState)
                Andrew.LoadContent(Content);

            
            OldState = Mouse.GetState();
            OldKState = Keyboard.GetState();
            base.Update(gameTime);
        }
        
        
        /// <summary>
        /// Checks whether a) time has expired or b) all bubbles have been popped
        /// </summary>
        private void UpdateTimeAndEndLevels()
        {
            // Time has not expired, there are still bubbles in the court, and Andrew Li is alive.
            if (currentTime > 0 && b.Count > 0 && Andrew.isAlive)
            {
                // General gameplay
                currentTime -= timeSpeed;
                timebar = new Line(GraphicsDevice, 40f, Color.Brown, new Vector2(10, 50), new Vector2(10 + (float)currentTime, 50));
                livebar = new Line(GraphicsDevice, 10f, Color.Red, new Vector2(10, 10), new Vector2(10 + Andrew.lives * 20, 10));
            }
            // Time is up.
            else if(currentTime <= 0 && b.Count > 0)
            {
                Andrew.isAlive = false;
                //Andrew.lives--;
                if (hasWaited(2))
                {
                    LoadLevel(1);
                }
            }
            // All bubbles have been popped.
            else if (b.Count == 0)
            {
                Andrew.isAlive = false;
                if (hasWaited(2))
                {
                    currentTime -= 5;
                    score += 5*scoreToTimeRatio;
                    timebar = new Line(GraphicsDevice, 40f, Color.Brown, new Vector2(10, 50), new Vector2(10 + (float)currentTime, 50));
                }
                if (currentTime <= 0)
                {
                    score += (int)currentTime * scoreToTimeRatio; 
                    currentTime = 0;

                    LoadLevel(level + 1);
                }
            }
        }

        /// <summary>
        /// Method to end the level, restart all the bubbles to the beginning of level one.
        /// </summary>
        private void LoadLevel(int l)
        {
            level = l;
            Andrew.isAlive = true;
            b = new List<Bubble>();
            InitializeLevel();
            Watch.Reset();
        }
        
        
        /// <summary>
        /// Updating the shooting aspect.
        /// </summary>
        public void UpdateShooting()
        {
            int n = isBubbleBeingHit();
            
            if (Andrew.shotlength >= Game1.HEIGHT)
            {
                // If the shot hits the top of the screen
                Andrew.shooting = false;
                Andrew.shotlength = 0;
            }
            else if (n >= 0)
            {
                // If the shot hits a bubble
                PopBubble(n);
                Andrew.shooting = false;
                Andrew.shotlength = 0;
            }
            else if (Andrew.isAlive)
            {
                // If he's still alive and the shot hasn't hit anything
                Andrew.shoot();
            }
        }

        /// <summary>
        /// Checks if Andrew is being hit by a ball, and does the methods required post-mortem.
        /// </summary>
        private void CheckGettingHit()
        {
            for (int i = 0; i < b.Count; i++)
            {
                if (b[i].Position.X > Andrew.Position.X - b[i].width && b[i].Position.X < Andrew.Position.X + Andrew.width)
                {
                    if (b[i].Position.Y > Andrew.Position.Y - b[i].height && b[i].Position.Y < Andrew.Position.Y + Andrew.height)
                    {
                        // He just died.
                        if (Andrew.lives >= 0)
                        {
                            Andrew.isAlive = false;
                            if (!justDied)
                            {
                                // He just died, and he lost a life.
                                Andrew.lives--;
                                justDied = true;
                            }
                            // After two seconds, restart the game.
                            if (hasWaited(2))
                            {
                                LoadLevel(level);
                                // He has been revived! :O
                                justDied = false;
                            }
                        }
                        else
                        {
                            if (hasWaited(2))
                                ExitGame();
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Checks which bubble is being hit.
        /// </summary>
        /// <returns>The index of the bubble in b is being hit.</returns>
        private int isBubbleBeingHit()
        {
            Line S = Andrew.Shot;
            for (int i = 0; i < b.Count; i++)
            {
                //if(Math.Pow(S.linept2.X - b[i].Origin.X, 2) + )

                if (S.linept2.X >= b[i].Position.X && S.linept2.X <= b[i].Position.X + b[i].width)
                {
                    if (S.linept2.Y <= b[i].Position.Y + b[i].height)
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Pops the bubble.
        /// </summary>
        /// <param name="bubbleNumber">The index of the bubble that is popped.</param>
        private void PopBubble(int bubbleNumber)
        {
            Bubble kBubble = b[bubbleNumber];
            if (kBubble.bounceHeight >= 210)
            {
                b.Add(new Bubble(new Vector2(kBubble.Position.X + 5, kBubble.Position.Y), "DefaultBall", new Vector2(Math.Abs(kBubble.Velocity.X), getYVelocity(kBubble.Velocity.Y)), 1f, (int)kBubble.bounceHeight / 2, kBubble.myColor));
                b.Add(new Bubble(new Vector2(kBubble.Position.X - kBubble.width - 5, kBubble.Position.Y), "DefaultBall", new Vector2(-Math.Abs(kBubble.Velocity.X), getYVelocity(kBubble.Velocity.Y)), 1f, (int)kBubble.bounceHeight / 2, kBubble.myColor));
            }
            b.Remove(b[bubbleNumber]);
        }

        /// <summary>
        /// Used for determining the y-velocity of a just-popped bubble.
        /// </summary>
        /// <param name="currentY">The current y-velocity of the bubble.</param>
        /// <returns>The y-velocity of the bubble of given y-velocity.</returns>
        private float getYVelocity(float currentY)
        {
            if (currentY > 0)
            {
                return -8;
            }
            else
            {
                return -15;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            foreach(Bubble b1 in b)
                b1.DrawAndLoadContent(spriteBatch, Content);
            Andrew.DrawPlayer(spriteBatch);
            timebar.DrawLine(spriteBatch);
            livebar.DrawLine(spriteBatch);


            spriteBatch.DrawString(font, "Score: " + score + "\nLevel: " + level, new Vector2(WIDTH - 180, 10), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Method as a substitute for "wait" in other languages. Uses the Stopwatch "Watch".
        /// Must be in a infinite loop, such as Update() or Draw().
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        /// <returns>Whether the given time has expired.</returns>
        private bool hasWaited(int seconds)
        {
            if(!Watch.IsRunning)
                Watch.Start();
            return (Watch.Elapsed.TotalMilliseconds >= seconds * 1000);
          
        }

        void setGraphics(int w, int h)
        {
            graphics.PreferredBackBufferHeight = h;
            graphics.PreferredBackBufferWidth = w;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// The sequence to end the game; saves the current score to highscores.txt
        /// </summary>
        void ExitGame()
        {
            List<String> lines = File.ReadAllLines("highscores.txt").ToList<String>();
            bool temp=false;
            for (int i = 0; i < lines.Count; i++)
            {
                if (score < Convert.ToInt16(lines[i]))
                {
                    lines.Insert(i, score + "");
                    i++;
                    temp = true;
                }
            }
            if (!temp)
            {
                lines.Add(score + "");
            }
            File.WriteAllLines("highscores.txt", lines.ToArray());
            Exit();
        }
    }
}

