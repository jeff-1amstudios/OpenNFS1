using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework.Audio;
using GameEngine;
using Microsoft.Xna.Framework;
using OpenNFS1.Physics;
using System.Diagnostics;
using OpenNFS1.Audio;
using OpenNFS1.Parsers.Audio;

namespace OpenNFS1
{
	class VehicleAudioProvider2
	{
		const int GRASS_SLIDE_INDEX = 5;
		DrivableVehicle _car;

		SoundEffect _engineOnEffect, _engineOffEffect, _grassSlide;
		SoundEffectInstance _engineOn, _engineOff, _skidInstance, _grassSlideInstance;
		SoundEffect _gearChange;
		List<SoundEffect> _skids = new List<SoundEffect>();
		bool _isActive = false;

		public VehicleAudioProvider2(DrivableVehicle car)
		{
			_car = car;
		}


		public void Initialize()
		{
			if (_engineOn != null)
			{
				return;
			}

			_isActive = true;

			BnkFile bnk = new BnkFile(_car.Descriptor.SoundBnkFile);
			var sample = bnk.Samples[0];
			_engineOnEffect = new SoundEffect(sample.PCMData, sample.SampleRate, sample.NbrChannels == 2 ? AudioChannels.Stereo : AudioChannels.Mono);
			_engineOn = _engineOnEffect.CreateInstance();
			sample = bnk.Samples[1];
			_engineOffEffect = new SoundEffect(sample.PCMData, sample.SampleRate, sample.NbrChannels == 2 ? AudioChannels.Stereo : AudioChannels.Mono);
			_engineOff = _engineOffEffect.CreateInstance();
			sample = bnk.Samples[3];
			_gearChange = new SoundEffect(sample.PCMData, sample.SampleRate, sample.NbrChannels == 2 ? AudioChannels.Stereo : AudioChannels.Mono);
			

			//temp = Engine.Instance.ContentManager.Load<SoundEffect>(String.Format("Content/Audio/Vehicles/{0}/engine-on-low", _car.Descriptor.Name));
			//_engineOnLow = temp.CreateInstance();
			////.Play(0.3f, 0, 0);
			//temp = Engine.Instance.ContentManager.Load<SoundEffect>(String.Format("Content/Audio/Vehicles/{0}/engine-on-high", _car.Descriptor.Name));
			//_engineOn = temp.CreateInstance(); // temp.Play(0.3f, 0, 0);
			//temp = Engine.Instance.ContentManager.Load<SoundEffect>(String.Format("Content/Audio/Vehicles/{0}/engine-off-low", _car.Descriptor.Name));
			//_engineOffLow = temp.CreateInstance(); //temp.Play(0.3f, 0, 0);
			//temp = Engine.Instance.ContentManager.Load<SoundEffect>(String.Format("Content/Audio/Vehicles/{0}/engine-off-high", _car.Descriptor.Name));
			//_engineOff = temp.CreateInstance(); //temp.Play(0.3f, 0, 0);

			BnkFile envBnk = new BnkFile("COLL_SW.BNK");
			sample = envBnk.Samples[GRASS_SLIDE_INDEX];
			_grassSlide = new SoundEffect(sample.PCMData, sample.SampleRate, sample.NbrChannels == 2 ? AudioChannels.Stereo : AudioChannels.Mono);
			_grassSlideInstance = _grassSlide.CreateInstance();

			_skids.Add(Engine.Instance.ContentManager.Load<SoundEffect>("Content/Audio/Vehicles/common/skid1"));
			_skids.Add(Engine.Instance.ContentManager.Load<SoundEffect>("Content/Audio/Vehicles/common/skid2"));
			_skids.Add(Engine.Instance.ContentManager.Load<SoundEffect>("Content/Audio/Vehicles/common/skid3"));
		}

		public void UpdateEngine()
		{
			if (!_isActive) return;
			float engineRpmFactor = ((_car.Motor.Rpm - 0.8f) / _car.Motor.RedlineRpm) - 0.5f;
			if (_car.Motor.Throttle == 0 && !_car.Motor.AtRedline)
			{
				_engineOn.Pause();
				_engineOff.Resume();
				_engineOff.Pitch = engineRpmFactor;
			}
			else
			{
				_engineOn.Resume();
				_engineOff.Pause();
				_engineOn.Pitch = engineRpmFactor;
			}
		}

		public void PlaySkid(bool play)
		{
			if (!_isActive) return;
			if (play)
			{
				if (_skidInstance != null && _skidInstance.State == SoundState.Playing)
					return;
				if (_skidInstance != null && _skidInstance.State != SoundState.Playing)
				{
					_skidInstance.Resume();
				}
				else
				{
					_skidInstance = _skids[Engine.Instance.Random.Next(_skids.Count)].CreateInstance(); //.Play(0.3f, 0, 0);
				}
			}
			else
			{
				if (_skidInstance != null && _skidInstance.State == SoundState.Playing)
				{
					_skidInstance.Stop();
					_skidInstance = null;
				}
			}
		}

		public void ChangeGear()
		{
			if (!_isActive) return;
			_gearChange.Play();
		}

		public void HitGround()
		{
			if (!_isActive) return;
			EnvironmentAudioProvider.Instance.PlayCollision(2);
			SoundEngine2.Instance.PlayEffect(_skids[2].CreateInstance(), 0.2f);
		}


		public void PlayOffRoad(bool play)
		{
			if (!_isActive) return;
			if (play)
			{
				_grassSlideInstance.Volume = 0.4f;
				if (_grassSlideInstance.State == SoundState.Playing)
					return;
				else
					_grassSlideInstance.Resume();
			}
			else
			{
				_grassSlideInstance.Pause();
			}
		}

		public void StopAll()
		{
			if (!_isActive) return;
			if (_skidInstance != null)
				_skidInstance.Stop();
			_engineOff.Stop();
			_engineOn.Stop();
		}
	}
}
