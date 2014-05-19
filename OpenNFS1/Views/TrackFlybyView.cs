using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using OpenNFS1.Parsers.Track;
using NfsEngine;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Tracks;

namespace OpenNFS1.Views
{
    class TrackFlybyView : IView
    {
        private List<TrackNode> _nodes;
        int _currentNode;
        ChaseCamera _camera = new ChaseCamera();
        double _currentNodeTime = 0;

        public TrackFlybyView(Track track)
        {
            //_nodes = track.ro;

            _camera.DesiredPositionOffset = new Vector3(0.0f, 0.0f, 5.0f);
            _camera.LookAtOffset = Vector3.Zero;
            _camera.SetPosition(_nodes[0].Position + _camera.DesiredPosition);
            _camera.ChasePosition = _nodes[0].Position + new Vector3(0, 40, 0);
            _camera.Stiffness = 500;
            _camera.Damping = 800;
            
        }

        #region IView Members

        public bool Selectable
        {
            get { return false; }
        }

		public bool ShouldRenderPlayer { get { return true; } }

        public void Update(GameTime gameTime)
        {
            _currentNodeTime += gameTime.ElapsedGameTime.TotalMilliseconds * 0.5f;
            if (_currentNodeTime > 40)
            {
                _currentNodeTime = 0;
                _currentNode += 2;
                _currentNode %= _nodes.Count;

                //_camera.ChaseDirection = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(MathHelper.ToRadians(_nodes[_currentNode].Orientation)));
                _camera.ChasePosition = _nodes[_currentNode].Position + new Vector3(0, 40, 0);
                
            }            
        }

        public void Render()
        {
            
        }

        public void Activate()
        {
            Engine.Instance.Camera = _camera;
        }

        public void Deactivate()
        {
        }

        #endregion
    }
}
