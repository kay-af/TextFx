using UnityEditor;

using UnityEngine;

namespace TextFx.Editor
{
    /// <summary>
    /// The custom property drawer for the BaseModifier class.
    /// </summary>
    [CustomPropertyDrawer(typeof(BaseModifier), true)]
    public class ModifierDrawer : PropertyDrawer
    {
        // Padding of the box containing the modifier properties.
        private const float BOX_PADDING = 8f;

        // Color of the inactive box.
        private Color BOX_INACTIVE_BG_COLOR = new(0.5f, 0.5f, 0.5f, 0.15f);

        // Color of the active box.
        private Color BOX_ACTIVE_BG_COLOR = new(0.1f, 0.5f, 0.1f, 0.15f);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var enabledProperty = property.FindPropertyRelative("_enabled");

            EditorGUI.DrawRect(
                EditorGUI.IndentedRect(
                    new(position.x, position.y, position.width, position.height)
                ),
                enabledProperty.boolValue ? BOX_ACTIVE_BG_COLOR : BOX_INACTIVE_BG_COLOR
            );

            var boxRect = new Rect(position.x + BOX_PADDING, position.y + BOX_PADDING, position.width - BOX_PADDING * 2, position.height - BOX_PADDING * 2);

            // Draw the enabled property to act like a foldout group.
            EditorGUI.PropertyField(new(boxRect.x, boxRect.y, 20, EditorGUIUtility.singleLineHeight), enabledProperty, GUIContent.none);

            EditorGUI.LabelField(new(boxRect.x + 20, boxRect.y, boxRect.width - 20, EditorGUIUtility.singleLineHeight), label);

            if (enabledProperty.boolValue)
            {
                var y = boxRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var childProp = property.Copy();
                var endProp = property.GetEndProperty();

                var enterChildren = true;
                while (childProp.NextVisible(enterChildren) && !SerializedProperty.EqualContents(childProp, endProp))
                {
                    // Draw all the other properties except enabled property.
                    if (childProp.name == "_enabled")
                    {
                        enterChildren = false;
                        continue;
                    }

                    var childHeight = EditorGUI.GetPropertyHeight(childProp, true);
                    EditorGUI.PropertyField(new(boxRect.x, y, boxRect.width, childHeight), childProp, true);
                    y += childHeight + EditorGUIUtility.standardVerticalSpacing;

                    enterChildren = false;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight + BOX_PADDING * 2;
            var enabledProperty = property.FindPropertyRelative("_enabled");
            if (enabledProperty.boolValue)
            {
                var childProperty = property.Copy();
                var endProperty = property.GetEndProperty();

                var enterChildren = true;
                while (childProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(childProperty, endProperty))
                {
                    // Adjust height for the custom enabled layout.
                    if (childProperty.name == "_enabled")
                    {
                        enterChildren = false;
                        continue;
                    }

                    height += EditorGUI.GetPropertyHeight(childProperty, null) + EditorGUIUtility.standardVerticalSpacing;
                    enterChildren = false;
                }
            }

            return height;
        }
    }
}