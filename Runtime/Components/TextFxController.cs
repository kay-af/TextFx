using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using TMPro;

using UnityEngine;
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("TextFx.Editor")]
#endif
namespace TextFx
{
    /// <summary>
    /// The main TextFx controller provides an interface to create text effects using modifier groups and exposes controls to drive animations.
    /// <para>
    /// This controller provides functionalities that support any <see cref="TMP_Text"/> implementations.
    /// </para>
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("TextFx/TextFxController")]
    public class TextFxController : MonoBehaviour
    {
        // Vertex indices for accessing the TextMeshPro vertex buffer.
        internal const int VERTEX_IDX_BL = 0;
        internal const int VERTEX_IDX_TL = 1;
        internal const int VERTEX_IDX_TR = 2;
        internal const int VERTEX_IDX_BR = 3;

        [SerializeField]
        [Tooltip("When enabled, the text mesh updates every frame. Disable it and call UpdateMesh() only on changes for performance. Effective only during play mode.")]
        private bool _autoUpdate = false;

        /// <summary>
        /// When enabled, the text mesh updates every frame. Disable it and call <see cref="UpdateMesh"/> only on changes for performance. Effective only during play mode.
        /// </summary>
        public bool AutoUpdate { get => _autoUpdate; set => _autoUpdate = value; }

        [SerializeField]
        [Tooltip("Enable right to left mode?")]
        private bool _rightToLeft = false;

        /// <summary>
        /// Enable right to left mode?
        /// </summary>
        public bool RightToLeft { get => _rightToLeft; set => _rightToLeft = value; }

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The global strength multiplier for all the effects.")]
        private float _strength = 1f;

        /// <summary>
        /// The global strength multiplier for all the effects.
        /// <para>
        /// The value must be in range [0,1]. The assigned value is clamped to this range.
        /// </para>
        /// </summary>
        public float Strength
        {
            get => _strength;
            set => _strength = Mathf.Clamp01(_strength);
        }

        [SerializeField]
        [Tooltip("The global fractional offset that is added to all the group offsets.")]
        private float _offset = 0f;

        /// <summary>
        /// The global fractional offset that is added to all the group offsets.
        /// <para>
        /// Offset is the starting fractional position of the range affected by the effects.
        /// Paired with extent of groups, it defines the range for which the group modifiers are applied.
        /// </para>
        /// </summary>
        public float Offset
        {
            get => _offset;
            set => _offset = value;
        }

        [SerializeField]
        [Tooltip("The list of modifier groups.")]
        private List<ModifierGroup> _modifierGroups = new();

        // The base class of TextMeshPro text components.
        private TMP_Text _tmpText;

#if UNITY_EDITOR
        // Pre-render information used by the custom editor to display editor handles in the scene.
        internal Transform _textTransform;
        internal TMP_MeshInfo[] _meshInfos;
        internal TMP_LineInfo[] _lineInfos;
        internal TMP_CharacterInfo[] _charInfos;
#endif

        /// <summary>
        /// Gets the modifier group at the given index.
        /// </summary>
        /// <param name="index">The index of the group to get.</param>
        /// <returns>The modifier group at the given index. If the index is out of range, null is returned.</returns>
        public ModifierGroup GetModifierGroupAt(int index)
        {
            if (index < 0 || index >= _modifierGroups.Count)
            {
                Debug.LogError("Parameter 'index' is out of range.");
                return null;
            }

            return _modifierGroups[index];
        }

        /// <summary>
        /// Adds a modifier group to the list.
        /// </summary>
        /// <param name="group">The modifier group to add. Should not be null.</param>
        public void AddModifierGroup(ModifierGroup group)
        {
            if (group == null)
            {
                Debug.LogError("Parameter 'group' can not be null.");
                return;
            }

            _modifierGroups.Add(group);
        }

        /// <summary>
        /// Inserts a modifier group at the given index.
        /// </summary>
        /// <param name="index">The index to insert the group at.</param>
        /// <param name="group">The modifier group to insert. Should not be null.</param>
        public void InsertModifierGroup(int index, ModifierGroup group)
        {
            if (index < 0 || index > _modifierGroups.Count)
            {
                Debug.LogError("Parameter 'index' is out of range.");
                return;
            }

            if (group == null)
            {
                Debug.LogError("Parameter 'group' can not be null.");
                return;
            }

            _modifierGroups.Insert(index, group);
        }

        /// <summary>
        /// Removes a modifier group at the given index.
        /// </summary>
        /// <param name="index">The index of the group to remove.</param>
        public void RemoveModifierGroupAt(int index)
        {
            if (index < 0 || index > _modifierGroups.Count)
            {
                Debug.LogError("Parameter 'index' is out of range.");
                return;
            }

            _modifierGroups.RemoveAt(index);
        }

        /// <summary>
        /// Removes a modifier group.
        /// </summary>
        /// <param name="group">The modifier group to remove.</param>
        /// <returns>True if the group was removed else false.</returns>
        public bool RemoveModifierGroup(ModifierGroup group) => _modifierGroups.Remove(group);

        /// <summary>
        /// Forces the text mesh to update when auto update is disabled.
        /// This method is only effective when auto update is disabled and the application is playing.
        /// </summary>
        public void UpdateMesh()
        {
            if (!_autoUpdate && Application.isPlaying)
            {
                CheckActiveAndUpdateTextMesh();
            }
        }

        /// <summary>
        /// Call base.Awake() in the beginning if overriding.
        /// </summary>
        protected virtual void Awake() => _tmpText = GetComponent<TMP_Text>();

        /// <summary>
        /// Call base.OnEnable() in the beginning if overriding.
        /// </summary>
        protected virtual void OnEnable()
        {
            _tmpText.OnPreRenderText += OnPreRenderText;

            CheckActiveAndUpdateTextMesh();
        }

        /// <summary>
        /// Call base.OnDisable() in the beginning if overriding.
        /// </summary>
        protected virtual void OnDisable()
        {
            _tmpText.OnPreRenderText -= OnPreRenderText;

            CheckActiveAndUpdateTextMesh();
        }

        /// <summary>
        /// Call base.Update() in the beginning if overriding.
        /// </summary>
        protected virtual void Update()
        {
            if (_autoUpdate || !Application.isPlaying)
            {
                CheckActiveAndUpdateTextMesh();
            }

#if UNITY_EDITOR
            // OnPreRenderText text is not called when the text becomes empty.
            if (_tmpText.text.Trim().Length == 0)
            {
                ClearEditorHandlesInfo();
            }
#endif
        }

        /// <summary>
        /// Forces the text mesh to update if the component is active and enabled.
        /// </summary>
        private void CheckActiveAndUpdateTextMesh()
        {
            if (_tmpText.isActiveAndEnabled)
            {
                _tmpText.ForceMeshUpdate();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Clears the info stored to draw handles in the scene.
        /// TextMeshPro does not call OnPreRender text when the text becomes empty after erasing some content.
        /// The data must be cleared manually to make the scene view handles work properly in the Editor.
        /// </summary>
        private void ClearEditorHandlesInfo()
        {
            _textTransform = null;
            _meshInfos = null;
            _lineInfos = null;
            _charInfos = null;
        }
#endif

        /// <summary>
        /// Called before the text is rendered.
        /// </summary>
        /// <param name="textInfo">The text info to render.</param>
        private void OnPreRenderText(TMP_TextInfo textInfo)
        {
#if UNITY_EDITOR
            // Store the pre-render information to show editor handles.
            _textTransform = textInfo.textComponent.transform;

            _meshInfos = textInfo.CopyMeshInfoVertexData();

            _lineInfos = new TMP_LineInfo[textInfo.lineCount];
            Array.Copy(textInfo.lineInfo, _lineInfos, textInfo.lineCount);

            // Only copy the required characters not the whole buffer.
            _charInfos = new TMP_CharacterInfo[textInfo.characterCount];
            Array.Copy(textInfo.characterInfo, _charInfos, textInfo.characterCount);
#endif

            var totalContributingChars = CountContributingChars(textInfo.characterInfo, textInfo.characterCount);

            // Do nothing if there are no contributing characters.
            if (totalContributingChars == 0)
            {
                return;
            }

            // Each contributing character takes up this much extent out of the total extent = 1f.
            var extentPerChar = 1f / totalContributingChars;

            // Counter to keep track of the number of contributing characters found so far.
            var contributingCharsSoFar = 0;

            // Process linewise to support RTL.
            foreach (var lineInfo in _lineInfos)
            {
                // Adjust the indices for RTL support.
                var fromCharIdx = _rightToLeft ? lineInfo.lastCharacterIndex : lineInfo.firstCharacterIndex;
                var toCharIdx = _rightToLeft ? lineInfo.firstCharacterIndex : lineInfo.lastCharacterIndex;
                var deltaIdx = _rightToLeft ? -1 : 1;

                // Iterate over the characters in the line.
                for (var i = fromCharIdx; i != toCharIdx + deltaIdx; i += deltaIdx)
                {
                    var charInfo = textInfo.characterInfo[i];

                    // If the character does not contribute to the calculations, skip it.
                    if (!TextFxUtils.IsContributingChar(charInfo.character))
                    {
                        continue;
                    }

                    contributingCharsSoFar++;

                    var vertexIndex = charInfo.vertexIndex;
                    var materialIndex = charInfo.materialReferenceIndex;
                    var meshInfo = textInfo.meshInfo[materialIndex];

                    var vertices = meshInfo.vertices;
                    var vertexColors = meshInfo.colors32;

                    // Get the baseline pivot for this character.
                    var baselinePivot = GetBaselinePivot(charInfo);

                    // How much is the center of this character is away from the first contributing character fractionally.
                    var charOffset = extentPerChar * (contributingCharsSoFar - 0.5f);

                    // Passed accross all groups to calculate the final effect for this character.
                    var context = new ModificationContext
                    {
                        Transformation = Matrix4x4.identity,
                        Pivot = baselinePivot,
                        Color = charInfo.color,
                        // Default opacity is considered 1.
                        // The color's opacity is adjusted with this using linear interpolation during the final calculation.
                        Opacity = 1f,
                    };

                    // Apply the effects of all applicable groups to the character.
                    foreach (var modifierGroup in _modifierGroups)
                    {
                        foreach (var modifier in modifierGroup.OrderedModifiers)
                        {
                            // Character is outside the mask area.
                            // The mask takes into account, the center of the bounding box during calculations.
                            if (charOffset < modifierGroup.Mask.Start || charOffset > modifierGroup.Mask.End)
                            {
                                continue;
                            }

                            // Only calculate if the modifier is enabled.
                            if (!modifier.Enabled)
                            {
                                continue;
                            }

                            // Calculate the start and end of the group's effect range.
                            var effectStart = _offset + modifierGroup.Offset;
                            var effectEnd = _offset + modifierGroup.Offset + modifierGroup.Extent;

                            // Character is outside the group's effect range.
                            if (charOffset < effectStart || charOffset > effectEnd)
                            {
                                context.Strength = 0f;
                            }
                            else
                            {
                                // Evaluate the strength based on the plateau curve that must be applied
                                // to this character for this group.
                                var charOffsetFactor = Mathf.InverseLerp(effectStart, effectEnd, charOffset);
                                context.Strength = modifierGroup.Curve.Evaluate(charOffsetFactor) * _strength * modifierGroup.Strength;
                            }

                            // Invert the strength if the modifier is inverted.
                            if (modifier.Invert)
                            {
                                context.Strength = 1 - context.Strength;
                            }

                            // Adjust the pivot for future groups.
                            context.Pivot += modifierGroup.Pivot;

                            // Apply this modifier.
                            modifier.Apply(ref context);
                        }
                    }

                    // Update with the transformations received in the final context.
                    vertices[vertexIndex + VERTEX_IDX_BL] = context.Transformation.MultiplyPoint3x4(charInfo.bottomLeft);
                    vertices[vertexIndex + VERTEX_IDX_TL] = context.Transformation.MultiplyPoint3x4(charInfo.topLeft);
                    vertices[vertexIndex + VERTEX_IDX_TR] = context.Transformation.MultiplyPoint3x4(charInfo.topRight);
                    vertices[vertexIndex + VERTEX_IDX_BR] = context.Transformation.MultiplyPoint3x4(charInfo.bottomRight);

                    // Update with the color and opacity received in the final context.
                    var color = context.Color;

                    // Final color is computed with the context's opacity and color.
                    var effectiveOpacity = Mathf.Lerp(0f, color.a / 255f, context.Opacity);
                    color = new Color32(color.r, color.g, color.b, (byte)(effectiveOpacity * 255));

                    vertexColors[vertexIndex + VERTEX_IDX_BL] = color;
                    vertexColors[vertexIndex + VERTEX_IDX_TL] = color;
                    vertexColors[vertexIndex + VERTEX_IDX_TR] = color;
                    vertexColors[vertexIndex + VERTEX_IDX_BR] = color;
                }
            }

            // Update the data.
            textInfo.textComponent.UpdateVertexData();
        }

        /// <summary>
        /// Gets the baseline pivot (Center of each character bound on the baseline) for the given charInfo.
        /// </summary>
        /// <param name="charInfo">The character info.</param>
        /// <returns>The baseline pivot.</returns>
        internal static Vector3 GetBaselinePivot(TMP_CharacterInfo charInfo)
        {
            return new Vector3(
                (charInfo.bottomLeft.x + charInfo.bottomRight.x) * 0.5f,
                charInfo.baseLine,
                (charInfo.bottomLeft.z + charInfo.bottomRight.z) * 0.5f
            );
        }

        /// <summary>
        /// Counts the number of contributing characters.
        /// </summary>
        /// <param name="charInfos">The character infos buffer.</param>
        /// <param name="charCount">The number of characters to check.</param>
        /// <returns>The number of contributing characters.</returns>
        internal static int CountContributingChars(TMP_CharacterInfo[] charInfos, int charCount)
        {
            var count = 0;

            for (int i = 0; i < charCount; i++)
            {
                var charInfo = charInfos[i];

                if (TextFxUtils.IsContributingChar(charInfo.character))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
