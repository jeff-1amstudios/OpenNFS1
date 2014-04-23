using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NeedForSpeed.Parsers
{
    enum PolygonShape
    {
        Unknown = 0,
        Quad2 = 0x4,
        Triangle = 0x83,
        Quad = 0x84,
        UnTexturedQuad = 0x8C,
        Triangle2 = 0x8B
    }

    enum PolygonType
    {
        Normal,
        WheelFrontLeft,
        WheelFrontRight,
        WheelRearRight,
        WheelRearLeft
    }

    class Polygon
    {
        private string _textureName;
        private PolygonShape _shape;
        private PolygonType _type;
        List<Vector3> _vertices = new List<Vector3>();
        List<Vector2> _textureCoords = new List<Vector2>();
        private int _vertexBufferIndex;
        private Texture2D _texture;

        public PolygonShape Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

        internal PolygonType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string TextureName
        {
            get { return _textureName; }
            set { _textureName = value; }
        }

        public List<Vector3> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        public List<Vector2> TextureCoords
        {
            get { return _textureCoords; }
            set { _textureCoords = value; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public int VertexCount
        {
            get
            {
                if (_shape == PolygonShape.Triangle)
                    return 3;
                else
                    return 6;
            }
        }

        public int VertexBufferIndex
        {
            get { return _vertexBufferIndex; }
            set { _vertexBufferIndex = value; }
        }

        public Polygon(PolygonShape type)
        {
            _shape = type;
        }

        public void ResolveTexture(BitmapEntry texture)
        {
            if (texture == null)
            {
                return;
            }
            _texture = texture.Texture;
            if (_shape == PolygonShape.UnTexturedQuad)
                return;

            for (int i =0; i < _textureCoords.Count; i++)
            {
                Vector2 coord = _textureCoords[i];
                coord.X /= texture.Texture.Width;
                coord.Y /= texture.Texture.Height;
                _textureCoords[i] = coord;
            }
        }

        public List<VertexPositionTexture> GetVertices()
        {
            List<VertexPositionTexture> verts = new List<VertexPositionTexture>();
            //if (_type == PolygonType.Triangle || _type == PolygonType.Quad)
            //{
                verts.Add(new VertexPositionTexture(_vertices[0], _textureCoords[0]));
                verts.Add(new VertexPositionTexture(_vertices[1], _textureCoords[1]));
                verts.Add(new VertexPositionTexture(_vertices[2], _textureCoords[2]));
            //}
            if (_shape == PolygonShape.Quad || _shape == PolygonShape.UnTexturedQuad)
            {
                verts.Add(new VertexPositionTexture(_vertices[3], _textureCoords[3]));
                verts.Add(new VertexPositionTexture(_vertices[4], _textureCoords[4]));
                verts.Add(new VertexPositionTexture(_vertices[5], _textureCoords[5]));
            }
            //if ()
            //{
            //    verts.Add(new VertexPositionTexture(_vertices[0], Vector2.Zero));
            //    verts.Add(new VertexPositionTexture(_vertices[1], Vector2.Zero));
            //    verts.Add(new VertexPositionTexture(_vertices[2], Vector2.Zero));
            //    verts.Add(new VertexPositionTexture(_vertices[3], Vector2.Zero));
            //    verts.Add(new VertexPositionTexture(_vertices[4], Vector2.Zero));
            //    verts.Add(new VertexPositionTexture(_vertices[5], Vector2.Zero));
            //}
            return verts;
        }
    }
}
