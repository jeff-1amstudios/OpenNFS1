using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace NeedForSpeed.Physics
{
    class ManualGearbox : BaseGearbox
    {
        public ManualGearbox(List<float> ratios, float changeTime)
            : base(ratios, changeTime)
        { }


        public override void Update(float motorRpmPercent)
        {
            if (Engine.Instance.Input.WasPressed(Keys.A) && _currentGear < Ratios.Count - 1)
                GearUp();
            if (Engine.Instance.Input.WasPressed(Keys.Z) && CurrentGear > -1 && _motor.CanChangeDown)
                GearDown();

            base.Update(motorRpmPercent);
        }
    }
}
