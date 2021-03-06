﻿#region License
/**
 * Copyright (c) 2010-2012 Lightning Development Studios <lightningdevelopmentstudios@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
#endregion

using System;
using System.ComponentModel;

using OpenTK;
using System.Drawing;
using System.Collections.Generic;

namespace CircuitCrawlerEditor
{
	public enum TileType { Floor, Wall, Pit, Slip }

	[DefaultPropertyAttribute("Tile")]
	public class Tile
	{
		public const float SIZE_F = 72.0f;
		public const int SIZE = 72;

		private Tileset tileset;
		private Point tilesetPos;
		private TileType type;
		private Vector2 pos;
		private byte borders;

		[CategoryAttribute("Tile"), DescriptionAttribute("The tile's current type")]
		public TileType TileType
		{
			get
			{
				return type;
			}

			set
			{
				type = value;

				if (tileset != null)
					tileset.Reload();
			}
		}

		[CategoryAttribute("Basic Properties"), DefaultValueAttribute("Vector2"), DescriptionAttribute("The position of the entity"), BrowsableAttribute(false)]
		public Vector2 Position { get { return pos; } set { pos = value; } }

		public Tile(Point tilesetPos, int tilesetLengthX, int tilesetLengthY, TileType type)
		{
			TilesetHelper.setInitialTileOffset(this, tilesetPos, tilesetLengthX, tilesetLengthY);

			this.tilesetPos = tilesetPos;
			this.type = type;
		}

		public void SetTileset(Tileset t)
		{
			this.tileset = t;
		}

		/**
	 * Gets the vertex data for this tile.
	 * @return The tile's vertices.
	 */
		public float[] getVertexData()
		{
			//calculate vertices
			float s = SIZE_F / 2;

			float[] vertices = null;
			float[] texCoords = null;
			float[] normals = null;

			switch (type)
			{
				case TileType.Floor:
					vertices = new float[]
					{
						-s,  s, 0,
						-s, -s, 0,
						 s, -s, 0,
						 s,  s, 0
					};
					texCoords = new float[]
					{
						0, 0,
						0, 64f / 256f - 1f / 512f,
						64f / 512f - 1f / 1024f, 64.0f / 256.0f - 1f / 512f,
						64f / 512f - 1f / 1024f, 0
					};
					normals = new float[]
					{
						0, 0, 1,
						0, 0, 1,
						0, 0, 1,
						0, 0, 1
					};
					break;
				case TileType.Wall:
					vertices = TilesetHelper.getTileVertices(this.borders, s, SIZE_F);
					texCoords = TilesetHelper.getWallTexCoords(this.borders);
					normals = TilesetHelper.getTileNormals(this.borders);
					break;
				case TileType.Pit:
					vertices = TilesetHelper.getTileVertices(this.borders, s, -SIZE_F);
					texCoords = TilesetHelper.getPitTexCoords(this.borders);
					normals = TilesetHelper.getTileNormals(this.borders);
					break;
				default:
					//by default use the same vertices as the floor.
					vertices = new float[]
					{
						-s,  s, 0,
						-s, -s, 0,
						 s, -s, 0,
						 s,  s, 0
					};
					texCoords = new float[]
					{
						0, 0,
						0, 64f / 256f - 1f / 512f,
						64f / 512f - 1f / 1024f, 64.0f / 256.0f - 1f / 512f,
						64f / 512f - 1f / 1024f, 0
					};
					normals = new float[]
					{
						0, 0, 1,
						0, 0, 1,
						0, 0, 1,
						0, 0, 1
					};
					break;
			}

			//shift vertices to position
			for (int i = 0; i < vertices.Length; i += 3)
			{
				vertices[i] += pos.X;
				vertices[i + 1] += pos.Y;
			}

			float[] vertexData = new float[(vertices.Length / 3) * 8];

			//interleave arrays
			for (int i = 0; i < vertices.Length / 3; i++)
			{
				vertexData[i * 8] = vertices[i * 3];
				vertexData[i * 8 + 1] = vertices[i * 3 + 1];
				vertexData[i * 8 + 2] = vertices[i * 3 + 2];
				vertexData[i * 8 + 3] = texCoords[i * 2];
				vertexData[i * 8 + 4] = texCoords[i * 2 + 1];
				vertexData[i * 8 + 5] = normals[i * 3];
				vertexData[i * 8 + 6] = normals[i * 3 + 1];
				vertexData[i * 8 + 7] = normals[i * 3 + 2];
			}

			return vertexData;
		}

		/**
		 * Gets the index data for this tile.
		 * @param startVert The starting index for this tile.
		 * @return An int array containing the tile's indices.
		 */
		public ushort[] getIndexData(ushort startVert)
		{
			ushort[] order;

			switch (type)
			{
				case TileType.Floor:
					order = new ushort[] { 0, 1, 2, 0, 2, 3 };
					break;
				case TileType.Wall:
					order = TilesetHelper.getTileIndices(this.borders);
					break;
				case TileType.Pit:
					order = TilesetHelper.getTileIndices(this.borders);
					break;
				default:
					order = new ushort[] { 0, 1, 2, 0, 2, 3 };
					break;
			}

			for (int i = 0; i < order.Length; i++)
			{
				order[i] += startVert;
			}

			return order;
		}

		/**
		 * Returns this tile's type.
		 * @return This tile's type.
		 */
		public TileType getTileType()
		{
			return type;
		}

		/**
		 * Calculates the bordering tile bitfield.
		 * A tile is considered a "bordering tile" of a given tile if it meets the following criteria:
		 * <ul>
		 * <li> The bordering tile shares an edge or vertex with the given tile.</li>
		 * <li> The bordering tile is not of the same type as the given tile.</li>
		 * </ul>
		 * The result of this method is stored as a byte. Each potential bordering tile is represented as a flag in a bitfield. 
		 * The flags are defined as follows:
		 * \verbatim
		 * +------+------+------+
		 * |      |      |      |
		 * | 0x01 | 0x02 | 0x04 |
		 * |      |      |      |
		 * +------+------+------+
		 * |      |      |      |
		 * | 0x08 |      | 0x10 |
		 * |      |      |      |
		 * +------+------+------+
		 * |      |      |      |
		 * | 0x20 | 0x40 | 0x80 |
		 * |      |      |      |
		 * +------+------+------+
		 * \endverbatim
		 * @param tileset The tileset that contains this tile.
		 */
		public void calculateBorders(Tile[][] tileset)
		{
			int x = tilesetPos.X;
			int y = tilesetPos.Y;
			//store all the bordering tile indices in top left to bottom right order.
			List<Point> points = new List<Point>();
			points.Add(new Point(x - 1, y - 1));
			points.Add(new Point(x, y - 1));
			points.Add(new Point(x + 1, y - 1));
			points.Add(new Point(x - 1, y));
			points.Add(new Point(x + 1, y));
			points.Add(new Point(x - 1, y + 1));
			points.Add(new Point(x, y + 1));
			points.Add(new Point(x + 1, y + 1));

			//check each of the bordering tiles, set it's corresponding bit to 1 if it's not the same type of tile.
			for (int i = 0; i < points.Count; i++)
			{
				Point p = points[i];

				//make sure tileset index is within tileset bounds
				if (p.X >= 0 && p.X < tileset[0].Length && p.Y >= 0 && p.Y < tileset.Length)
				{
					Tile t = tileset[p.Y][p.X];

					//if the tile type is not the same, it's considered a bordering tile.
					if (t != null && t.getTileType() != type)
						borders |= (byte)(0x01 << i);
				}
			}
		}
	}
}
