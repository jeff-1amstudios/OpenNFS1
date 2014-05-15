using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace NeedForSpeed.Views
{
    interface IView
    {
        bool Selectable { get; }
		bool ShouldRenderPlayer { get; }
        void Update(GameTime gameTime);
        void Render();
        void Activate();
        void Deactivate();
		
    }
}
