# Modifier Groups

Modifier groups are the building blocks of effects. They encapsulate a collection of modifiers and define how effects are applied to specific ranges of the text. They are additive i.e., the effects of the overlapping groups are added up.

## Properties

The following section provides a comprehensive description of each property associated with modifier groups.

### Strength

Controls the overall intensity of all effects in the group. The value is in range [0,1].

### Offset

Offset controls the starting point of the effect range. The effect range defines how strength is applied on each character. Paired with the [curve](#curve) property, you can define how the effects transition when sliding the range.

### Extent

Defines how much of the text the group affects as a fractional width. The effect range spans from **Offset** to **Offset + Extent**.

### Pivot

Shifts the transformation pivot for rotation and scaling effects for a group. The default pivot is at each character's baseline, and the pivot shift is applied to all characters in the group.

> [!TIP]
> When a group is selected, the character bounds and the pivot positions are drawn in the scene view.

### Curve

Defines how effect strength varies across the group's effect range using a [plateau curve](/manual/concepts/plateau-curve.html). The curve controls the transition from no effect to full effect and back, allowing for smooth fade-in and fade-out zones.

Check [plateau curve](/manual/concepts/plateau-curve.html) for an extensive overview.

### Mask

The mask defines a range that limits which characters are affected. The value is fractional and is in range [0,1].

Check [Masks](/manual/concepts/masks.html) for an extensive overview.

### Modifiers

The list of modifiers provides various transformation and visual effects that can be applied to characters within the group's range. Each modifier group contains built-in modifiers that are applied in a specific order for predictable results. See [Modifiers](/manual/concepts/modifiers.html) for more information.
