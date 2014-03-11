using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MIOCXO
{
    static class Field
    {
        static public Box[, , ,] boxes = new Box[3, 3, 3, 3];
        static public Box[,] bigBoxes = new Box[3, 3];
        static public Vector2 MomentlyPlayingBigBox;

        // TODO: poopæiti i gamesettings screen namistit i algoritam za hard i dodat zvuk!! // dodati još da se crta znak kao rukom
        const int BOX_BUFFER = 8;
        const int TRIMMED_BOD_HEIGHT_AND_WIDTH = 50;
        const int UNTRIMMED_BOD_HEIGHT_AND_WIDTH = 67;
        const int LINES_WIDTH = 3;

        static Field()
        {
            MomentlyPlayingBigBox = Vector2.One;
        }

        static public List<Vector4> GetListOfVaildBoxes()
        {
            List<Vector4> list = new List<Vector4>();

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (boxes[(int) MomentlyPlayingBigBox.X, (int) MomentlyPlayingBigBox.Y, i, j] == Box.Empty)
                        list.Add(new Vector4(MomentlyPlayingBigBox.X, MomentlyPlayingBigBox.Y, i, j));

            return list;
        }

        static public Vector4 GetIndexOfPointedBox(MouseState mouseState)
        {
            Vector4 vector = Vector4.Zero;

            int indexX = (int)(mouseState.X / (UNTRIMMED_BOD_HEIGHT_AND_WIDTH + LINES_WIDTH));
            int indexY = (int)(mouseState.Y / (UNTRIMMED_BOD_HEIGHT_AND_WIDTH + LINES_WIDTH));

            vector.Z = indexX % 3;
            vector.W = indexY % 3;
            
            int realIndexX = 0;
            int realIndexY = 0;

            while (indexX > 2)
            {
                indexX -= 3;
                realIndexX++;
            }
            while (indexY > 2)
            {
                indexY -= 3;
                realIndexY++;
            }

            vector.X = realIndexX;
            vector.Y = realIndexY;

            return vector;
        }

        static public Rectangle GetTrimmedRectangleOfBox(Vector4 box)
        {
            int indexX = (int)box.X * 3 + (int)box.Z;
            int indexY = (int)box.Y * 3 + (int)box.W;

            int linesOffsetX = indexX * LINES_WIDTH;
            int linesOffsetY = indexY * LINES_WIDTH;

            return new Rectangle(indexX * UNTRIMMED_BOD_HEIGHT_AND_WIDTH + BOX_BUFFER + linesOffsetX + 1,
                                 indexY * UNTRIMMED_BOD_HEIGHT_AND_WIDTH + BOX_BUFFER + linesOffsetY + 1,
                                 TRIMMED_BOD_HEIGHT_AND_WIDTH, TRIMMED_BOD_HEIGHT_AND_WIDTH);
        }

        static public Rectangle GetUnTrimmedRectangleOfBox(Vector4 box)
        {
            int indexX = (int)box.X * 3 + (int)box.Z;
            int indexY = (int)box.Y * 3 + (int)box.W;

            int linesOffsetX = indexX * LINES_WIDTH;
            int linesOffsetY = indexY * LINES_WIDTH;

            return new Rectangle(indexX * UNTRIMMED_BOD_HEIGHT_AND_WIDTH + linesOffsetX + 1,
                                 indexY * UNTRIMMED_BOD_HEIGHT_AND_WIDTH + linesOffsetY + 1,
                                 (int) UNTRIMMED_BOD_HEIGHT_AND_WIDTH, (int) UNTRIMMED_BOD_HEIGHT_AND_WIDTH);
        }

        static public Rectangle GetRectangleOfBIgBox(Vector2 bigBox)
        {
            int bigBoxSize = UNTRIMMED_BOD_HEIGHT_AND_WIDTH * 3 + 2 * LINES_WIDTH;

            int indexX = (int)bigBox.X;
            int indexY = (int)bigBox.Y;

            return new Rectangle(indexX * (bigBoxSize + LINES_WIDTH) + 1, indexY * (bigBoxSize + LINES_WIDTH) + 1, bigBoxSize, bigBoxSize);
        }

        static public void IsBigBoxWon()
        {
            for(int i = 0; i < 3 ; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (boxes[i, j, 0, 0] == Box.O && boxes[i, j, 0, 1] == Box.O && boxes[i, j, 0, 2] == Box.O ||
                        boxes[i, j, 1, 0] == Box.O && boxes[i, j, 1, 1] == Box.O && boxes[i, j, 1, 2] == Box.O ||
                        boxes[i, j, 2, 0] == Box.O && boxes[i, j, 2, 1] == Box.O && boxes[i, j, 2, 2] == Box.O ||
                        boxes[i, j, 0, 0] == Box.O && boxes[i, j, 1, 0] == Box.O && boxes[i, j, 2, 0] == Box.O ||
                        boxes[i, j, 0, 1] == Box.O && boxes[i, j, 1, 1] == Box.O && boxes[i, j, 2, 1] == Box.O ||
                        boxes[i, j, 0, 2] == Box.O && boxes[i, j, 1, 2] == Box.O && boxes[i, j, 2, 2] == Box.O ||
                        boxes[i, j, 0, 0] == Box.O && boxes[i, j, 1, 1] == Box.O && boxes[i, j, 2, 2] == Box.O ||
                        boxes[i, j, 2, 0] == Box.O && boxes[i, j, 1, 1] == Box.O && boxes[i, j, 0, 2] == Box.O)
                        {
                            if (bigBoxes[i, j] == Box.Empty)
                                bigBoxes[i, j] = Box.O;
                        }

                    else if (boxes[i, j, 0, 0] == Box.X && boxes[i, j, 0, 1] == Box.X && boxes[i, j, 0, 2] == Box.X ||
                            boxes[i, j, 1, 0] == Box.X && boxes[i, j, 1, 1] == Box.X && boxes[i, j, 1, 2] == Box.X ||
                            boxes[i, j, 2, 0] == Box.X && boxes[i, j, 2, 1] == Box.X && boxes[i, j, 2, 2] == Box.X ||
                            boxes[i, j, 0, 0] == Box.X && boxes[i, j, 1, 0] == Box.X && boxes[i, j, 2, 0] == Box.X ||
                            boxes[i, j, 0, 1] == Box.X && boxes[i, j, 1, 1] == Box.X && boxes[i, j, 2, 1] == Box.X ||
                            boxes[i, j, 0, 2] == Box.X && boxes[i, j, 1, 2] == Box.X && boxes[i, j, 2, 2] == Box.X ||
                            boxes[i, j, 0, 0] == Box.X && boxes[i, j, 1, 1] == Box.X && boxes[i, j, 2, 2] == Box.X ||
                            boxes[i, j, 2, 0] == Box.X && boxes[i, j, 1, 1] == Box.X && boxes[i, j, 0, 2] == Box.X)
                                {
                                    if (bigBoxes[i, j] == Box.Empty)
                                        bigBoxes[i, j] = Box.X;
                                }
                }
        }

        static public void IsFiledWon()
        {
            if (bigBoxes[0, 0] == Box.O && bigBoxes[0, 1] == Box.O && bigBoxes[0, 2] == Box.O ||
                bigBoxes[1, 0] == Box.O && bigBoxes[1, 1] == Box.O && bigBoxes[1, 2] == Box.O ||
                bigBoxes[2, 0] == Box.O && bigBoxes[2, 1] == Box.O && bigBoxes[2, 2] == Box.O ||
                bigBoxes[0, 0] == Box.O && bigBoxes[1, 0] == Box.O && bigBoxes[2, 0] == Box.O ||
                bigBoxes[0, 1] == Box.O && bigBoxes[1, 1] == Box.O && bigBoxes[2, 1] == Box.O ||
                bigBoxes[0, 2] == Box.O && bigBoxes[1, 2] == Box.O && bigBoxes[2, 2] == Box.O ||
                bigBoxes[0, 0] == Box.O && bigBoxes[1, 1] == Box.O && bigBoxes[2, 2] == Box.O ||
                bigBoxes[2, 0] == Box.O && bigBoxes[1, 1] == Box.O && bigBoxes[0, 2] == Box.O)
                    Player.OIsWinner();

            else if (bigBoxes[0, 0] == Box.X && bigBoxes[0, 1] == Box.X && bigBoxes[0, 2] == Box.X ||
                     bigBoxes[1, 0] == Box.X && bigBoxes[1, 1] == Box.X && bigBoxes[1, 2] == Box.X ||
                     bigBoxes[2, 0] == Box.X && bigBoxes[2, 1] == Box.X && bigBoxes[2, 2] == Box.X ||
                     bigBoxes[0, 0] == Box.X && bigBoxes[1, 0] == Box.X && bigBoxes[2, 0] == Box.X ||
                     bigBoxes[0, 1] == Box.X && bigBoxes[1, 1] == Box.X && bigBoxes[2, 1] == Box.X ||
                     bigBoxes[0, 2] == Box.X && bigBoxes[1, 2] == Box.X && bigBoxes[2, 2] == Box.X ||
                     bigBoxes[0, 0] == Box.X && bigBoxes[1, 1] == Box.X && bigBoxes[2, 2] == Box.X ||
                     bigBoxes[2, 0] == Box.X && bigBoxes[1, 1] == Box.X && bigBoxes[0, 2] == Box.X)
                        Player.XIsWinner();
        }

        static public void CheckForBigBoxFill()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (boxes[(int)MomentlyPlayingBigBox.X, (int)MomentlyPlayingBigBox.Y, i, j] == Box.Empty)
                        return;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (boxes[i, j, (int)MomentlyPlayingBigBox.X, (int)MomentlyPlayingBigBox.Y] == Box.Empty)
                        boxes[i, j, (int)MomentlyPlayingBigBox.X, (int)MomentlyPlayingBigBox.Y] = Box.FullBigBox;
        }

        static public void EmptyBoxes()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 3; l++)
                            boxes[i, j, k, l] = Box.Empty;
        }

        static public void EmptyBigBoxes()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    bigBoxes[i, j] = Box.Empty;
        }

        public static void IsBigBoxDraw()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (boxes[(int)MomentlyPlayingBigBox.X, (int)MomentlyPlayingBigBox.Y, i, j] == Box.Empty)
                        return;

            if(bigBoxes[(int)MomentlyPlayingBigBox.X, (int)MomentlyPlayingBigBox.Y] == Box.Empty)
                bigBoxes[(int)MomentlyPlayingBigBox.X, (int)MomentlyPlayingBigBox.Y] = Box.FullBigBox;
        }
    }
}
