using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace specterworks
{
    class ParticleWindow : GameWindow
    {
        private bool AxesOn = true;
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e?.Key == Key.A)
                AxesOn = !AxesOn;
        }
        //Run() method is like glutMainLoop
        public ParticleWindow() : base(Consts.DefaultWindowSize, Consts.DefaultWindowSize) { }

        public LinkedList<Particle> Particles { get; } = new LinkedList<Particle>();

        public RandomSource Rand { get; } = new RandomSource();

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

            var first = Particles.AddFirst(new Particle(10000, 10000, 10000)).Value;
            first.EmitParticle = CoolEmitter;
            first.TimeLifeSpan = 1000;
            first.Start();
        }

        private IEnumerable<Particle> CoolEmitter(Particle p)
        {
            if (Rand.NextInt(0, 10) == 0)
            {
                p.TimeAge = p.TimeLifeSpan ?? p.TimeAge;
                if (Rand.NextInt(0, 50) == 0)
                {
                    //Two Children
                    return new[] { MakeParticle(p, true), MakeParticle(p, true) };
                }
                else
                {
                    //One Child
                    return new[] { MakeParticle(p, true) };
                }
            }
            else
            {
                return Enumerable.Range(0, Rand.NextInt(0, 20)).Select(i =>
                {
                    Particle part = MakeParticle(p);

                    return part;
                });
            }
        }

        private Particle MakeParticle(Particle p, bool make_emitter = false)
        {
            var part = new Particle(p.XBound, p.YBound, p.ZBound);
            var velocity = 10;
            part.StartLocation = new Vector3(p.CurrentLocation.X + Rand.Next(-5, 5), p.CurrentLocation.Y + Rand.Next(-5, 5), p.CurrentLocation.Z + Rand.Next(-5, 5));
            part.StartVelocity = new Vector3(Rand.Next(-velocity, velocity), Rand.Next(-velocity, velocity), Rand.Next(-velocity, velocity));

            if (p.CurrentColor.B > 10)
                part.StartColor = Color.FromArgb(p.CurrentColor.A, p.CurrentColor.R, p.CurrentColor.G, p.CurrentColor.B - 10);
            else if (p.CurrentColor.G > 10)
                part.StartColor = Color.FromArgb(p.CurrentColor.A, p.CurrentColor.R, p.CurrentColor.G - 10, 0);
            else if (p.CurrentColor.B > 10)
                part.StartColor = Color.FromArgb(p.CurrentColor.A, p.CurrentColor.R - 10, 0, 0);
            else
            {
                part.StartColor = Color.White;
            }

            if (make_emitter)
            {
                part.EmitParticle = CoolEmitter;
                part.TimeLifeSpan = 1000;
            }

            part.TimeLifeSpan = 5;

            return part;
        }

        Stopwatch timer = Stopwatch.StartNew();

        protected override void OnRenderFrame(FrameEventArgs e) //Same As Display Function in C++
        {
            base.OnRenderFrame(e);

            if (timer.ElapsedMilliseconds > 100)
            {
                UpdateParticles(((float)timer.ElapsedMilliseconds / 1000));
                timer = Stopwatch.StartNew();
            }

            //Erase Background
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //No Shading
            GL.ShadeModel(ShadingModel.Flat);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 modelview = Matrix4.LookAt(200, 200, 200, 0, 0, 0, 0, 1, 0);
            GL.LoadMatrix(ref modelview);
            if (AxesOn)
                GL.CallList(_axesList);
            GL.CallList(_pList);
            SwapBuffers();
        }

        int _pList;
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
                part.Forward(time, p => Particles.AddBefore(current, p));
                if (part.IsAlive && part.IsWithinBounds)
                {
                    GL.Color3(part.CurrentColor);
                    GL.Vertex3(part.CurrentLocation);
                }
                else
                {
                    Particles.Remove(current);
                }
            }

            GL.End();
            GL.EndList();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //Set the viewport to a square centered in the window
            var v = Math.Min(Width, Height);          // minimum dimension
            var xl = (Width - v) / 2; //Lower Left
            var yb = (Height - v) / 2; //Lower Right
            //GL.Viewport(xl, yb, v, v);
            GL.Viewport(0, 0, Width, Height);

            // set the viewing volume:
            // remember that the Z clipping  values are actually
            // given as DISTANCES IN FRONT OF THE EYE
            // ONLY USE gluOrtho2D() IF YOU ARE DOING 2D !

            GL.MatrixMode(MatrixMode.Projection);
            //if (true) //TODO: ability to change to perspective view
            //glOrtho( XMIN, XMAX,   YMIN, YMAX, 0.1, 1000. );
            GL.LoadIdentity();
            GL.Ortho(-300f, 300f, -300f, 300f, 0.1, 1000f);
            //else
            //    (90., 1., 0.1, 1000. );



            //GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref projection);
        }

        private int _axesList;

        void Axes(float length)
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
