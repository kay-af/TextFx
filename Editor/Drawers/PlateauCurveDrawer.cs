using UnityEditor;

using UnityEngine;

namespace TextFx.Editor
{
    /// <summary>
    /// The custom property drawer for the PlateauCurve class.
    /// </summary>
    [CustomPropertyDrawer(typeof(PlateauCurve))]
    public class PlateauCurveDrawer : PropertyDrawer
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

                    if (itr.name == "_risingProfile" || itr.name == "_fallingProfile")
                    {
                        var separateProfilesProp = property.FindPropertyRelative("_separateProfiles");

                        if (itr.name == "_risingProfile")
                        {
                            EditorGUI.PropertyField(
                                new(position.x, y, position.width, height), itr,
                                new GUIContent(
                                    separateProfilesProp.boolValue ? "Rising Profile" : "Profile",
                                    separateProfilesProp.boolValue ?
                                        "Smoothly trasitions the rise using the curve." :
                                        "Smoothly transitions the rise & fall using the curve. Falling profile is evaluated right to left (x = 1 to x = 0)."
                                ),
                                true
                            );

                            y += height + EditorGUIUtility.standardVerticalSpacing;
                        }
                        else if (separateProfilesProp.boolValue)
                        {
                            EditorGUI.PropertyField(new(position.x, y, position.width, height), itr, true);
                            y += height + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }
                    else
                    {
                        EditorGUI.PropertyField(new(position.x, y, position.width, height), itr, true);
                        y += height + EditorGUIUtility.standardVerticalSpacing;
                    }

                    // Show warning after the fall property if the rise exceeds the fall.
                    if (itr.name == "_fall")
                    {
                        var riseProp = property.FindPropertyRelative("_rise");
                        var fallProp = property.FindPropertyRelative("_fall");

                        var riseExceedsFall = riseProp.floatValue > fallProp.floatValue;

                        if (riseExceedsFall)
                        {
                            EditorGUI.HelpBox(
                                EditorGUI.IndentedRect(new(position.x, y, position.width, WARNING_BOX_HEIGHT)),
                                "Rise should not exceed Fall",
                                MessageType.Warning
                            );

                            y += WARNING_BOX_HEIGHT + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }

                    // Show warning after the falling profile property if the profiles are not smooth.
                    if (itr.name == "_fallingProfile")
                    {
                        if (IsRoughProfile(property))
                        {
                            EditorGUI.HelpBox(
                                EditorGUI.IndentedRect(new(position.x, y, position.width, WARNING_BOX_HEIGHT)),
                                "Profiles must evaluate to y = 0 at x = 0 and y = 1 at x = 1 for a smooth transition",
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
                // Adjust the height for the falling profile property.
                if (itr.name == "_fallingProfile")
                {
                    var separateProfilesProp = property.FindPropertyRelative("_separateProfiles");
                    if (separateProfilesProp.boolValue)
                    {
                        totalHeight += EditorGUI.GetPropertyHeight(itr, true) + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                else
                {
                    totalHeight += EditorGUI.GetPropertyHeight(itr, true) + EditorGUIUtility.standardVerticalSpacing;
                }

                // Adjust the height for the warning boxes.
                if (itr.name == "_fall")
                {
                    var riseProp = property.FindPropertyRelative("_rise");
                    var fallProp = property.FindPropertyRelative("_fall");
                    var riseExceedsFall = riseProp.floatValue > fallProp.floatValue;

                    if (riseExceedsFall)
                    {
                        totalHeight += WARNING_BOX_HEIGHT + EditorGUIUtility.standardVerticalSpacing;
                    }
                }

                if (itr.name == "_fallingProfile")
                {
                    if (IsRoughProfile(property))
                    {
                        totalHeight += WARNING_BOX_HEIGHT + EditorGUIUtility.standardVerticalSpacing;
                    }
                }

                enterChildren = false;
            }

            return totalHeight;
        }

        // Tells whether the profiles are rough and do not form a smooth plateau.
        private static bool IsRoughProfile(SerializedProperty rootProperty)
        {
            // Time 0 must evaluate approximately to 0 for both profiles.
            // Time 1 must evaluate approximately to 1 for both profiles.
            var separateProfilesProp = rootProperty.FindPropertyRelative("_separateProfiles");
            var risingProfileProp = rootProperty.FindPropertyRelative("_risingProfile");
            var fallingProfileProp = rootProperty.FindPropertyRelative("_fallingProfile");

            var risingProfileAtZero = risingProfileProp.animationCurveValue.Evaluate(0f);
            var risingProfileAtOne = risingProfileProp.animationCurveValue.Evaluate(1f);
            var isInvalidRisingProfile = !Mathf.Approximately(risingProfileAtZero, 0f) || !Mathf.Approximately(risingProfileAtOne, 1f);

            var fallingProfileAtZero = fallingProfileProp.animationCurveValue.Evaluate(0f);
            var fallingProfileAtOne = fallingProfileProp.animationCurveValue.Evaluate(1f);
            var isInvalidFallingProfile = !Mathf.Approximately(fallingProfileAtZero, 0f) || !Mathf.Approximately(fallingProfileAtOne, 1f);

            var isInvalidProfile = separateProfilesProp.boolValue ? (isInvalidRisingProfile || isInvalidFallingProfile) : isInvalidRisingProfile;

            return isInvalidProfile;
        }
    }
}