using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace BubbleTrouble
{
    class Li : Sprite
    {
        public int shotlength = 0, shotSpeed;
        public Line Shot;
        public bool shooting, isAlive = true;
        public int lives = 5;

        public Li(Vector2 Position, string asset, int newShotSpeed, GraphicsDevice gd) : base(Position, asset)
        {
            Shot = new Line(gd, 1, Color.White, Vector2.Zero, Vector2.Zero);
            shotSpeed = newShotSpeed;
        }

        public void UpdateMovement(KeyboardState KState, KeyboardState OldKState, ContentManager Content)
        {
            string oldAsset = AssetName;

            // All methods, for when he is not shooting.
            if (!shooting)
            {
                // When spacebar pressed, set the x-value
                if (KState.IsKeyDown(Keys.Space) && !OldKState.IsKeyDown(Keys.Space))
                {
                    shotlength = 20;
                    shooting = true;
                    Shot.linept1 = new Vector2(Position.X + width / 2, Game1.HEIGHT);
                }
                else
                {
                    // When spacebar not pressed, and just moving.
                    Shot.linept1 = new Vector2(Position.X + width / 2, Game1.HEIGHT);
                    Shot.linept2 = new Vector2(Shot.linept1.X, Game1.HEIGHT - shotlength);
                }
            }
            else
            {
                //Shot.linept2 = new Vector2(Shot.linept1.X, Game1.HEIGHT - height - shotlength);
                shoot();
            }
            
            // Left-right movement
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                Position = new Vector2(Position.X - 5, Position.Y);
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                Position = new Vector2(Position.X + 5, Position.Y);

            AssetName = (KState.IsKeyDown(Keys.Space)) ? "BenBack" : "Ben";
            if (AssetName.Equals(oldAsset))
                LoadContent(Content);

            // Checking x-boundaries
            Position = (Position.X < 0) ? new Vector2(0, Position.Y) : Position;
            Position = (Position.X > Game1.WIDTH - width) ? new Vector2(Game1.WIDTH - width, Position.Y) : Position;
            
        }

        public void shoot()
        {
            shotlength += shotSpeed;
            //Shot.linept1 = new Vector2(Position.X + width / 2, Game1.HEIGHT - height);
            Shot.linept2 = new Vector2(Shot.linept1.X, Game1.HEIGHT - shotlength);
        }
        
        public void DrawPlayer(SpriteBatch spriteBatch)
        {
            if(shooting)
                Shot.DrawLine(spriteBatch);
            this.Draw(spriteBatch);
        }

        
    }

}
