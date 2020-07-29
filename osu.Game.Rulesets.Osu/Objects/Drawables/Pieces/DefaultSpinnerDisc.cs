// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Osu.Objects.Drawables.Pieces
{
    public class DefaultSpinnerDisc : CompositeDrawable
    {
        private DrawableSpinner drawableSpinner;

        private Spinner spinner;

        private const float idle_alpha = 0.2f;
        private const float tracking_alpha = 0.4f;

        private Color4 normalColour;
        private Color4 completeColour;

        private SpinnerTicks ticks;

        private int completeTick;
        private SpinnerFill fill;
        private Container mainContainer;
        private SpinnerCentreLayer centre;
        private SpinnerBackgroundLayer background;

        public DefaultSpinnerDisc()
        {
            RelativeSizeAxes = Axes.Both;

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours, DrawableHitObject drawableHitObject)
        {
            drawableSpinner = (DrawableSpinner)drawableHitObject;
            spinner = (Spinner)drawableSpinner.HitObject;

            normalColour = colours.BlueDark;
            completeColour = colours.YellowLight;

            InternalChildren = new Drawable[]
            {
                mainContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        background = new SpinnerBackgroundLayer(),
                        fill = new SpinnerFill
                        {
                            Alpha = idle_alpha,
                            AccentColour = normalColour
                        },
                        ticks = new SpinnerTicks
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            AccentColour = normalColour
                        },
                    }
                },
                centre = new SpinnerCentreLayer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            centre.ScaleTo(0);
            mainContainer.ScaleTo(0);
            this.ScaleTo(1);

            drawableSpinner.RotationTracker.Complete.BindValueChanged(complete => updateComplete(complete.NewValue, 200));
            drawableSpinner.State.BindValueChanged(updateStateTransforms, true);
        }

        private void updateStateTransforms(ValueChangedEvent<ArmedState> state)
        {
            using (BeginAbsoluteSequence(spinner.StartTime - spinner.TimePreempt / 2, true))
            {
                float phaseOneScale = spinner.Scale * 0.7f;

                centre.ScaleTo(phaseOneScale, spinner.TimePreempt / 4, Easing.OutQuint);

                mainContainer
                    .ScaleTo(phaseOneScale * drawableSpinner.RelativeHeight * 1.6f, spinner.TimePreempt / 4, Easing.OutQuint);

                this.RotateTo((float)(25 * spinner.Duration / 2000), spinner.TimePreempt + spinner.Duration);

                using (BeginDelayedSequence(spinner.TimePreempt / 2, true))
                {
                    centre.ScaleTo(spinner.Scale, spinner.TimePreempt / 2, Easing.OutQuint);
                    mainContainer.ScaleTo(1, spinner.TimePreempt / 2, Easing.OutQuint);
                }
            }

            // transforms we have from completing the spinner will be rolled back, so reapply immediately.
            updateComplete(state.NewValue == ArmedState.Hit, 0);

            using (BeginDelayedSequence(spinner.Duration, true))
            {
                switch (state.NewValue)
                {
                    case ArmedState.Hit:
                        this.ScaleTo(Scale * 1.2f, 320, Easing.Out);
                        this.RotateTo(mainContainer.Rotation + 180, 320);
                        break;

                    case ArmedState.Miss:
                        this.ScaleTo(Scale * 0.8f, 320, Easing.In);
                        break;
                }
            }
        }

        private void updateComplete(bool complete, double duration)
        {
            var colour = complete ? completeColour : normalColour;

            ticks.FadeAccent(colour.Darken(1), duration);
            fill.FadeAccent(colour.Darken(1), duration);

            background.FadeAccent(colour, duration);
            centre.FadeAccent(colour, duration);
        }

        private bool updateCompleteTick() => completeTick != (completeTick = (int)(drawableSpinner.RotationTracker.CumulativeRotation / 360));

        protected override void Update()
        {
            base.Update();

            if (drawableSpinner.RotationTracker.Complete.Value && updateCompleteTick())
            {
                fill.FinishTransforms(false, nameof(Alpha));
                fill
                    .FadeTo(tracking_alpha + 0.2f, 60, Easing.OutExpo)
                    .Then()
                    .FadeTo(tracking_alpha, 250, Easing.OutQuint);
            }

            float relativeCircleScale = spinner.Scale * drawableSpinner.RelativeHeight;
            float targetScale = relativeCircleScale + (1 - relativeCircleScale) * drawableSpinner.Progress;

            fill.Scale = new Vector2((float)Interpolation.Lerp(fill.Scale.X, targetScale, Math.Clamp(Math.Abs(Time.Elapsed) / 100, 0, 1)));
            mainContainer.Rotation = drawableSpinner.RotationTracker.Rotation;
        }
    }
}
