using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentkgamewindow
{
    class ParticleWindow : GameWindow
    {
        //Run() method is like glutMainLoop
        public ParticleWindow() : base(Consts.DefaultWindowSize, Consts.DefaultWindowSize) { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Title = "Particle System";
            GL.ClearColor(Color.Black);
            _particleList = GL.GenLists(1);
            GL.NewList(_particleList, ListMode.Compile);
            GL.PointSize(5);
            GL.Color3(Color.White);
            GL.Begin(BeginMode.Points);
            GL.Vertex3(20, 0, 20);
            GL.End();
            GL.EndList();

            _axesList = GL.GenLists(1);
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
            Matrix4 modelview = Matrix4.LookAt(200, 200, 200, 0, 0, 0, 0, 1, 0);
            GL.LoadMatrix(ref modelview);
            GL.CallList(_axesList);
            GL.CallList(_particleList);
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //Set the viewport to a square centered in the window
            var v = Math.Min(Width, Height);          // minimum dimension
            var xl = (Width - v) / 2; //Lower Left
            var yb = (Height - v) / 2; //Lower Right
            GL.Viewport(xl, yb, v, v);

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



        private int _particleList;
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
