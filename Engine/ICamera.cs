using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine;

namespace GameEngine
{
    public interface ICamera
    {
        /// <summary>
        /// Returns the camera's current view matrix.
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// Returns the camera's current perspective projection matrix.
        /// </summary>
        Matrix Projection { get; }

        void FollowObject(GameObject obj);

        void Update(GameTime time);

        Vector3 Position { get;}

        void SetPosition(Vector3 position);
    }
}
