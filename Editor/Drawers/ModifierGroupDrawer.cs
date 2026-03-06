using UnityEditor;

using UnityEngine;

namespace TextFx.Editor
{
    /// <summary>
    /// The custom property drawer for the ModifierGroup class.
    /// </summary>
    [CustomPropertyDrawer(typeof(ModifierGroup))]
    public class ModifierGroupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(
                new(position.x, position.y + 1, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                label,
                true
            );

            // Automatically draw the default inspector.
            if (property.isExpanded)
            {
                var itr = property.Copy();
                var end = itr.GetEndProperty();

                var y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var enterChildren = true;
                while (itr.NextVisible(enterChildren) && !SerializedProperty.EqualContents(itr, end))
                {
                    // Draw the group settings label before the first property i.e. strength.
                    if (itr.name == "_strength")
                    {
                        y += 8;

                        EditorGUI.LabelField(
                            new(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Group Settings"),
                            EditorStyles.boldLabel
                        );

                        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }

                    var height = EditorGUI.GetPropertyHeight(itr, true);

                    EditorGUI.PropertyField(new(position.x, y, position.width, height), itr, true);

                    y += height + EditorGUIUtility.standardVerticalSpacing;

                    // Draw the modifiers label before rendering the modifier properties.
                    if (itr.name == "_mask")
                    {
                        y += 8;

                        EditorGUI.LabelField(
                            new(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Modifiers"),
                            EditorStyles.boldLabel
                        );

                        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }

                    enterChildren = false;
                }
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
                // Adjust the height for the group settings label.
                if (itr.name == "_strength")
                {
                    totalHeight += 8f;
                    totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                // Adjust the height for the modifiers label.
                if (itr.name == "_mask")
                {
                    totalHeight += 8f;
                    totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                totalHeight += EditorGUI.GetPropertyHeight(itr, true) + EditorGUIUtility.standardVerticalSpacing;
                enterChildren = false;
            }

            return totalHeight;
        }
    }
}