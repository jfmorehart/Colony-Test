# Colony-Test
This project was made in five hours for a unity development take-home test. 
It is a simple RTS game, with two modes, Simulation (AI-Control) and Singleplayer (User-control)

The entry point for this demo is the [GameManager](Assets/Scripts/PlayerFacing/GameManager.cs).
GameManager takes some UI input, and starts the game, which mainly involves the creation of a Map object. 

The [Map](Assets/Scripts/Generative/Map.cs) is resposible for everything level-related.
From there, the map spawns the [UnitFactory](Assets/Scripts/Generative/UnitFactory.cs), and the [Grid](Assets/Scripts/Generative/Grid.cs).

The unit factory is swappable, and allows decoupling between each level and the [unit](Assets/Scripts/Units/Unit.cs) prefabs and [data](Assets/Scripts/Objects/InsectData.cs). 
It also facilitates the combination of those two, allowing for data-driven design for any unit subclass. 

It's an imperfect system, but it is intuitive, readable, and quite scalable. 

<img width="901" alt="Screenshot 2025-05-09 at 1 07 44 AM" src="https://github.com/user-attachments/assets/3817ffe3-ef44-43a2-9e1b-43c4772815aa" />
