//using System;
//using System.Collections.Generic;

//using System.Text;
//using NeedForSpeed.Parsers.Track;
//using System.IO;
//using Microsoft.Xna.Framework;

//namespace NeedForSpeed.Loaders
//{

//	class Track10Loader:ITrackLoader
//	{

//		int _fenceTextureId;

//		public int FenceTextureId
//		{
//			get { return _fenceTextureId; }
//		}

//		public long PhysicalRoadOffset
//		{
//			get { return 0x12F8; }
//		}

//		public long SceneryOffset
//		{
//			get { return 0x16478; }
//		}

//		public long TerrainOffset
//		{
//			get { return 0x1B000; }
//		}

//		public List<TrackNode> RoadNodes
//		{
//			set { }
//		}


//		public void LoadTerrain(BinaryReader reader, List<TerrainSegment> terrainSegments, List<TerrainRow> terrainRows, List<TrackNode> roadNodes)
//		{
//			long fileSize = reader.BaseStream.Length;
//			TerrainRow firstRow = null;
            
//			bool atEnd = false, lastBlockNoPosition=false;

//			while (true)
//			{
//				reader.ReadChars(4);
//				int blockLength = reader.ReadInt32();
//				int blockNumber = reader.ReadInt32();

//				byte unk1 = reader.ReadByte();
//				byte fenceType = reader.ReadByte();

//				byte[] textures = reader.ReadBytes(10);

//				reader.BaseStream.Position += 24;

//				List<TerrainRow> rows = new List<TerrainRow>();

//				rows.Add(ReadTerrainRow(reader));
//				reader.BaseStream.Position += 12;
//				rows.Add(ReadTerrainRow(reader));
//				reader.BaseStream.Position += 12;
//				rows.Add(ReadTerrainRow(reader));
//				reader.BaseStream.Position += 12;
//				rows.Add(ReadTerrainRow(reader));
//				reader.BaseStream.Position += 12;
//				reader.BaseStream.Position += 12;
//				rows.Add(ReadTerrainRow(reader));

//				//read to end of block
//				reader.BaseStream.Position += 596;

//				if (terrainSegments.Count == 0)
//					firstRow = rows[0];

//				if (reader.BaseStream.Position >= fileSize)
//				{
//					rows[4] = firstRow;
//					atEnd = true;
//				}

//				if (lastBlockNoPosition)
//				{
//					terrainRows[terrainRows.Count - 1] = rows[0];
//					lastBlockNoPosition = false;
//				}
//				if (rows[4].Points[0] == Vector3.Zero)
//				{
//					//lap start/end position - should replace with first row of next segment
//					lastBlockNoPosition = true;
//				}

//				terrainRows.AddRange(rows);

//				TerrainSegment terrainSegment = new TerrainSegment();
//				terrainSegment.TextureIds = textures;

//				if (fenceType == 223 || fenceType == 159)
//				{
//					terrainSegment.HasLeftFence = true;
//				}
//				if (fenceType == 223 || fenceType == 95)
//				{
//					terrainSegment.HasRightFence = true;
//				}

//				terrainSegments.Add(terrainSegment);
                
//				if (atEnd)
//					break;
//			}
//		}

//		private TerrainRow ReadTerrainRow(BinaryReader reader)
//		{
//			TerrainRow row = new TerrainRow();
//			row.Points = new List<Vector3>();
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.ScaleFactor);
//			return row;
//		}
//	}
//}