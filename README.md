
# Introduction

Welcome to Blade Boogie, an action RPG prototype inspired by games like Diablo! In this game, you'll battle fierce enemies, collect powerful loot, and customize your character with modular armor rigging and variety of spell types.

Blade Boogie features a randomized and tiered loot system, carefully designed levels, destructible items, built with the strengths of the genre in mind to maximise player experience. We're still in the prototype stage, and we welcome your feedback on the game's mechanics and features. Thank you for trying out Blade Boogie, and we hope you have a blast playing it!

# Functionality

## Player State Machine

We have implemented the player controller using a hierarchical state machine. We have two root states:
- Default state: Represents the player's default state.
- Invulnerable state: Represents a temporary state where the player is invulnerable to enemies.

And the following sub-states:
- Attack state: The player is performing an attack (within the PlayerAttackState class we differentiate between different types of attacks (melee, projectile, AOE).
- Idle state: The player is not moving or performing any actions.
- Interact state: The player is interacting with an object / NPC in the game.
- Roll state: The player is performing a roll/dodge move.
- Run state: The player is running.
- Walk state: The player is moving at a slower pace.

Each sub-state can be accessed from either of the two root states. For example, the player could be in the Invulnerable state and still be able to perform an attack, or the player could be in the Interact state while in the Default state. This was done to avoid code repetition and to allow greater flexibility as the state machine can be easily expanded by further root or sub states depending on the game mechanics we want to implement.

## Dynamic Loot System

Our loot system in Blade Boogie is a tiered system, with items spawning at four levels of rarity: Common, Uncommon, Rare, and Legendary. Each rarity tier corresponds to a different color and a stat modifier, making higher rarities more desirable. When a chest is opened or a breakable object smashed, 1 or more pieces of loot. The number of items dropped and the suggested level of the items can be determined using public variables stored within each lootable object.

The stats of an item are modified based on the suggested level and the rarity of the item. For example, a weapon with a higher rarity will have higher base stats, and a suggested level of 3 will scale the stats of the item accordingly. Additionally, there is a chance for common items to be a level below the suggested level. When an item drops, it is instantiated as a GameObject and given a LootRarity object to store its rarity and color. Finally, the GameObject is instantiated into the world with a physics force to simulate it being dropped by the enemy.

In it's current state, and with a rarity modifier of 1, each rarity type has the following percentage chance of dropping:
- Common: 50%
- Uncommon: 30%
- Rare: 15%
- Legendary: 5% (this number was inflated for the sake of playtesting)

Additionally, each loot rarity is coloured using a rough adaptation of the *Borderlands 2* rarity tier system, where loot is coloured in order of most-least rare as orange, purple, green, then white.

![](https://lh6.googleusercontent.com/23PO2kdbGBFQrMMOlWA-vpDUPcBxeiYqFNRRSvO6SiyK8APyRgkVcR1Fjupu1qh2m0M=w2400)
![](https://lh6.googleusercontent.com/nsH5MEGM5AjwfFc_FZzj1DVgN5flZXSVyeE0pA7bpn-tbjZAkiuqmqYvt06TkEgGxvc=w2400)
## Destructible Objects

Our destructible item mechanics add an exciting level of realism to the game. Each destructible or explodeable object is first created using Blender, the original mesh is imported then the "Cell Fracture" add-on is used to split the mesh into multiple broken pieces. This results in a highly realistic and dynamic appearance when the object is broken apart in the game.

We then bring these objects into Unity and create two prefabs: one for the complete object and another for the fractured object. Each piece contains a rigidbody component which makes the object physically interactive within the game. When the object is broken, the complete object prefab is removed and replaced with the fractured prefab. Then, every rigidbody within the fractured object has an explosionforce added to it, giving the impression of a realistic explosion.

For objects that can be exploded, we implement similar mechanics, with the added bonus of an explosion particle effect. The system also checks for other nearby breakable objects to fracture, resulting in a cascade of destruction that adds to the excitement of the gameplay.

![](https://lh4.googleusercontent.com/333SImtNJsOmYMKgAizgsOL3QqWf7_IpcHMQfMSlDfPLkNkHXJIaloLpwfy2im7Uu_s=w2400)

Similarly to chests, it is possible to highlight destructible objects using the mouse, it was deemed important to ensure the player is aware which objects in the scene are and aren't interactive, applying an outline to the object is a simple yet effective solution to this problem.
 
 ![](https://lh5.googleusercontent.com/2jRheVu-CWuwMA58GJhZBmikQQLjwMzXXxeFiCwvCAX80wiZc5SwqtjVuNjPt82GD1s=w2400)

## Environment Creation

### Main Area Level

For our prototype, we constructed levels using existing models and prefabs from imported asset packs. While we only had a small slice of the game to create, it was crucial to build an environment that matched the atmosphere of the action RPG genre.

The main area level of our game is a medieval fantasy-themed outdoor environment created using Unity's built-in terrain system. The terrain was sculpted and textured to create a realistic and immersive environment for players to explore.

The level features a prominent castle with an entrance leading to the dungeon level. This adds a sense of mystery and adventure to the game, as players will want to explore the dungeon and uncover its secrets. In addition to the castle, there is also a house and a tent with a merchant NPC near the lake. This creates a sense of liveliness in the environment, as players can interact with the merchant and trade items.

![](https://lh3.googleusercontent.com/pw/AMWts8CMf3oBhdd17eJpjTL4bw3pXpCWNk8hi-5Mp4S-RI_mOFmCdObt_pISnmAYxtouit7v2WeIJf6TXPe8yoJtOdLwgmuPgP7Rskc61BxDX3P8VbyleyhD1bD5TE7RbZ8Bc-ibN1di41Lbxtydy0R-M56x=w1764-h897-s-no)

### Crypt / Dungeon Level

This level leads to a final throne room area, intended to be the grand culmination to the player's journey through the first level of the game. The crypt/dungeon level was sprinkled throughout with chests and other lootable objects, ending in the throne room with 2 chests, each twice as likely to drop rare loot. This aims to ensure that those who may playtest this prototype get the opportunity to see rare loot as it is otherwise left to chance.

Overall, the levels we created provide players with a rich and immersive environment to explore, and set the stage for a compelling and exciting gameplay experience.

![](https://lh3.googleusercontent.com/j1ONEOjyhnkCaadkRhz8OGlbyQqDgiuVnLlobibgyLnrD9Yt_t6gWYl1gOV-NJtMgmE=w2400)

## Enemy AI

For the prototype, we have created a handfull of different enemy types for the player to encounter and battle. We have done this to provide a varied experience for our players to engage in different combat scenarios and really test their skills. There are two main enemie types for players to take on:

### Melee Enemy types

The melee enemies are the most common challenge players will encounter. They will recognise the players and persue them until they are close enough to unleash some deadly close quarter attacks to deal damage. 

### Ranged Enemy types

The ranged enemy is powered by the same basic logic as the previous types but instead of attacking up close with weapons, fires a spell projectile to deal damage from afar. These spells are dynamically created game objects launched in a projectile arc at the player that will deal damage and be destroyed either upon impact or after their lifespan is concluded. 

### Summoner Enemy Type

To conclude the short prototype of Blade Boogie, in the final throne room we are met with a special summoner enemy type, this enemy type functions mostly alike the other ranged enemy types, only with an additional behaviour. Every 30 seconds, while alive, this summoner enemy will spawn 1-3 additional melee enemies for the player to tackle. This was achieved by sampling a random position on the navmesh within a fixed range of the summoner enemy, a melee enemy is then instantiated along with a special particle effect to help sell the summon behaviour.

## User Interface

### Inventory and Merchant
The inventory system allows the player to manage a collection of items that they acquire throughout the game, including weapons, armor, and potions. The system includes a user interface that allows players to view their inventory, select items to use, drop items, or equip them if possible.

The inventory system is implemented using a script that manages the player's inventory data and interactions. 

The merchant NPC allows players to trade items in exchange for gold. The merchant NPC includes a user interface that displays the NPC's inventory, the player's inventory, and the trading options available to the player.

![](https://lh3.googleusercontent.com/pw/AMWts8Dz7u11hWuYpySzkaBmkwlam9sQesmChFsiiOa8-8unqaseBvekf98ippK3-_i3j-D5UzrV62ynn4XHJKeoeWyXD70NScAlEnIwHjKd4eeVEE6axTEusFBNAwg0Z23JjwLTeSQV0qHudtZ6JM9k_u1C=w1919-h1073-s-no?authuser=0)

# Asset Sources

## Characters, Animations, and Equipment

https://assetstore.unity.com/packages/3d/characters/lowpoly-modular-armors-free-pack-199890

https://assetstore.unity.com/packages/3d/characters/humanoids/character-elf-114445

https://assetstore.unity.com/packages/3d/animations/free-32-rpg-animations-215058

https://assetstore.unity.com/packages/3d/props/potions-115115

https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/lowpoly-medieval-skeleton-free-pack-181883

https://assetstore.unity.com/packages/vfx/particles/spells/toon-projectiles-158199

https://assetstore.unity.com/packages/vfx/particles/spells/magic-effects-free-247933

## Loot System

https://sketchfab.com/3d-models/clay-pot-a8a4b6f1c851491ba79c84706ed745fa

https://sketchfab.com/3d-models/explosive-barrel-8024c48cab9e4cd8bc8cdfa224b2d3f1

https://assetstore.unity.com/packages/3d/props/interior/treasure-set-free-chest-72345

https://assetstore.unity.com/packages/vfx/particles/legacy-particle-pack-73777

https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488

## Environment

https://assetstore.unity.com/packages/tools/utilities/fence-layout-tool-162856

https://assetstore.unity.com/packages/2d/textures-materials/free-fantasy-terrain-textures-233640

https://assetstore.unity.com/packages/2d/textures-materials/water/stylize-water-texture-153577

https://assetstore.unity.com/packages/3d/environments/lowpoly-environment-nature-pack-free-187052

https://assetstore.unity.com/packages/3d/environments/dungeons/ultimate-low-poly-dungeon-143535

https://assetstore.unity.com/packages/3d/props/stylized-fantasy-props-sample-234139

https://assetstore.unity.com/packages/3d/environments/fantasy/mega-fantasy-props-pack-87811

https://assetstore.unity.com/packages/3d/environments/campfires-torches-models-and-fx-242552

## User Interface

https://assetstore.unity.com/packages/2d/gui/icons/sleek-essential-ui-pack-170650#description

https://opengameart.org/content/health-orb-11

https://assetstore.unity.com/packages/2d/gui/icons/gui-parts-159068

https://assetstore.unity.com/packages/2d/gui/icons/free-rpg-fantasy-spell-icons-200511

https://assetstore.unity.com/packages/2d/gui/classic-rpg-gui-160253

https://assetstore.unity.com/packages/2d/gui/icons/basic-rpg-icons-181301

https://assetstore.unity.com/packages/2d/gui/icons/500-armor-icons-138145
