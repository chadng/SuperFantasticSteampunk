using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class MapRoom
    {
        #region Instance Properties
        public Point Position { get; private set; }
        public List<MapRoom> ConnectedRooms { get; private set; }

        public bool Connected
        {
            get { return ConnectedRooms.Count > 0; }
        }
        #endregion

        #region Constructors
        public MapRoom(Point position)
        {
            Position = position;
            ConnectedRooms = new List<MapRoom>();
        }
        #endregion

        #region Instance Methods
        public void ConnectTo(MapRoom room)
        {
            ConnectedRooms.Add(room);
            room.ConnectedRooms.Add(this);
        }

        public MapRoom GetNeighborWhere(MapRoom[,] rooms, int columns, int rows, Func<MapRoom, bool> predicate)
        {
            List<MapRoom> neighbors = new List<MapRoom>();

            for (int x = Position.X - 1; x <= Position.X + 1; ++x)
            {
                if (x < 0 || x >= columns)
                    continue;

                for (int y = Position.Y; y <= Position.Y + 1; ++y)
                {
                    if (y < 0 || y >= rows)
                        continue;
                    if (Position.X == x && Position.Y == y)
                        continue;

                    if (predicate(rooms[x, y]))
                        neighbors.Add(rooms[x, y]);
                }
            }

            if (neighbors.Count > 0)
                return neighbors.Sample();
            else
                return null;
        }
        #endregion
    }

    class Map
    {
        #region Instance Properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool[,] CollisionMap { get; private set; }
        #endregion

        #region Constructors
        public Map(int width, int height, int columns, int rows)
        {
            Width = width;
            Height = height;
            CollisionMap = new bool[width, height];
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                    CollisionMap[x, y] = false;
            }

            generateCollisionMapFromRooms(generateRoomConnections(columns, rows), columns, rows);
        }
        #endregion

        #region Instance Methods
        private void generateCollisionMapFromRooms(MapRoom[,] rooms, int columns, int rows)
        {
            int roomWidth = Width / columns;
            int roomHeight = Height / rows;

            for (int x = 0; x < columns; ++x)
            {
                for (int y = 0; y < rows; ++y)
                    wallOffRoom(x, y, roomWidth, roomHeight);
            }

            for (int x = 0; x < columns; ++x)
            {
                for (int y = 0; y < rows; ++y)
                {
                    foreach (MapRoom room in rooms[x, y].ConnectedRooms)
                        digCorridoor(rooms[x, y].Position, room.Position, roomWidth, roomHeight);
                }
            }
        }

        private void wallOffRoom(int roomX, int roomY, int roomWidth, int roomHeight)
        {
            int startX = roomX * roomWidth;
            int finishX = startX + roomWidth - 1;
            int startY = roomY * roomHeight;
            int finishY = startY + roomHeight - 1;

            for (int x = startX; x <= finishX; ++x)
            {
                CollisionMap[x, startY] = true;
                CollisionMap[x, finishY] = true;
            }

            for (int y = startY; y <= finishY; ++y)
            {
                CollisionMap[startX, y] = true;
                CollisionMap[finishX, y] = true;
            }
        }

        private void digCorridoor(Point start, Point finish, int roomWidth, int roomHeight)
        {
            if (start.Y == finish.Y)
            {
                if (start.X > finish.X)
                {
                    int temp = start.X;
                    start.X = finish.X;
                    finish.X = temp;
                }

                int startX = (start.X * roomWidth) + (roomWidth / 2);
                int finishX = startX + roomWidth;
                int y = (start.Y * roomHeight) + (roomHeight / 2);
                for (int x = startX; x < finishX; ++x)
                    CollisionMap[x, y] = false;
            }
            else
            {
                if (start.Y > finish.Y)
                {
                    int temp = start.Y;
                    start.Y = finish.Y;
                    finish.Y = temp;
                }

                int startY = (start.Y * roomHeight) + (roomHeight / 2);
                int finishY = startY + roomHeight;
                int x = (start.X * roomWidth) + (roomWidth / 2);
                for (int y = startY; y < finishY; ++y)
                    CollisionMap[x, y] = false;
            }
        }

        private MapRoom[,] generateRoomConnections(int columns, int rows)
        {
            // http://kuoi.com/~kamikaze/GameDesign/art07_rogue_dungeon.php

            MapRoom[,] rooms = new MapRoom[columns, rows];
            List<MapRoom> disconnectedRooms = new List<MapRoom>(columns * rows);
            for (int x = 0; x < columns; ++x)
            {
                for (int y = 0; y < rows; ++y)
                {
                    rooms[x, y] = new MapRoom(new Point(x, y));
                    disconnectedRooms.Add(rooms[x, y]);
                }
            }

            MapRoom currentRoom = disconnectedRooms.Sample();
            MapRoom nextRoom;
            while ((nextRoom = currentRoom.GetNeighborWhere(rooms, columns, rows, room => !room.Connected)) != null)
            {
                disconnectedRooms.Remove(currentRoom);
                disconnectedRooms.Remove(nextRoom);
                currentRoom.ConnectTo(nextRoom);
                currentRoom = nextRoom;
            }

            while (disconnectedRooms.Count > 0)
            {
                currentRoom = disconnectedRooms.Sample();
                nextRoom = currentRoom.GetNeighborWhere(rooms, columns, rows, room => room.Connected);

                if (nextRoom != null)
                {
                    disconnectedRooms.Remove(currentRoom);
                    disconnectedRooms.Remove(nextRoom);
                    currentRoom.ConnectTo(nextRoom);
                }
            }

            return rooms;
        }
        #endregion
    }
}
