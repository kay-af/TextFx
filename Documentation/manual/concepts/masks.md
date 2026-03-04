# Masks

Masks let you control the characters affected by a group. By combining multiple groups with different masks, you can precisely control the overall behaviour of a sentence, a word or even a single character.

## Properties

The following section provides a description of each property associated with masks.

### Start

Determines where the mask begins. The value must be in range [0,1].

### End

Determines where the mask stops. The value must be in range [0,1].

> [!WARNING]
> There may be unpredictable behaviour if **start** exceeds **end**.
