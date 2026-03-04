# Usage

Learn the basics of working with **TextFx**.

## Quickstart

The main component of **TextFx** is the **TextFxController**. Add it to an existing _TextMeshPro_ or _TextMeshProUGUI_ game object to enable dynamic effects on the text.

To add the **TextFxController**, select the game object and add the component via the inspector.

**Add Component > TextFx > TextFxController**.

## Configuring Modifiers

A group encapsulates a set of properties that affect a range of characters. To add a new group, click the **+** button under the modifier groups list in the inspector.

Keep the default settings, scroll down to the modifiers section inside the group, and enable **Position Modifier**. The position modifier adjusts the relative position of characters. Set **y = 15** to see the middle of the text rise.

Experiment by changing the **offset** of the group to observe how the characters move. When a group is selected, helpers are drawn in the scene view to guide you in editing the group's parameters.

Next, enable the **Rotation modifier** and set **z = 30**. Notice how characters within the group’s range are rotated.

Play around with different settings and modifiers to see how the text responds. Check the [Concepts](/manual/controller.html) section for a detailed explanation of all the available settings.

## Animating

Once the text is rigged with the controller, you can drive the controller’s values via script to animate the text. Using a tweening library such as [DoTween](https://dotween.demigiant.com/) is recommended.

For this example, we will use coroutines to drive the offset value and see how the text reacts to the change.

> [!Note]
> When the **auto update** is enabled, the text updates every frame while the **TextFxController** component is enabled. For optimization, disable the controller when the text is static or after the animation completes. This removes the effects and stops updates. You can also turn off **auto update** and call the `TextFxController.UpdateMesh` method whenever you change any params.
