using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NfsEngine;
using NeedForSpeed.Parsers.Track;
using Microsoft.Xna.Framework.Input;

namespace NeedForSpeed.UI.Screens
{
    class MenuPoster
    {
        Vector3 _position;
        BillboardScenery _billboard;
        protected string _textureName, _title;

        public MenuPoster(string textureName, string title, Vector3 position)
        {
            _textureName = textureName;
            _title = title;
            _position = position;
            _billboard = new BillboardScenery(null);
            _billboard.Orientation = 0.43f;
            _billboard.Position = _position + new Vector3(0, -19, 0);
            _billboard.Size = new Vector2(58, 38);
        }

        public virtual void Render(AlphaTestEffect effect, bool selected)
        {
            //_billboard.Texture = Engine.Instance.ContentManager.Load<Texture2D>(_textureName);
            Vector3 position = _position;
            if (selected)
            {
                position.Z += 8;
                _billboard.Position.Z = position.Z + 1.1f;
            }
            else
            {
                _billboard.Position.Z = _position.Z + 1.1f;
            }
            _billboard.Initialize();

            Matrix world = Matrix.CreateScale(60, 40, 1.5f) * Matrix.CreateRotationY(0.43f) * Matrix.CreateTranslation(position);
            //Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube, world, Color.White, null);

						
            TrackBillboardModel.BeginBatch();
						effect.World = world;
            _billboard.Render(effect);

        }

        public virtual string Title
        {
            get
            {
                return _title;
            }
        }
    }
}
