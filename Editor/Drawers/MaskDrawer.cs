using UnityEditor;

using UnityEngine;

namespace TextFx.Editor
{
    /// <summary>
    /// The custom property drawer for the Mask class.
    /// </summary>
    [CustomPropertyDrawer(typeof(Mask))]
    public class MaskDrawer : PropertyDrawer
    {
        // Height of the warning box shown when the inspector has invalid values.
        private const float WARNING_BOX_HEIGHT = 40f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(
                new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                label,
                true
            );

            // Automatically iterate over and draw the default inspector.
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var itr = property.Copy();
                var end = itr.GetEndProperty();

                var y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var enterChildren = true;
                while (itr.NextVisible(enterChildren) && !SerializedProperty.EqualContents(itr, end))
                {
                    var height = EditorGUI.GetPropertyHeight(itr, true);
                    EditorGUI.PropertyField(new(position.x, y, position.width, height), itr, true);
                    y += height + EditorGUIUtility.standardVerticalSpacing;

                    // Show warning after the end property if the values are invalid.
                    if (itr.name == "_end")
                    {
                        var startProp = property.FindPropertyRelative("_start");
                        var endProp = property.FindPropertyRelative("_end");

                        var startExceedsEnd = startProp.floatValue > endProp.floatValue;

                        if (startExceedsEnd)
                        {
                            EditorGUI.HelpBox(
                                EditorGUI.IndentedRect(new(position.x, y, position.width, WARNING_BOX_HEIGHT)),
                                "Start should not exceed End",
                                MessageType.Warning
                            );

                            y += WARNING_BOX_HEIGHT + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }

                    enterChildren = false;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var itr = property.Copy();
            var end = itr.GetEndProperty();

            var enterChildren = true;
            while (itr.NextVisible(enterChildren) && !SerializedProperty.EqualContents(itr, end))
            {
                totalHeight += EditorGUI.GetPropertyHeight(itr, true) + EditorGUIUtility.standardVerticalSpacing;

                // Adjust the height for the warning box.
                if (itr.name == "_end")
                {
                    var startProp = property.FindPropertyRelative("_start");
                    var endProp = property.FindPropertyRelative("_end");

                    var startExceedsEnd = startProp.floatValue > endProp.floatValue;

                    if (startExceedsEnd)
                    {
                        totalHeight += WARNING_BOX_HEIGHT + EditorGUIUtility.standardVerticalSpacing;
                    }
                }

                enterChildren = false;
            }

            return totalHeight;
        }
    }
}
