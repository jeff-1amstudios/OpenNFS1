//-----------------------------------------------------------------------------
// Copyright (c) 2007 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NfsEngine;

namespace NfsEngine
{
    /// <summary>
    /// The FirstPersonCamera class implements the logic for a first person
    /// style 3D camera. This class also handles player input that is used
    /// to control the camera. To use this class, create an instance of the
    /// FirstPersonCamera class and then call the Update() method once a
    /// frame from your game's main loop. The FirstPersonCamera's Update()
    /// method will process mouse and keyboard input used to manipulate the
    /// camera. To change the default movement key bindings call the
    /// MapActionToKey() method. Most of the code in this class is used to
    /// simulate camera view bobbing, crouching, and jumping.
    /// </summary>
    public class FPSCamera : GameObject, ICamera
    {
        
        public const float DEFAULT_FOVX = 60.0f;
        public const float DEFAULT_ROTATION_SPEED = 0.25f;
        public const float DEFAULT_ZFAR = 10000.0f;
        public const float DEFAULT_ZNEAR = 0.1f;

        private const float GRAVITY = -9.8f;
        private const float DECELERATION = -0.5f;
        private const float STRAFE_SPEED_MULTIPLIER = 1.5f;
        
               
        private float fovx;
        private float znear;
        private float zfar;
        
        private Matrix _viewMatrix;
        private Matrix _projMatrix;

        public FPSCamera(Game game)
        {

            fovx = DEFAULT_FOVX;
            znear = DEFAULT_ZNEAR;
            zfar = DEFAULT_ZFAR;            
            
            _position = new Vector3(0.0f, 0.0f, 0.0f);
           

            int w = game.Window.ClientBounds.Width;
            int h = game.Window.ClientBounds.Height;
            float aspect = (float)w / (float)h;

            // Setup initial default view and projection matrices.
            Perspective(DEFAULT_FOVX, aspect, DEFAULT_ZNEAR, DEFAULT_ZFAR);
            _viewMatrix = Matrix.Identity;


        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Render()
        {
            
        }

               
        /// <summary>
        /// Builds a perspective projection matrix based on a horizontal field
        /// of view. The aspect ratio is calculated by dividing the viewport's
        /// width by its height.
        /// </summary>
        /// <param name="fovx">Horizontal field of view in degrees.</param>
        /// <param name="aspect">Aspect ratio.</param>
        /// <param name="znear">Near plane distance.</param>
        /// <param name="zfar">Far plane distance.</param>
        public void Perspective(float fovx, float aspect, float znear, float zfar)
        {
            this.fovx = fovx;
            this.znear = znear;
            this.zfar = zfar;

            float e = 1.0f / (float)Math.Tan(MathHelper.ToRadians(fovx) / 2.0f);
            float aspectInv = 1.0f / aspect;
            float fovy = 2.0f * (float)Math.Atan(aspectInv / e);
            float xScale = 1.0f / (float)Math.Tan(0.5f * fovy);
            float yScale = xScale / aspectInv;

            _projMatrix.M11 = xScale;
            _projMatrix.M12 = 0.0f;
            _projMatrix.M13 = 0.0f;
            _projMatrix.M14 = 0.0f;

            _projMatrix.M21 = 0.0f;
            _projMatrix.M22 = yScale;
            _projMatrix.M23 = 0.0f;
            _projMatrix.M24 = 0.0f;

            _projMatrix.M31 = 0.0f;
            _projMatrix.M32 = 0.0f;
            _projMatrix.M33 = (zfar + znear) / (znear - zfar);
            _projMatrix.M34 = -1.0f;

            _projMatrix.M41 = 0.0f;
            _projMatrix.M42 = 0.0f;
            _projMatrix.M43 = (2.0f * zfar * znear) / (znear - zfar);
            _projMatrix.M44 = 0.0f;
        }


        public void FollowObject(GameObject obj)
        {
            _position = -obj.GetCameraPosition();
            _orientation = obj.Orientation;
            UpdateViewMatrix();
        } 

        private void UpdateViewMatrix()
        {
            Matrix view = Matrix.CreateTranslation(_position);
            view *= Matrix.CreateRotationY(-_orientation.X);
            view *= Matrix.CreateRotationX(_orientation.Y);
            view *= Matrix.CreateRotationZ(_orientation.Z);
            
            _viewMatrix = view;
        }
       

        /// <summary>
        /// Returns the camera's current view matrix.
        /// </summary>
        public Matrix View
        {
            get { return _viewMatrix; }
        }

        /// <summary>
        /// Returns the camera's current perspective projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return _projMatrix; }
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }
    }
}