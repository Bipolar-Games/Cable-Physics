# Cable Physics System for Unity

[![Unity 2020.3+](https://img.shields.io/badge/unity-2020.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://opensource.org/license/mit)

## Description

The Cable Physics System is a Unity asset that simulates cable behavior with wrapping around walls, inspired by the mechanics seen in games such as Filament and Poppy's Playtime. This system provides dynamic and visually appealing cable simulation, which can be used for a variety of applications in games development.

## Table of Contents
- [Usage](#usage)
- [Components](#components)

## Usage

To start using the Cable Physics System, add the asset folder into your Unity project. Add the Cable component to any GameObject to simulate cable behavior.

## Components
Descriptions of included components


### Cable
Simulates single cable. 
- Origin: The starting anchor point of the cable.
- Ending: The ending anchor point of the cable.
- Thickness: Adjust the thickness of the cable, used for walls detection.
- Detected Layers: Layers which cable collides with.
- Length: Output value of cable length.

### Cable Line Renderer


## Used Packages
### NaughtyAttributes (optional)
NaughtyAttributes (https://github.com/dbrizov/NaughtyAttributes) is an extension for the Unity Inspector.
It expands the range of attributes that Unity provides so that you can create powerful inspectors without the need of custom editors or property drawers. It also provides attributes that can be applied to non-serialized fields or functions.
It's optional, so you might add it to your project or not. To enable its features in my scripts define NAUGHTY_ATTRIBUTES scripting define symbol in Project Settings.
