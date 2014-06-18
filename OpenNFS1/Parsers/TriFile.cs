using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1;
using OpenNFS1.Parsers.Track;
using OpenNFS1.Tracks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenNFS1.Parsers
{
	enum SceneryType
	{
		Model = 1,
		Bitmap = 4,
		TwoSidedBitmap = 6
	}

	[Flags]
	enum SceneryFlags
	{
		None = 0,
		Unk1 = 1,
		Animated = 4
	}

	class SceneryObjectDescriptor
	{
		public int Id;
		public float Width, Height;
		public SceneryFlags Flags;
		public SceneryType Type;
		public int ResourceId, Resource2Id;
		public int AnimationFrameCount;
	}

	class SceneryObject
	{
		public SceneryObjectDescriptor Descriptor;
		public int ReferenceNode;
		public float Orientation;
		public Vector3 RelativePosition;
	}


	class TriFile
	{
		public const int NbrTerrainPointsPerSide = 6;  //includes the middle-point (duplicated on both sides)
		public const int NbrRowsPerSegment = 4;

		public List<TrackNode> Nodes { get; private set; }
		public List<SceneryObject> Scenery { get; private set; }
		public List<SceneryObjectDescriptor> ObjectDescriptors { get; private set; }
		public List<TerrainSegment> Segments { get; private set; }
		public bool IsOpenRoad { get; private set; }
		public string FileName { get; private set; }
		public int FenceTextureId { get; private set; }

		public TriFile(string filename)
		{
			FileName = Path.GetFileName(filename);
			filename = Path.Combine(GameConfig.CdDataPath, filename);
			BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open));

			byte trackVersion = reader.ReadByte();
			if (trackVersion != 0x11)
			{
				throw new Exception("Tri file version not supported: " + trackVersion);
			}

			Nodes = new List<TrackNode>();
			ObjectDescriptors = new List<SceneryObjectDescriptor>();
			Scenery = new List<SceneryObject>();
			Segments = new List<TerrainSegment>();

			ParseTrackNodesBlock(reader);
			ParseSceneryObjectsBlock(reader);
			ParseTerrainBlock(reader);
			ComputeAbsoluteTerrainPoints();
			
			reader.Close();
		}

		void ParseTrackNodesBlock(BinaryReader reader)
		{
			reader.BaseStream.Position = 2444;  //start of node list

			TrackNode prevNode = null;

			for (int i = 0; i < 2400; i++)
			{
				var node = new TrackNode();
				node.Number = Nodes.Count;
				float vergeScale = 8000;

				node.DistanceToLeftVerge = reader.ReadByte();
				node.DistanceToLeftVerge *= GameConfig.TerrainScale * vergeScale;
				node.DistanceToRightVerge = reader.ReadByte();
				node.DistanceToRightVerge *= GameConfig.TerrainScale * vergeScale;
				node.DistanceToLeftBarrier = reader.ReadByte();
				node.DistanceToLeftBarrier *= GameConfig.TerrainScale * vergeScale;
				node.DistanceToRightBarrier = reader.ReadByte();
				node.DistanceToRightBarrier *= GameConfig.TerrainScale * vergeScale;

				node.b = reader.ReadBytes(4);

				// unused trackNodes are filled with zeroes, so stop when we have a node with a zero position
				if (node.b[1] == 0 && node.b[3] == 0)
				{
					break;
				}
				
				//Debug.WriteLine("{0},{1},{2},{3}", node.b[0], node.b[1], node.b[2], node.b[3]);

				if (node.b[2] != 0 && node.b[2] != 34 && node.b[2] != 2)
				{

				}

				node.Position = new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * GameConfig.TerrainScale;


				// Slope is stored as a 2's complement value.  Convert it back to signed value
				Int16 slope = reader.ReadInt16();
				//bool msbSet = (slope & (0x1 << 13)) != 0;
				//if (msbSet)
				//{
				//	slope = (short)~slope;
				//	slope++;
				//	slope *= -1;
				//}
				if (slope > 0x2000)
				{
					slope -= 0x3FFF;
				}
				
				node.Slope = slope;

				node.Slant = reader.ReadInt16(); //weird slant-A

				float orientation = (float)reader.ReadInt16();
				//convert to signed degrees
				//0 = forwards, 0x1000 = right, 0x2000 = back, 0x3000 = left, 0x3FFF back to forwards

				if (orientation > 0x2000)
				{
					orientation -= 0x3FFF;
				}
				node.Orientation = ((orientation / 0x3FFF) * -360);

				node.unk1 = reader.ReadBytes(2);
				node.ZOrientation = reader.ReadInt16();
				node.Slant = reader.ReadInt16(); // slant-B
				node.XOrientation = reader.ReadInt16();
				node.unk2 = reader.ReadBytes(2);

				if (node.Number == 365)
				{
					node.GetLeftBoundary();
				}

				if (prevNode != null)
				{
					prevNode.Next = node;
					node.Prev = prevNode;
				}
				prevNode = node;
				Nodes.Add(node);
			}

			// If this is a circuit track, hook the last node up to the first
			if (Vector3.Distance(Nodes[0].Position, Nodes[Nodes.Count - 1].Position) > 100)
			{
				IsOpenRoad = true;
			}
			else
			{
				prevNode.Next = Nodes[0];
				Nodes[0].Prev = prevNode;
			}
			

			for (int i = 0; i < Nodes.Count - 1; i++)
			{
				var node = Nodes[i];
				var normal = Vector3.Cross(node.GetRightBoundary() - node.GetLeftBoundary(),
					node.Next.Position - node.GetLeftBoundary());
				node.Up = Vector3.Normalize(normal);
			}
		}


		private void ParseSceneryObjectsBlock(BinaryReader reader)
		{
			reader.BaseStream.Position = 0x15B0C;

			reader.BaseStream.Position += 1800; //jump over unknown index
			int objectDescriptorCount = reader.ReadInt32();
			int objectCount = reader.ReadInt32();
			reader.ReadChars(4); //'OBJS'
			reader.BaseStream.Position += 8;

			for (int i = 0; i < objectDescriptorCount; i++)
			{
				SceneryObjectDescriptor sd = new SceneryObjectDescriptor();
				byte[] bytes = reader.ReadBytes(16);

				sd.Flags = (SceneryFlags)bytes[0];
				sd.Type = (SceneryType)bytes[1];
				sd.Width = BitConverter.ToInt16(bytes, 6);
				sd.Width *= GameConfig.TerrainScale * 65000;
				sd.Height = BitConverter.ToInt16(bytes, 14);
				sd.Height *= GameConfig.TerrainScale * 65000;
				sd.ResourceId = bytes[2];

				if (sd.Type == SceneryType.TwoSidedBitmap)
				{
					sd.Resource2Id = bytes[3];
				}

				if ((sd.Flags & SceneryFlags.Animated) == SceneryFlags.Animated)
				{
					sd.AnimationFrameCount = bytes[8];
				}
				//Debug.WriteLine(String.Format("sc: Tex: {0} {1} {2} {3} {4}", (sd.ResourceId / 4).ToString("00") + "00", sd.ResourceId, sd.Resource2Id, sd.AnimationFrameCount, sd.Type));

				//foreach (var b in bytes)
				//{
				//	Debug.Write(b.ToString("000") + " ");
				//}

				if (bytes[0] != 4 && bytes[0] != 0)
				{

				}

				if (bytes[0] == 4 && bytes[8] == 0)
				{

				}
				if (bytes[0] == 0 && bytes[8] > 0)
				{

				}

				sd.Id = ObjectDescriptors.Count;
				ObjectDescriptors.Add(sd);
			}

			long currentPos = reader.BaseStream.Position;

			Random random = new Random();

			for (int i = 0; i < objectCount; i++)
			{
				int referenceNode = reader.ReadInt32();
				if (referenceNode == -1)
				{
					break;
				}
				
				if (referenceNode > Nodes.Count)
				{

				}

				SceneryObject obj = new SceneryObject();
				obj.ReferenceNode = referenceNode % Nodes.Count;  //why do some tracks reference nodes above the node count?
				int descriptorRef = reader.ReadByte();
				obj.Descriptor = ObjectDescriptors[descriptorRef];

				obj.Orientation = (float)reader.ReadByte();
				byte[] flags = reader.ReadBytes(4);
				obj.RelativePosition = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
				Scenery.Add(obj);
			}
			reader.BaseStream.Position = currentPos + 16000; //jump to end of billboards            
			reader.BaseStream.Position += 492; //blank space
		}

		private void ParseTerrainBlock(BinaryReader reader)
		{
			reader.BaseStream.Position = 0x1A4A8;

			long fileSize = reader.BaseStream.Length;
			long position = reader.BaseStream.Position;

			TerrainSegment last = null;
			
			while (true)
			{
				char[] trkd = reader.ReadChars(4);  //"TRKD"
				if (trkd[0] == 'C')  //Strange format un-used AL22.TRI
				{
					return;
				}
				int blockLength = reader.ReadInt32();
				int blockNumber = reader.ReadInt32();
				byte unk1 = reader.ReadByte();
				byte fenceType = reader.ReadByte();

				TerrainSegment terrainSegment = new TerrainSegment();
				terrainSegment.Number = Segments.Count;
				terrainSegment.TextureIds = reader.ReadBytes(10);
				
				terrainSegment.Rows[0] = ReadTerrainRow(reader);
				terrainSegment.Rows[1] = ReadTerrainRow(reader);
				terrainSegment.Rows[2] = ReadTerrainRow(reader);
				terrainSegment.Rows[3] = ReadTerrainRow(reader);								

				//fenceType stores the sides of the road the fence lives, and the textureId to use for it.
				// If the top bit is set, fence on the left exists, if next bit is set, fence is on the right.  Both can also be set. 
				//The other 6 bits seem to the texture number
				if (fenceType != 0)
				{
					bool bit7 = (fenceType & (0x1 << 7)) != 0;
					bool bit6 = (fenceType & (0x1 << 6)) != 0;

					terrainSegment.HasLeftFence = bit7;
					terrainSegment.HasRightFence = bit6;

					// Ignore the top 2 bits to find the texture to use
					terrainSegment.FenceTextureId = fenceType & (0xff >> 2);
					Debug.WriteLine("fence: " + fenceType + ", texture: " + terrainSegment.FenceTextureId + ", " + terrainSegment.HasLeftFence + ", " + terrainSegment.HasRightFence);
					if (!terrainSegment.HasLeftFence && !terrainSegment.HasRightFence)
					{

					}
				}

				//Debug.WriteLine("TRKD: " + i + ", " + terrainSegment.Rows[0].RightPoints[5] + ", fence: " + terrainSegment.FenceTextureId + " , " + terrainSegment.HasLeftFence + ", " + terrainSegment.HasRightFence);

				if (last != null)
				{
					last.Next = terrainSegment;
					terrainSegment.Prev = last;
				}
				last = terrainSegment;
				Segments.Add(terrainSegment);

				//skip to end of block (+12 to include block header)
				position += blockLength + 12;
				reader.BaseStream.Position = position;
				
				if (reader.BaseStream.Position >= fileSize)
				{
					break;
				}
			}

			// If this is a circuit track, hook the last segment up to the first
			if (!IsOpenRoad)
			{
				last.Next = Segments[0];
				Segments[0].Prev = last;
			}
		}

		private TerrainRow ReadTerrainRow(BinaryReader reader)
		{
			TerrainRow row = new TerrainRow();
			row.MiddlePoint = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			
			row.RightPoints[0] = row.MiddlePoint;
			row.RightPoints[1] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.RightPoints[2] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.RightPoints[3] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.RightPoints[4] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.RightPoints[5] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());

			row.LeftPoints[0] = row.MiddlePoint;
			row.LeftPoints[1] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.LeftPoints[2] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.LeftPoints[3] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.LeftPoints[4] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());
			row.LeftPoints[5] = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());

			row.MiddlePoint *= GameConfig.TerrainScale;
			for (int i = 0; i < NbrTerrainPointsPerSide; i++)
				row.RightPoints[i] *= GameConfig.TerrainScale;
			for (int i = 0; i < NbrTerrainPointsPerSide; i++)
				row.LeftPoints[i] *= GameConfig.TerrainScale;

			// Each point is relative to the previous point
			for (int i = 1; i < NbrTerrainPointsPerSide; i++)
			{
				row.RightPoints[i] += row.RightPoints[i - 1];
			}

			for (int i = 1; i < NbrTerrainPointsPerSide; i++)
			{
				row.LeftPoints[i] += row.LeftPoints[i - 1];
			}

			return row;
		}

		private void ComputeAbsoluteTerrainPoints()
		{
			var segmentIndex = 0;
			var nodeIndex = 0;
			foreach (var segment in Segments)
			{

				if (segmentIndex == 48)
				{

				}
				for (int i = 0; i < NbrRowsPerSegment; i++)
				{
					var relativePoint = Nodes[nodeIndex].Position;
					segment.Rows[i].RelativizeTo(relativePoint);
					nodeIndex++;
				}
				segmentIndex++;
			}
		}
	}
}
