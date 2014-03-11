using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MIOCXO
{
    class Player
    {
        static private Random random;
        private PlayerControlling controlling;
        private Box sign;
        private bool isFirst;
        private bool isItsTurn;
        private bool isWon;

        #region Properties

        public Box Sign
        {
            get { return sign; }
        }

        #endregion

        static private List<Player> Players;        

        public Player(Box sign, PlayerControlling controlling, bool isFirst)
        {
            this.controlling = controlling;
            this.sign = sign;
            this.isFirst = isFirst;
            this.isItsTurn = isFirst;
            this.isWon = false;

            Players.Add(this);
        }

        static Player()
        {
            Players = new List<Player>();
            random = new Random();
        }

        static public void GetInput(MouseState mouseState, MouseState previousMouseState)
        {
            int i, j, k, l;
            i = j = k = l = 0;

            if (MomentlyPlaying().controlling == PlayerControlling.ComputerEasy && mouseState != previousMouseState)
            {
                int index = random.Next(0, Field.GetListOfVaildBoxes().Count);
                
                Vector4 vector = new Vector4();
                vector = Field.GetListOfVaildBoxes()[index];

                i = (int)vector.X;
                j = (int)vector.Y;
                k = (int)vector.Z;
                l = (int)vector.W;

                Field.boxes[i, j, k, l] = MomentlyPlaying().sign;

                Players[0].isItsTurn = !Players[0].isItsTurn;
                Players[1].isItsTurn = !Players[1].isItsTurn;

                Field.CheckForBigBoxFill();
                Field.IsBigBoxWon();
                Field.IsBigBoxDraw();
                Field.IsFiledWon();

                Field.MomentlyPlayingBigBox = new Vector2(k, l);
            }
            else if (Field.GetListOfVaildBoxes().Contains(Field.GetIndexOfPointedBox(mouseState)) && mouseState.LeftButton == ButtonState.Pressed && mouseState != previousMouseState)
            {
                i = (int) Field.GetIndexOfPointedBox(mouseState).X;
                j = (int) Field.GetIndexOfPointedBox(mouseState).Y;
                k = (int) Field.GetIndexOfPointedBox(mouseState).Z;
                l = (int) Field.GetIndexOfPointedBox(mouseState).W;

                Field.boxes[i, j, k, l] = MomentlyPlaying().sign;

                Players[0].isItsTurn = !Players[0].isItsTurn;
                Players[1].isItsTurn = !Players[1].isItsTurn;

                Field.CheckForBigBoxFill();
                Field.IsBigBoxWon();
                Field.IsBigBoxDraw();
                Field.IsFiledWon();

                Field.MomentlyPlayingBigBox = new Vector2(k, l);
            }
        }

        static public Player MomentlyPlaying()
        {
            if (Players[0].isItsTurn)
                return Players[0];
            return Players[1];
        }

        static public Player PreviouslyPlayed()
        {
            if (!Players[0].isItsTurn)
                return Players[0];
            return Players[1];
        }

        static public Player Winner()
        {
            if (Players[0].isWon)
                return Players[0];
            if (Players[1].isWon)
                return Players[1];

            return null;
        }

        static public void XIsWinner()
        {
            if (Players[0].sign == Box.X)
                Players[0].isWon = true;
            else
                Players[1].isWon = true;
        }

        static public void OIsWinner()
        {
            if (Players[0].sign == Box.O)
                Players[0].isWon = true;
            else
                Players[1].isWon = true;
        }

        static public void EmptyPlayers()
        {
            Players.Clear();
        }
    }
}
