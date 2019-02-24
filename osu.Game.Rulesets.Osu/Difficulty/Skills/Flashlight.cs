// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Osu.Difficulty.Skills
{
    /// <summary>
    /// Represents the skill required to adjust your movement and tapping to varying spacing and rhythms in short periods of time.
    /// </summary>
    public class Flashlight : Skill
    {
        private readonly IBeatmap beatmap;
        private readonly Mod[] mods;
        private readonly double clockRate;

        public Flashlight (IBeatmap beatmap, Mod[] mods, double clockRate)
        {
            this.beatmap = beatmap;
            this.mods = mods;
            this.clockRate = clockRate;
        }

        protected override double SkillMultiplier => 5000;
        protected override double StrainDecayBase => 1;
        protected override double DecayWeight => 1.0;

        private double flashlightRadius;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            double preempt = (int)BeatmapDifficulty.DifficultyRange(beatmap.BeatmapInfo.BaseDifficulty.ApproachRate, 1800, 1200, 450) / clockRate;
            double approachRate = preempt > 1200 ? (1800 - preempt) / 120 : (1200 - preempt) / 150 + 5;

            if (current.BaseObject is Spinner)
                return 0;

            if (Previous.Count > 0) 
            {
                double calculateDistance(OsuDifficultyHitObject obj) => obj.JumpDistance + obj.TravelDistance;

                var osuCurrent = (OsuDifficultyHitObject)current;
                var osuCurrentBaseObject = current.BaseObject as OsuHitObject;

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

                return 0;

            } else return 0;
        }
    }
}
