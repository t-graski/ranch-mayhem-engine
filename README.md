---

<p align="center">
  <img src="https://img.shields.io/badge/Language-C%23-178600?style=for-the-badge&logo=csharp&logoColor=white">
  <img src="https://img.shields.io/badge/Framework-MonoGame-blue?style=for-the-badge&logo=monogame">
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge">
  <img src="https://img.shields.io/badge/Status-Active%20Development-orange?style=for-the-badge">
</p>

---

# Ranch Mayhem Engine

Ranch Mayhem Engine is the custom game engine powering **[Ranch Mayhem](https://store.steampowered.com/app/2818590/Ranch_Mayhem__Active_Idler/)**.  
It’s built in **C#** and uses **MonoGame** rendering under the hood.  

This engine is focused on **2D pixel-art games** with an emphasis on flexible UI systems, scalable layouts, and smooth game-feel features like particles, animations, and custom shaders.

---

## Features

### UI-Components
- Built-in elements: **Box, Button, Container, Grid, ProgressBar, Text, TextBox**  
- **Highly customizable** styles and behavior  
- **Nested layouts** with UI anchors  
- **Relative sizing** using percentages of the parent  

### Animations
- Animate numeric values directly  
- Move UI elements smoothly with built-in **easing functions**  

### Pages
- Pages are the **fundamental building block** of the UI layer  
- Each page = one menu, screen, or feature  
- **Keybinding system** to toggle visibility instantly  

### Resolution Scaling
- Automatic scaling to the current window size  
- All coordinates are treated as if in **1920×1080 (FullHD)**, scaled dynamically  

### Particle System
- A set of ready-to-use particle effects  
- Spawn by **position** or **area**  
- Great for polish and feedback effects  

### UI Manager & Render Queues
- Handles all **rendering and scaling** of UI elements  
- **Four render queues** to control draw order:  
  - `Background`  
  - `UI`  
  - `Popup`  
  - `Overlay`  

### Shaders
- Easily extendable shader integration  
- Apply custom effects to sprites and UI  

### Content Management
- **Texture Atlas**: packed textures with JSON metadata  
- **Single Textures**: individual files with external definitions  
- Unified access through a **ContentManager** API  

---

## Notes

- Designed to scale with resolution, ensuring **consistent UI layouts** across devices  
- Modular structure makes it simple to **extend UI, add particles, or integrate shaders**  
- While built for **Ranch Mayhem**, it can serve as a general-purpose **MonoGame UI & rendering engine**  

---

## License

This project is licensed under the **MIT License**.  
You are free to use, modify, and distribute it in your own projects.  

---

<p align="center">
  Made with ❤️ using <strong>C#</strong> and <strong>MonoGame</strong>
</p>
