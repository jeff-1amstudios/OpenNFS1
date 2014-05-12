using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using Microsoft.Xna.Framework;

namespace NeedForSpeed.Parsers
{
    class CarModel
    {
        HeaderChunk _root;
        bool _wasBraking;

        public CarModel(string filename)
        {
            Parse(filename);
        }

        private void Parse(string filename)
        {
            BitmapChunk.ResetPalette();
			string carFile = Path.Combine(GameConfig.CdDataPath, filename);
            BinaryReader br = new BinaryReader(File.Open(carFile, FileMode.Open));
            _root = new HeaderChunk();
            _root.Read(br, true);

            _root.MeshChunks[0].Load(br);
            _root.BitmapChunks[0].Load(br);

            br.Close();

            _root.MeshChunks[0].Resolve(_root.BitmapChunks[0]);
        }

        public void Render(Effect effect, bool braking)
        {
					effect.CurrentTechnique.Passes[0].Apply();

            if (_wasBraking != braking)
            {
                if (braking)
                {
                    _root.MeshChunks[0].ChangeTextures("rsid", "rsid_brake", _root.BitmapChunks[0]);
                }
                else
                {
                    _root.MeshChunks[0].ChangeTextures("rsid", "rsid", _root.BitmapChunks[0]);
                }
                _wasBraking = braking;
            }

            _root.MeshChunks[0].Render(effect);
        }

        public Texture2D TyreTexture
        {
            get
            {
                BitmapEntry tyre = _root.BitmapChunks[0].FindByName("tyr1");
                if (tyre == null)
                    tyre = _root.BitmapChunks[0].FindByName("tire");

                return tyre.Texture;
            }
        }
    }
}
