using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NeedForSpeed.Loaders;
using NfsEngine;

namespace NeedForSpeed.Parsers.Track
{
    abstract class SceneryObject
    {
        public int SegmentRef;
        public Texture2D Texture;
        public Vector3 Position;
        public float Orientation;
        public Vector2 Size;
        public SceneryType Type;

        public abstract void Initialize();
        public virtual void Update(GameTime gameTime) { }
        public abstract void Render(AlphaTestEffect effect);
    }

		class BillboardScenery : SceneryObject
		{
			Matrix _matrix;
			public int TextureId;

			public BillboardScenery(Texture2D texture) : this(texture, 0)
			{
				
			}
			public BillboardScenery(Texture2D texture, int textureId)
			{
				Texture = texture;
				TextureId = textureId;
			}

			public override void Initialize()
			{
				if (Size.X == 0)
					Size.X = Texture.Width * TrackAssembler.ScaleFactor.X * 10000;
				if (Size.Y == 0)
					Size.Y = Texture.Height * TrackAssembler.ScaleFactor.Y * 10000;

				_matrix = Matrix.CreateScale(Size.X, Size.Y, 1) *
								Matrix.CreateRotationY(Orientation) *
								Matrix.CreateTranslation(Position);
			}

			public override void Render(AlphaTestEffect effect)
			{
				effect.World = _matrix;
				effect.Texture = Texture;
				effect.CurrentTechnique.Passes[0].Apply();
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			}
		}

    class AnimatedBillboardScenery : BillboardScenery
    {
        List<Texture2D> _textures;
        int _currentTexture;
        double _textureChangeTime;

        public AnimatedBillboardScenery(List<Texture2D> textures, double frameChange)
            : base(textures[0])
        {
            _textures = textures;
            _textures.RemoveAll(delegate(Texture2D tex)
            {
                return tex == null;
            });
            _textureChangeTime = frameChange;
        }

        public override void Update(GameTime gameTime)
        {
            _textureChangeTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_textureChangeTime < 0.0f)
            {
                _currentTexture++;
                _currentTexture %= _textures.Count;
                Texture = _textures[_currentTexture];
                _textureChangeTime = 0.2f;
            }
        }
    }

    class TwoSidedBillboardScenery : SceneryObject
    {
        Texture2D _texture1, _texture2;
        Matrix _matrix1, _matrix2;

        public TwoSidedBillboardScenery(Texture2D texture1, Texture2D texture2)
        {
            _texture1 = texture1;
            _texture2 = texture2;
        }

        public override void Initialize()
        {
            _matrix1 = Matrix.CreateScale(Size.X, Size.Y, 1) *
                    Matrix.CreateRotationY(Orientation) *
                    Matrix.CreateTranslation(Position);

            _matrix2 = Matrix.CreateScale(Size.X, Size.Y, 1) *
                    Matrix.CreateTranslation(Size.X / 2, 0, Size.X / 2) *
                    Matrix.CreateRotationY(Orientation + MathHelper.ToRadians(90)) *
                    Matrix.CreateTranslation(Position);
        }

        public override void Render(AlphaTestEffect effect)
        {
					//side 1
					effect.World = _matrix1;
					effect.Texture = _texture1;
					effect.CurrentTechnique.Passes[0].Apply();
					Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

					//side 2
					effect.World = _matrix2;
					effect.Texture = _texture2;
					effect.CurrentTechnique.Passes[0].Apply();
					Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }
    }

    class ModelScenery : SceneryObject
    {
        MeshChunk _mesh;
        Matrix _matrix;

        public ModelScenery(MeshChunk mesh)
        {
            _mesh = mesh;
        }

        public override void Initialize()
        {
            _matrix = Matrix.CreateScale(TrackAssembler.ScaleFactor * 3650f) *
                   Matrix.CreateRotationY(Orientation) *
                   Matrix.CreateTranslation(Position);
        }

				public override void Render(AlphaTestEffect effect)
				{
					effect.World = _matrix;
					_mesh.Render(effect);
				}
    }
}
