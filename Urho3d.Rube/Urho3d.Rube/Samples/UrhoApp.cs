﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Urho;
using Urho.Urho2D;

namespace Urho3d.Rube.Samples
{
    public class UrhoApp : Application
    {
        private MonoDebugHud _monoDebugHud;

        private Scene _scene;
        private Camera _camera;

        private bool _drawDebug = true;



        [Preserve]
        public UrhoApp(ApplicationOptions options = null) : base(options)
        {
        }

        static UrhoApp()
        {
            UnhandledException += (s, e) =>
            {
                if (Debugger.IsAttached) Debugger.Break();
                e.Handled = true;
            };
        }




        protected override void Start()
        {
            Log.LogMessage += e => Debug.WriteLine($"[{e.Level}] {e.Message}");
            base.Start();

#if DEBUG
            _monoDebugHud = new MonoDebugHud(this);
            _monoDebugHud.Show();
#endif

            this._createScene();
            this._createCamera();
            this._setupViewport();

            
            this._subscribeToEvents();


            Rube rube = new Rube();
            rube.LoadWorld(this._scene);
        }



        protected override void OnUpdate(float timeStep)
        {
        }


        
        private void _createScene()
        {            
            // create scene
            this._scene = new Scene();
            this._scene.CreateComponent<Octree>();
            this._scene.CreateComponent<DebugRenderer>();

            // add touches component for test physics
            this._scene.CreateComponent<Components.HandlePhysicsTouches>();
        }


        private void _createCamera()
        {
            // Create camera
            Node CameraNode = _scene.CreateChild(SamplesConfig.mainCameraNodeName);
            CameraNode.Position = (new Vector3(0.0f, 0.0f, -0.10f));
            this._camera = CameraNode.CreateComponent<Camera>();
            this._camera.Orthographic = true;

            var graphics = Graphics;
            // x = Screen Width (px)
            // y = Screen Height(px)
            // s = Desired Height of Photoshop Square(px)
            // Camera Size = x / ((( x / y ) * 2 ) * s ) = 10 sprites de 's'
            // this._camera.OrthoSize = graphics.Width / (((graphics.Width / graphics.Height) * 2) * 32);

            this._camera.OrthoSize = (float)graphics.Height * PixelSize;
            // set zoom with design resolution for view all sample (zomm (1.0) for view in 3:2 1080X720)
            this._camera.Zoom = 1.0f * Math.Min((float)graphics.Width / 1080.0f, (float)graphics.Height / 720.0f);
        }




        private void _setupViewport()
        {
            Viewport viewport = new Viewport(Context, this._scene, this._camera, null);
            var renderer = Renderer;

            renderer.SetViewport(0, viewport);

            // Set background color for the scene
            // Zone zone = renderer.DefaultZone;
            // zone.FogColor = (new Color(1f, 0.1f, 0.1f));
        }




        private void _subscribeToEvents()
        {
            Engine.PostRenderUpdate += (PostRenderUpdateEventArgs obj) =>
            {
                // If draw debug mode is enabled, draw viewport debug geometry, which will show eg. drawable bounding boxes and skeleton
                // bones. Note that debug geometry has to be separately requested each frame. Disable depth test so that we can see the
                // bones properly
                if (_drawDebug)
                {
                    this._scene.GetComponent<PhysicsWorld2D>().DrawDebugGeometry();
                    Renderer.DrawDebugGeometry(false);
                }
            };
        }


    }
}
