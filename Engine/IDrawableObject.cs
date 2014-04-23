using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace NfsEngine
{
    public interface IDrawableObject
    {
        void Update(GameTime gameTime);
        void Draw();        
    }
}
