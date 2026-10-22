# C#
## WalkerGenerator.cs

Context : This is for a rogue-like game with a randomly generated map.

The map uses a random walk algorithm, featuring multiple random walkers to give a branch-like feel to the map. 

Special rooms are featured on the map, like boss rooms, challenge rooms, shop rooms and more.

# C++
## Loot.h

Context : This is for a looter-shooter style game with random loot drops.

This is a base class, which has derivatives such as "AmmoDrop", "CommonDrop", "LegendaryDrop" etc..

Has everything that the derivitive classes need, like a mesh, a marker to designate it as loot and collision handling.

## Loot.cpp

The source file for Loot.h, featuring loot lifetimes, collision handling and making the loot look enticing.

## EnemyController.h

Base class for the AI controller for enemies, relatively short file which utilises blackboards and behaviour trees as the method of running AI.

## EnemyController.cpp

Initialises and begins running a behaviour tree.

## vec3.h

A 3D vector class used in the ray tracing project. Features operator overloading and common vector utilities, as well as utilities like generating a random vector, reflecting vectors from each other and refraction. 

## ray.h

A class which simulates the position of a ray of light, made extremely simple thanks to the above vector class. 

# Python
## arduino_connection_centre.py

Responsible for loading in parameters for an arduino controlled motor from json files, then passes the parameter payload forward to the arduino.

## camera_detail_parser.py

Runs a bash script to fetch the capabilities of an attached USB Webcam, then parses them to find the best option available.
