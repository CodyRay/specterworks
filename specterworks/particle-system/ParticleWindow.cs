using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace specterworks
{
    internal class ParticleWindow : GameWindow
    {
        private int _axesList;
        private int _pList;

        public ParticleWindow(Particle startParticle) : base(Consts.DefaultWindowSize, Consts.DefaultWindowSize)
        {
            Particles.AddFirst(startParticle);
        }

        public bool AxesOn { get; set; } = false;
        public int HorizontalCameraRotation { get; set; } = 0;
        public LinkedList<Particle> Particles { get; } = new LinkedList<Particle>();
        public bool Pause { get; set; } = false;
        public RandomSource Rand { get; } = new RandomSource();
        public int VerticalCameraRotation { get; set; } = 0;

        public IEnumerable<Particle> CoolEmitter(Particle parent)
        {
            if (Rand.NextInt(0, 50) == 0)
            {
                parent.IsDead = true;
                parent.EmitParticle = null;
                if (parent.CanHasTwoBebes)
                {
                    //Two Child
                    var childs = new[] { MakeParticle(parent, true), MakeParticle(parent, true) };
                    var colorModes = Enum.GetValues(typeof(PointColorMode)).Cast<PointColorMode>().ToArray();
                    childs[0].ColorMode = colorModes[Rand.NextInt(0, colorModes.Length)];
                    childs[1].ColorMode = colorModes[Rand.NextInt(0, colorModes.Length)];
                    childs[0].Color = Color.White;
                    childs[1].Color = Color.White;
                    childs[0].CanHasTwoBebes = false;
                    childs[1].CanHasTwoBebes = false;
                    return childs;
                }
                else
                {
                    //One Child
                    return new[] { MakeParticle(parent, true) };
                }
            }
            else
            {
                return Enumerable.Range(0, Rand.NextInt(0, 5)).Select(i =>
                {
                    Particle part = MakeParticle(parent);
                    return part;
                });
            }
        }

        private Particle MakeParticle(Particle p, bool make_emitter = false)
        {
            var part = new Particle();
            var velocity = 5;

            part.Location.X = p.Location.X + Rand.Next(-5, 5);
            part.Location.Y = p.Location.Y + Rand.Next(-5, 5);
            part.Location.Z = p.Location.Z + Rand.Next(-5, 5);

            part.Velocity.X = p.Velocity.X + Rand.Next(-velocity, velocity);
            part.Velocity.Y = p.Velocity.Y + Rand.Next(-velocity, velocity);
            part.Velocity.Z = p.Velocity.Z + Rand.Next(-velocity, velocity);

            part.Color = p.Color;

            if (make_emitter)
            {
                var acc_scale = 0.01f;
                part.Acceleration.X = (p.Location.X * p.Location.X) * acc_scale;
                part.Acceleration.Y = (p.Location.Y * p.Location.Y) * acc_scale;
                part.Acceleration.Z = (p.Location.Z * p.Location.Z) * acc_scale;

                int c;
                int color_dec = 5;
                switch (Rand.NextInt(0, 3))
                {
                    case 0: //Red
                        c = p.Color.R + -color_dec;
                        if (c < 0 || c > 255 || p.ColorMode.HasFlag(PointColorMode.Red))
                            break;
                        part.Color = Color.FromArgb(p.Color.A, c, p.Color.G, p.Color.B);
                        break;

                    case 1: //Green
                        c = p.Color.G + -color_dec;
                        if (c < 0 || c > 255 || p.ColorMode.HasFlag(PointColorMode.Green))
                            break;
                        part.Color = Color.FromArgb(p.Color.A, p.Color.R, c, p.Color.B);
                        break;

                    case 2: //Blue
                        c = p.Color.B + -color_dec;
                        if (c < 0 || c > 255 || p.ColorMode.HasFlag(PointColorMode.Blue))
                            break;
                        part.Color = Color.FromArgb(p.Color.A, p.Color.R, p.Color.G, c);
                        break;
                }
                part.EmitParticle = CoolEmitter;
                part.ColorMode = p.ColorMode;

                switch (part.ColorMode)
                {
                    case PointColorMode.Red:
                    case PointColorMode.Green:
                    case PointColorMode.Blue:
                        if (part.Color.B + part.Color.G + part.Color.R <= 265)
                            part.CanHasTwoBebes = true;
                        break;

                    case PointColorMode.RedGreen:
                    case PointColorMode.RedBlue:
                    case PointColorMode.GreenBlue:
                        if (part.Color.B + part.Color.G + part.Color.R <= 520)
                            part.CanHasTwoBebes = true;
                        break;

                    case PointColorMode.RedGreenBlue:
                        part.IsDead = true;
                        part.EmitParticle = null;
                        break;
                }
            }
            else
            {
                part.TimeLifeSpan = 20;
            }

            return part;
        }

        private void UpdateParticles(float time)
        {
            GL.NewList(_pList, ListMode.Compile);
            GL.PointSize(1);
            GL.Begin(BeginMode.Points);

            LinkedListNode<Particle> next;
            for (var current = Particles.First; current != null; current = next)
            {
                next = current.Next; //This is needed because if we remove a particle its pointer will no longer work
                var part = current.Value;
                part.Forward(time, p => Particles.AddFirst(p));
                if (part.IsAlive)
                {
                    GL.Color3(part.Color);
                    GL.Vertex3(part.Location.X, part.Location.Y, part.Location.Z);
                }
                else
                {
                    Particles.Remove(current);
                }
            }

            GL.End();
            GL.EndList();
        }
        #region Overrides

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e?.Key == Key.Up)
                VerticalCameraRotation++;
            if (e?.Key == Key.Down)
                VerticalCameraRotation--;
            if (e?.Key == Key.Left)
                HorizontalCameraRotation++;
            if (e?.Key == Key.Right)
                HorizontalCameraRotation--;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e?.Key == Key.A)
                AxesOn = !AxesOn;
            if (e?.Key == Key.Space)
                Pause = !Pause;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Title = "Particle System";
            GL.ClearColor(Color.Black);

            _axesList = GL.GenLists(1);
            _pList = GL.GenLists(1);
            GL.NewList(_axesList, ListMode.Compile);
            GL.Color3(Consts.AxesColor);
            GL.LineWidth(Consts.AxesWidth);
            Axes(Consts.AxesLength);
            GL.LineWidth(1);
            GL.EndList();
        }

        protected override void OnRenderFrame(FrameEventArgs e) //Same As Display Function in C++
        {
            base.OnRenderFrame(e);

            //Erase Background
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //No Shading
            GL.ShadeModel(ShadingModel.Flat);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 modelview = Matrix4.LookAt(300.0f, 400.0f, 100.0f, 0, 0, 0, 1, 0, 0);
            GL.LoadMatrix(ref modelview);
            if (AxesOn)
                GL.CallList(_axesList);
            if (!Pause)
                UpdateParticles(0.02f);
            GL.CallList(_pList);
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-300f, 300f, -300f, 300f, 0.1, 1000f);
        }

        #endregion Overrides

        private void Axes(float length)
        {
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(length, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, length, 0);
            GL.End();
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, length);
            GL.End();

            // All of this is for labeling the axes with X, Y, Z

            float[] xx = { 0, 1, 0, 1 };
            float[] xy = { -.5f, .5f, .5f, -.5f };
            int[] xorder = { 1, 2, -3, 4 };

            float[] yx = { 0, 0, -.5f, .5f };
            float[] yy = { 0, .6f, 1, 1 };
            int[] yorder = { 1, 2, 3, -2, 4 };

            float[] zx = { 1, 0, 1, 0, .25f, .75f };
            float[] zy = { .5f, .5f, -.5f, -.5f, 0, 0 };
            int[] zorder = { 1, 2, 3, 4, -5, 6 };

            var factor = Consts.AxesLengthFraction * length;
            var start = Consts.AxesStartFraction * length;

            GL.Begin(BeginMode.LineStrip);
            for (int i = 0; i < 4; i++)
            {
                int j = xorder[i];
                if (j < 0)
                {
                    GL.End();
                    GL.Begin(BeginMode.LineStrip);
                    j = -j;
                }
                j--;
                GL.Vertex3(start + factor * xx[j], factor * xy[j], 00);
            }
            GL.End();

            GL.Begin(BeginMode.LineStrip);
            for (int i = 0; i < 5; i++)
            {
                int j = yorder[i];
                if (j < 0)
                {
                    GL.End();
                    GL.Begin(BeginMode.LineStrip);
                    j = -j;
                }
                j--;
                GL.Vertex3(factor * yx[j], start + factor * yy[j], 00);
            }
            GL.End();

            GL.Begin(BeginMode.LineStrip);
            for (int i = 0; i < 6; i++)
            {
                int j = zorder[i];
                if (j < 0)
                {
                    GL.End();
                    GL.Begin(BeginMode.LineStrip);
                    j = -j;
                }
                j--;
                GL.Vertex3(00, factor * zy[j], start + factor * zx[j]);
            }
            GL.End();
        }
    }
}