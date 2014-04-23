using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace NfsEngine
{
    public interface IWorld : IDrawableObject
    {
        float GetHeightAtPoint(Vector3 position);
        void Reset();
    }
}
