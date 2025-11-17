using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace HeartSender.Components
{
    public enum IconAnimationType
    {
        None,
        Spinner,
        FlyingPlane,
        Bounce,
        Pulse,
        Gear,
        TypingDots,
        Envelope,
        Atomic,
        Wave,
        Shake,
        Flip,
        Slide,
        Zoom,
        Swing,
        HeartBeat,
        Float,
        Rotate3D,
        Sparkle,
        Ripple,
        Fade,
        Dance,
        Orbit,
        Breathe,
        Jello,
        RubberBand,
        Tada,
        Flash,
        Wobble,
        ElasticBounce,
        Writing,
        EarthRotate,
        DoorExit
    }

    [ToolboxItem(true)]
    [Description("Ultra modern animated button with 30+ smooth professional animations")]
    public class ModernButton : Button
    {
        private int borderRadius = 12;
        private Color primaryColor = Color.FromArgb(245, 245, 245);
        private Color hoverColor = Color.FromArgb(185, 28, 28);
        private Color pressedColor = Color.FromArgb(153, 27, 27);
        private Color borderColor = Color.FromArgb(220, 220, 220);
        private Color shadowColor = Color.FromArgb(185, 28, 28);
        private Color hoverForeColor = Color.Empty; // Empty means use default ForeColor
        private IconAnimationType animationType = IconAnimationType.Spinner;

        private bool isHovered = false;
        private bool isPressed = false;
        private float iconScale = 1.0f;
        private float iconRotation = 0f;
        private float buttonScale = 1.0f;
        private float shadowOpacity = 0f;
        private int shadowOffset = 0;
        private float colorTransition = 0f;
        private float iconOpacity = 1.0f;
        private float iconYOffset = 0f;
        private float textColorTransition = 0f;
        private float iconXOffset = 0f;
        private float breathingScale = 1.0f;
        private bool animationPlayed = false;
        private float animationPhase = 0f;
        private float secondaryPhase = 0f;
        private float tertiaryPhase = 0f;
        private float iconSkewX = 0f;
        private float iconSkewY = 0f;
        private float iconGlow = 0f;
        private System.Windows.Forms.Timer animationTimer;
        private Image? buttonIcon;
        private int iconSize = 28;
        private ContentAlignment iconAlignment = ContentAlignment.MiddleLeft;
        private bool enableRipple = true;
        private List<RippleEffect> ripples = new List<RippleEffect>();
        private List<SparkleParticle> sparkles = new List<SparkleParticle>();

        private class RippleEffect
        {
            public PointF Origin { get; set; }
            public float Radius { get; set; }
            public float Opacity { get; set; }
        }

        private class SparkleParticle
        {
            public PointF Position { get; set; }
            public float Size { get; set; }
            public float Opacity { get; set; }
            public float Velocity { get; set; }
            public float Angle { get; set; }
        }

        public ModernButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(60, 60, 70);
            Font = new Font(new FontFamily("Segoe UI"), 9.5F, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Padding = new Padding(24, 12, 24, 12);
            Size = new Size(140, 48);

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null)
            {
                Parent.Invalidate(Bounds, true);
            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            Invalidate();
        }

        // Properties
        [Category("Appearance")]
        [DefaultValue(12)]
        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PrimaryColor
        {
            get => primaryColor;
            set { primaryColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverColor
        {
            get => hoverColor;
            set { hoverColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedColor
        {
            get => pressedColor;
            set { pressedColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ShadowColor
        {
            get => shadowColor;
            set { shadowColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Text color when hovering. Leave empty to use default ForeColor.")]
        public Color HoverForeColor
        {
            get => hoverForeColor;
            set { hoverForeColor = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IconAnimationType AnimationType
        {
            get => animationType;
            set { animationType = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? ButtonIcon
        {
            get => buttonIcon;
            set { buttonIcon = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Image? Image
        {
            get => buttonIcon;
            set { buttonIcon = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(28)]
        public int IconSize
        {
            get => iconSize;
            set { iconSize = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment IconAlignment
        {
            get => iconAlignment;
            set { iconAlignment = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool EnableRipple
        {
            get => enableRipple;
            set { enableRipple = value; }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            animationPlayed = false;
            animationTimer.Start();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            animationTimer.Start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            isPressed = true;

            if (enableRipple && e.Button == MouseButtons.Left)
            {
                ripples.Add(new RippleEffect
                {
                    Origin = e.Location,
                    Radius = 0,
                    Opacity = 1.0f
                });
            }

            animationTimer.Start();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isPressed = false;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            bool needsUpdate = false;

            // Icon scale animation
            float targetIconScale = isHovered ? 1.10f : 1.0f;
            if (isPressed) targetIconScale = 0.95f;

            if (Math.Abs(iconScale - targetIconScale) > 0.0001f)
            {
                iconScale += (targetIconScale - iconScale) * 0.30f;
                needsUpdate = true;
            }

            breathingScale = 1.0f;

            // Animation based on AnimationType
            switch (animationType)
            {
                case IconAnimationType.Spinner:
                    needsUpdate |= AnimateSpinner();
                    break;

                case IconAnimationType.FlyingPlane:
                    needsUpdate |= AnimateFlyingPlane();
                    break;

                case IconAnimationType.Bounce:
                    needsUpdate |= AnimateBounce();
                    break;

                case IconAnimationType.Pulse:
                    needsUpdate |= AnimatePulse();
                    break;

                case IconAnimationType.Gear:
                    needsUpdate |= AnimateGear();
                    break;

                case IconAnimationType.TypingDots:
                    needsUpdate |= AnimateTypingDots();
                    break;

                case IconAnimationType.Envelope:
                    needsUpdate |= AnimateEnvelope();
                    break;

                case IconAnimationType.Atomic:
                    needsUpdate |= AnimateAtomic();
                    break;

                case IconAnimationType.Wave:
                    needsUpdate |= AnimateWave();
                    break;

                case IconAnimationType.Shake:
                    needsUpdate |= AnimateShake();
                    break;

                case IconAnimationType.Flip:
                    needsUpdate |= AnimateFlip();
                    break;

                case IconAnimationType.Slide:
                    needsUpdate |= AnimateSlide();
                    break;

                case IconAnimationType.Zoom:
                    needsUpdate |= AnimateZoom();
                    break;

                case IconAnimationType.Swing:
                    needsUpdate |= AnimateSwing();
                    break;

                case IconAnimationType.HeartBeat:
                    needsUpdate |= AnimateHeartBeat();
                    break;

                case IconAnimationType.Float:
                    needsUpdate |= AnimateFloat();
                    break;

                case IconAnimationType.Rotate3D:
                    needsUpdate |= AnimateRotate3D();
                    break;

                case IconAnimationType.Sparkle:
                    needsUpdate |= AnimateSparkle();
                    break;

                case IconAnimationType.Ripple:
                    needsUpdate |= AnimateRippleWaves();
                    break;

                case IconAnimationType.Fade:
                    needsUpdate |= AnimateFade();
                    break;

                case IconAnimationType.Dance:
                    needsUpdate |= AnimateDance();
                    break;

                case IconAnimationType.Orbit:
                    needsUpdate |= AnimateOrbit();
                    break;

                case IconAnimationType.Breathe:
                    needsUpdate |= AnimateBreathe();
                    break;

                case IconAnimationType.Jello:
                    needsUpdate |= AnimateJello();
                    break;

                case IconAnimationType.RubberBand:
                    needsUpdate |= AnimateRubberBand();
                    break;

                case IconAnimationType.Tada:
                    needsUpdate |= AnimateTada();
                    break;

                case IconAnimationType.Flash:
                    needsUpdate |= AnimateFlash();
                    break;

                case IconAnimationType.Wobble:
                    needsUpdate |= AnimateWobble();
                    break;

                case IconAnimationType.ElasticBounce:
                    needsUpdate |= AnimateElasticBounce();
                    break;

                case IconAnimationType.Writing:
                    needsUpdate |= AnimateWriting();
                    break;

                case IconAnimationType.EarthRotate:
                    needsUpdate |= AnimateEarthRotate();
                    break;

                case IconAnimationType.DoorExit:
                    needsUpdate |= AnimateDoorExit();
                    break;

                case IconAnimationType.None:
                default:
                    ResetAnimation();
                    break;
            }

            // Common animations
            needsUpdate |= UpdateTextColorTransition();
            needsUpdate |= UpdateButtonScale();
            needsUpdate |= UpdateShadow();
            needsUpdate |= UpdateColorTransition();
            needsUpdate |= UpdateRipples();

            if (needsUpdate)
                Invalidate();
            else if (ripples.Count == 0 && sparkles.Count == 0 && !isHovered)
                animationTimer.Stop();
        }

        #region Animation Methods

        private bool AnimateSpinner()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 9f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }
                iconRotation = animationPhase;
                float progress = animationPhase / 360f;
                breathingScale = 1.0f + ((float)Math.Sin(progress * Math.PI) * 0.08f);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (animationPhase > 0f)
                {
                    if (animationPhase > 340f)
                        animationPhase = 0f;
                    else
                        animationPhase += (360f - animationPhase) * 0.20f;
                    iconRotation = animationPhase;
                    updated = true;
                }
            }
            ResetOffsets();
            return updated;
        }

        private bool AnimateFlyingPlane()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                iconYOffset -= 2.8f;
                iconOpacity -= 0.026f;
                if (iconOpacity < 0f) iconOpacity = 0f;

                if (iconYOffset < -45f || iconOpacity <= 0f)
                    animationPlayed = true;

                float targetRot = -90f;
                if (Math.Abs(iconRotation - targetRot) > 0.3f)
                    iconRotation += (targetRot - iconRotation) * 0.20f;
                else
                    iconRotation = targetRot;

                breathingScale = 1.0f - ((Math.Abs(iconYOffset) / 45f) * 0.15f);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(iconYOffset) > 0.1f)
                {
                    iconYOffset *= 0.88f;
                    if (Math.Abs(iconYOffset) < 0.1f) iconYOffset = 0f;
                    updated = true;
                }

                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.20f;
                    updated = true;
                }

                if (Math.Abs(iconRotation) > 0.3f)
                {
                    iconRotation *= 0.88f;
                    if (Math.Abs(iconRotation) < 0.3f) iconRotation = 0f;
                    updated = true;
                }

                if (Math.Abs(breathingScale - 1.0f) > 0.01f)
                {
                    breathingScale += (1.0f - breathingScale) * 0.22f;
                    updated = true;
                }
            }
            iconXOffset = 0f;
            return updated;
        }

        private bool AnimateBounce()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                iconYOffset += 0.50f;
                if (iconYOffset > 8f)
                {
                    iconYOffset = 8f;
                    animationPlayed = true;
                }
                float progress = Math.Min(iconYOffset / 8f, 1f);
                breathingScale = 1.0f + ((float)Math.Sin(progress * Math.PI) * 0.10f);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(iconYOffset) > 0.05f)
                {
                    iconYOffset *= 0.85f;
                    updated = true;
                }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f)
                {
                    breathingScale += (1.0f - breathingScale) * 0.25f;
                    updated = true;
                }
            }
            iconXOffset = 0f;
            iconRotation = 0f;
            return updated;
        }

        private bool AnimatePulse()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                breathingScale += 0.042f;
                if (breathingScale > 1.25f)
                    animationPlayed = true;
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(breathingScale - 1.0f) > 0.01f)
                {
                    breathingScale += (1.0f - breathingScale) * 0.24f;
                    updated = true;
                }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateGear()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 4.0f;
                float progress = animationPhase / 360f;
                iconXOffset = (float)Math.Sin(progress * Math.PI * 8f) * 0.3f;

                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }
                iconRotation = animationPhase;
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (animationPhase > 0f)
                {
                    float nearest = (float)(Math.Round(animationPhase / 45.0) * 45.0);
                    if (nearest >= 360f) nearest = 0f;

                    if (Math.Abs(animationPhase - nearest) > 0.3f)
                        animationPhase += (nearest - animationPhase) * 0.25f;
                    else
                    {
                        animationPhase = nearest;
                        if (animationPhase >= 360f) animationPhase = 0f;
                    }
                    iconRotation = animationPhase;
                    updated = true;
                }

                if (Math.Abs(iconXOffset) > 0.01f)
                {
                    iconXOffset *= 0.82f;
                    updated = true;
                }
            }
            iconYOffset = 0f;
            return updated;
        }

        private bool AnimateTypingDots()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.070f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                float wave = (float)Math.Sin(rad);
                float absWave = Math.Abs(wave);
                float eased = 1f - (float)Math.Pow(1f - absWave, 4);

                iconYOffset = -(eased * 4f);
                breathingScale = 1.0f + (eased * 0.08f);
                iconOpacity = 0.84f + (eased * 0.16f);
                iconXOffset = wave * 0.7f;
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.90f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.88f; updated = true; }
                if (Math.Abs(iconXOffset) > 0.05f) { iconXOffset *= 0.88f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }

                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.22f;
                    updated = true;
                }
            }
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateEnvelope()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                // Smooth opening animation
                if (animationPhase < 1.0f)
                {
                    animationPhase += 0.055f;
                    if (animationPhase >= 1.0f)
                    {
                        animationPhase = 1.0f;
                        animationPlayed = true;
                    }
                    updated = true;
                }

                // Cubic ease-out for smooth natural motion
                float t = animationPhase;
                float eased = t < 0.5f ? 4f * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 3f) / 2f;

                // Don't rotate the icon - we're drawing the opening effect separately
                iconRotation = 0f;
                
                // Slight scale growth for emphasis
                breathingScale = 1.0f + (eased * 0.14f);
                
                // Gentle upward float
                iconYOffset = -(eased * 3.5f);
                
                // Slight horizontal sway during opening
                iconXOffset = (float)Math.Sin(eased * Math.PI) * 1.2f;
                
                // Glow effect during opening
                iconGlow = (float)Math.Sin(eased * Math.PI) * 0.7f;
                
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                // Smooth closing animation
                if (animationPhase > 0f)
                {
                    animationPhase -= 0.045f;
                    if (animationPhase <= 0f) animationPhase = 0f;
                    updated = true;
                }

                float t = animationPhase;
                float eased = t < 0.5f ? 4f * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 3f) / 2f;

                iconRotation = 0f;
                breathingScale = 1.0f + (eased * 0.14f);
                iconYOffset = -(eased * 3.5f);
                iconXOffset = (float)Math.Sin(eased * Math.PI) * 1.2f;
                iconGlow = (float)Math.Sin(eased * Math.PI) * 0.7f;
            }
            
            iconOpacity = 1.0f;
            return updated;
        }

        private bool AnimateAtomic()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                // Faster orbit speeds for more dynamic effect
                animationPhase += 4.5f; // Primary electron orbit
                if (animationPhase >= 360f) animationPhase -= 360f;

                secondaryPhase -= 3.8f; // Secondary electron (opposite direction)
                if (secondaryPhase <= -360f) secondaryPhase += 360f;

                tertiaryPhase += 5.2f; // Tertiary electron (fastest)
                
                // Stop animation after one complete cycle of the slowest orbit
                if (tertiaryPhase >= 360f)
                {
                    tertiaryPhase = 360f;
                    animationPlayed = true;
                }

                // Nucleus rotation
                iconRotation += 3.0f;
                if (iconRotation >= 360f) iconRotation -= 360f;

                // Energy pulsing effect
                breathingScale = 1.0f + (float)(Math.Sin(animationPhase * Math.PI / 90.0) * 0.12f);
                
                // Glow intensity based on orbit positions
                iconGlow = 0.5f + (float)(Math.Abs(Math.Sin(animationPhase * Math.PI / 180.0)) * 0.4f);

                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                // Smooth reset
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.88f; updated = true; }
                else { iconRotation = 0f; }

                if (Math.Abs(tertiaryPhase) > 0.5f) { tertiaryPhase *= 0.90f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.24f; updated = true; }
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.90f; updated = true; }
                if (Math.Abs(secondaryPhase) > 1f) { secondaryPhase *= 0.90f; updated = true; }
                if (Math.Abs(iconGlow) > 0.01f) { iconGlow *= 0.88f; updated = true; }
            }
            iconXOffset = 0f;
            iconYOffset = 0f;
            return updated;
        }

        private bool AnimateWave()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.08f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                iconXOffset = (float)Math.Sin(rad) * 6f;
                iconYOffset = (float)Math.Cos(rad * 2) * 3f;
                iconRotation = (float)Math.Sin(rad) * 8f;
                breathingScale = 1.0f + (float)(Math.Abs(Math.Sin(rad)) * 0.06f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.88f; updated = true; }
                if (Math.Abs(iconXOffset) > 0.05f) { iconXOffset *= 0.86f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.86f; updated = true; }
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.86f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.20f; updated = true; }
            }
            return updated;
        }

        private bool AnimateShake()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 25f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;
                float intensity = (float)Math.Sin(progress * Math.PI);
                iconXOffset = (float)Math.Sin(animationPhase * Math.PI / 180.0 * 10f) * 4f * intensity;
                breathingScale = 1.0f + (intensity * 0.04f);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.85f; updated = true; }
                if (Math.Abs(iconXOffset) > 0.05f) { iconXOffset *= 0.82f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.20f; updated = true; }
            }
            iconYOffset = 0f;
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateFlip()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 8f;
                if (animationPhase >= 180f)
                {
                    animationPhase = 180f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 180f;
                float scaleEffect = (float)Math.Abs(Math.Cos(progress * Math.PI));
                iconScale = 0.3f + scaleEffect * 0.7f;
                iconOpacity = 0.5f + scaleEffect * 0.5f;
                iconRotation = animationPhase;
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(animationPhase) > 1f)
                {
                    animationPhase *= 0.85f;
                    float progress = animationPhase / 180f;
                    float scaleEffect = (float)Math.Abs(Math.Cos(progress * Math.PI));
                    iconScale = 0.3f + scaleEffect * 0.7f;
                    iconOpacity = 0.5f + scaleEffect * 0.5f;
                    iconRotation = animationPhase;
                    updated = true;
                }
                else
                {
                    animationPhase = 0f;
                    iconScale = 1.0f;
                    iconOpacity = 0.88f;
                    iconRotation = 0f;
                }
            }
            iconXOffset = 0f;
            iconYOffset = 0f;
            return updated;
        }

        private bool AnimateSlide()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                iconXOffset += 1.5f;
                if (iconXOffset > 30f)
                {
                    iconXOffset = 30f;
                    animationPlayed = true;
                }
                // Keep opacity visible - fade only slightly
                iconOpacity = 1.0f - (iconXOffset / 60f); // Max fade to 0.5 (50%)
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(iconXOffset) > 0.1f)
                {
                    iconXOffset *= 0.82f;
                    iconOpacity = Math.Max(0.88f, 1.0f - (Math.Abs(iconXOffset) / 60f));
                    updated = true;
                }
            }
            iconYOffset = 0f;
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateZoom()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                breathingScale += 0.065f;
                if (breathingScale > 1.5f)
                {
                    breathingScale = 1.5f;
                    animationPlayed = true;
                }
                iconOpacity = Math.Max(0.3f, 1.5f - breathingScale);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(breathingScale - 1.0f) > 0.01f)
                {
                    breathingScale += (1.0f - breathingScale) * 0.22f;
                    iconOpacity = Math.Max(0.88f, Math.Min(1.0f, iconOpacity + 0.05f));
                    updated = true;
                }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateSwing()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.09f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                float swing = (float)Math.Sin(rad) * 15f;
                iconRotation = swing;
                iconYOffset = Math.Abs(swing) * 0.15f;
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.88f; updated = true; }
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.85f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.85f; updated = true; }
            }
            iconXOffset = 0f;
            return updated;
        }

        private bool AnimateHeartBeat()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.12f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                float beat = (float)(Math.Sin(rad * 2) * 0.5 + 0.5);
                breathingScale = 1.0f + (beat * 0.18f);
                iconOpacity = 0.88f + (beat * 0.12f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.88f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }

                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.22f;
                    updated = true;
                }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateFloat()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.06f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                iconYOffset = (float)Math.Sin(rad) * 6f;
                breathingScale = 1.0f + ((float)Math.Abs(Math.Sin(rad)) * 0.05f);
                iconOpacity = 0.85f + ((float)Math.Abs(Math.Sin(rad)) * 0.15f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.90f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.88f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.20f; updated = true; }

                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.20f;
                    updated = true;
                }
            }
            iconXOffset = 0f;
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateRotate3D()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 6f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;
                iconRotation = animationPhase;
                float perspectiveScale = (float)Math.Abs(Math.Cos(progress * Math.PI * 2));
                breathingScale = 0.5f + perspectiveScale * 0.5f;
                iconOpacity = 0.6f + perspectiveScale * 0.4f;
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(animationPhase) > 1f)
                {
                    animationPhase *= 0.85f;
                    float progress = animationPhase / 360f;
                    float perspectiveScale = (float)Math.Abs(Math.Cos(progress * Math.PI * 2));
                    breathingScale = 0.5f + perspectiveScale * 0.5f;
                    iconOpacity = 0.6f + perspectiveScale * 0.4f;
                    iconRotation = animationPhase;
                    updated = true;
                }
                else
                {
                    animationPhase = 0f;
                    iconRotation = 0f;
                    breathingScale = 1.0f;
                    iconOpacity = 0.88f;
                }
            }
            ResetOffsets();
            return updated;
        }

        private bool AnimateSparkle()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.15f;
                if (animationPhase >= 100f) animationPhase = 0f;

                // Generate sparkles
                if (animationPhase < 2f && sparkles.Count < 8)
                {
                    Random rand = new Random();
                    sparkles.Add(new SparkleParticle
                    {
                        Position = new PointF(0, 0),
                        Size = 2f + (float)rand.NextDouble() * 2f,
                        Opacity = 1.0f,
                        Velocity = 1f + (float)rand.NextDouble() * 2f,
                        Angle = (float)(rand.NextDouble() * Math.PI * 2)
                    });
                }

                // Update sparkles
                for (int i = sparkles.Count - 1; i >= 0; i--)
                {
                    var sparkle = sparkles[i];
                    sparkle.Position = new PointF(
                        sparkle.Position.X + (float)Math.Cos(sparkle.Angle) * sparkle.Velocity,
                        sparkle.Position.Y + (float)Math.Sin(sparkle.Angle) * sparkle.Velocity
                    );
                    sparkle.Opacity -= 0.025f;
                    sparkle.Size -= 0.05f;

                    if (sparkle.Opacity <= 0 || sparkle.Size <= 0)
                        sparkles.RemoveAt(i);
                }

                breathingScale = 1.0f + ((float)Math.Sin(animationPhase * 0.1) * 0.06f);
                updated = true;
            }
            else
            {
                sparkles.Clear();
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.90f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.20f; updated = true; }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateRippleWaves()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.10f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                breathingScale = 1.0f + ((float)Math.Sin(rad * 2) * 0.08f);
                iconOpacity = 0.85f + ((float)(Math.Sin(rad) * 0.5 + 0.5) * 0.15f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.88f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }

                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.22f;
                    updated = true;
                }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateFade()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                iconOpacity -= 0.035f;
                if (iconOpacity < 0.2f)
                {
                    iconOpacity = 0.2f;
                    animationPlayed = true;
                }
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.18f;
                    updated = true;
                }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateDance()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.15f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                secondaryPhase += 0.20f;
                if (secondaryPhase >= 360f) secondaryPhase -= 360f;

                double rad1 = animationPhase * Math.PI / 180.0;
                double rad2 = secondaryPhase * Math.PI / 180.0;

                iconXOffset = (float)Math.Sin(rad1) * 4f;
                iconYOffset = (float)Math.Cos(rad2) * 4f;
                iconRotation = (float)Math.Sin(rad1) * 12f;
                breathingScale = 1.0f + ((float)Math.Abs(Math.Sin(rad1)) * 0.08f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.88f; updated = true; }
                if (Math.Abs(secondaryPhase) > 0.1f) { secondaryPhase *= 0.88f; updated = true; }
                if (Math.Abs(iconXOffset) > 0.05f) { iconXOffset *= 0.85f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.85f; updated = true; }
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.85f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.20f; updated = true; }
            }
            return updated;
        }

        private bool AnimateOrbit()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 3.5f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                float radius = 8f;
                iconXOffset = (float)Math.Cos(rad) * radius;
                iconYOffset = (float)Math.Sin(rad) * radius;
                iconRotation = animationPhase;
                breathingScale = 1.0f + ((float)Math.Sin(rad * 2) * 0.05f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.90f; updated = true; }
                if (Math.Abs(iconXOffset) > 0.05f) { iconXOffset *= 0.88f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.88f; updated = true; }
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.88f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
            }
            return updated;
        }

        private bool AnimateBreathe()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.055f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                float breath = (float)(Math.Sin(rad) * 0.5 + 0.5);
                breathingScale = 1.0f + (breath * 0.12f);
                iconOpacity = 0.82f + (breath * 0.18f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.90f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }

                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.22f;
                    updated = true;
                }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateJello()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 18f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;
                float intensity = (float)Math.Sin(progress * Math.PI);
                iconSkewX = (float)Math.Sin(animationPhase * Math.PI / 180.0 * 4f) * 10f * intensity;
                breathingScale = 1.0f + ((float)Math.Sin(animationPhase * Math.PI / 180.0 * 2f) * 0.08f * intensity);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.82f; updated = true; }
                if (Math.Abs(iconSkewX) > 0.1f) { iconSkewX *= 0.80f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.20f; updated = true; }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateRubberBand()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 15f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;
                float intensity = (float)Math.Sin(progress * Math.PI);

                float stretchX = 1.0f + ((float)Math.Sin(progress * Math.PI * 4f) * 0.15f * intensity);
                float stretchY = 1.0f + ((float)Math.Cos(progress * Math.PI * 4f) * 0.15f * intensity);
                breathingScale = (stretchX + stretchY) / 2f;
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.82f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateTada()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 12f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;
                float intensity = (float)Math.Sin(progress * Math.PI);

                iconRotation = (float)Math.Sin(progress * Math.PI * 6f) * 12f * intensity;
                breathingScale = 1.0f + ((float)Math.Sin(progress * Math.PI * 3f) * 0.15f * intensity);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.85f; updated = true; }
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.82f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
            }
            ResetOffsets();
            return updated;
        }

        private bool AnimateFlash()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.25f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                float flash = (float)(Math.Sin(rad * 4) * 0.5 + 0.5);
                iconOpacity = 0.5f + (flash * 0.5f);
                breathingScale = 1.0f + (flash * 0.08f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.88f; updated = true; }

                float targetOp = 0.88f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.22f;
                    updated = true;
                }

                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
            }
            ResetOffsets();
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateWobble()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                animationPhase += 0.12f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                double rad = animationPhase * Math.PI / 180.0;
                iconRotation = (float)Math.Sin(rad * 2) * 10f;
                iconXOffset = (float)Math.Sin(rad * 3) * 3f;
                breathingScale = 1.0f + ((float)Math.Abs(Math.Sin(rad)) * 0.06f);
                updated = true;
            }
            else
            {
                if (Math.Abs(animationPhase) > 0.1f) { animationPhase *= 0.88f; updated = true; }
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.85f; updated = true; }
                if (Math.Abs(iconXOffset) > 0.05f) { iconXOffset *= 0.85f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
            }
            iconYOffset = 0f;
            return updated;
        }

        private bool AnimateElasticBounce()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                animationPhase += 8f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;
                float elastic = (float)(Math.Sin(progress * Math.PI * 4) * Math.Exp(-progress * 3));
                iconYOffset = elastic * 15f;
                breathingScale = 1.0f + (Math.Abs(elastic) * 0.15f);
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.85f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.82f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
            }
            iconXOffset = 0f;
            iconRotation = 0f;
            return updated;
        }

        private bool AnimateWriting()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                // Writing animation - pencil writes on paper with realistic motion
                animationPhase += 4.5f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;

                // Pencil stays at natural writing angle (tilted for right-handed writing)
                float baseAngle = 0f;

                // Natural hand tremor/shake while writing
                float waveFreq = 12f; // Frequency of natural hand movement
                float wave = (float)Math.Sin(progress * Math.PI * waveFreq);
                float slowWave = (float)Math.Sin(progress * Math.PI * 3f); // Slower wave for writing lines

                // Pencil moves horizontally as it writes (small movement)
                // The pencil tip stays mostly in one area, writing on the paper
                iconXOffset = progress * 8f + wave * 0.8f; // Small rightward movement + micro shake

                // Vertical movement simulates writing on lines + natural hand variation
                iconYOffset = slowWave * 1.5f + wave * 0.4f; // Move between lines + small wobble

                // Rotation variation - natural hand angle changes while writing
                iconRotation = baseAngle + wave * 3f + slowWave * 1.5f;

                // Pressure variation - pencil presses down as you write
                float pressure = (float)Math.Sin(progress * Math.PI * 6f) * 0.5f + 0.5f; // 0 to 1
                breathingScale = 1.0f + pressure * 0.05f;

                // Store progress for ink trail drawing
                secondaryPhase = progress;

                // Pencil tip glow (simulates graphite/ink being deposited)
                iconGlow = 0.3f + pressure * 0.5f;

                iconOpacity = 1.0f;
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                // Smooth reset to original position
                if (Math.Abs(animationPhase) > 1f)
                {
                    animationPhase *= 0.88f;
                    updated = true;
                }

                if (Math.Abs(iconXOffset) > 0.05f)
                {
                    iconXOffset *= 0.85f;
                    updated = true;
                }

                if (Math.Abs(iconYOffset) > 0.05f)
                {
                    iconYOffset *= 0.85f;
                    updated = true;
                }

                // Reset rotation smoothly
                float targetRotation = 0f;
                if (Math.Abs(iconRotation - targetRotation) > 0.5f)
                {
                    iconRotation += (targetRotation - iconRotation) * 0.85f;
                    updated = true;
                }
                else
                {
                    iconRotation = 0f;
                }

                if (Math.Abs(breathingScale - 1.0f) > 0.01f)
                {
                    breathingScale += (1.0f - breathingScale) * 0.22f;
                    updated = true;
                }

                if (Math.Abs(secondaryPhase) > 0.01f)
                {
                    secondaryPhase *= 0.88f;
                    updated = true;
                }

                if (Math.Abs(iconGlow) > 0.01f)
                {
                    iconGlow *= 0.85f;
                    updated = true;
                }

                iconOpacity = 1.0f;
            }
            return updated;
        }

        private bool AnimateEarthRotate()
        {
            bool updated = false;
            if (isHovered && !isPressed)
            {
                // Continuous horizontal rotation like Earth spinning
                animationPhase += 4.0f;
                if (animationPhase >= 360f) animationPhase -= 360f;

                // Horizontal axis rotation creates 3D spinning effect
                float angle = animationPhase * (float)Math.PI / 180f;
                
                // Calculate horizontal scale to simulate 3D rotation
                // When facing forward (0/180°), full width. When sideways (90/270°), very narrow
                float horizontalScale = (float)Math.Abs(Math.Cos(angle));
                
                // Use iconSkewX to store the horizontal compression ratio
                // This will be applied as a width scale in DrawIcon
                iconSkewX = 0.3f + horizontalScale * 0.7f; // Range: 0.3 to 1.0
                
                // Vertical scale stays constant for horizontal rotation
                iconSkewY = 1.0f;
                
                // Overall scale slight pulse
                breathingScale = 0.98f + horizontalScale * 0.08f;
                
                // Subtle tilt as it rotates for realism
                iconRotation = (float)Math.Sin(angle) * 8f;
                
                // Opacity fade when edge-on for depth
                iconOpacity = 0.80f + horizontalScale * 0.20f;
                
                // Gentle floating
                iconYOffset = (float)Math.Sin(angle * 1.5f) * 2.0f;
                
                updated = true;
            }
            else
            {
                // Smooth reset
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.90f; updated = true; }
                if (Math.Abs(iconSkewX - 1.0f) > 0.01f) { iconSkewX += (1.0f - iconSkewX) * 0.22f; updated = true; }
                if (Math.Abs(iconSkewY - 1.0f) > 0.01f) { iconSkewY += (1.0f - iconSkewY) * 0.22f; updated = true; }
                if (Math.Abs(iconRotation) > 0.5f) { iconRotation *= 0.88f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
                if (Math.Abs(iconYOffset) > 0.05f) { iconYOffset *= 0.88f; updated = true; }
                
                float targetOp = 1.0f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.22f;
                    updated = true;
                }
            }
            
            iconXOffset = 0f;
            return updated;
        }

        private bool AnimateDoorExit()
        {
            bool updated = false;
            if (isHovered && !isPressed && !animationPlayed)
            {
                // Smooth door closing animation
                animationPhase += 5.5f;
                if (animationPhase >= 360f)
                {
                    animationPhase = 360f;
                    animationPlayed = true;
                }

                float progress = animationPhase / 360f;
                
                // Smooth ease-out
                float eased = progress < 0.5f 
                    ? 2f * progress * progress 
                    : 1f - (float)Math.Pow(-2f * progress + 2f, 2f) / 2f;
                
                // Icon stays in place - door slides over it
                iconXOffset = 0f;
                iconYOffset = 0f;
                
                // No rotation or skewing
                iconRotation = 0f;
                iconSkewX = 1.0f;
                iconSkewY = 1.0f;
                
                // Icon fades as door covers it
                iconOpacity = 1.0f - (eased * 0.85f); // Fade to 15%
                
                // Slight scale down as it gets covered
                breathingScale = 1.0f - (eased * 0.15f);
                
                // Store door progress
                secondaryPhase = eased;
                
                updated = true;
            }
            else if (!isHovered || isPressed)
            {
                // Smooth reset
                if (Math.Abs(animationPhase) > 1f) { animationPhase *= 0.88f; updated = true; }
                if (Math.Abs(breathingScale - 1.0f) > 0.01f) { breathingScale += (1.0f - breathingScale) * 0.22f; updated = true; }
                if (Math.Abs(secondaryPhase) > 0.01f) { secondaryPhase *= 0.88f; updated = true; }
                
                float targetOp = 1.0f;
                if (Math.Abs(iconOpacity - targetOp) > 0.01f)
                {
                    iconOpacity += (targetOp - iconOpacity) * 0.22f;
                    updated = true;
                }
            }
            
            return updated;
        }

        private void ResetAnimation()
        {
            iconRotation = 0f;
            iconXOffset = 0f;
            iconYOffset = 0f;
            iconSkewX = 0f;
            iconSkewY = 0f;
            animationPhase = 0f;
            secondaryPhase = 0f;
            tertiaryPhase = 0f;
        }

        private void ResetOffsets()
        {
            iconXOffset = 0f;
            iconYOffset = 0f;
        }

        #endregion

        #region Common Update Methods

        private bool UpdateTextColorTransition()
        {
            float target = (isHovered || isPressed) ? 1.0f : 0f;
            if (Math.Abs(textColorTransition - target) > 0.001f)
            {
                textColorTransition += (target - textColorTransition) * 0.32f;
                return true;
            }
            return false;
        }

        private bool UpdateButtonScale()
        {
            float target = isPressed ? 0.98f : (isHovered ? 1.01f : 1.0f);
            if (Math.Abs(buttonScale - target) > 0.0001f)
            {
                buttonScale += (target - buttonScale) * 0.30f;
                return true;
            }
            return false;
        }

        private bool UpdateShadow()
        {
            float targetOpacity = isHovered ? 0.26f : 0.10f;
            int targetOffset = isHovered ? 5 : 2;
            if (isPressed)
            {
                targetOpacity = 0.06f;
                targetOffset = 1;
            }

            bool updated = false;
            if (Math.Abs(shadowOpacity - targetOpacity) > 0.001f)
            {
                shadowOpacity += (targetOpacity - shadowOpacity) * 0.26f;
                updated = true;
            }

            if (Math.Abs(shadowOffset - targetOffset) > 0)
            {
                int diff = targetOffset - shadowOffset;
                shadowOffset += (int)Math.Ceiling(diff * 0.32f);
                if (Math.Abs(shadowOffset - targetOffset) <= 1)
                    shadowOffset = targetOffset;
                updated = true;
            }

            return updated;
        }

        private bool UpdateColorTransition()
        {
            float target = (isHovered || isPressed) ? 1.0f : 0f;
            if (Math.Abs(colorTransition - target) > 0.001f)
            {
                colorTransition += (target - colorTransition) * 0.30f;
                return true;
            }
            return false;
        }

        private bool UpdateRipples()
        {
            bool updated = false;
            for (int i = ripples.Count - 1; i >= 0; i--)
            {
                var ripple = ripples[i];
                ripple.Radius += 4.5f;
                ripple.Opacity -= 0.032f;

                if (ripple.Opacity <= 0)
                    ripples.RemoveAt(i);
                updated = true;
            }
            return updated;
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Draw parent background for transparency
            if (Parent != null)
            {
                using (var bmp = new Bitmap(Width, Height))
                using (var parentGraphics = Graphics.FromImage(bmp))
                {
                    parentGraphics.TranslateTransform(-Left, -Top);
                    InvokePaintBackground(Parent, new PaintEventArgs(parentGraphics, ClientRectangle));
                    InvokePaint(Parent, new PaintEventArgs(parentGraphics, ClientRectangle));
                    parentGraphics.ResetTransform();
                    g.DrawImage(bmp, 0, 0);
                }
            }
            else
            {
                g.Clear(Parent?.BackColor ?? Color.Transparent);
            }

            int scaledWidth = Math.Max(1, (int)(Width * buttonScale));
            int scaledHeight = Math.Max(1, (int)(Height * buttonScale));
            int offsetX = (Width - scaledWidth) / 2;
            int offsetY = (Height - scaledHeight) / 2;

            Rectangle rect = new Rectangle(offsetX, offsetY, scaledWidth, scaledHeight);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            int effectiveBorderRadius = Math.Min(borderRadius, Math.Min(rect.Width / 2, rect.Height / 2));
            using GraphicsPath path = GetRoundedRectangle(rect, effectiveBorderRadius);

            // Professional shadow
            DrawShadow(g, rect, effectiveBorderRadius, offsetX, offsetY, scaledWidth, scaledHeight);

            // Button background with gradient
            DrawButtonBackground(g, rect, path);

            // Minimal glass overlay
            DrawGlassOverlay(g, rect, path);

            // Ripple effects
            DrawRipples(g, path);

            // Clean inner border
            DrawInnerBorder(g, rect, effectiveBorderRadius);

            // Content (icon + text)
            DrawContent(g, rect);
        }

        private void DrawShadow(Graphics g, Rectangle rect, int radius, int offsetX, int offsetY, int w, int h)
        {
            if (shadowOpacity <= 0) return;

            for (int i = 2; i > 0; i--)
            {
                float layerProgress = (float)i / 2;
                int layerOffset = (int)(shadowOffset * layerProgress);
                int layerAlpha = (int)(shadowOpacity * 32 / layerProgress);
                int blur = (int)(1.8f * layerProgress);

                Rectangle shadowRect = new Rectangle(
                    offsetX - blur / 2,
                    offsetY + layerOffset - blur / 2,
                    w + blur,
                    h + blur
                );

                if (shadowRect.Width > 0 && shadowRect.Height > 0)
                {
                    int shadowRadius = Math.Min(radius + blur / 2, shadowRect.Width / 2);
                    using (GraphicsPath shadowPath = GetRoundedRectangle(shadowRect, shadowRadius))
                    using (Pen shadowPen = new Pen(Color.FromArgb(layerAlpha, 0, 0, 0), blur * 1.1f))
                    {
                        shadowPen.LineJoin = LineJoin.Round;
                        g.DrawPath(shadowPen, shadowPath);
                    }
                }
            }
        }

        private void DrawButtonBackground(Graphics g, Rectangle rect, GraphicsPath path)
        {
            // Flat design - solid color only, no gradients (Tailwind/Bootstrap style)
            Color baseColor = primaryColor;
            Color activeColor = isPressed ? pressedColor : hoverColor;

            int r = (int)(baseColor.R + (activeColor.R - baseColor.R) * colorTransition);
            int gC = (int)(baseColor.G + (activeColor.G - baseColor.G) * colorTransition);
            int b = (int)(baseColor.B + (activeColor.B - baseColor.B) * colorTransition);

            Color bgColor = Color.FromArgb(r, gC, b);

            // Solid fill for completely flat design
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawGlassOverlay(Graphics g, Rectangle rect, GraphicsPath path)
        {
            // Flat design - no glass overlay
            return;
        }

        private void DrawRipples(Graphics g, GraphicsPath path)
        {
            foreach (var ripple in ripples)
            {
                if (ripple.Radius > 0 && ripple.Radius < 1000)
                {
                    int alpha = (int)(ripple.Opacity * 48);
                    float diameter = ripple.Radius * 2;

                    if (diameter > 0)
                    {
                        using (GraphicsPath ripplePath = new GraphicsPath())
                        {
                            ripplePath.AddEllipse(
                                ripple.Origin.X - ripple.Radius,
                                ripple.Origin.Y - ripple.Radius,
                                diameter, diameter);

                            if (ripplePath.PointCount > 0)
                            {
                                using (PathGradientBrush rippleBrush = new PathGradientBrush(ripplePath))
                                {
                                    rippleBrush.CenterColor = Color.FromArgb(alpha, 255, 255, 255);
                                    rippleBrush.SurroundColors = new[] { Color.FromArgb(0, 255, 255, 255) };
                                    rippleBrush.FocusScales = new PointF(0.3f, 0.3f);

                                    using (Region oldClip = g.Clip.Clone())
                                    {
                                        g.SetClip(path);
                                        g.FillPath(rippleBrush, ripplePath);
                                        g.Clip = oldClip;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawInnerBorder(Graphics g, Rectangle rect, int radius)
        {
            // Flat design - no inner border for clean Tailwind/Bootstrap look
            return;
        }

        private void DrawContent(Graphics g, Rectangle rect)
        {
            int contentPadding = 8;
            Rectangle contentRect = new Rectangle(
                rect.X + contentPadding, rect.Y,
                rect.Width - contentPadding * 2, rect.Height);

            SizeF textSize = g.MeasureString(Text, Font);
            int iconSpace = (buttonIcon != null) ? (int)(iconSize * iconScale * breathingScale) + 8 : 0;
            int totalWidth = iconSpace + (int)textSize.Width;
            int startX = contentRect.X + (contentRect.Width - totalWidth) / 2;
            int centerY = contentRect.Y + contentRect.Height / 2;

            // Draw icon
            if (buttonIcon != null)
            {
                DrawIcon(g, startX, centerY);
            }

            // Draw sparkles for Sparkle animation
            if (animationType == IconAnimationType.Sparkle && sparkles.Count > 0)
            {
                DrawSparkles(g, startX + iconSpace / 2, centerY);
            }

            // Draw text
            DrawText(g, startX + iconSpace, centerY, textSize);
        }

        private void DrawIcon(Graphics g, float startX, float centerY)
        {
            try
            {
                if (buttonIcon == null || buttonIcon.Width <= 0 || buttonIcon.Height <= 0) return;

                float combinedScale = iconScale * breathingScale;
                float scaledSize = iconSize * combinedScale;
                float iconX = startX + iconXOffset;
                float iconY = centerY - scaledSize / 2 + iconYOffset;

                GraphicsState state = g.Save();
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                float iconCenterX = iconX + scaledSize / 2;
                float iconCenterY = iconY + scaledSize / 2;

                g.TranslateTransform(iconCenterX, iconCenterY);
                
                // Apply 3D rotation effect if needed (for EarthRotate and DoorExit animations)
                if ((animationType == IconAnimationType.EarthRotate || animationType == IconAnimationType.DoorExit) 
                    && (iconSkewX != 1.0f || iconSkewY != 1.0f))
                {
                    g.ScaleTransform(iconSkewX, iconSkewY);
                }
                
                g.RotateTransform(iconRotation);
                g.TranslateTransform(-iconCenterX, -iconCenterY);

                // Apply horizontal/vertical scaling for 3D effect
                float finalWidth = scaledSize;
                float finalHeight = scaledSize;
                
                if (animationType == IconAnimationType.EarthRotate || animationType == IconAnimationType.DoorExit)
                {
                    finalWidth *= iconSkewX;
                    finalHeight *= iconSkewY;
                }
                
                RectangleF iconRect = new RectangleF(
                    iconX + (scaledSize - finalWidth) / 2,
                    iconY + (scaledSize - finalHeight) / 2,
                    finalWidth,
                    finalHeight);

                if (iconOpacity < 0.99f)
                {
                    using (ImageAttributes imageAttr = new ImageAttributes())
                    {
                        ColorMatrix colorMatrix = new ColorMatrix();
                        colorMatrix.Matrix33 = iconOpacity;
                        imageAttr.SetColorMatrix(colorMatrix);

                        Rectangle r = Rectangle.Round(iconRect);
                        g.DrawImage(buttonIcon, r,
                            0, 0, buttonIcon.Width, buttonIcon.Height,
                            GraphicsUnit.Pixel, imageAttr);
                    }
                }
                else
                {
                    g.DrawImage(buttonIcon, iconRect);
                }

                g.Restore(state);

                // Special animation overlays
                DrawAnimationOverlays(g, iconCenterX, iconCenterY, scaledSize);
            }
            catch { }
        }

        private void DrawAnimationOverlays(Graphics g, float centerX, float centerY, float size)
        {
            if (animationType == IconAnimationType.Atomic && isHovered && !isPressed)
            {
                DrawAtomicOrbits(g, centerX, centerY, size);
            }
            
            if (animationType == IconAnimationType.Envelope && animationPhase > 0f)
            {
                DrawEnvelopeOpening(g, centerX, centerY, size);
            }
            
            if (animationType == IconAnimationType.Writing && animationPhase > 0f)
            {
                DrawWritingTrail(g, centerX, centerY, size);
            }
            
            if (animationType == IconAnimationType.DoorExit && secondaryPhase > 0f)
            {
                DrawDoorClosing(g, centerX, centerY, size);
            }
        }

        private void DrawAtomicOrbits(Graphics g, float centerX, float centerY, float size)
        {
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw 3 orbiting electrons with different speeds and radii
            int numOrbits = 3;
            float[] orbitRadii = new float[] { size * 0.6f, size * 0.8f, size * 1.0f };
            float[] orbitAngles = new float[] { animationPhase, secondaryPhase, tertiaryPhase };
            float[] orbitTilts = new float[] { 0f, 30f, 60f }; // Different orbital plane angles

            for (int orbit = 0; orbit < numOrbits; orbit++)
            {
                float orbitRadius = orbitRadii[orbit];
                float currentAngle = orbitAngles[orbit];
                float tilt = orbitTilts[orbit];

                // Draw orbital path (elliptical for 3D effect)
                using (Pen orbitPen = new Pen(Color.FromArgb(35, hoverColor), 1.2f))
                {
                    orbitPen.DashStyle = DashStyle.Dash;
                    
                    // Tilted ellipse for 3D orbital planes
                    GraphicsState orbitState = g.Save();
                    g.TranslateTransform(centerX, centerY);
                    g.RotateTransform(tilt);
                    g.TranslateTransform(-centerX, -centerY);
                    
                    RectangleF orbitRect = new RectangleF(
                        centerX - orbitRadius, 
                        centerY - orbitRadius * 0.5f, // Compressed for 3D effect
                        orbitRadius * 2, 
                        orbitRadius);
                    g.DrawEllipse(orbitPen, orbitRect);
                    
                    g.Restore(orbitState);
                }

                // Calculate electron position with 3D effect
                float radAngle = currentAngle * (float)Math.PI / 180.0f;
                float electronX = centerX + (float)(Math.Cos(radAngle) * orbitRadius);
                float electronY = centerY + (float)(Math.Sin(radAngle) * orbitRadius * 0.5f * Math.Cos(tilt * Math.PI / 180.0));

                // Electron size varies with depth (closer = larger)
                float depth = (float)Math.Sin(radAngle) * 0.3f + 0.7f; // 0.4 to 1.0
                float electronSize = 4.5f * depth;

                // Draw electron glow (larger, transparent layers)
                for (int i = 3; i >= 1; i--)
                {
                    float glowSize = electronSize + i * 3.5f;
                    int glowAlpha = (int)(45 / i * depth);
                    using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(glowAlpha, hoverColor)))
                    {
                        g.FillEllipse(glowBrush,
                            electronX - glowSize / 2,
                            electronY - glowSize / 2,
                            glowSize, glowSize);
                    }
                }

                // Draw solid electron core
                using (SolidBrush electronBrush = new SolidBrush(hoverColor))
                {
                    g.FillEllipse(electronBrush,
                        electronX - electronSize / 2,
                        electronY - electronSize / 2,
                        electronSize, electronSize);
                }

                // Bright highlight on electron
                using (SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(180, Color.White)))
                {
                    float highlightSize = electronSize * 0.5f;
                    g.FillEllipse(highlightBrush,
                        electronX - highlightSize / 2 + electronSize * 0.15f,
                        electronY - highlightSize / 2 - electronSize * 0.15f,
                        highlightSize, highlightSize);
                }
            }

            // Draw nucleus glow in center
            if (iconGlow > 0.1f)
            {
                for (int i = 3; i >= 1; i--)
                {
                    float nucleusGlowSize = size * 0.25f + i * 4f;
                    int nucleusAlpha = (int)(iconGlow * 60 / i);
                    using (SolidBrush nucleusBrush = new SolidBrush(Color.FromArgb(nucleusAlpha, hoverColor)))
                    {
                        g.FillEllipse(nucleusBrush,
                            centerX - nucleusGlowSize / 2,
                            centerY - nucleusGlowSize / 2,
                            nucleusGlowSize, nucleusGlowSize);
                    }
                }
            }

            g.Restore(state);
        }

        private void DrawEnvelopeOpening(Graphics g, float centerX, float centerY, float size)
        {
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float progress = animationPhase; // 0 to 1
            
            // Envelope dimensions (smaller)
            float envWidth = size * 0.85f;
            float envHeight = size * 0.62f;
            float flapHeight = envHeight * 0.45f;
            
            RectangleF envBody = new RectangleF(
                centerX - envWidth / 2,
                centerY - envHeight / 2,
                envWidth,
                envHeight
            );

            // Draw envelope body (back layer) - keep orange color
            Color envelopeColor = Color.FromArgb(255, 140, 0); // Orange color
            using (SolidBrush bodyBrush = new SolidBrush(envelopeColor))
            {
                g.FillRectangle(bodyBrush, envBody);
            }

            // Draw letter paper emerging from envelope
            if (progress > 0.1f)
            {
                float letterProgress = Math.Min(1f, (progress - 0.1f) / 0.7f); // Starts emerging at 10% open
                float letterHeight = envHeight * 0.7f;
                float letterWidth = envWidth * 0.85f;
                float letterEmergence = letterProgress * letterHeight * 0.6f; // How much sticks out
                
                RectangleF letter = new RectangleF(
                    centerX - letterWidth / 2,
                    centerY - envHeight / 2 - letterEmergence,
                    letterWidth,
                    letterHeight
                );

                // Letter shadow
                using (SolidBrush letterShadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    RectangleF shadowRect = letter;
                    shadowRect.Offset(2, 2);
                    g.FillRectangle(letterShadowBrush, shadowRect);
                }

                // Letter paper (white/light)
                using (SolidBrush letterBrush = new SolidBrush(Color.FromArgb(245, 245, 245)))
                {
                    g.FillRectangle(letterBrush, letter);
                }

                // Letter border
                using (Pen letterPen = new Pen(Color.FromArgb(200, 200, 200), 1f))
                {
                    g.DrawRectangle(letterPen, letter.X, letter.Y, letter.Width, letter.Height);
                }

                // Draw lines on letter to simulate text
                if (letterProgress > 0.3f)
                {
                    using (Pen linePen = new Pen(Color.FromArgb(150, 150, 150), 1f))
                    {
                        float lineY = letter.Y + letterHeight * 0.25f;
                        float lineSpacing = 6f;
                        for (int i = 0; i < 4; i++)
                        {
                            if (lineY < letter.Bottom - 10)
                            {
                                g.DrawLine(linePen,
                                    letter.X + 10,
                                    lineY,
                                    letter.Right - 10,
                                    lineY);
                                lineY += lineSpacing;
                            }
                        }
                    }
                }
            }

            // Draw envelope flap (top triangular piece that opens)
            GraphicsPath flapPath = new GraphicsPath();
            
            // Flap opens from the back center
            float flapAngle = progress * 140f; // Opens up to 140 degrees
            float flapTipX = centerX;
            float flapTipY = centerY - envHeight / 2;
            
            // Calculate flap corners when opening
            double angleRad = flapAngle * Math.PI / 180.0;
            float flapOpenHeight = flapHeight * (float)Math.Cos(angleRad);
            float flapLift = flapHeight * (float)Math.Sin(angleRad);
            
            PointF[] flapPoints = new PointF[]
            {
                new PointF(centerX - envWidth / 2, flapTipY), // Left corner
                new PointF(centerX + envWidth / 2, flapTipY), // Right corner
                new PointF(flapTipX, flapTipY - flapOpenHeight - flapLift) // Tip (opens upward)
            };

            flapPath.AddPolygon(flapPoints);

            // Flap shadow (darker as it opens)
            using (SolidBrush flapShadowBrush = new SolidBrush(Color.FromArgb((int)(50 * progress), 0, 0, 0)))
            {
                GraphicsPath shadowPath = (GraphicsPath)flapPath.Clone();
                Matrix shadowMatrix = new Matrix();
                shadowMatrix.Translate(2, 2);
                shadowPath.Transform(shadowMatrix);
                g.FillPath(flapShadowBrush, shadowPath);
            }

            // Flap color (slightly darker than body to show it's the flap)
            int flapDarken = 30;
            using (SolidBrush flapBrush = new SolidBrush(Color.FromArgb(
                Math.Max(0, envelopeColor.R - flapDarken),
                Math.Max(0, envelopeColor.G - flapDarken),
                Math.Max(0, envelopeColor.B - flapDarken))))
            {
                g.FillPath(flapBrush, flapPath);
            }

            // Flap outline
            using (Pen flapPen = new Pen(Color.FromArgb(Math.Max(0, envelopeColor.R - 50),
                Math.Max(0, envelopeColor.G - 50),
                Math.Max(0, envelopeColor.B - 50)), 1.5f))
            {
                g.DrawPath(flapPen, flapPath);
            }

            // Envelope body outline (front)
            using (Pen envPen = new Pen(Color.FromArgb(Math.Max(0, envelopeColor.R - 40),
                Math.Max(0, envelopeColor.G - 40),
                Math.Max(0, envelopeColor.B - 40)), 2f))
            {
                g.DrawRectangle(envPen, envBody.X, envBody.Y, envBody.Width, envBody.Height);
            }

            g.Restore(state);
        }


        private void DrawWritingTrail(Graphics g, float centerX, float centerY, float size)
        {
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float progress = secondaryPhase; // Progress from animation (0 to 1)

            // Only draw if there's progress
            if (progress > 0.02f)
            {
                // Number of points for smooth writing trail
                int numPoints = (int)(progress * 60); // More points = smoother line

                if (numPoints >= 2)
                {
                    PointF[] inkPoints = new PointF[numPoints];

                    // Calculate each point along the writing path
                    for (int i = 0; i < numPoints; i++)
                    {
                        float t = i / 59f; // Normalized progress (0 to 1)

                        // Match the pencil's exact motion from AnimateWriting
                        float waveFreq = 12f;
                        float wave = (float)Math.Sin(t * Math.PI * waveFreq);
                        float slowWave = (float)Math.Sin(t * Math.PI * 3f);

                        // Calculate position on paper where ink is deposited
                        // Start slightly to the left of center
                        float startX = centerX - (size * 0.25f);
                        float startY = centerY;

                        // Horizontal writing motion
                        float inkX = startX + (t * 8f * size / 28f) + wave * 0.8f * size / 28f;

                        // Vertical motion (writing on lines)
                        float inkY = startY + (slowWave * 1.5f * size / 28f) + wave * 0.4f * size / 28f;

                        inkPoints[i] = new PointF(inkX, inkY);
                    }

                    // Draw the ink trail in segments with varying opacity
                    for (int i = 1; i < numPoints; i++)
                    {
                        float segmentProgress = i / (float)numPoints;

                        // Ink appears gradually and gets darker as it dries
                        int alpha = (int)(240 * Math.Pow(segmentProgress, 0.5)); // Fade in effect

                        // Vary line thickness slightly for natural look
                        float thickness = 2.5f + (float)Math.Sin(segmentProgress * Math.PI * 15f) * 0.3f;

                        using (Pen inkPen = new Pen(Color.FromArgb(alpha, hoverColor), thickness))
                        {
                            inkPen.StartCap = LineCap.Round;
                            inkPen.EndCap = LineCap.Round;
                            inkPen.LineJoin = LineJoin.Round;

                            g.DrawLine(inkPen, inkPoints[i - 1], inkPoints[i]);
                        }
                    }

                    // Add ink dots at writing points for texture (like ballpoint pen)
                    if (numPoints > 15)
                    {
                        for (int i = 8; i < numPoints; i += 4) // Every 4th point
                        {
                            float dotProgress = i / (float)numPoints;
                            int dotAlpha = (int)(180 * Math.Sqrt(dotProgress));
                            float dotSize = 1.8f;

                            using (SolidBrush dotBrush = new SolidBrush(Color.FromArgb(dotAlpha, hoverColor)))
                            {
                                g.FillEllipse(dotBrush,
                                    inkPoints[i].X - dotSize / 2,
                                    inkPoints[i].Y - dotSize / 2,
                                    dotSize, dotSize);
                            }
                        }
                    }

                    // Add slight shadow/depth to ink trail
                    if (progress > 0.3f)
                    {
                        for (int i = 1; i < numPoints; i++)
                        {
                            float segmentProgress = i / (float)numPoints;
                            int shadowAlpha = (int)(40 * segmentProgress);

                            using (Pen shadowPen = new Pen(Color.FromArgb(shadowAlpha, 0, 0, 0), 3f))
                            {
                                shadowPen.StartCap = LineCap.Round;
                                shadowPen.EndCap = LineCap.Round;

                                // Offset shadow slightly
                                PointF p1 = new PointF(inkPoints[i - 1].X + 0.5f, inkPoints[i - 1].Y + 0.5f);
                                PointF p2 = new PointF(inkPoints[i].X + 0.5f, inkPoints[i].Y + 0.5f);

                                g.DrawLine(shadowPen, p1, p2);
                            }
                        }
                    }

                    // Add slight "wet ink" glow at the current writing position
                    if (progress < 0.95f) // Not at the end yet
                    {
                        int currentPoint = numPoints - 1;
                        if (currentPoint >= 0 && currentPoint < inkPoints.Length)
                        {
                            // Glow effect at pencil tip position
                            for (int i = 3; i >= 1; i--)
                            {
                                float glowSize = 3f + i * 2f;
                                int glowAlpha = (int)(60 / i);

                                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(glowAlpha, hoverColor)))
                                {
                                    g.FillEllipse(glowBrush,
                                        inkPoints[currentPoint].X - glowSize / 2,
                                        inkPoints[currentPoint].Y - glowSize / 2,
                                        glowSize, glowSize);
                                }
                            }
                        }
                    }
                }
            }

            g.Restore(state);
        }

        private void DrawDoorClosing(Graphics g, float centerX, float centerY, float size)
        {
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float progress = secondaryPhase; // 0 to 1
            
            // Clean, minimal door design - no frame, just sliding panel
            float doorWidth = size * 2.5f;
            float doorHeight = size * 2.0f;
            
            // Door slides in from right to cover icon
            if (progress > 0.02f)
            {
                // Door panel width grows as it closes
                float closedWidth = doorWidth * progress;
                
                RectangleF door = new RectangleF(
                    centerX + (doorWidth / 2) - closedWidth, // Slides from right
                    centerY - doorHeight / 2,
                    closedWidth,
                    doorHeight
                );
                
                // Elegant gradient door (light to dark for depth)
                using (LinearGradientBrush doorBrush = new LinearGradientBrush(
                    door,
                    Color.FromArgb(210, 220, 220, 220), // Light gray
                    Color.FromArgb(210, 180, 180, 180), // Slightly darker
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(doorBrush, door);
                }
                
                // Subtle shadow on left edge for depth
                if (closedWidth > 5f)
                {
                    using (LinearGradientBrush shadowBrush = new LinearGradientBrush(
                        new PointF(door.X, door.Y),
                        new PointF(door.X + 12f, door.Y),
                        Color.FromArgb(60, 0, 0, 0),
                        Color.Transparent))
                    {
                        RectangleF shadowRect = new RectangleF(
                            door.X, door.Y, Math.Min(12f, closedWidth), door.Height);
                        g.FillRectangle(shadowBrush, shadowRect);
                    }
                }
                
                // Minimal vertical accent line (clean modern look)
                if (progress > 0.3f && closedWidth > 15f)
                {
                    using (Pen accentPen = new Pen(Color.FromArgb(80, 140, 140, 140), 1.5f))
                    {
                        float lineX = door.X + 10f;
                        g.DrawLine(accentPen,
                            lineX, door.Y + 15f,
                            lineX, door.Bottom - 15f);
                    }
                }
                
                // Soft edge highlight (polish)
                using (Pen highlightPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f))
                {
                    g.DrawLine(highlightPen,
                        door.Right - 1, door.Y + 2,
                        door.Right - 1, door.Bottom - 2);
                }
            }

            g.Restore(state);
        }


        private void DrawSparkles(Graphics g, float centerX, float centerY)
        {
            foreach (var sparkle in sparkles)
            {
                float x = centerX + sparkle.Position.X;
                float y = centerY + sparkle.Position.Y;
                int alpha = (int)(sparkle.Opacity * 255);

                using (SolidBrush sparkleBrush = new SolidBrush(Color.FromArgb(alpha, hoverColor)))
                {
                    g.FillEllipse(sparkleBrush,
                        x - sparkle.Size / 2,
                        y - sparkle.Size / 2,
                        sparkle.Size, sparkle.Size);
                }
            }
        }

        private void DrawText(Graphics g, float textX, float centerY, SizeF textSize)
        {
            float textY = centerY - textSize.Height / 2;

            // Use ForeColor as base, HoverForeColor if set (otherwise use white as default hover)
            Color baseTextColor = ForeColor;
            Color hoverTextColor = hoverForeColor.IsEmpty ? Color.White : hoverForeColor;

            int r = (int)(baseTextColor.R + (hoverTextColor.R - baseTextColor.R) * textColorTransition);
            int gC = (int)(baseTextColor.G + (hoverTextColor.G - baseTextColor.G) * textColorTransition);
            int b = (int)(baseTextColor.B + (hoverTextColor.B - baseTextColor.B) * textColorTransition);

            Color currentTextColor = Color.FromArgb(r, gC, b);

            // Text shadow for depth
            int shadowAlpha = 20 + (int)(textColorTransition * 10);
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(shadowAlpha, 0, 0, 0)))
            {
                g.DrawString(Text, Font, shadowBrush, textX + 0.5f, textY + 0.6f);
            }

            // Main text
            using (SolidBrush textBrush = new SolidBrush(currentTextColor))
            {
                g.DrawString(Text, Font, textBrush, textX, textY);
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new Rectangle(0, 0, 1, 1));
                return path;
            }

            int diameter = Math.Max(0, radius * 2);
            if (diameter > rect.Height) diameter = rect.Height;
            if (diameter > rect.Width) diameter = rect.Width;

            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                animationTimer?.Dispose();
                ripples?.Clear();
                sparkles?.Clear();
            }
            base.Dispose(disposing);
        }
    }
}