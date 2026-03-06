using System.Linq;

using TMPro;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace TextFx.Editor
{
    /// <summary>
    /// The custom editor scripts for the TextFx controllers.
    /// </summary>
    [CustomEditor(typeof(TextFxController), true)]
    public class TextFxControllerEditor : UnityEditor.Editor
    {
        #region Handle Visual Constants

        // Inspector UI Colors
        private static readonly Color GROUP_SELECTED_BG_COLOR = new(0.1f, 0.35f, 0.1f, 0.15f);

        // Scene Handle Colors
        private static readonly Color MASK_RANGE_BOX_COLOR = new(1f, 1f, 1f, 0.1f);
        private static readonly Color EFFECT_RANGE_BOX_COLOR = new(0.65f, 0.65f, 1f, 0.2f);
        private static readonly Color CHAR_BOUNDS_COLOR = new(1f, 1f, 1f, 0.4f);
        private static readonly Color PIVOT_OUTER_COLOR = new(0.1f, 0.1f, 0.1f, 1f);
        private static readonly Color PIVOT_INNER_COLOR = new(1f, 1f, 1f, 1f);

        // Handle Sizes and Dimensions
        private const float BOUNDS_DOTTED_LINE_SIZE = 1.25f;
        private const float PIVOT_RADIUS_MULTIPLIER = 0.065f;
        private const float PIVOT_INNER_RADIUS_MULTIPLIER = 0.75f;

        #endregion

        // Properties of the target.
        private SerializedProperty _autoUpdateProp;
        private SerializedProperty _rightToLeftProp;
        private SerializedProperty _strengthProp;
        private SerializedProperty _offsetProp;
        private SerializedProperty _modifierGroupsProp;
        private ReorderableList _modifierGroupsList;

        private void OnEnable()
        {
            _autoUpdateProp = serializedObject.FindProperty("_autoUpdate");
            _rightToLeftProp = serializedObject.FindProperty("_rightToLeft");
            _strengthProp = serializedObject.FindProperty("_strength");
            _offsetProp = serializedObject.FindProperty("_offset");
            _modifierGroupsProp = serializedObject.FindProperty("_modifierGroups");

            // Configure the modifier groups list.
            _modifierGroupsList = new(serializedObject, _modifierGroupsProp, true, false, true, true)
            {
                multiSelect = false,

                drawElementBackgroundCallback = DrawGroupBackground,
                drawElementCallback = DrawModifierGroup,
                elementHeightCallback = GetModifierGroupHeight,
                onAddCallback = OnAddModifierGroup,

                onSelectCallback = OnSelectModifierGroup,
            };
        }

        private void DrawGroupBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isActive || isFocused)
            {
                EditorGUI.DrawRect(rect, GROUP_SELECTED_BG_COLOR);
            }
        }

        private void DrawModifierGroup(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(rect, _modifierGroupsProp.GetArrayElementAtIndex(index), new GUIContent($"Group ({index})"), true);

            EditorGUI.indentLevel--;
        }

        private float GetModifierGroupHeight(int index)
        {
            return EditorGUI.GetPropertyHeight(_modifierGroupsProp.GetArrayElementAtIndex(index), true);
        }

        private void OnAddModifierGroup(ReorderableList list)
        {
            // Unity resets all the properties of a new item added to a list to their default values.
            // So, we need to set the properties of the new item to their default values.

            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;

            var group = list.serializedProperty.GetArrayElementAtIndex(index);
            group.FindPropertyRelative("_strength").floatValue = 1f;
            group.FindPropertyRelative("_offset").floatValue = 0f;
            group.FindPropertyRelative("_pivot").vector3Value = Vector3.zero;
            group.FindPropertyRelative("_extent").floatValue = 1f;

            var curveProp = group.FindPropertyRelative("_curve");
            curveProp.FindPropertyRelative("_rise").floatValue = 0.25f;
            curveProp.FindPropertyRelative("_fall").floatValue = 0.75f;
            curveProp.FindPropertyRelative("_separateProfiles").boolValue = false;
            curveProp.FindPropertyRelative("_risingProfile").animationCurveValue = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            curveProp.FindPropertyRelative("_fallingProfile").animationCurveValue = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

            var maskProp = group.FindPropertyRelative("_mask");
            maskProp.FindPropertyRelative("_start").floatValue = 0f;
            maskProp.FindPropertyRelative("_end").floatValue = 1f;

            var positionModifierProp = group.FindPropertyRelative("_positionModifier");
            positionModifierProp.FindPropertyRelative("_enabled").boolValue = false;
            positionModifierProp.FindPropertyRelative("_invert").boolValue = false;
            positionModifierProp.FindPropertyRelative("_position").vector3Value = Vector3.zero;

            var rotationModifierProp = group.FindPropertyRelative("_rotationModifier");
            rotationModifierProp.FindPropertyRelative("_enabled").boolValue = false;
            rotationModifierProp.FindPropertyRelative("_invert").boolValue = false;
            rotationModifierProp.FindPropertyRelative("_rotation").vector3Value = Vector3.zero;

            var scaleModifierProp = group.FindPropertyRelative("_scaleModifier");
            scaleModifierProp.FindPropertyRelative("_enabled").boolValue = false;
            scaleModifierProp.FindPropertyRelative("_invert").boolValue = false;
            scaleModifierProp.FindPropertyRelative("_scale").vector3Value = Vector3.one;

            var colorProp = group.FindPropertyRelative("_colorModifier");
            colorProp.FindPropertyRelative("_enabled").boolValue = false;
            colorProp.FindPropertyRelative("_invert").boolValue = false;
            colorProp.FindPropertyRelative("_color").colorValue = Color.gray;

            var opacityProp = group.FindPropertyRelative("_opacityModifier");
            opacityProp.FindPropertyRelative("_enabled").boolValue = false;
            opacityProp.FindPropertyRelative("_invert").boolValue = false;
            opacityProp.FindPropertyRelative("_opacity").floatValue = 1f;
        }

        /// Repaints when a group is selected in the inspector.
        private void OnSelectModifierGroup(ReorderableList _) => SceneView.RepaintAll();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_autoUpdateProp);
            EditorGUILayout.PropertyField(_rightToLeftProp);
            EditorGUILayout.PropertyField(_strengthProp);
            EditorGUILayout.PropertyField(_offsetProp);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Modifier Groups", EditorStyles.boldLabel);

            _modifierGroupsList.DoLayoutList();

            if (serializedObject.ApplyModifiedProperties())
            {
                // Update mesh for the targets for which auto update is false.
                foreach (var target in targets)
                {
                    if (target is TextFxController controller)
                    {
                        if (!controller.AutoUpdate) {
                            controller.UpdateMesh();
                        }
                    }
                }
            }
        }

        // Draws the scene handles.
        private void OnSceneGUI()
        {
            // Do nothing if there are multiple targets.
            if (targets.Length != 1)
            {
                return;
            }

            // Do nothing if the target is not active and enabled.
            var controller = target as TextFxController;

            if (controller == null)
            {
                return;
            }

            if (!controller.isActiveAndEnabled)
            {
                return;
            }

            // Do nothing if no group is selected.
            if (_modifierGroupsList.selectedIndices.Count != 1)
            {
                return;
            }

            var selectedGroupIndex = _modifierGroupsList.selectedIndices.First();
            var selectedGroup = controller.GetModifierGroupAt(selectedGroupIndex);

            var textTransform = controller._textTransform;
            var lineInfos = controller._lineInfos;
            var meshInfos = controller._meshInfos;
            var charInfos = controller._charInfos;

            if (textTransform == null)
            {
                return;
            }

            // Draw the dotted lines representing the resting bounds of the affected characters.
            DrawCharBounds(
                textTransform,
                meshInfos,
                lineInfos,
                charInfos,
                selectedGroup,
                controller.RightToLeft
            );

            // Draw the mask range box.
            DrawRangeBox(
                selectedGroup.Mask.Start,
                selectedGroup.Mask.End,
                textTransform,
                lineInfos,
                charInfos,
                MASK_RANGE_BOX_COLOR,
                controller.RightToLeft
            );

            // Draw the adjusted offset range box. Only draw the box inside the mask range.
            var adjustedOffsetStart = controller.Offset + selectedGroup.Offset;
            var adjustedOffsetEnd = controller.Offset + selectedGroup.Offset + selectedGroup.Extent;

            DrawRangeBox(
                Mathf.Max(adjustedOffsetStart, selectedGroup.Mask.Start),
                Mathf.Min(adjustedOffsetEnd, selectedGroup.Mask.End),
                textTransform,
                lineInfos,
                charInfos,
                EFFECT_RANGE_BOX_COLOR,
                controller.RightToLeft
            );

            // Draw the curve range box. Only draw the box inside the mask range.
            DrawRangeBox(
                Mathf.Max(Mathf.Lerp(adjustedOffsetStart, adjustedOffsetEnd, selectedGroup.Curve.Rise), selectedGroup.Mask.Start),
                Mathf.Min(Mathf.Lerp(adjustedOffsetStart, adjustedOffsetEnd, selectedGroup.Curve.Fall), selectedGroup.Mask.End),
                textTransform,
                lineInfos,
                charInfos,
                EFFECT_RANGE_BOX_COLOR,
                controller.RightToLeft
            );

            // Draw the pivots for each character.
            DrawPivots(
                textTransform,
                lineInfos,
                charInfos,
                selectedGroup,
                controller.RightToLeft
            );
        }

        private static void DrawCharBounds(Transform textTransform, TMP_MeshInfo[] meshInfos, TMP_LineInfo[] lineInfos, TMP_CharacterInfo[] charInfos, ModifierGroup selectedGroup, bool rightToLeft)
        {
            // The charInfos buffer for the editor has the actual character count unlike the runtime where TextMeshPro does some optimizations.
            var totalContributingChars = TextFxController.CountContributingChars(charInfos, charInfos.Length);

            if (totalContributingChars == 0)
            {
                return;
            }

            var extentPerChar = 1f / totalContributingChars;

            var contributingCharsSoFar = 0;

            for (var i = 0; i < lineInfos.Length; i++)
            {
                var lineInfo = lineInfos[i];

                var fromCharIdx = rightToLeft ? lineInfo.lastCharacterIndex : lineInfo.firstCharacterIndex;
                var toCharIdx = rightToLeft ? lineInfo.firstCharacterIndex : lineInfo.lastCharacterIndex;
                var deltaIdx = rightToLeft ? -1 : 1;

                for (var j = fromCharIdx; j != toCharIdx + deltaIdx; j += deltaIdx)
                {
                    var charInfo = charInfos[j];

                    if (!TextFxUtils.IsContributingChar(charInfo.character))
                    {
                        continue;
                    }

                    contributingCharsSoFar++;

                    var vertexIndex = charInfo.vertexIndex;
                    var materialIndex = charInfo.materialReferenceIndex;
                    var meshInfo = meshInfos[materialIndex];

                    var vertices = meshInfo.vertices;

                    var bottomLeft = textTransform.TransformPoint(vertices[vertexIndex + TextFxController.VERTEX_IDX_BL]);
                    var topLeft = textTransform.TransformPoint(vertices[vertexIndex + TextFxController.VERTEX_IDX_TL]);
                    var topRight = textTransform.TransformPoint(vertices[vertexIndex + TextFxController.VERTEX_IDX_TR]);
                    var bottomRight = textTransform.TransformPoint(vertices[vertexIndex + TextFxController.VERTEX_IDX_BR]);

                    var charOffset = extentPerChar * (contributingCharsSoFar - 0.5f);

                    // Only draw the bounds inside the mask range.
                    if (charOffset < selectedGroup.Mask.Start)
                    {
                        continue;
                    }
                    else if (charOffset > selectedGroup.Mask.End)
                    {
                        return;
                    }

                    // Draw the bounds.
                    Handles.color = CHAR_BOUNDS_COLOR;
                    Handles.DrawDottedLines(
                        new Vector3[] {
                        bottomLeft, topLeft,
                        topLeft, topRight,
                        topRight, bottomRight,
                        bottomRight, bottomLeft,
                        },
                        BOUNDS_DOTTED_LINE_SIZE
                    );
                }
            }
        }

        private static void DrawPivots(Transform textTransform, TMP_LineInfo[] lineInfos, TMP_CharacterInfo[] charInfos, ModifierGroup selectedGroup, bool rightToLeft)
        {
            var totalContributingChars = TextFxController.CountContributingChars(charInfos, charInfos.Length);

            if (totalContributingChars == 0)
            {
                return;
            }

            var extentPerChar = 1f / totalContributingChars;

            var contributingCharsSoFar = 0;

            for (var i = 0; i < lineInfos.Length; i++)
            {
                var lineInfo = lineInfos[i];

                var fromCharIdx = rightToLeft ? lineInfo.lastCharacterIndex : lineInfo.firstCharacterIndex;
                var toCharIdx = rightToLeft ? lineInfo.firstCharacterIndex : lineInfo.lastCharacterIndex;
                var deltaIdx = rightToLeft ? -1 : 1;

                for (var j = fromCharIdx; j != toCharIdx + deltaIdx; j += deltaIdx)
                {
                    var charInfo = charInfos[j];

                    if (!TextFxUtils.IsContributingChar(charInfo.character))
                    {
                        continue;
                    }

                    contributingCharsSoFar++;

                    var pivot = textTransform.TransformPoint(
                        TextFxController.GetBaselinePivot(charInfo) +
                        selectedGroup.Pivot
                    );

                    var charOffset = extentPerChar * (contributingCharsSoFar - 0.5f);

                    // Only draw the pivots inside the mask range.
                    if (charOffset < selectedGroup.Mask.Start)
                    {
                        continue;
                    }
                    else if (charOffset > selectedGroup.Mask.End)
                    {
                        return;
                    }

                    // Point the disc towards the scene camera.
                    var sceneCamera = SceneView.currentDrawingSceneView.camera;

                    var discNormal = Vector3.back;
                    if (sceneCamera != null)
                    {
                        discNormal = -sceneCamera.transform.forward;
                    }

                    // Draw the pivots.
                    var outerRadius = HandleUtility.GetHandleSize(pivot) * PIVOT_RADIUS_MULTIPLIER;
                    Handles.color = PIVOT_OUTER_COLOR;
                    Handles.DrawSolidDisc(pivot, discNormal, outerRadius);
                    Handles.color = PIVOT_INNER_COLOR;
                    Handles.DrawSolidDisc(pivot, discNormal, outerRadius * PIVOT_INNER_RADIUS_MULTIPLIER);
                }
            }
        }

        // This method takes a range [0,1] and draws a box inside the text's bounds respecting each
        // contributing character's extent.
        private static void DrawRangeBox(
            float from,
            float to,
            Transform textTransform,
            TMP_LineInfo[] lineInfos,
            TMP_CharacterInfo[] charInfos,
            Color color,
            bool rightToLeft
        )
        {
            // Do nothing if the box is outside the range [0,1].
            if (from > 1 || to < 0)
            {
                return;
            }

            // Do nothing if the box is empty.
            if (from > to)
            {
                return;
            }

            // Do nothing if the text has no lines or characters.
            if (lineInfos.Length == 0 || charInfos.Length == 0)
            {
                return;
            }

            var totalContributingChars = TextFxController.CountContributingChars(charInfos, charInfos.Length);

            if (totalContributingChars == 0)
            {
                return;
            }

            // Set the color of the handles and start drawing logic.
            Handles.color = color;

            // Edge case. If both are 1, we want a thin line at the end of the last contributing character.
            if (from == 1 && to == 1)
            {
                // Find the last contributing line.
                var lastContribLineIdx = FindLastContribLineIdx(lineInfos, charInfos);
                if (lastContribLineIdx == -1)
                {
                    return;
                }

                // Find the last contributing character in the line with RTL support.
                var lastContribCharIdx = FindLastContribCharIdxInLine(lineInfos[lastContribLineIdx], charInfos, rightToLeft);
                if (lastContribCharIdx == -1)
                {
                    return;
                }

                // Draw the box.
                var charInfo = charInfos[lastContribCharIdx];
                var lineInfo = lineInfos[charInfo.lineNumber];

                Handles.DrawAAConvexPolygon(
                    textTransform.TransformPoint(GetBottomEndX(charInfo, rightToLeft), lineInfo.descender, 0f),
                    textTransform.TransformPoint(GetBottomEndX(charInfo, rightToLeft), lineInfo.ascender, 0f),
                    textTransform.TransformPoint(GetBottomEndX(charInfo, rightToLeft), lineInfo.descender, 0f),
                    textTransform.TransformPoint(GetBottomEndX(charInfo, rightToLeft), lineInfo.ascender, 0f)
                );
            }

            // Clamp the range to [0,1].
            from = Mathf.Max(from, 0f);
            to = Mathf.Min(to, 1f);

            // Calculate the extent per character.
            var extentPerChar = 1f / totalContributingChars;
            var contributingCharsSoFar = 0;
            var startFound = false;

            for (var lineIdx = 0; lineIdx < lineInfos.Length; lineIdx++)
            {
                var lineInfo = lineInfos[lineIdx];

                var fromIdx = rightToLeft ? lineInfo.lastCharacterIndex : lineInfo.firstCharacterIndex;
                var toIdx = rightToLeft ? lineInfo.firstCharacterIndex : lineInfo.lastCharacterIndex;
                var deltaIdx = rightToLeft ? -1 : 1;

                // If the start has not been found yet, we need to find it.
                if (!startFound)
                {
                    // Iterate over the characters in the line.
                    for (var i = fromIdx; i != toIdx + deltaIdx; i += deltaIdx)
                    {
                        var startCharInfo = charInfos[i];

                        // If the character does not contribute to the calculations, skip it.
                        if (!TextFxUtils.IsContributingChar(startCharInfo.character))
                        {
                            continue;
                        }

                        // Increment the counter of contributing characters so far.
                        contributingCharsSoFar++;

                        var startCharStartOffset = extentPerChar * (contributingCharsSoFar - 1);
                        var startCharEndOffset = extentPerChar * contributingCharsSoFar;

                        // If the start is found, we need to draw the box.
                        if (from >= startCharStartOffset && from < startCharEndOffset)
                        {
                            startFound = true;

                            var startFactor = Mathf.InverseLerp(startCharStartOffset, startCharEndOffset, from);
                            var startX = Mathf.Lerp(GetBottomStartX(startCharInfo, rightToLeft), GetBottomEndX(startCharInfo, rightToLeft), startFactor);

                            // Decrement the counter of contributing characters so far as the i'th character
                            // itself can mark the end.
                            contributingCharsSoFar--;

                            var lastContribCharIdx = FindLastContribCharIdxInLine(lineInfo, charInfos, rightToLeft);

                            // Find the end on this line.
                            for (int j = i; j != lastContribCharIdx + deltaIdx; j += deltaIdx)
                            {
                                var endCharInfo = charInfos[j];

                                if (!TextFxUtils.IsContributingChar(endCharInfo.character))
                                {
                                    continue;
                                }

                                contributingCharsSoFar++;

                                var endCharStartOffset = extentPerChar * (contributingCharsSoFar - 1);
                                var endCharEndOffset = extentPerChar * contributingCharsSoFar;

                                if (to >= endCharStartOffset && to < endCharEndOffset)
                                {
                                    var endFactor = Mathf.InverseLerp(endCharStartOffset, endCharEndOffset, to);
                                    var endX = Mathf.Lerp(GetBottomStartX(endCharInfo, rightToLeft), GetBottomEndX(endCharInfo, rightToLeft), endFactor);

                                    Handles.DrawAAConvexPolygon(
                                        textTransform.TransformPoint(startX, lineInfo.descender, 0f),
                                        textTransform.TransformPoint(startX, lineInfo.ascender, 0f),
                                        textTransform.TransformPoint(endX, lineInfo.ascender, 0f),
                                        textTransform.TransformPoint(endX, lineInfo.descender, 0f)
                                    );

                                    return;
                                }

                                // The end was not found but the box must be drawn with the last character.
                                if (j == lastContribCharIdx)
                                {
                                    var endX = GetBottomEndX(endCharInfo, rightToLeft);

                                    Handles.DrawAAConvexPolygon(
                                        textTransform.TransformPoint(startX, lineInfo.descender, 0f),
                                        textTransform.TransformPoint(startX, lineInfo.ascender, 0f),
                                        textTransform.TransformPoint(endX, lineInfo.ascender, 0f),
                                        textTransform.TransformPoint(endX, lineInfo.descender, 0f)
                                    );
                                }
                            }

                            break;
                        }
                    }
                }
                else
                {
                    // If the start has already been found, we need to find the end.
                    var startX = float.NegativeInfinity;
                    var lastContribCharIndex = FindLastContribCharIdxInLine(lineInfo, charInfos, rightToLeft);

                    // Iterate over the characters in the line.
                    for (var i = fromIdx; i != lastContribCharIndex + deltaIdx; i += deltaIdx)
                    {
                        var endCharInfo = charInfos[i];

                        // If the character does not contribute to the calculations, skip it.
                        if (!TextFxUtils.IsContributingChar(endCharInfo.character))
                        {
                            continue;
                        }

                        // Increment the counter of contributing characters so far.
                        contributingCharsSoFar++;

                        var endCharStartOffset = extentPerChar * (contributingCharsSoFar - 1);
                        var endCharEndOffset = extentPerChar * contributingCharsSoFar;

                        // If the start has not been assigned yet, the first contributing character's starting
                        // bound is the startX.
                        if (startX == float.NegativeInfinity)
                        {
                            startX = GetBottomStartX(endCharInfo, rightToLeft);
                        }

                        // If the end is found, we need to draw the box.
                        if (to >= endCharStartOffset && to < endCharEndOffset)
                        {
                            var endFactor = Mathf.InverseLerp(endCharStartOffset, endCharEndOffset, to);
                            var endX = Mathf.Lerp(GetBottomStartX(endCharInfo, rightToLeft), GetBottomEndX(endCharInfo, rightToLeft), endFactor);

                            Handles.DrawAAConvexPolygon(
                                textTransform.TransformPoint(startX, lineInfo.descender, 0f),
                                textTransform.TransformPoint(startX, lineInfo.ascender, 0f),
                                textTransform.TransformPoint(endX, lineInfo.ascender, 0f),
                                textTransform.TransformPoint(endX, lineInfo.descender, 0f)
                            );

                            return;
                        }

                        // The end was not found but the box must be drawn with the last character.
                        if (i == lastContribCharIndex)
                        {
                            var endX = GetBottomEndX(endCharInfo, rightToLeft);

                            Handles.DrawAAConvexPolygon(
                                textTransform.TransformPoint(startX, lineInfo.descender, 0f),
                                textTransform.TransformPoint(startX, lineInfo.ascender, 0f),
                                textTransform.TransformPoint(endX, lineInfo.ascender, 0f),
                                textTransform.TransformPoint(endX, lineInfo.descender, 0f)
                            );
                        }
                    }
                }
            }
        }

        // Gets the bottom start x coordinate of the character with RTL support.
        private static float GetBottomStartX(TMP_CharacterInfo charInfo, bool rightToLeft)
        {
            if (rightToLeft)
            {
                return charInfo.bottomRight.x;
            }

            return charInfo.bottomLeft.x;
        }

        // Gets the bottom end x coordinate of the character with RTL support.
        private static float GetBottomEndX(TMP_CharacterInfo charInfo, bool rightToLeft)
        {
            if (rightToLeft)
            {
                return charInfo.bottomLeft.x;
            }

            return charInfo.bottomRight.x;
        }

        // Finds the last line of the text that has a contributing character.
        private static int FindLastContribLineIdx(TMP_LineInfo[] lineInfos, TMP_CharacterInfo[] charInfos)
        {
            // Iterate in reverse.
            for (var i = lineInfos.Length - 1; i >= 0; i--)
            {
                var lineInfo = lineInfos[i];

                // Find any contributing character in the line.
                for (var j = lineInfo.firstCharacterIndex; j <= lineInfo.lastCharacterIndex; j++)
                {
                    var charInfo = charInfos[j];

                    // If found, return the index of the line.
                    if (TextFxUtils.IsContributingChar(charInfo.character))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        // Finds the last contributing character in the line with RTL support.
        private static int FindLastContribCharIdxInLine(TMP_LineInfo lineInfo, TMP_CharacterInfo[] charInfos, bool rightToLeft)
        {
            // Adjust indices for RTL support.
            var fromCharIdx = rightToLeft ? lineInfo.firstCharacterIndex : lineInfo.lastCharacterIndex;
            var toCharIdx = rightToLeft ? lineInfo.lastCharacterIndex : lineInfo.firstCharacterIndex;
            var deltaIdx = rightToLeft ? 1 : -1;

            // Iterate over the characters in the range.
            for (var i = fromCharIdx; i != toCharIdx + deltaIdx; i += deltaIdx)
            {
                // If the character contributes to the calculations, return its index.
                if (TextFxUtils.IsContributingChar(charInfos[i].character))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}