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
    class Player: People
    {
        

        public Player(Model _model, Vector3 _position) :base(_model, _position)
        {
            
        }


        public override void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Matrix projection, Matrix view, Game game)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && game.IsActive) //if user has clicked mouse, indicating they want to move the avatar
            {
                targetPosition = MouseInput.MousePosToWorldCoordinates(graphicsDevice, projection, view); //finds the position user has clicked in the 3d world and sets this as the avatar's target position
                

                if (targetPosition.Y != -100)
                {
                    targetVector = getTargetVector(gameTime); //assigns target vector to variable
                    motionState = PeopleMotionStates.rotating; //avatar must first rotate to look at the point they are going to move towards so the avatar state is set to rotating
                    getNewRotationMatrix(gameTime); // updates rotation matrix

                }
                else
                {
                    motionState = PeopleMotionStates.moving;
                }
                
                

            }

            else if (motionState == PeopleMotionStates.rotating) //if avatar is still rotating to view target
            {
                getNewRotationMatrix(gameTime); //updates the rotation matrix
            }

            else if (motionState == PeopleMotionStates.moving) //if the avatar is facing in the correct direction and now is moving towards position
            {
                rotationMatrix = Matrix.Identity;
                targetVector = getTargetVector(gameTime); //update the target vector for avatar's current position
                getNewTranslationMatrix(gameTime); //updates the translation matrix

            }

            updateTransformationMatrix(); //updates transformation matrix with transformations occured in current frame 

            avatar.worldMatrix *= transformationMatrix; //transforms world matrix by transformations specified in transformation matrix
            viewVector = avatar.worldMatrix.Forward; //adjusts view vector of model
            position = avatar.worldMatrix.Translation; //adjusts position variable to position in model's world matrix

        }









    }
}
