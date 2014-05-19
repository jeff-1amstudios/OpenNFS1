//using System;
//using System.Collections.Generic;

//using System.Text;
//using System.IO;
//using OpenNFS1.Parsers.Track;
//using Microsoft.Xna.Framework;
//using System.Diagnostics;

//namespace OpenNFS1.Loaders
//{
//	class Track11Loader : ITrackLoader
//	{
//		int _fenceTextureId;
//		TrackDescription _description;

//		public int FenceTextureId
//		{
//			get { return _fenceTextureId; }
//		}

//		#region ITrackLoader Members

//		public long PhysicalRoadOffset
//		{
//			get { return 2444; }
//		}

//		public long SceneryOffset
//		{
//			get { return 0x15B0C; }
//		}

//		public long TerrainOffset
//		{
//			get { return 0x1A4A8; }
//		}

//		public Track11Loader(TrackDescription description)
//		{
//			_description = description;
//		}

//		public void LoadTerrain(BinaryReader reader, List<TerrainSegment> terrainSegments, List<TerrainRow> terrainRows, List<TrackNode> roadNodes)
//		{
//			long fileSize = reader.BaseStream.Length;
			
//			bool atEnd = false;

//			long position = reader.BaseStream.Position;

//			int segmentNbr=0;

//			while (true)
//			{
//				string trkd = new string(reader.ReadChars(4));
//				int blockLength = reader.ReadInt32();
//				int blockNumber = reader.ReadInt32();
//				byte unk1 = reader.ReadByte();
//				byte fenceType = reader.ReadByte();
//				byte[] textures = reader.ReadBytes(10);

//				List<TerrainRow> rows = new List<TerrainRow>();

//				rows.Add(ReadTerrainRow(reader));
//				rows.Add(ReadTerrainRow(reader));
//				rows.Add(ReadTerrainRow(reader));
//				rows.Add(ReadTerrainRow(reader));

                
//				 // Each terrain point is relative to the reference point
//				foreach (TerrainRow row in rows)
//				{
//					row.RefNode = roadNodes[segmentNbr];
//					for (int i = 0; i < row.Points.Count; i++)
//					{
//						row.Points[i] = roadNodes[segmentNbr].Position + (row.Points[i] * 500);
//					}
//					segmentNbr++;
//					if (segmentNbr >= roadNodes.Count)
//						segmentNbr = roadNodes.Count - 1;
//				}                

//				//read to end of block
//				position += 0x120;
//				reader.BaseStream.Position = position;

//				if (reader.BaseStream.Position >= fileSize)
//				{
//					atEnd = true;
//					if (_description.IsOpenRoad)
//					{
//						terrainRows.Add(terrainRows[terrainRows.Count - 1]);
//						//else
//						//    terrainRows.Add(terrainRows[0]);
//						return;
//					}
//				}
				
//				if (terrainRows.Count > 0)
//				{
//					terrainRows.Add(rows[0]);
//				}
//				terrainRows.AddRange(rows);

//				TerrainSegment terrainSegment = new TerrainSegment();
//				terrainSegment.TextureIds = textures;

//				if (fenceType >= 192)
//				{
//					terrainSegment.HasLeftFence = terrainSegment.HasRightFence = true;
//				}
//				else if (fenceType >= 128)
//				{
//					terrainSegment.HasLeftFence = true;
//				}
//				else if (fenceType >= 64)
//				{
//					terrainSegment.HasRightFence = true;
//				}

//				if (fenceType != 0)
//				{
//					if (fenceType % 64 > _fenceTextureId)
//						_fenceTextureId = fenceType % 64;
//				}				

//				terrainSegments.Add(terrainSegment);
                
//				if (atEnd)
//					break;
//			}
//			terrainRows.Add(terrainRows[0]);
//		}

//		#endregion

//		private TerrainRow ReadTerrainRow(BinaryReader reader)
//		{
//			TerrainRow row = new TerrainRow();
//			row.Points = new List<Vector3>();
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);
//			row.Points.Add(new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16()) * GameConfig.ScaleFactor);

//			for (int i = 0; i < 5; i++)
//			{
                
//				row.Points[i + 1] = row.Points[i + 1] + (row.Points[i]);
//			}

//			Vector3 lastPoint = row.Points[0];
//			for (int i = 6; i < row.Points.Count; i++)
//			{
//				row.Points[i] = row.Points[i] + (lastPoint);
//				lastPoint = row.Points[i];
//			}

//			return row;
			
//		}
//	}
//}
