# Plateau Curve

Plateau curve defines how effect strength varies across a modifier group's range. It controls the transition from no effect to full effect and back (shaped like a plateau), allowing for smooth fade-in and fade-out zones within the group's effect window.

## Properties

The following section provides a description of each property associated with plateau curves.

### Rise

Defines the point where the curve reaches maximum strength as a fractional position from 0 to 1 based on the size of the effect window. This determines how quickly the effect transitions from zero to full strength at the beginning of the range.

### Fall

Defines the point where the curve starts falling from maximum strength as a fractional position from 0 to 1 based on the size of the effect window. This determines where the effect begins transitioning from full strength back to zero at the end of the range.

### Separate profiles

Enables the use of separate animation curves for rising and falling transitions. When disabled, a single curve profile is used for both transitions.

### Rising profile

The animation curve used for the rise transition when Separate Profiles is enabled. This curve controls the shape of the transition from zero to full effect strength.

### Falling profile

The animation curve used for the fall transition when Separate Profiles is enabled. This curve controls the shape of the transition from full effect strength back to zero. Note that the `x` value used to evaluate the curve goes from `x = 1` to `x = 0` (Right to Left) for falling profiles. This is done intentionally to have a similar behaviour when using a single profile to control both rise and fall.

> [!TIP]
> Even though the rising and falling profiles must adhere to `y = 0 when x = 0` and `y = 1 when x = 1` for a rising and falling transition for the effect range, it is not a hard rule and the warning can be ignored to achieve certain effects.
