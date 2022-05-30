using DTT.UI.ProceduralUI.Unsafe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.UI.ProceduralUI
{
    /// <summary>
    /// Image that contains additional behaviour for procedurally rounding the corners of the image.
    /// </summary>
    [AddComponentMenu("UI/Rounded Image")]
    [DisallowMultipleComponent]
    public class RoundedImage : Image
    {
        #region Variables
        #region Public
        /// <summary>
        /// The thickness of the border between 0 and 1.
        /// </summary>
        public float BorderThickness
        {
            get
            {
                switch (_selectedUnit)
                {
                    case RoundingUnit.PERCENTAGE:
                        return _borderThickness;
                    case RoundingUnit.WORLD:
                        return _borderThickness / rectTransform.rect.GetShortLength() * 2;
                    default:
                        throw new NotSupportedException("This unit is not supported " +
                                                        "for getting border thickness.");
                }
            }
            set
            {
                switch (_selectedUnit)
                {
                    case RoundingUnit.PERCENTAGE:
                        if (_borderThickness != value)
                            _propertyChanged = true;

                        _borderThickness = value;
                        break;
                    case RoundingUnit.WORLD:
                        if (_borderThickness != value)
                            _propertyChanged = true;

                        _borderThickness = value * rectTransform.rect.GetShortLength() / 2;
                        break;
                    default:
                        throw new NotSupportedException("This unit is not supported " +
                                                        "for setting border thickness.");
                }
            }
        }

        /// <summary>
        /// The default material used for rendering the image.
        /// </summary>
        public override Material defaultMaterial => RoundedImageAssetManager.GetRoundingMaterial();

        /// <summary>
        /// The material used for rendering the image.
        /// </summary>
        public override Material material
        {
            get => base.material;
            set
            {
                if (value != null && value.shader == RoundedImageAssetManager.RoundingShader)
                    base.material = value;
            }
        }

        /// <summary>
        /// The mode used for rendering the rounded image.
        /// </summary>
        public RoundingMode Mode
        {
            get => _roundingMode;
            set
            {
                _roundingMode = value;
                base.material = this.material;
            }
        }

        /// <summary>
        /// The amount of distance fall off the Rounded Image has.
        /// Value assigned should be positive.
        /// </summary>
        public float DistanceFalloff
        {
            get => _distanceFalloff;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException($"Distance fall off can't be a value below zero. Error value: {value}");

                if (_distanceFalloff != value)
                    _propertyChanged = true;
                _distanceFalloff = value;
            }
        }

        /// <summary>
        /// Whether the outside hitbox is being used.
        /// </summary>
        public bool UseHitboxOutside
        {
            get => _useHitboxOutside;
            set
            {
                _useHitboxOutside = value;

                // If the Outer Hitbox setting has been turned off
                // this makes sure the Inner Hitbox setting is also turned off.
                if (!_useHitboxOutside)
                    _useHitboxInside = false;
            }
        }

        /// <summary>
        /// Whether the inside hitbox is being used.
        /// </summary>
        public bool UseHitboxInside
        {
            get => _useHitboxInside;
            set => _useHitboxInside = value && _useHitboxOutside;
        }

        /// <summary>
        /// The error handler checks and throws errors for relevant problems.
        /// </summary>
        public RoundedImageErrorHandler ErrorHandler
        {
            get
            {
                if (_errorHandler == null)
                    _errorHandler = new RoundedImageErrorHandler(this);
                return _errorHandler;
            }
        }

        /// <summary>
        /// The hitbox used for the rounded image graphic.
        /// </summary>
        public RoundedImageHitbox Hitbox
        {
            get
            {
                if (_hitbox == null)
                    _hitbox = new RoundedImageHitbox(this);
                return _hitbox;
            }
        }
        #region Consts
        /// <summary>
        /// Used to determine the value of the border before it's send to the shader.
        /// </summary>
        public const float MAX_FACTOR_BORDER = 0.25f;
        #endregion
        #endregion

        #region Private
        /// <summary>
        /// The error handler checks and throws errors for relevant problems.
        /// </summary>
        private RoundedImageErrorHandler _errorHandler;

        /// <summary>
        /// The mode used to round the image.
        /// </summary>
        [SerializeField]
        private RoundingMode _roundingMode;

        /// <summary>
        /// The rounding amount for each corner.
        /// </summary>
        [SerializeField]
        private float[] _roundingAmount = new float[ShapeInfo.RECTANGLE_CORNER_AMOUNT];

        /// <summary>
        /// The border thickness amount.
        /// </summary>
        [SerializeField]
        private float _borderThickness = 0.5f;

        /// <summary>
        /// Whether the outside hitbox is being used.
        /// </summary>
        [SerializeField]
        private bool _useHitboxOutside;

        /// <summary>
        /// Whether the inside hitbox is being used.
        /// </summary>
        [SerializeField]
        private bool _useHitboxInside;

        /// <summary>
        /// The falloff distance amount.
        /// </summary>
        [SerializeField]
        private float _distanceFalloff = 0.5f;

        /// <summary>
        /// The unit that is currently being used.
        /// </summary>
        [SerializeField]
        private RoundingUnit _selectedUnit = RoundingUnit.PERCENTAGE;

        // These are only relevant for the custom inspector.
#if UNITY_EDITOR
        /// <summary>
        /// The corner mode that is currently being used.
        /// </summary>
        [SerializeField]
        private RoundingCornerMode _cornerMode;

        /// <summary>
        /// The side rounding that currently is being used.
        /// </summary>
        [SerializeField]
        private RoundingSide _side;
#endif
        /// <summary>
        /// The hitbox that handles the hit detection.
        /// </summary>
        private RoundedImageHitbox _hitbox;

        /// <summary>
        /// A flag that is used to determine whether a value for the rounded image has changed.
        /// This will then take it into account for the next frame and accordingly update the image.
        /// </summary>
        private bool _propertyChanged = false;
        #endregion
        #endregion

        #region Initialization
        /// <summary>
        /// Private constructor to disallow using <c>new</c> with this class.
        /// </summary>
        private RoundedImage() { }
        #endregion

        #region Methods
        #region Protected
        /// <summary>
        /// Initialized the component.
        /// </summary>
        protected override void OnEnable() => base.OnEnable();

        /// <summary>
        /// Sends data of the image to the shader so we can create the rounded effect.
        /// </summary>
        /// <param name="vh">
        /// Contains the vertices of the images.
        /// </param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var rounding = GetCornerRounding();
            Vector2 displaySize = GetImageSize();
            float borderData = (_roundingMode == RoundingMode.BORDER ? BorderThickness : 1) * Mathf.Min(displaySize.x, displaySize.y) * MAX_FACTOR_BORDER;
            // Account for falloff.
            borderData += DistanceFalloff;

            // Encode corner values into single floats. Optimization so we only have to use one UV.
            // Data should be decoded in the shader.
            float rightSideEncoded = Encoding.EncodeFloats(rounding[Corner.TOP_RIGHT],
                                                           rounding[Corner.BOTTOM_RIGHT]);
            float leftSideEncoded = Encoding.EncodeFloats(rounding[Corner.TOP_LEFT],
                                                           rounding[Corner.BOTTOM_LEFT]);

            Vector4 spriteOuterUV = sprite == null ? new Vector4(0, 0, 1, 1) : UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);

            // Populate the vertices with the UV data.
            UIVertex vert = new UIVertex();
            float x = DistanceFalloff / displaySize.x;
            float y = DistanceFalloff / displaySize.y;


            Vector2 uv1 = displaySize;
            Vector2 uv2 = new Vector2(rightSideEncoded, leftSideEncoded);

            float falloff = DistanceFalloff / (sprite == null ? 1 : 2);
            Vector2 uv3 = new Vector2(falloff, borderData);

            Vector3 positionScalar = Vector3.one + new Vector3(x, y, 0) * 2;
            Vector2 uv0Offset = new Vector2((spriteOuterUV.z - spriteOuterUV.x) / 2 + spriteOuterUV.x, (spriteOuterUV.w - spriteOuterUV.y) / 2 + spriteOuterUV.y);

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.position.Scale(positionScalar);

#if UNITY_2020_2_OR_NEWER
				if(sprite != null)
				{
					vert.uv0 -= (Vector4)uv0Offset;
					vert.uv0.Scale(positionScalar);
					vert.uv0 += (Vector4)uv0Offset;
				}
#else
                vert.uv0 -= uv0Offset;
                vert.uv0.Scale(positionScalar);
                vert.uv0 += uv0Offset;
#endif

#if UNITY_2020_2_OR_NEWER
				float z = (vert.uv0.x - spriteOuterUV.x) / (spriteOuterUV.z - spriteOuterUV.x);
				float w = (vert.uv0.y - spriteOuterUV.y) / (spriteOuterUV.w - spriteOuterUV.y);
				vert.uv1 = new Vector4(uv1.x, uv1.y, z, w);
#else
                vert.uv1 = uv1;
#endif
                vert.uv2 = uv2;
                vert.uv3 = uv3;

                vh.SetUIVertex(vert, i);
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="screenPoint">
        /// The point on the screen where the graphic was sampled.
        /// </param>
        /// <param name="eventCamera">
        /// The camera used for creating the event.
        /// </param>
        /// <returns>
        /// Whether the screen point hit the graphic.
        /// </returns>
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
            => Hitbox.HitTest(screenPoint) && base.IsRaycastLocationValid(screenPoint, eventCamera);

        /// <summary>
        /// Returns the percentage rounding of the given corner.
        /// </summary>
        /// <param name="corner">
        /// The corner to get the rounding from.
        /// </param>
        /// <returns>
        /// The rounding of the given corner in percentages.
        /// </returns>
        public float GetCornerRounding(Corner corner)
        {
            switch (_selectedUnit)
            {
                case RoundingUnit.PERCENTAGE:
                    return _roundingAmount[(int)corner];
                case RoundingUnit.WORLD:
                    return Mathf.Clamp01(_roundingAmount[(int)corner] / rectTransform.rect.GetShortLength() * 2);
                default:
                    throw new NotSupportedException("This unit is not supported " +
                                                    "for getting corner rounding.");
            }
        }

        /// <summary>
        /// Returns a collection with the rounding in percentages from every corner mapped by the corner.
        /// </summary>
        /// <returns>
        /// A collection with the rounding in percentages from every corner mapped by the corner.
        /// </returns>
        public ReadOnlyDictionary<Corner, float> GetCornerRounding()
        {
            Dictionary<Corner, float> output = new Dictionary<Corner, float>();
            for (int i = 0; i < _roundingAmount.Length; i++)
            {
                Corner c = (Corner)i;
                output.Add(c, GetCornerRounding(c));
            }
            return new ReadOnlyDictionary<Corner, float>(output);
        }

        /// <summary>
        /// Sets the rounding of the given corner.
        /// Amount should be within the bounds of zero to one.
        /// </summary>
        /// <param name="corner">
        /// The corner to set the rounding for.
        /// </param>
        /// <param name="amount">
        /// How much the corner should be rounded.
        /// Amount should be within the bounds of zero to one.
        /// </param>
        public void SetCornerRounding(Corner corner, float amount)
        {
            if (amount < 0 || amount > 1)
                throw new ArgumentOutOfRangeException($"Given amount should be within a range of zero to one. Error value: {amount}");

            ApplyCornerRounding(corner, amount);
        }

        /// <summary>
        /// Sets the rounding of all the corners.
        /// Amount should be within the bounds of zero to one.
        /// </summary>
        /// <param name="amount">
        /// How much the corner should be rounded.
        /// Amount should be within the bounds of zero to one.
        /// </param>
        public void SetCornerRounding(float amount)
        {
            if (amount < 0 || amount > 1)
                throw new ArgumentOutOfRangeException($"Given amount should be within a range of zero to one. Error value: {amount}");

            SetCornerRounding(amount, amount, amount, amount);
        }

        /// <summary>
        /// Sets the corner amount of every corner.
        /// Amounts should be within the bounds of zero to one.
        /// </summary>
        /// <param name="topRight">
        /// The rounding of the top right corner.
        /// Amount should be within the bounds of zero to one.
        /// </param>
        /// <param name="topLeft">
        /// The rounding of the top left corner
        /// Amount should be within the bounds of zero to one.
        /// </param>
        /// <param name="bottomLeft">
        /// The rounding of the bottom left corner.
        /// Amount should be within the bounds of zero to one.
        /// </param>
        /// <param name="bottomRight">
        /// The rounding of the bottom right corner.
        /// Amount should be within the bounds of zero to one.
        /// </param>
        public void SetCornerRounding(float topLeft, float topRight, float bottomLeft, float bottomRight)
        {
            float[] rounding = new float[] { topLeft, topRight, bottomLeft, bottomRight };
            for (int i = 0; i < ShapeInfo.RECTANGLE_CORNER_AMOUNT; i++)
            {
                if (rounding[i] < 0 || rounding[i] > 1)
                    throw new ArgumentOutOfRangeException($"Given amount should be within a range of zero to one. Error value: {rounding[i]}");

                ApplyCornerRounding((Corner)i, rounding[i]);
            }
        }

        /// <summary>
        /// The visual size of the image inside the Rect Transform.
        /// </summary>
        /// <returns>The visual size of the image inside the Rect Transform.</returns>
        public Vector2 GetImageSize()
        {
            Rect imageRect = GetPixelAdjustedRect();
            if (preserveAspect && sprite != null)
                PreserveSpriteAspectRatio(ref imageRect, sprite.rect.size);

            return imageRect.size;
        }
        #endregion

        #region Private
        /// <summary>
        /// Checks whether to update the image.
        /// </summary>
        private void Update()
        {
            if (_propertyChanged)
            {
                UpdateGeometry();
                _propertyChanged = false;
            }
        }

        /// <summary>
        /// Applies rounding amount to given corner.
        /// </summary>
        /// <param name="corner">
        /// Corner to round.
        /// </param>
        /// <param name="amount">
        /// By how much to round.
        /// </param>
        private void ApplyCornerRounding(Corner corner, float amount)
        {
            switch (_selectedUnit)
            {
                case RoundingUnit.PERCENTAGE:
                    if (amount != _roundingAmount[(int)corner])
                        _propertyChanged = true;
                    _roundingAmount[(int)corner] = amount;
                    break;
                case RoundingUnit.WORLD:
                    float newValue = amount * rectTransform.rect.GetShortLength() / 2;
                    if (newValue != _roundingAmount[(int)corner])
                        _propertyChanged = true;
                    _roundingAmount[(int)corner] = newValue;
                    break;
                default:
                    throw new NotSupportedException("This unit is not supported " +
                                                    "for getting corner rounding.");
            }
        }

        /// <summary>
        /// Updates the given Rect with the values where the sprite maintains aspect ratio.
        /// </summary>
        /// <param name="rect">The rect with the old values that will be updated with new.</param>
        /// <param name="spriteSize">The size of the sprite.</param>
        private void PreserveSpriteAspectRatio(ref Rect rect, Vector2 spriteSize)
        {
            float spriteRatio = spriteSize.x / spriteSize.y;
            float rectRatio = rect.width / rect.height;

            if (spriteRatio > rectRatio)
            {
                float oldHeight = rect.height;
                rect.height = rect.width * (1.0f / spriteRatio);
                rect.y += (oldHeight - rect.height) * rectTransform.pivot.y;
            }
            else
            {
                float oldWidth = rect.width;
                rect.width = rect.height * spriteRatio;
                rect.x += (oldWidth - rect.width) * rectTransform.pivot.x;
            }
        }
        #endregion
        #endregion
    }
}