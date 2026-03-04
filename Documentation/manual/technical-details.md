# Technical details

This section covers some details of the system which may help you understand how things work under the hood.

## Contributing characters

Whitespace characters do not contribute to the effect (You may have noticed the effect range snapping from character to character while editing). All the other characters are called **Contributing characters**. Each contributing character is assigned an extent of `1 / (Number of contributing characters)`. All the fractional calculations in the system account for this factor. For example, an **offset** has to travel the same amount of length for each character regardless of what their actual width is. This makes sure that the sliding effects have consistent timings for all contributing characters.

Using the fact that each contributing character takes up `1 / (Number of contributing characters)` extent, you can set modifier groups dynamically in the runtime targeting different groups. Some helper methods are provided in **TextFxUtils** class to do these calculations. You may refer to the scripting API of the same for more information.

## Character bounds

When you select a modifier group in the editor, character bounds become visible and serve as guides for positioning the group's pivots. Note that italic text causes these bounds to be skewed for each character. **TextFx** performs all calculations based on the bottom edge of each character—specifically, the midpoint of the bottom border serves as the reference point for determining effect strength and other character-specific properties.

## Pivots

By default, scale and rotation pivots are positioned at the horizontal midpoint of each character's bottom edge and aligned vertically with the baseline. You can adjust these pivot positions through the group's settings.
