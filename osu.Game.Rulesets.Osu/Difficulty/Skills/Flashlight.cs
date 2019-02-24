// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Difficulty.Skills
{
    /// <summary>
    /// Represents the skill required to adjust your movement and tapping to varying spacing and rhythms in short periods of time.
    /// </summary>
    public class Flashlight : Skill
    {
        protected override double SkillMultiplier => 5000;
        protected override double StrainDecayBase => 1;
        protected override double DecayWeight => 1.0;

        private double flashlightRadius;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            if (current.BaseObject is Spinner)
                return 0;

            if (Previous.Count > 0) 
            {
                double calculateDistance(OsuDifficultyHitObject obj) => obj.JumpDistance + obj.TravelDistance;

                var osuCurrent = (OsuDifficultyHitObject)current;
                var osuPrevious = (OsuDifficultyHitObject)Previous[0];

                var osuCurrentBaseObject = current.BaseObject as OsuHitObject;
                var osuPreviousBaseObject = Previous[0].BaseObject as OsuHitObject;

                if (osuCurrentBaseObject.CumulativeCombo < 100)
                {
                    flashlightRadius = 160;
                } else if (osuCurrentBaseObject.CumulativeCombo < 200)
                {
                    flashlightRadius = 120;
                } else
                {
                    flashlightRadius = 100;
                }

                return (osuCurrentBaseObject.CumulativeCombo);         
            } else return 0;
        }
    }
}
