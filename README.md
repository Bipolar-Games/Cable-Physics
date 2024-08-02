# Cable Physics System for Unity

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue.svg)

## Description

The Cable Physics System is a Unity asset that simulates realistic cable behavior, inspired by the mechanics seen in games such as Filament and Poppy's Playtime. This system provides dynamic and visually appealing cable simulations, which can be used for a variety of applications in game development, from interactive cables to complex wiring systems.

## Table of Contents
- [Usage](#usage)
- [Configuration](#configuration)
- [License](#license)
- [Acknowledgements](#acknowledgements)
- [Used Packages](#UsedPackages)



## Usage

To start using the Cable Physics System, import the asset package into your Unity project. Add the Cable component to any GameObject to simulate cable behavior.

### Example Usage:

## Configuration
Configure the Cable component through the Unity Inspector or via script to adjust its properties.

### Inspector Configuration:
Origin: The starting anchor point of the cable.
Ending: The ending anchor point of the cable.
Thickness: Adjust the thickness of the cable, used for walls detection.

## Used Packages
### NaughtyAttributes (optional)
NaughtyAttributes (https://github.com/dbrizov/NaughtyAttributes) is an extension for the Unity Inspector.
It expands the range of attributes that Unity provides so that you can create powerful inspectors without the need of custom editors or property drawers. It also provides attributes that can be applied to non-serialized fields or functions.
It's optional, so you might add it to your project or not. To enable its features in my scripts define NAUGHTY_ATTRIBUTES scripting define symbol in Project Settings.
