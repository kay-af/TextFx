# TextFx Controller

**TextFxController** is the main component of the system. It exposes various parameters that can be used to control the behaviour of a text.

## Properties

The following section provides a comprehensive description of each property associated with **TextFxController**.

### Enabled

The controller only applies effects while the component is enabled. Disabling the component removes all applied effects.

### Auto Update

When enabled, the text mesh updates automatically every frame. If your controller's values aren't changing between frames, this can be inefficient. Disabling this option requires you to manually call `TextFxController.UpdateMesh` in play mode whenever you modify the controller, giving you precise control over mesh updates and better performance optimization. Outside play mode, this setting is ignored.

### Right To Left

Enables _Right To Left_ mode. In this mode, all properties work in right-to-left orientation without changing their behavior. Multiple lines are also supported.

> [!NOTE]
> When **Right To Left** is enabled, calculations for every line starts from the right hand side. This means that an **offset** of **0** starts at the right hand side instead of the left for every line, extent grows towards left, curves rise from right, etc.

### Strength

The global strength of all modifier groups applied to the text. This property provides a single control to adjust the intensity of all modifier groups.

The strength is a fractional value in range [0,1].

### Offset

Adds a global offset to all the local offsets of the modifier groups. Offset controls the starting point of the effect range of each group.

### Modifier Groups

The list of modifier groups that provide several modifiers and settings to control the range of characters affected. Modifier groups are additive i.e., the effects sum up for overlapping groups. Check [Modifier Groups](/manual/concepts/modifier-groups.html) for more information.
