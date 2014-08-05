using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine
{
    public interface IWorld : IDrawableObject
    {
        float GetHeightAtPoint(Vector3 position);
        void Reset();
    }
}
