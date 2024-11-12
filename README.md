# WalkerGenerator.cs

Context : This is for a rogue-like game with a randomly generated map.

The map uses a random walk algorithm, featuring multiple random walkers to give a branch-like feel to the map. 

Special rooms are featured on the map, like boss rooms, challenge rooms, shop rooms and more.

# Loot.h

Context : This is for a looter-shooter style game with random loot drops.

This is a base class, which has derivatives such as "AmmoDrop", "CommonDrop", "LegendaryDrop" etc..

Has everything that the derivitive classes need, like a mesh, a marker to designate it as loot and collision handling.

# Loot.cpp

The source file for Loot.h, featuring loot lifetimes, collision handling and making the loot look enticing.

# EnemyController.h

Base class for the AI controller for enemies, relatively short file which utilises blackboards and behaviour trees as the method of running AI.

# EnemyController.cpp

Initialises and begins running a behaviour tree.
