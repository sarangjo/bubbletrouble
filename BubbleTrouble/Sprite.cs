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
    class Sprite
    {
        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Origin = position + new Vector2(width / 2, height / 2);
            }
        }

        public Vector2 InitialPos;
        public Vector2 Origin;
        public string AssetName;
        public Texture2D Texture;
        public int height, width;
        
        public float theta = 0;
        public Rectangle hitbox;
        public Color myColor = Color.White;

        public Vector2 Velocity, acceleration;
        public float mass;
        
        public Sprite(Vector2 Position, string theAssetName)
        {
            this.Position = Position;
            this.AssetName = theAssetName;
            InitialPos = Position;
        }
        public Sprite(Vector2 Position, string theAssetName, Vector2 NewVel) : this(Position, theAssetName)
        {
            this.Velocity = NewVel;
        } 
        public Sprite()
        {
            this.Position = new Vector2(0, 0);
            this.AssetName = "";
        }

        public bool isOver(Vector2 OtherPosition)
        {
            return (hitbox.Contains(new Point((int)OtherPosition.X, (int)OtherPosition.Y)));
        }
        public bool isOver(MouseState State)
        {
            return (hitbox.Contains(new Point((int)State.X, (int)State.Y)));
        }
        public bool isColliding(Sprite Other)
        {
            return hitbox.Contains(Other.hitbox);
        }
        public bool isColliding(Rectangle Other)
        {
            return hitbox.Contains(Other);
        }

        //public void MoveNaturally()
        //{
        //    Velocity.Y += Game1.gravityScale * accelRate;
        //    Velocity.X -= Game1.airResistance * Velocity.X;

        //    if ((Position.X < 0 && Velocity.X < 0)|| (Position.X > Game1.GAME_WIDTH - width && Velocity.X > 0))
        //    {
        //        Velocity.X = 0 - Velocity.X * elasticity;
        //    }
        //    if ((Position.Y < 0 && Velocity.Y < 0) || Position.Y > Game1.GAME_HEIGHT - height && Velocity.Y > 0)
        //    {
        //        Velocity.Y = 0 - Velocity.Y * elasticity;
        //    }

        //    Position += Velocity;
        //}

        public void UpdateHitbox()
        {
            hitbox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public void LoadContent(ContentManager theContentManager)
        {
            Texture = theContentManager.Load<Texture2D>(AssetName);
            height = Texture.Height;
            width = Texture.Width;
            UpdateHitbox();
        }
        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(Texture, Position,
                 new Rectangle(0, 0, Texture.Width, Texture.Height), myColor,
                 theta, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
        }
        public void DrawAndLoadContent(SpriteBatch theSpriteBatch, ContentManager theContentManager)
        {
            LoadContent(theContentManager);
            Draw(theSpriteBatch);
        }
    }
}
