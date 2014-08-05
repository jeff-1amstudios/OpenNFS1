using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace OpenNFS1.Physics
{
    class ManualGearbox : BaseGearbox
    {
        public ManualGearbox(List<float> ratios, float changeTime)
            : base(ratios, changeTime)
        { }


        public override void Update(float motorRpmPercent, GearboxAction action)
        {
            if (action == GearboxAction.GearUp && _currentGear < Ratios.Count - 1)
                GearUp();
            if (action == GearboxAction.GearDown && CurrentGear > -1 && _motor.CanChangeDown)
                GearDown();

            base.Update(motorRpmPercent, action);
        }
    }
}
