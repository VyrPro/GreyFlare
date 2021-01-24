using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;


namespace ValeEdit
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Placer
    {
        public float X;
        public float Y;
        public Texture2D T;
        public Placer(float x, float y, Texture2D t)
        {
            X = x;
            Y = y;
            T = t;
        }
    }
    public class Tile
    {
        public Rectangle r;
        public Texture2D T;
        public Tile(Texture2D t, int X, int Y, int W, int H)
        {
            T = t;
            r = new Rectangle(X, Y, W, H);
        }
    }
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Dictionary<string, Texture2D> TextureList = new Dictionary<string, Texture2D>();
        public static List<Tile> MapList = new List<Tile>();
        public static List<Tile> NewList = new List<Tile>();
        public static Placer p = null;
        public static string DataSource = @"Data Source=C:\Users\Alex\source\repos\DarkVale\DarkVale\bin\Windows\x86\Debug\dv.db";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"C:\Users\Alex\source\repos\DarkVale\DarkVale\Content\bin\Windows";
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TextureList.Add("testblock", Content.Load<Texture2D>("testblock"));
            TextureList.Add("brick1", Content.Load<Texture2D>("brick1"));
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM map";
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        MapList.Add(new Tile(TextureList[rdr[0].ToString()], Int32.Parse(rdr[1].ToString()), Int32.Parse(rdr[2].ToString()), 25, 25));
                    }
                }
            }
            
            p = new Placer(0, 0, TextureList["brick1"]);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Keyboard.HasBeenPressed(Keys.Left))
            {
                p.X -= 25;
                this.Window.Title = p.X + " " + p.Y;
            }
            if(Keyboard.HasBeenPressed(Keys.Right))
            {
                p.X += 25;
                this.Window.Title = p.X + " " + p.Y;
            }
            if(Keyboard.HasBeenPressed(Keys.Up))
            {
                p.Y -= 25;
                this.Window.Title = p.X + " " + p.Y;
            }
            if(Keyboard.HasBeenPressed(Keys.Down))
            {
                p.Y += 25;
                this.Window.Title = p.X + " " + p.Y;
            }
            if(Keyboard.HasBeenPressed(Keys.A))
            {
                Tile ft = NewList.Find(x => x.r.X == p.X && x.r.Y == p.Y);
                Tile ot = MapList.Find(x => x.r.X == p.X && x.r.Y == p.Y);
                if (ft == null && ot == null)
                {
                    NewList.Add(new Tile(p.T, (int)p.X, (int)p.Y, 25, 25));
                }
            }
            if(Keyboard.IsPressed(Keys.LeftControl) && Keyboard.HasBeenPressed(Keys.S))
            {
                using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO map (texture, x, y, w, h) VALUES (@t, @x, @y, @w, @h)";
                    foreach (Tile t in NewList)
                    {
                        cmd.Parameters.AddWithValue("@t", t.T.Name);
                        cmd.Parameters.AddWithValue("@x", t.r.X);
                        cmd.Parameters.AddWithValue("@y", t.r.Y);
                        cmd.Parameters.AddWithValue("@w", "25");
                        cmd.Parameters.AddWithValue("@h", "25");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public static float CamX = 0;
        public static float CamY = 0;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Camera2d cam = new Camera2d();
            cam.Pos = new Vector2(p.X, p.Y);
            spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        cam.get_transformation(GraphicsDevice));
            foreach (Tile t in MapList)
            {
                spriteBatch.Draw(t.T, new Vector2(t.r.X, t.r.Y), new Rectangle(0, 0, 25, 25), Color.White);
            }
            foreach (Tile t in NewList)
            {
                spriteBatch.Draw(t.T, new Vector2(t.r.X, t.r.Y), new Rectangle(0, 0, 25, 25), Color.White);
            }
            spriteBatch.Draw(p.T, new Vector2(p.X, p.Y), new Rectangle(0, 0, 25, 25), Color.White);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
    public class Keyboard
    {
        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        public static KeyboardState GetState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            return currentKeyState;
        }

        public static bool IsPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key);
        }

        public static bool HasBeenPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }
    }
    public class Camera2d
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera2d()
        {
            _zoom = 1.3f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }
    }
}
