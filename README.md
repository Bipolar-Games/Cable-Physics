# Cable Physics System for Unity

[![Unity 2020.3+](https://img.shields.io/badge/unity-2020.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://opensource.org/license/mit)

## Description
The Cable Physics System is a Unity asset that simulates cable behavior with wrapping around walls, inspired by the mechanics seen in games such as Filament and Poppy's Playtime. This system provides dynamic and visually appealing cable simulation, which can be used for a variety of applications in games development.

## Components
Overview of key components of the system

### Cable
Simulates single cable. 
- **Origin:** The starting anchor point of the cable.
- **Ending:** The ending anchor point of the cable.
- **Thickness:** Adjust the thickness of the cable, used for walls detection.
- **Detected Layers:** Layers which cable collides with.
- **Length:** Output value of cable length.

### Cable Line Renderer
Add it to Line Renderer to sychronize its shape with the cable
- **Cable:** Cable component from which shape should be taken.

### Cable Length Constraint
Constraints cable length and adds physical forces to cable ends if the maximum length is reached.
- **Origin Rigidbody:** Rigidbody on which cable origin will exert force
- **Ending Rigidbody:** Rigidbody on which cable ending will exert force
- **Max Length:** Max length of the cable. Upon reaching it cable will exert forces on connected rigidbodies. 
- **Base Force:** Starting force that is exerted on rigidbodies opon reaching the maximum length.
- **Force Modifier:** Spring force of the cable. The more cable's length exceeds maximum length, the more force will be applied.
- **Damping:** Damping force, proportional to velocity of rigidbody along the cable.

## Used Packages
### NaughtyAttributes (optional)
NaughtyAttributes (https://github.com/dbrizov/NaughtyAttributes) is an extension for the Unity Inspector.
It expands the range of attributes that Unity provides so that you can create powerful inspectors without the need of custom editors or property drawers. It also provides attributes that can be applied to non-serialized fields or functions.
It's optional, so you might add it to your project or not. To enable its features in my scripts define NAUGHTY_ATTRIBUTES scripting define symbol in Project Settings.
