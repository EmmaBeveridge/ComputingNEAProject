using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class Camera
    {
        public Vector3 posPrev = Vector3.Zero;

        GraphicsDevice graphicsDevice = null;
        GameWindow gameWindow = null;
        MouseState mouseState = default(MouseState);
        KeyboardState keyboardState = default(KeyboardState);

        public float translationUnitsps { get; set; } = 200f;
        public float rotationRadiansps { get; set; } = 70f;

        public float fieldOfViewDeg = 80f;
        public float nearClipPlane = 0.1f;
        public float farClipPlane = 2000f;
        private const float nearPlaneDistance = 0.1f;
        private const float farPlaneDistance = 2000f;


        bool usingMouseLook = false;
        
        bool mouseLookOn = false;

        Vector3 up = Vector3.Up;

        /// <summary>
        ///  Matrix situating camera within the world space. 
        /// </summary>
        Matrix cameraWorldMatrix = Matrix.Identity;

        /// <summary>
        /// Matrix to transform geometry from world space into view space.  
        /// </summary>
        Matrix viewMatrix = Matrix.Identity;

        /// <summary>
        /// Matrix to transform scene from view space and project onto an imaginary screen in front of camera. This camera uses perspective projection wherein the area seen by the camera is modelled as a frustrum and must be remapped onto a screen.
        /// </summary>
        Matrix projectionMatrix = Matrix.Identity;

        Vector3 TargetPositionToLookAt;

        public Matrix world
        {
            get { return cameraWorldMatrix; }
            set
            {
                cameraWorldMatrix = value;
                viewMatrix = Matrix.CreateLookAt(cameraWorldMatrix.Translation, cameraWorldMatrix.Forward + cameraWorldMatrix.Translation, cameraWorldMatrix.Up);

            }
        }

        public Matrix view
        {
            get { return viewMatrix; }


        }

        public Matrix projection
        {
            get { return projectionMatrix; }

        }

        public Vector3 position
        {
            get
            {
                return cameraWorldMatrix.Translation;
            }
            set
            {
                cameraWorldMatrix.Translation = value;
                CreateWorldandViewMatrices();
            }
        }

        public Vector3 forwardDirVector
        {
            get
            {
                return cameraWorldMatrix.Forward;
            }
            set
            {
                cameraWorldMatrix = Matrix.CreateWorld(cameraWorldMatrix.Translation, value, up);
                CreateWorldandViewMatrices();
            }
        }

        public Vector3 lookAtDirVector //gs direction in which camera is looking 
        {
            get
            { 
                return cameraWorldMatrix.Forward;
            }

            set
            {
                cameraWorldMatrix = Matrix.CreateWorld(cameraWorldMatrix.Translation, value, cameraWorldMatrix.Up); //changes worldMatrix to have forward as direction in which camera is looking
                CreateWorldandViewMatrices(); //updates viewMatrix as worldMatrix changed
            }
        }

        public Vector3 targetToLookAtPosVector //establishes target for camera to look at in world
        {
            set 
            {
                cameraWorldMatrix = Matrix.CreateWorld(cameraWorldMatrix.Translation, Vector3.Normalize(value-cameraWorldMatrix.Translation), cameraWorldMatrix.Up);
                CreateWorldandViewMatrices();
            }


        }



        public Camera (GraphicsDevice _graphicsDevice, GameWindow _gameWindow, Vector3 _position, Vector3 _lookAtTarget)
        {
            graphicsDevice = _graphicsDevice;
            gameWindow = _gameWindow;
            
            
            position = _position;
            TargetPositionToLookAt = _lookAtTarget;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfViewDeg), (float)graphicsDevice.Viewport.AspectRatio, nearClipPlane, farClipPlane);
            CreateWorldandViewMatrices();

           
        }

        /// <summary>
        /// Creates world and view matrices for camera given its current position and rotation. 
        /// </summary>
        private void CreateWorldandViewMatrices()
        {


            cameraWorldMatrix = Matrix.CreateWorld(cameraWorldMatrix.Translation, cameraWorldMatrix.Forward, cameraWorldMatrix.Up);
            viewMatrix = Matrix.CreateLookAt(cameraWorldMatrix.Translation, cameraWorldMatrix.Forward+cameraWorldMatrix.Translation, cameraWorldMatrix.Up);


        }

        /// <summary>
        /// Creates projection matrix using perspective projection. 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="fieldOfViewDeg"></param>
        public void CreatePerspectiveProjectionMatrix(GraphicsDevice graphicsDevice, float fieldOfViewDeg)
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfViewDeg), graphicsDevice.Viewport.AspectRatio, nearPlaneDistance, farPlaneDistance);

        }

        /// <summary>
        /// Creates projection matrix using perspective projection. 
        /// </summary>
        /// <param name="fieldOfViewInDegrees"></param>
        /// <param name="nearPlane"></param>
        /// <param name="farPlane"></param>
        public void CreatePerspectiveProjectionMatrix(float fieldOfViewInDegrees, float nearPlane, float farPlane)
        {
            // create the projection matrix.
            this.fieldOfViewDeg = MathHelper.ToRadians(fieldOfViewInDegrees);
            nearClipPlane = nearPlane;
            farClipPlane = farPlane;
            float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(this.fieldOfViewDeg, aspectRatio, nearClipPlane, farClipPlane);
        }


        /// <summary>
        /// Registers user keyboard input and calls relevant methods in order to translate or rotate the camera. Calls methods to update world, view, and projection matrices. 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update (GameTime gameTime)
        {
            MouseState _mouseState = Mouse.GetState(gameWindow);
            KeyboardState _keyboardState = Keyboard.GetState();

            if (_keyboardState.IsKeyDown(Keys.W)) { MoveForward(gameTime); }
            else if (_keyboardState.IsKeyDown(Keys.S)) { MoveBackward(gameTime); }

            if (_keyboardState.IsKeyDown(Keys.A)) { MoveLeft(gameTime); }
            else if (_keyboardState.IsKeyDown(Keys.D)) { MoveRight(gameTime); }
            
            if (_keyboardState.IsKeyDown(Keys.Q)) { MoveUp(gameTime); }
            else if (_keyboardState.IsKeyDown(Keys.E)) { MoveDown(gameTime); }

            if (_keyboardState.IsKeyDown(Keys.Left)) { RotateLeft(gameTime); }
            else if (_keyboardState.IsKeyDown(Keys.Right)) { RotateRight(gameTime); }

            if (_keyboardState.IsKeyDown(Keys.Up)) { RotateUp(gameTime); }
            else if (_keyboardState.IsKeyDown(Keys.Down)) { RotateDown(gameTime); }



            if (usingMouseLook)
            {
                if (_mouseState.LeftButton == ButtonState.Pressed)
                {
                    mouseLookOn = !mouseLookOn;
                }

                if (mouseLookOn)
                {
                    Vector2 diffMousePos = _mouseState.Position.ToVector2() - mouseState.Position.ToVector2();

                    if (diffMousePos.X != 0f)
                    {
                        RotateLeftOrRight(gameTime, diffMousePos.X);

                    }

                    if (diffMousePos.Y != 0f)
                    {
                        RotateUpOrDown(gameTime, diffMousePos.Y);
                    }
                }
            }




            mouseState = _mouseState;
            keyboardState = _keyboardState;

          
            CreateWorldandViewMatrices();
            CreatePerspectiveProjectionMatrix(graphicsDevice, fieldOfViewDeg);

            
            

        }


        public void MoveForward(GameTime gameTime)
        {
            position += (cameraWorldMatrix.Forward * translationUnitsps) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void MoveBackward(GameTime gameTime)
        {
            position += (cameraWorldMatrix.Backward * translationUnitsps) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void MoveLeft(GameTime gameTime)
        {
            position += (cameraWorldMatrix.Left * translationUnitsps) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void MoveRight(GameTime gameTime)
        {
            position += (cameraWorldMatrix.Right * translationUnitsps) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void MoveUp(GameTime gameTime)
        {
            position += (cameraWorldMatrix.Up * translationUnitsps) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void MoveDown(GameTime gameTime)
        {
            position += (cameraWorldMatrix.Down * translationUnitsps) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }


        public void RotateUp(GameTime gameTime)
        {
            var radiansToRotateThrough = rotationRadiansps * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix rotationMomentMatrix = Matrix.CreateFromAxisAngle(cameraWorldMatrix.Right, MathHelper.ToRadians(radiansToRotateThrough));
            lookAtDirVector = Vector3.TransformNormal(lookAtDirVector, rotationMomentMatrix);
            CreateWorldandViewMatrices();
        }

        public void RotateDown(GameTime gameTime)
        {
            var radiansToRotateThrough = -(rotationRadiansps * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Matrix rotationMomentMatrix = Matrix.CreateFromAxisAngle(cameraWorldMatrix.Right, MathHelper.ToRadians(radiansToRotateThrough));
            lookAtDirVector = Vector3.TransformNormal(lookAtDirVector, rotationMomentMatrix);
            CreateWorldandViewMatrices();
        }

        public void RotateLeft(GameTime gameTime)
        {
            var radiansToRotateThrough = rotationRadiansps * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix rotationMomentMatrix = Matrix.CreateFromAxisAngle(cameraWorldMatrix.Up, MathHelper.ToRadians(radiansToRotateThrough));
            lookAtDirVector = Vector3.TransformNormal(lookAtDirVector, rotationMomentMatrix);
            CreateWorldandViewMatrices();
        }

        public void RotateRight(GameTime gameTime)
        {
            var radiansToRotateThrough = -(rotationRadiansps * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Matrix rotationMomentMatrix = Matrix.CreateFromAxisAngle(cameraWorldMatrix.Up, MathHelper.ToRadians(radiansToRotateThrough));
            lookAtDirVector = Vector3.TransformNormal(lookAtDirVector, rotationMomentMatrix);
            CreateWorldandViewMatrices();
        }

        
        public void RotateLeftOrRight (GameTime gameTime, float amount)
        {
            var radiansToRotateThrough = -rotationRadiansps * amount * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix rotationMomentMatrix = Matrix.CreateFromAxisAngle(cameraWorldMatrix.Up, MathHelper.ToRadians(radiansToRotateThrough));
            lookAtDirVector = Vector3.TransformNormal(lookAtDirVector, rotationMomentMatrix);
            CreateWorldandViewMatrices();
        }

        public void RotateUpOrDown(GameTime gameTime, float amount)
        {
            var radiansToRotateThrough = -rotationRadiansps * amount * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix rotationMomentMatrix = Matrix.CreateFromAxisAngle(cameraWorldMatrix.Right, MathHelper.ToRadians(radiansToRotateThrough));
            lookAtDirVector = Vector3.TransformNormal(lookAtDirVector, rotationMomentMatrix);
            CreateWorldandViewMatrices();
        }


    }
}
