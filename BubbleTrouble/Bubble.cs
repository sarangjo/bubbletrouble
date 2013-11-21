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
    class Bubble : GravitySprite
    {
        //static Bubble bubbleOne = new Bubble(new Vector2(100, 100), "bubble", new Vector2(0,3f), 0.8f, 1f, 10);

        public int bounceHeight;
        public static Bubble Default = new Bubble(new Vector2(0,0), "BlueBall", new Vector2(1,0), 1f, 100, Color.SkyBlue);
        //public Color myColor = Color.White;

        public Bubble(Vector2 Position, string theAssetName, Vector2 NewVel, float newEl,
            int newBHeight, Color newColor)
            : base(Position, theAssetName, NewVel, 0.23f, newEl, true)
        {
            bounceHeight = newBHeight;
            myColor = newColor;
        }

        public void MoveNaturallyBubble()
        {
            // Exert gravity only if it's not on the flo.
            if (!isOnFloor)
                Velocity.Y += Game1.gravityScale * accelRate;
            
            // If it should bounce off walls and the floor, bounce logic
            if (Position.Y > Game1.HEIGHT - height - Velocity.Y && Velocity.Y > 0)
            {
                // For bubbles, we KNOW what height/velocity we want.
                Velocity.Y = -(float)Math.Sqrt((2 * Game1.gravityScale * accelRate * bounceHeight));
            }
            if ((Position.X < 0 && Velocity.X < 0) || (Position.X > Game1.WIDTH - width - Velocity.X && Velocity.X > 0))
            {
                Velocity.X *= -elasticity;
                //Velocity.Y = (float)Math.Sqrt(-2*(Game1.gravityScale*accelRate)*(bounceHeight - Position.Y));
            }
            

            // Finally, updates the position! Yay!
            Position += Velocity;
        }

        
    }
}
