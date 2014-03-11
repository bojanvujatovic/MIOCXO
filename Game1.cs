using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace MIOCXO
{
    #region Enums
    
    enum GameState
    {
        TitleScreen,
        SettingsScreen,
        Playing,
        GameOver
    }

    enum PlayerControlling
    {
        Human,
        ComputerEasy
    }

    enum Box
    {
        Empty,
        X,
        O,
        FullBigBox
    }

    #endregion

    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Textures and colors fields

        Texture2D xTexture;
        Texture2D oTexture;
        Texture2D fillFrameTexture;
        Texture2D fillTexture;
        Texture2D mousePointer;
        Texture2D fieldTexture;
        Texture2D titleScreenTexture;
        SpriteFont mainFont;
        Color xTextureColor = Color.Red;
        Color oTextureColor = Color.Aqua;
        Color yellow = new Color(255, 255, 0, 100);

        #endregion

        #region Screen constants

        public const int SCREEN_WIDTH = 627;
        public const int SCREEN_HEIGHT = 627;

        #endregion

        GameState gameState;
        PlayerControlling secondPlayer;

        MouseState mouseState;
        MouseState previousMouseState;
        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameState = GameState.TitleScreen;

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Loading textures and fonts
            mainFont = Content.Load<SpriteFont>(@"Fonts\MainFont");

            fillFrameTexture = Content.Load<Texture2D>(@"Textures\FillFrame");
            fillTexture = Content.Load<Texture2D>(@"Textures\Fill");
            fieldTexture = Content.Load<Texture2D>(@"Textures\Field");
            mousePointer = Content.Load<Texture2D>(@"Textures\MouseCursor");
            oTexture = Content.Load<Texture2D>(@"Textures\O");
            xTexture = Content.Load<Texture2D>(@"Textures\X");
            titleScreenTexture = Content.Load<Texture2D>(@"Textures\Fill");

            previousMouseState = Mouse.GetState();
        }
        
        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();

            switch (gameState)
            {
                case GameState.TitleScreen:

                    if (keyboardState.IsKeyDown(Keys.D1) || keyboardState.IsKeyDown(Keys.NumPad1))
                    {
                        secondPlayer = PlayerControlling.Human;
                        gameState = GameState.SettingsScreen;
                    }
                    else if (keyboardState.IsKeyDown(Keys.D2) || keyboardState.IsKeyDown(Keys.NumPad2))
                    {
                        secondPlayer = PlayerControlling.ComputerEasy;
                        gameState = GameState.SettingsScreen;
                    }


                    break;

                case GameState.SettingsScreen:

                    StartGame();

                    break;

                case GameState.Playing:
                    Player.GetInput(mouseState, previousMouseState);

                    if (Player.Winner() != null)
                        gameState = GameState.GameOver;

                    if (Field.GetListOfVaildBoxes().Count == 0)
                        gameState = GameState.GameOver;

                    break;

                case GameState.GameOver:

                    if (mouseState.LeftButton == ButtonState.Pressed && mouseState != previousMouseState)
                        gameState = GameState.TitleScreen;
                    
                    break;
            }

            previousMouseState = mouseState;
            previousKeyboardState = keyboardState;

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Yellow);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.TitleScreen:

                    spriteBatch.Draw(titleScreenTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.Red);
                    spriteBatch.DrawString(mainFont, "MIOC XO", new Vector2(SCREEN_WIDTH / 2, 30), Color.Yellow, 0f, new Vector2(mainFont.MeasureString("MIOC XO").X / 2, 0), 1.5f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainFont, "PRESS 1 FOR PLAYER vs PLAYER", new Vector2(20, 120), Color.White);
                    spriteBatch.DrawString(mainFont, "PRESS 2 FOR PLAYER vs COMPUTER", new Vector2(20, 180), Color.White);
                    break;

                case GameState.SettingsScreen:

                    break;

                case GameState.Playing:

                    // Draws field
                    spriteBatch.Draw(fieldTexture, Vector2.Zero, Color.White);
                    spriteBatch.Draw(mousePointer, new Vector2(mouseState.X, mouseState.Y), Color.White);

                    // Draws filled boxes
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            for (int k = 0; k < 3; k++)
                                for (int l = 0; l < 3; l++)
                                {
                                    if (Field.boxes[i, j, k, l] == Box.X)
                                        spriteBatch.Draw(xTexture, Field.GetTrimmedRectangleOfBox(new Vector4(i,j,k,l)), xTextureColor);
                                    else if (Field.boxes[i, j, k, l] == Box.O)
                                        spriteBatch.Draw(oTexture, Field.GetTrimmedRectangleOfBox(new Vector4(i, j, k, l)), oTextureColor);
                                    else if(Field.boxes[i, j, k, l] == Box.FullBigBox)
                                        spriteBatch.Draw(fillTexture, Field.GetUnTrimmedRectangleOfBox(new Vector4(i, j, k, l)), Color.Black);

                                }

                    // Draws momently playing big box fill
                    spriteBatch.Draw(fillTexture, Field.GetRectangleOfBIgBox(Field.MomentlyPlayingBigBox), yellow);
                    
                    // Draws won big boxes if not momently playing in
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            if (Field.bigBoxes[i, j] == Box.X)
                            {
                                if (Field.MomentlyPlayingBigBox != new Vector2(i, j))
                                    spriteBatch.Draw(fillTexture, Field.GetRectangleOfBIgBox(new Vector2(i,j)), Color.DimGray);
                                
                                spriteBatch.Draw(xTexture, Field.GetRectangleOfBIgBox(new Vector2(i,j)), new Color(xTextureColor, 50));
                            }
                            else if (Field.bigBoxes[i, j] == Box.O)
                            {
                                if(Field.MomentlyPlayingBigBox != new Vector2(i, j))
                                    spriteBatch.Draw(fillTexture, Field.GetRectangleOfBIgBox(new Vector2(i,j)), Color.DimGray);
                                
                                spriteBatch.Draw(oTexture, Field.GetRectangleOfBIgBox(new Vector2(i,j)), new Color(oTextureColor, 50));
                            }
                            else if (Field.bigBoxes[i, j] == Box.FullBigBox)
                                spriteBatch.Draw(fillTexture, Field.GetRectangleOfBIgBox(new Vector2(i, j)), Color.DimGray);
                        }

                    // Draws big box mouse points and blurry sign 
                    if (Field.GetListOfVaildBoxes().Contains(Field.GetIndexOfPointedBox(mouseState)))
                    {
                        if(Player.MomentlyPlaying().Sign == Box.X)
                            spriteBatch.Draw(xTexture, Field.GetTrimmedRectangleOfBox(Field.GetIndexOfPointedBox(mouseState)), new Color(xTextureColor, 120));
                        else
                            spriteBatch.Draw(oTexture, Field.GetTrimmedRectangleOfBox(Field.GetIndexOfPointedBox(mouseState)), new Color(oTextureColor, 120));
                        
                        Vector2 bigBox = new Vector2(Field.GetIndexOfPointedBox(mouseState).Z, Field.GetIndexOfPointedBox(mouseState).W);
                        spriteBatch.Draw(fillFrameTexture, Field.GetRectangleOfBIgBox(bigBox), Color.Red);
                    }

                    // Draws pointer
                    spriteBatch.Draw(mousePointer, new Vector2(mouseState.X, mouseState.Y), Color.White);

                    break;


                case GameState.GameOver:

                    // Draws field
                    spriteBatch.Draw(fieldTexture, Vector2.Zero, Color.White);
                    spriteBatch.Draw(mousePointer, new Vector2(mouseState.X, mouseState.Y), Color.White);

                    // Draws filled boxes
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            for (int k = 0; k < 3; k++)
                                for (int l = 0; l < 3; l++)
                                {
                                    if (Field.boxes[i, j, k, l] == Box.X)
                                        spriteBatch.Draw(xTexture, Field.GetTrimmedRectangleOfBox(new Vector4(i, j, k, l)), xTextureColor);
                                    else if (Field.boxes[i, j, k, l] == Box.O)
                                        spriteBatch.Draw(oTexture, Field.GetTrimmedRectangleOfBox(new Vector4(i, j, k, l)), oTextureColor);
                                    else if (Field.boxes[i, j, k, l] == Box.FullBigBox)
                                        spriteBatch.Draw(fillTexture, Field.GetUnTrimmedRectangleOfBox(new Vector4(i, j, k, l)), Color.Black);

                                }

                    // Draws won big boxes
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            if (Field.bigBoxes[i, j] == Box.X)
                            {
                                spriteBatch.Draw(fillTexture, Field.GetRectangleOfBIgBox(new Vector2(i, j)), Color.DimGray);
                                spriteBatch.Draw(xTexture, Field.GetRectangleOfBIgBox(new Vector2(i, j)), new Color(xTextureColor, 120));
                            }
                            else if (Field.bigBoxes[i, j] == Box.O)
                            {
                                spriteBatch.Draw(fillTexture, Field.GetRectangleOfBIgBox(new Vector2(i, j)), Color.DimGray);
                                spriteBatch.Draw(oTexture, Field.GetRectangleOfBIgBox(new Vector2(i, j)), new Color(oTextureColor, 120));
                            }
                            else if (Field.bigBoxes[i, j] == Box.FullBigBox)
                                spriteBatch.Draw(fillTexture, Field.GetRectangleOfBIgBox(new Vector2(i, j)), Color.DimGray);
                        }

                    
                    spriteBatch.Draw(fillTexture, new Rectangle(0, 0 ,SCREEN_WIDTH, SCREEN_HEIGHT), Color.Black);
                    
                    if(Player.Winner() == null)
                    {
                        spriteBatch.DrawString(mainFont, "IT'S A DRAW!", new Vector2(SCREEN_WIDTH / 2 - mainFont.MeasureString("IT'S A DRAW!").X / 2, 100), Color.White);
                    }
                    else if (Player.Winner().Sign == Box.X)
                    {
                        spriteBatch.Draw(xTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), new Color(xTextureColor, 120));
                        spriteBatch.DrawString(mainFont, "GAME OVER! WINNER IS: ", new Vector2(SCREEN_WIDTH / 2 - mainFont.MeasureString("GAME OVER! WINNER IS: ").X / 2, 100), Color.White);
                        spriteBatch.Draw(xTexture, new Rectangle(SCREEN_WIDTH / 2 + (int) mainFont.MeasureString("GAME OVER! WINNER IS: ").X / 2 + 5, 105, 40, 40), xTextureColor);
                        spriteBatch.DrawString(mainFont, "CLICK TO CONTINUE!", new Vector2(SCREEN_WIDTH / 2 - mainFont.MeasureString("CLICK TO CONTINUE!").X / 2, 160), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(oTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), new Color(oTextureColor, 120));
                        spriteBatch.DrawString(mainFont, "GAME OVER! WINNER IS: ", new Vector2(SCREEN_WIDTH / 2 - mainFont.MeasureString("GAME OVER! WINNER IS: ").X / 2, 100), Color.White);
                        spriteBatch.Draw(oTexture, new Rectangle(SCREEN_WIDTH / 2 + (int)mainFont.MeasureString("GAME OVER! WINNER IS: ").X / 2 + 5, 105, 40, 40), oTextureColor);
                        spriteBatch.DrawString(mainFont, "CLICK TO CONTINUE!", new Vector2(SCREEN_WIDTH / 2 - mainFont.MeasureString("CLICK TO CONTINUE!").X / 2, 160), Color.White);
                    }

                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void StartGame()
        {
            Field.EmptyBigBoxes();
            Field.EmptyBoxes();

            gameState = GameState.Playing;

            Player.EmptyPlayers();
            
            new Player(Box.X, PlayerControlling.Human, true);
            new Player(Box.O, secondPlayer, false);

            Field.MomentlyPlayingBigBox = Vector2.One;
        }
    }
}
