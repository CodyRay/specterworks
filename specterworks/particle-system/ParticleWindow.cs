using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace specterworks
{
    public class ParticleWindow : GameWindow
    {
        private int _axesList;
        private int _pList;

        public ParticleWindow(Particle startParticle) : base(Consts.DefaultWindowSize, Consts.DefaultWindowSize)
        {
            Particles.AddFirst(startParticle);
        }

        public LinkedList<Particle> Particles { get; } = new LinkedList<Particle>();
        public bool AxesOn { get; set; } = false;
        public bool Pause { get; set; } = false;
        public int HorizontalCameraRotation { get; set; } = 0;
        public int VerticalCameraRotation { get; set; } = 0;
        public float Scale { get; set; } = 200;

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
                if (part.IsAlive && part.IsVisible)
                {
                    GL.Color3(part.ParticleColor.R, part.ParticleColor.G, part.ParticleColor.B);
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
            if (e?.Key == Key.PageUp)
                Scale += 50;
            if (e?.Key == Key.PageDown)
                Scale -= 50;
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
            _pList = GL.GenLists(1);
            _axesList = GL.GenLists(1);
            Axes();
        }

        protected override void OnRenderFrame(FrameEventArgs e) //Same As Display Function in C++
        {
            base.OnRenderFrame(e);

            //Erase Background
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //No Shading
            GL.ShadeModel(ShadingModel.Flat);

            GL.MatrixMode(MatrixMode.Modelview);

            //Transform based on 
            Vector3 eye = new Vector3(300, 0, 0);
            Vector3 up = new Vector3(0, 1, 0);

            Vector3 rotation = Vector3.Cross(eye, up);

            Matrix4 rotate1 = Matrix4.CreateRotationY((float)(HorizontalCameraRotation * Math.PI / 180));

            eye = Vector3.TransformVector(eye, rotate1);
            up = Vector3.TransformVector(up, rotate1);
            rotation = Vector3.TransformVector(rotation, rotate1);

            Matrix4 rotate2 = Matrix4.CreateFromAxisAngle(rotation, (float)(VerticalCameraRotation * Math.PI / 180));

            eye = Vector3.TransformVector(eye, rotate2);
            up = Vector3.TransformVector(up, rotate2);

            Matrix4 modelview = Matrix4.LookAt(eye.X, eye.Y, eye.Z, 0, 0, 0, up.X, up.Y, up.Z);
            GL.LoadMatrix(ref modelview);

            if (AxesOn)
                GL.CallList(_axesList);
            if (!Pause)
                UpdateParticles(.1f);

            GL.CallList(_pList);
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            float aspect;
            if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0)
            {
                aspect = 300;
            }
            else
            {
                aspect = 300 * ClientRectangle.Width / ClientRectangle.Height;
            }
            GL.Ortho(-aspect, aspect, -300, 300, 0.1, 1000f);
        }

        #endregion Overrides

        private void Axes()
        {
            GL.NewList(_axesList, ListMode.Compile);

            GL.LineWidth(Consts.AxesWidth);

            //X Axes
            GL.Color3(Color.Red);
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(Consts.AxesLength, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.End();

            //Y Axes
            GL.Color3(Color.Green);
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(0, Consts.AxesLength, 0);
            GL.Vertex3(0, 0, 0);
            GL.End();

            //Z Axes
            GL.Color3(Color.Blue);
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(0, 0, Consts.AxesLength);
            GL.Vertex3(0, 0, 0);
            GL.End();

            GL.EndList();
            GL.LineWidth(1);

        }
    }
}