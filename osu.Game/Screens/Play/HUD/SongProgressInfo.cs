﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using System;


namespace osu.Game.Screens.Play.HUD
{
    public class SongProgressInfo : Container
    {
        private GrowToFitContainer timeCurrentContainer;
        private GrowToFitContainer timeLeftContainer;
        private GrowToFitContainer progressContainer;

        private OsuSpriteText timeCurrent;
        private OsuSpriteText timeLeft;
        private OsuSpriteText progress;

        private double startTime;
        private double endTime;

        private int? previousPercent;
        private int? previousSecond;

        private double songLength => endTime - startTime;

        private const int margin = 10;

        public double StartTime
        {
            set => startTime = value;
        }

        public double EndTime
        {
            set => endTime = value;
        }

        private GameplayClock gameplayClock;

        [BackgroundDependencyLoader(true)]
        private void load(OsuColour colours, GameplayClock clock)
        {
            if (clock != null)
                gameplayClock = clock;

            AutoSizeAxes = Axes.Y;
            Children = new Drawable[]
            {
                new Container
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    AutoSizeAxes = Axes.Both,
                    Child = timeCurrentContainer = new GrowToFitContainer
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Child = timeCurrent = new OsuSpriteText
                        {
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Colour = colours.BlueLighter,
                            Font = OsuFont.Numeric,
                        }
                    }
                },
                new Container
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Child = timeLeftContainer = new GrowToFitContainer
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Child = progress = new OsuSpriteText
                        {
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Colour = colours.BlueLighter,
                            Font = OsuFont.Numeric,
                         }
                    }
                },
                new Container
                {
                    Origin = Anchor.CentreRight,
                    Anchor = Anchor.CentreRight,
                    AutoSizeAxes = Axes.Both,
                    Child = progressContainer = new GrowToFitContainer
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Child = timeLeft = new OsuSpriteText
                        {
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Colour = colours.BlueLighter,
                            Font = OsuFont.Numeric,
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            keepTextSpritesUpright();
        }

        protected override void Update()
        {
            base.Update();

            double time = gameplayClock?.CurrentTime ?? Time.Current;

            double songCurrentTime = time - startTime;
            int currentPercent = Math.Max(0, Math.Min(100, (int)(songCurrentTime / songLength * 100)));
            int currentSecond = (int)Math.Floor(songCurrentTime / 1000.0);

            if (currentPercent != previousPercent)
            {
                progress.Text = currentPercent.ToString() + @"%";
                previousPercent = currentPercent;
            }

            if (currentSecond != previousSecond && songCurrentTime < songLength)
            {
                timeCurrent.Text = formatTime(TimeSpan.FromSeconds(currentSecond));
                timeLeft.Text = formatTime(TimeSpan.FromMilliseconds(endTime - time));

                previousSecond = currentSecond;
            }
        }

        private string formatTime(TimeSpan timeSpan) => $"{(timeSpan < TimeSpan.Zero ? "-" : "")}{Math.Floor(timeSpan.Duration().TotalMinutes)}:{timeSpan.Duration().Seconds:D2}";

        private void keepTextSpritesUpright()
        {
            timeCurrentContainer.OnUpdate += (timeCurrentContainer) => { Extensions.DrawableExtensions.KeepUprightAndUnscaled(timeCurrentContainer); };
            progressContainer.OnUpdate += (progressContainer) => { Extensions.DrawableExtensions.KeepUprightAndUnscaled(progressContainer); };
            timeLeftContainer.OnUpdate += (timeLeftContainer) => { Extensions.DrawableExtensions.KeepUprightAndUnscaled(timeLeftContainer); };
        }

    }
}
