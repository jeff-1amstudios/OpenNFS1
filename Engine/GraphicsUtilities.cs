#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace NfsEngine
{
    /// <summary>
    /// Used for text justification.
    /// </summary>
    public enum Justify
    {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        MIDDLE_LEFT,
        MIDDLE_CENTER,
        MIDDLE_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT
    }


    /// <summary>
    /// Type of shape to draw.
    /// </summary>
    public enum ShapeType
    {
        Cube
    }

    /// <summary>
    /// GraphicsUtilities
    ///   DrawableGameComponent for debug-graphics functionality.
    ///   Currently supports 3D lines, text, and basic solid shapes.
    ///   Registers self as service provider - IGraphicsUtilitiesService.
    ///   
    /// To use:
    ///   Create an instance of GraphicsUtilities
    ///   Add it to the list of components
    ///   Set its view/projection matrices every frame
    ///   Add lines/text/shapes every frame
    /// 
    /// </summary>
    public class GraphicsUtilities : IDrawableObject
    {
        #region Creation / Initialization
        public GraphicsUtilities(SpriteFont font)
        {
            CreateLineEffect();
            CreateShapeEffect();
            CreateCube();

			mFont1 = font;
            mSpriteBatch = new SpriteBatch(Engine.Instance.Device);

        }

        public void Update(GameTime gameTime)
        {
			if (Engine.Instance.Camera == null) return;
            SetViewMatrix(Engine.Instance.Camera.View);
            SetProjectionMatrix(Engine.Instance.Camera.Projection);
        }
       
        /// <summary>
        /// Draw utility graphics waiting to be rendered this pass.
        /// </summary>
        public void Draw()
        {
					Engine.Instance.Device.DepthStencilState = DepthStencilState.Default;
            // Draw shapes
            if (sShapeList.Count > 0)
            {
                int nbrPrimitives = 0;
                foreach (ShapeData shapeData in sShapeList)
                {

                    switch (shapeData.mType)
                    {
                        case ShapeType.Cube:
							Engine.Instance.Device.SetVertexBuffer(mCubeVertexBuffer);
                            nbrPrimitives = 12;
							Engine.Instance.Device.RasterizerState = RasterizerState.CullClockwise;
                            break;
                    }

                    
                    mBasicShapeEffect.DiffuseColor = shapeData.mColor.ToVector3() * 0.5f;
                    mBasicShapeEffect.SpecularColor = shapeData.mColor.ToVector3();

                    mBasicShapeEffect.TextureEnabled = false;

                    mBasicShapeEffect.World = shapeData.mWorldMatrix;
                    mBasicShapeEffect.View = mViewMatrix;
                    mBasicShapeEffect.Projection = mProjectionMatrix;
                    
                    foreach (EffectPass pass in mBasicShapeEffect.CurrentTechnique.Passes)
                    {
						pass.Apply();
                        Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, nbrPrimitives);
                    }
                }
            }
            ClearShapes();
            

            // Draw lines
            if (sLinesList.Count > 0)
            {
				mLineVertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionColor), sLinesList.Count, BufferUsage.WriteOnly);

                mLineVertexBuffer.SetData<VertexPositionColor>(sLinesList.ToArray());
                Engine.Instance.Device.SetVertexBuffer(mLineVertexBuffer);

                mBasicLineEffect.View = mViewMatrix;
                mBasicLineEffect.Projection = mProjectionMatrix;

                foreach (EffectPass pass in mBasicLineEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Engine.Instance.Device.DrawPrimitives(PrimitiveType.LineList, 0, sLinesList.Count / 2);
                }
            }
            ClearLines();
        }

        public void DrawText()
        {
            // Draw text
            if (sTextList.Count > 0)
            {
                mSpriteBatch.Begin();
                foreach (TextData textData in sTextList)
                {
                    Vector2 screenPos = new Vector2(textData.mPos.X, textData.mPos.Y);
                    if (!textData.mIsTransformed)
                    {
                        // If text was specified in 3D, transform it to 2D coordinates
                        Vector3 transformed = Engine.Instance.Device.Viewport.Project(textData.mPos,
                                                                              mProjectionMatrix,
                                                                              mViewMatrix,
                                                                              Matrix.Identity);

                        // Don't draw text for positions behind the camera
                        if (transformed.Z < 0.0f)
                        {
                            continue;
                        }

                        screenPos.X = transformed.X;
                        screenPos.Y = transformed.Y;
                    }

                    // Draw each string
                    JustifyText(mFont1, textData.mText, textData.mJustify, screenPos, out screenPos);
                    mSpriteBatch.DrawString(mFont1, textData.mText, screenPos, textData.mColor);
                }
                mSpriteBatch.End();
            }
            ClearText();
        }

        #endregion


        #region Utilities (Line/Text/Object drawing)

        public void AddCube(Matrix worldTransform, Color color)
        {
            if (sShapeList.Count >= MAX_SHAPES)
            {
                return;
            }
            ShapeData shapeData = new ShapeData();
            shapeData.mType = ShapeType.Cube;
            shapeData.mWorldMatrix = worldTransform;
            shapeData.mColor = color;
            shapeData.mTexture = null;
            sShapeList.Add(shapeData);
        }


        /// <summary>
        /// Add 3D line.
        /// </summary>
        /// <param name="startPos">3D world-space start position</param>
        /// <param name="endPos">3D world-space end position</param>
        /// <param name="color">Color of line</param>
        public void AddLine(Vector3 startPos, Vector3 endPos, Color color)
        {
            if (sLinesList.Count >= MAX_LINES * 2)
            {
                return;
            }
            VertexPositionColor lineVert = new VertexPositionColor();
            lineVert.Position = startPos;
            lineVert.Color = color;
            sLinesList.Add(lineVert);
            lineVert.Position = endPos;
            lineVert.Color = color;
            sLinesList.Add(lineVert);
        }


        /// <summary>
        /// Add text at 2D position.
        /// </summary>
        /// <param name="pos">XY screen coordinates (pixels)</param>
        /// <param name="text">Text to draw</param>
        /// <param name="color">Color of text</param>
        public void AddText(Vector2 pos, String text, Justify justify, Color color)
        {
            if (sTextList.Count >= MAX_TEXT_LINES)
            {
                return;
            }

            TextData textData = new TextData();
            textData.mPos.X = pos.X;
            textData.mPos.Y = pos.Y;
            textData.mPos.Z = 0.0f;
            textData.mText = text;
            textData.mColor = color;
            textData.mJustify = justify;
            textData.mIsTransformed = true;
            sTextList.Add(textData);
        }


        /// <summary>
        /// Add text at 3D position.
        /// </summary>
        /// <param name="worldPos">3D world-space position for text</param>
        /// <param name="text">Text to draw</param>
        /// <param name="color">Color of text</param>
        public void AddText(Vector3 worldPos, String text, Justify justify, Color color)
        {
            if (sTextList.Count >= MAX_TEXT_LINES)
            {
                return;
            }

            TextData textData = new TextData();
            textData.mPos = worldPos;
            textData.mText = text;
            textData.mColor = color;
            textData.mJustify = justify;
            textData.mIsTransformed = false;
            sTextList.Add(textData);
        }


        /// <summary>
        /// Add coordinate axis using the specified transformation.
        /// </summary>
        /// <param name="worldTransform">World transformation matrix</param>
        /// <param name="scale">Scale on drawn lines (1.0 units by default).</param>
        public void AddAxis(Matrix worldTransform, float scale)
        {
            AddLine(worldTransform.Translation, worldTransform.Translation + worldTransform.Forward, Color.Red);
            AddLine(worldTransform.Translation, worldTransform.Translation + worldTransform.Left, Color.Green);
            AddLine(worldTransform.Translation, worldTransform.Translation + worldTransform.Up, Color.Blue);
        }


        /// <summary>
        /// Add a cube using lines.
        /// </summary>
        /// <param name="worldTransform">World transformation matrix, specifies the center of the cube.</param>
        /// <param name="color">Color</param>
        public void AddWireframeCube(Matrix worldTransform, Color color)
        {
            Vector3 forwardVector = worldTransform.Forward / 2.0f;
            Vector3 leftVector = worldTransform.Left / 2.0f;
            Vector3 upVector = worldTransform.Up / 2.0f;

            Vector3 centerPosition = worldTransform.Translation;
            Vector3 forwardLeftUp = centerPosition + forwardVector + leftVector + upVector;
            Vector3 forwardRightUp = centerPosition + forwardVector - leftVector + upVector;
            Vector3 backwardLeftUp = centerPosition - forwardVector + leftVector + upVector;
            Vector3 backwardRightUp = centerPosition - forwardVector - leftVector + upVector;

            Vector3 forwardLeftDown = centerPosition + forwardVector + leftVector - upVector;
            Vector3 forwardRightDown = centerPosition + forwardVector - leftVector - upVector;
            Vector3 backwardLeftDown = centerPosition - forwardVector + leftVector - upVector;
            Vector3 backwardRightDown = centerPosition - forwardVector - leftVector - upVector;

            // Draw top
            AddLine(forwardLeftUp, forwardRightUp, color);
            AddLine(forwardRightUp, backwardRightUp, color);
            AddLine(backwardRightUp, backwardLeftUp, color);
            AddLine(backwardLeftUp, forwardLeftUp, color);

            // Draw bottom
            AddLine(forwardLeftDown, forwardRightDown, color);
            AddLine(forwardRightDown, backwardRightDown, color);
            AddLine(backwardRightDown, backwardLeftDown, color);
            AddLine(backwardLeftDown, forwardLeftDown, color);

            // Draw sides
            AddLine(forwardLeftUp, forwardLeftDown, color);
            AddLine(forwardRightUp, forwardRightDown, color);
            AddLine(backwardRightUp, backwardRightDown, color);
            AddLine(backwardLeftUp, backwardLeftDown, color);
        }


        /// <summary>
        /// Add a square grid using lines.
        /// </summary>
        /// <param name="worldTransform">World transformation matrix, specifies the center of the grid.</param>
        /// <param name="numRows">Number of rows (and columns).</param>
        /// <param name="color">Color.</param>
        public void AddSquareGrid(Matrix worldTransform, int numRows, Color color)
        {
            if (0 < numRows)
            {
                float scale = worldTransform.Forward.Length();
                Vector3 forwardVector = worldTransform.Forward / 2.0f;
                Vector3 leftVector = worldTransform.Left / 2.0f;
                Vector3 backwardsNormalizedVector = -forwardVector / forwardVector.Length();
                Vector3 rightNormalizedVector = -leftVector / leftVector.Length();

                Vector3 centerPosition = worldTransform.Translation;
                Vector3 forwardLeft = centerPosition + forwardVector + leftVector;
                Vector3 forwardRight = centerPosition + forwardVector - leftVector;
                Vector3 backwardLeft = centerPosition - forwardVector + leftVector;
                Vector3 backwardRight = centerPosition - forwardVector - leftVector;

                // Draw outline of the grid
                AddLine(forwardLeft, forwardRight, color);
                AddLine(forwardRight, backwardRight, color);
                AddLine(backwardRight, backwardLeft, color);
                AddLine(backwardLeft, forwardLeft, color);

                // Draw interior grid lines
                float stepSize = 1.0f / (float)(numRows);
                for (int ii = 1; ii < numRows; ++ii)
                {
                    float percentageAcross = (float)(ii) * stepSize;
                    // Front-to-back line
                    AddLine(forwardLeft + (rightNormalizedVector * percentageAcross * scale),
                              backwardLeft + (rightNormalizedVector * percentageAcross * scale),
                              color);

                    // Left-to-right line
                    AddLine(forwardLeft + (backwardsNormalizedVector * percentageAcross * scale),
                              forwardRight + (backwardsNormalizedVector * percentageAcross * scale),
                              color);
                }
            }
        }
        #endregion


        #region Data Access (used to set view/projection matrices)

        public void SetViewMatrix(Matrix view)
        {
            mViewMatrix = view;
        }

        public void SetProjectionMatrix(Matrix proj)
        {
            mProjectionMatrix = proj;
        }

        /// <summary>
        /// Clear list of shapes waiting to be rendered.
        /// </summary>
        public void ClearShapes()
        {
            sShapeList.Clear();
        }

        /// <summary>
        /// Clear list of lines waiting to be rendered.
        /// </summary>
        public void ClearLines()
        {
            sLinesList.Clear();
        }


        /// <summary>
        /// Clear list of text waiting to be rendered.
        /// </summary>
        public void ClearText()
        {
            sTextList.Clear();
        }
        #endregion


        #region Private Data & Methods

        /// <summary>
        /// Justify text based on enumerated value.
        /// </summary>
        private void JustifyText(SpriteFont font, String text, Justify justify, Vector2 inputPos, out Vector2 resultPos)
        {
            Vector2 textSize = font.MeasureString(text);

            // Default text to upper-left
            resultPos = inputPos;

            switch (justify)
            {
                case Justify.TOP_LEFT:
                    break;

                case Justify.TOP_CENTER:
                    resultPos.X -= (textSize.X / 2);
                    break;

                case Justify.TOP_RIGHT:
                    resultPos.X -= textSize.X;
                    break;

                case Justify.MIDDLE_LEFT:
                    resultPos.Y -= (textSize.Y / 2);
                    break;

                case Justify.MIDDLE_CENTER:
                    resultPos.X -= (textSize.X / 2);
                    resultPos.Y -= (textSize.Y / 2);
                    break;

                case Justify.MIDDLE_RIGHT:
                    resultPos.X -= textSize.X;
                    resultPos.Y -= (textSize.Y / 2);
                    break;

                case Justify.BOTTOM_LEFT:
                    resultPos.Y -= textSize.Y;
                    break;

                case Justify.BOTTOM_CENTER:
                    resultPos.X -= (textSize.X / 2);
                    resultPos.Y -= textSize.Y;
                    break;

                case Justify.BOTTOM_RIGHT:
                    resultPos.X -= textSize.X;
                    resultPos.Y -= textSize.Y;
                    break;
            }
        }


        /// <summary>
        /// Create the BasicEffect to be used by Lines.
        /// </summary>
        private void CreateLineEffect()
        {
            mBasicLineEffect = new BasicEffect(Engine.Instance.Device);
            mBasicLineEffect.VertexColorEnabled = true;
        }


        /// <summary>
        /// Create the BasicEffect to be used by shapes.
        /// </summary>
        private void CreateShapeEffect()
        {
            mBasicShapeEffect = new BasicEffect(Engine.Instance.Device);
            mBasicShapeEffect.Alpha = 1.0f;
            mBasicShapeEffect.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            mBasicShapeEffect.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
            mBasicShapeEffect.SpecularPower = 3.0f;
            mBasicShapeEffect.AmbientLightColor = new Vector3(0.75f, 0.75f, 0.75f);

            mBasicShapeEffect.DirectionalLight0.Enabled = true;
            mBasicShapeEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            mBasicShapeEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
            mBasicShapeEffect.DirectionalLight0.SpecularColor = Vector3.One;

            mBasicShapeEffect.DirectionalLight1.Enabled = true;
            mBasicShapeEffect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            mBasicShapeEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1.0f, -1.0f, 1.0f));
            mBasicShapeEffect.DirectionalLight1.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);

            mBasicShapeEffect.LightingEnabled = true;

            
        }


        /// <summary>
        /// Create vertices and vertex buffer for drawing a solid cube.
        /// </summary>
        private void CreateCube()
        {
            mCubeVertices = new VertexPositionNormalTexture[36];

            Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, 0.5f);
            Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, 0.5f);
            Vector3 topRightFront = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, 0.5f);
            Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, -0.5f);
            Vector3 topRightBack = new Vector3(0.5f, 0.5f, -0.5f);
            Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, -0.5f);
            Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, -0.5f);

            Vector2 textureTopLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureTopRight = new Vector2(1.0f, 0.0f);
            Vector2 textureBottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureBottomRight = new Vector2(1.0f, 1.0f);

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);


            // Front face.
            mCubeVertices[0] = new VertexPositionNormalTexture(topLeftFront, frontNormal, textureTopLeft);
            mCubeVertices[1] = new VertexPositionNormalTexture(bottomLeftFront, frontNormal, textureBottomLeft);
            mCubeVertices[2] = new VertexPositionNormalTexture(topRightFront, frontNormal, textureTopRight);
            mCubeVertices[3] = new VertexPositionNormalTexture(bottomLeftFront, frontNormal, textureBottomLeft);
            mCubeVertices[4] = new VertexPositionNormalTexture(bottomRightFront, frontNormal, textureBottomRight);
            mCubeVertices[5] = new VertexPositionNormalTexture(topRightFront, frontNormal, textureTopRight);

            // Back face.
            mCubeVertices[6] = new VertexPositionNormalTexture(topLeftBack, backNormal, textureTopRight);
            mCubeVertices[7] = new VertexPositionNormalTexture(topRightBack, backNormal, textureTopLeft);
            mCubeVertices[8] = new VertexPositionNormalTexture(bottomLeftBack, backNormal, textureBottomRight);
            mCubeVertices[9] = new VertexPositionNormalTexture(bottomLeftBack, backNormal, textureBottomRight);
            mCubeVertices[10] = new VertexPositionNormalTexture(topRightBack, backNormal, textureTopLeft);
            mCubeVertices[11] = new VertexPositionNormalTexture(bottomRightBack, backNormal, textureBottomLeft);

            // Top face.
            mCubeVertices[12] = new VertexPositionNormalTexture(topLeftFront, topNormal, textureBottomLeft);
            mCubeVertices[13] = new VertexPositionNormalTexture(topRightBack, topNormal, textureTopRight);
            mCubeVertices[14] = new VertexPositionNormalTexture(topLeftBack, topNormal, textureTopLeft);
            mCubeVertices[15] = new VertexPositionNormalTexture(topLeftFront, topNormal, textureBottomLeft);
            mCubeVertices[16] = new VertexPositionNormalTexture(topRightFront, topNormal, textureBottomRight);
            mCubeVertices[17] = new VertexPositionNormalTexture(topRightBack, topNormal, textureTopRight);

            // Bottom face. 
            mCubeVertices[18] = new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, textureTopLeft);
            mCubeVertices[19] = new VertexPositionNormalTexture(bottomLeftBack, bottomNormal, textureBottomLeft);
            mCubeVertices[20] = new VertexPositionNormalTexture(bottomRightBack, bottomNormal, textureBottomRight);
            mCubeVertices[21] = new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, textureTopLeft);
            mCubeVertices[22] = new VertexPositionNormalTexture(bottomRightBack, bottomNormal, textureBottomRight);
            mCubeVertices[23] = new VertexPositionNormalTexture(bottomRightFront, bottomNormal, textureTopRight);

            // Left face.
            mCubeVertices[24] = new VertexPositionNormalTexture(topLeftFront, leftNormal, textureTopRight);
            mCubeVertices[25] = new VertexPositionNormalTexture(bottomLeftBack, leftNormal, textureBottomLeft);
            mCubeVertices[26] = new VertexPositionNormalTexture(bottomLeftFront, leftNormal, textureBottomRight);
            mCubeVertices[27] = new VertexPositionNormalTexture(topLeftBack, leftNormal, textureTopLeft);
            mCubeVertices[28] = new VertexPositionNormalTexture(bottomLeftBack, leftNormal, textureBottomLeft);
            mCubeVertices[29] = new VertexPositionNormalTexture(topLeftFront, leftNormal, textureTopRight);

            // Right face. 
            mCubeVertices[30] = new VertexPositionNormalTexture(topRightFront, rightNormal, textureTopLeft);
            mCubeVertices[31] = new VertexPositionNormalTexture(bottomRightFront, rightNormal, textureBottomLeft);
            mCubeVertices[32] = new VertexPositionNormalTexture(bottomRightBack, rightNormal, textureBottomRight);
            mCubeVertices[33] = new VertexPositionNormalTexture(topRightBack, rightNormal, textureTopRight);
            mCubeVertices[34] = new VertexPositionNormalTexture(topRightFront, rightNormal, textureTopLeft);
            mCubeVertices[35] = new VertexPositionNormalTexture(bottomRightBack, rightNormal, textureBottomRight);

			mCubeVertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionNormalTexture), mCubeVertices.Length,
												 BufferUsage.WriteOnly);

            mCubeVertexBuffer.SetData<VertexPositionNormalTexture>(mCubeVertices);
        }

        

        // Internal data types
        struct TextData
        {
            public Vector3 mPos;
            public String mText;
            public Color mColor;
            public Justify mJustify;
            public bool mIsTransformed;
        }

        struct ShapeData
        {
            public ShapeType mType;
            public Matrix mWorldMatrix;
            public Color mColor;
            public Texture2D mTexture;
        }

        Matrix mViewMatrix = Matrix.Identity;
        Matrix mProjectionMatrix = Matrix.CreateLookAt(Vector3.One, Vector3.Zero, Vector3.Up);

        // Lists of data
        private List<VertexPositionColor> sLinesList = new List<VertexPositionColor>();
        private List<TextData> sTextList = new List<TextData>();
        private List<ShapeData> sShapeList = new List<ShapeData>();

        // Shape-drawing data
        BasicEffect mBasicShapeEffect;
        VertexPositionNormalTexture[] mCubeVertices;
        VertexBuffer mCubeVertexBuffer;
        const int MAX_SHAPES = 200;

        // Line-drawing data
        VertexBuffer mLineVertexBuffer;
        BasicEffect mBasicLineEffect;
        const int MAX_LINES = 1024;

        // Text-drawing data
        SpriteBatch mSpriteBatch;
        SpriteFont mFont1;
        const int MAX_TEXT_LINES = 50;
        #endregion

    }
}
