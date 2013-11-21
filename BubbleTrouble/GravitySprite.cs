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
    class GravitySprite : Sprite
    {
        // specific gravity on the object
        public float accelRate;// = 0.8f;

        /// <summary>
        /// If it's more than one... ;) ;)
        /// </summary>
        public float elasticity;// = 0.85f;
        public bool shouldBounce = false;
        public bool isOnFloor = false;
        public float maxVelocity = 13.0f;
        public float jumpVelocity = 15f;
        //public bool isReleased;

        //public List<Line> Track = new List<Line>();
        //public Vector2 LevelPosition;

        //public Vector2 force;
        //public float scalarForce;

        public GravitySprite(Vector2 Position, string theAssetName)
            : base(Position, theAssetName)
        { }
        public GravitySprite(Vector2 Position, string theAssetName, Vector2 NewVel)
            : base(Position, theAssetName, NewVel)
        { }
        public GravitySprite(Vector2 Position, string theAssetName, Vector2 NewVel, float newAcc, float newEl, bool newShouldBounce)
            : this(Position, theAssetName, NewVel)
        {
            accelRate = newAcc;
            elasticity = newEl;
            shouldBounce = newShouldBounce;
        }
        public GravitySprite(Vector2 Position, string theAssetName, Vector2 NewVel, float newAcc, float newEl, bool newShouldBounce, float newMass)
            : this(Position, theAssetName, NewVel, newAcc, newEl, newShouldBounce)
        {
            mass = newMass;
        }


        public GravitySprite()
            : base()
        { }

        
        /// <summary>
        /// Method to call when the sprite should move in the environment naturally, without external force.
        /// </summary>
        public void MoveNaturally()
        {
            Vector2 oldVel = Velocity;
            Vector2 oldPos = Position;
            float buffer = 3.25f;

            // Exert gravity only if it's not on the flo.
            if(!isOnFloor)
                Velocity.Y += Game1.gravityScale * accelRate;
            // Always exert x-inertia.
            //Velocity.X -= Game1.airResistance * Velocity.X * ((isOnFloor) ? Game1.groundResistance : 1.0f);
            
            // If it should bounce off walls and the floor, bounce logic
            if (shouldBounce)
            {
                // NOTE: the GAME_WIDTH - width - Velocity.X includes the - Velocity.X to accomodate for where the sprite WILL be as opposed
                // to where the sprite IS.
                if ((Position.X < 0 && Velocity.X < 0) || (Position.X > Game1.WIDTH - width - Velocity.X && Velocity.X > 0))
                {
                    Velocity.X *= -elasticity;
                }
                if (/*(Position.Y < 0 && Velocity.Y < 0) ||*/ Position.Y > Game1.HEIGHT - height - Velocity.Y && Velocity.Y > 0)
                {
                    // This is important because if this is not included, the elasticity is not perfect.
                    // Without this, there is a rounding error and the velocity INCREASES instead of remaining constant.
                    Velocity.Y = accelRate*(int)(Velocity.Y / accelRate);
                    Velocity.Y *= -elasticity;
                }
            }
            // No bouncing off anything.
            else
            {
                // If hits the flo
                if (Position.Y > Game1.HEIGHT - height - Velocity.Y && Velocity.Y > 0)
                {
                    isOnFloor = true;
                    Velocity.Y = 0;
                    // Reset position to just on the flo.
                    Position = new Vector2(Position.X, Game1.HEIGHT - height);
                    
                }
            }

            // Checks if the velocity is small enough to consider the sprite to be immobile.
            if (Math.Abs(Velocity.X) < buffer && Math.Abs(Velocity.Y) < buffer/*isOnFloor*/)
            {
                //Position = new Vector2(Position.X - Velocity.X, Position.Y);
                Velocity.X = 0;
            }
                
            if (Position.Y > Game1.HEIGHT - height - Velocity.Y - buffer)
            {
                if (Math.Abs(Velocity.Y) < buffer)
                {
                    isOnFloor = true;
                    Velocity.Y = 0;
                    // Reset position to just on the flo.
                    Position = new Vector2(Position.X, Game1.HEIGHT - height);
                }
            }
            
            // Finally, updates the position! Yay!
            Position += Velocity;
            acceleration = Velocity - oldVel;

            //force = mass * acceleration;
        }

    }
}
