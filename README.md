# Performant, highly configurable, semi-realistic engine simulator made in Godot
Made mainly for propeller plane in my [flight sim](https://github.com/LeaveMyAlpaca/Flight-sim)
## Main features:

-   **Performant:** Designed to be efficient and not hog resources, making it suitable for various hardware and tasks. It can run around 100k times per second on single thread. 
-   **Highly Configurable:** Offers extensive customization options to simulate a wide range of engine types and behaviors.
-   **Versatile:** Can be integrated into different projects.
-   **Modular:** Built with a modular architecture for easy expansion and modification.
-   **Car Chassis Simulation** Including: gears, wheal size, drag and breaks.
-   **Heat Simulation** Highly customizable and realistic system that can be used to affect engine performance and for overheating simulation 
-   **Sound Synthesis:** Includes sound synthesis to provide realistic audio feedback [(from my other project)](https://github.com/LeaveMyAlpaca/soundSynthTest).
-   **Visualizations:** Features cylinder, gas, and crankshaft visualizations for debugging and demonstration purposes.
-   **Learning Resources:** I tried to leave as much research resources in comments inside  code as i could.

## Configuration

The engine can be configured through the Godot editor. Key parameters include:

-   **Cylinder Configuration:** Count, placement, scale everything can be customized.
-   **Crankshaft Configuration:** Crankpin and connection rod affect engine performance and visualization.
-   **Cylinder Dimensions:** Displacement affects performance of engine.
-   **Firing Order:** The sequence in which cylinders fire.
-   **Airflow Settings:** Sizes and effectiveness of intake, throttle and exhaust ports can be customized to make engine perform as you need.  
-   **Cooling System:** Cylinder wall thickness, cooling area, coolant temperature etc.
-   **Chassis:** RPM Limiter, drag, gears, wheals, breaks are a important factors to make car behave how you would like.

## Limitations, places for improvements and things that I could change in the future 
#### Single Zone Combustion  
Currently combustion happens in one frame with approximation that all gas in the cylinder has the same conditions.
It is really performant but it makes the combustion less realistic.

####  Heat model 
Heat of cylinder walls currently doesn't affect engine performance ( first better combustion model is needed to make simulation more stable). 

#### Exhaust
Heat of exhaust gasses isn't calculated realistically in the current moment (but it should be pretty easy to add this in the future).


Learning resources on this topic are really scarce.
Neither am I a chemist nor professional physicist.
Some fields of simulation are just approximations and some could be just plain wrong.
So If you see some things that I could improve on tell me about them in issues page and I'll try to improve on them.  

# High-Performance, Configurable, Semi-Realistic Engine Simulator in Godot

This engine simulator is designed primarily for propeller plane simulations within my [flight sim](https://github.com/LeaveMyAlpaca/Flight-sim) project.

## Key Features:

-   **Performance:** Optimized for efficiency and minimal resource consumption, enabling approximately 100,000 physics iterations per second on a single thread. Suitable for various hardware configurations and applications.
-   **Configurability:** Offers extensive customization options to simulate diverse engine types and behaviors.
-   **Versatility:** Designed for seamless integration into various projects.
-   **Modularity:** Features a modular architecture to facilitate easy expansion and modification.
-   **Car Chassis Simulation:** Includes simulation of gears, wheel size, drag, and brakes.
-   **Heat Simulation:** A highly customizable and realistic system that simulates heat's effect on ~~engine performance~~TODO and overheating scenarios.
-   **Sound Synthesis:** Incorporates sound synthesis to generate realistic audio feedback. Utilizes code from my [sound synthesis project](https://github.com/LeaveMyAlpaca/soundSynthTest).
-   **Visualizations:** Provides visualizations of cylinders, gas flow, and crankshaft mechanics for debugging and demonstration.
-   **Learning Resources:** The codebase contains extensive comments and research resources to aid understanding and further development.

## Configuration Options:

The engine can be configured using the Godot editor. Key parameters include:

-   **Cylinder Configuration:** Customize count, placement, and scale.
-   **Crankshaft Configuration:** Adjust crankpin and connecting rod parameters to influence engine performance and visualization.
-   **Cylinder Dimensions:** Modify displacement to affect engine performance.
-   **Firing Order:** Define the sequence in which cylinders fire.
-   **Airflow Settings:** Customize the size and effectiveness of intake, throttle, and exhaust ports to achieve desired engine performance characteristics.
-   **Cooling System:** Adjust cylinder wall thickness, cooling area, coolant temperature, and other parameters.
-   **Chassis:** Fine-tune RPM limiter, drag, gear ratios, wheel properties, and brake parameters to achieve desired vehicle behavior.

## Limitations and Future Improvements:

#### Single-Zone Combustion

The current implementation uses a single-zone combustion model, approximating uniform conditions throughout the cylinder. While performant, this simplification reduces combustion realism.

#### Heat Model

Cylinder wall temperature does not currently influence engine performance. A more stable and accurate combustion model is required before incorporating this feedback mechanism.

#### Exhaust Modeling

Exhaust gas temperature is not currently calculated with high fidelity but could be implemented relatively easily in the future.

## summary

Given the scarcity of learning resources on this topic, and acknowledging my limitations as neither a chemist nor a physicist, some aspects of the simulation are approximations and may contain inaccuracies.

I encourage users to report potential improvements or identified issues on the issues page. I will strive to incorporate feedback and enhance the simulation accordingly.
