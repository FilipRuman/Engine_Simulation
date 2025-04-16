# High-Performance, Configurable, Semi-Realistic Engine Simulator in Godot

## [Video Demonstration](https://www.youtube.com/watch?v=1RHQCW3a_Tw)

This engine simulator is designed primarily for propeller plane simulations within my [flight sim](https://github.com/LeaveMyAlpaca/Flight-sim) project.

## Key Features:

-   **Performance:** Optimized for efficiency and minimal resource consumption, enabling approximately 400,000 iterations per second on a single thread while maintaining 144 FPS. Suitable for various hardware configurations and applications.
-   **Configurability:** Offers extensive customization options to simulate diverse engine types and behaviors.
-   **Versatility:** Designed for seamless integration into various projects.
-   **Modularity:** Features a modular architecture to facilitate easy expansion and modification.
-   **Car Chassis Simulation:** Includes simulation of gears, wheel size, drag, and brakes.
-   **Heat Simulation:** A highly customizable and realistic system that simulates the effects of heat and overheating scenarios. (Note: Impact on engine performance is under development.)
-   **Sound Synthesis:** Incorporates sound synthesis to generate realistic audio feedback. Utilizes code from my [sound synthesis project](https://github.com/LeaveMyAlpaca/soundSynthTest).
-   **Visualizations:** Provides visualizations of cylinders, gas flow, and crankshaft mechanics for debugging and demonstration.
-   **Learning Resources:** The codebase contains extensive comments and research resources to aid understanding and further development.
-   **Charting:** Integrates charting functionality from [my Charts project](https://github.com/FilipRuman/Charts) for clear engine performance display.
-   **Frame Rate Independence:** Employs a tick-based system that precisely controls simulation quality and ensures consistent behavior, regardless of the game's frame rate.
-   **Idle:** System that holds engine at minimal rpm

## Configuration Options:

The engine can be configured using the Godot editor. Key parameters include:

-   **Cylinder Configuration:** Customize count, placement, and scale.
-   **Crankshaft Configuration:** Adjust crankpin and connecting rod parameters to influence engine performance and visualization.
-   **Cylinder Dimensions:** Modify displacement to affect engine performance.
-   **Firing Order:** Define the sequence in which cylinders fire.
-   **Airflow Settings:** Customize the size and effectiveness of intake, throttle, and exhaust ports to achieve desired engine performance characteristics.
-   **Cooling System:** Adjust cylinder wall thickness, cooling area, coolant temperature, and other parameters.
-   **Chassis:** Fine-tune RPM limiter, drag, gear ratios, wheel properties, and brake parameters to achieve desired vehicle behavior.
-   **Simulation Quality:** Adjust the engine physics update rate according to needs; a minimum of 1,000 is recommended, with 1,500-2,000 providing optimal results.

## Limitations and Future Improvements:

#### Single-Zone Combustion

The current implementation uses a single-zone combustion model, approximating uniform conditions throughout the cylinder. While performant, this simplification reduces combustion realism.

#### Heat Model

Cylinder wall temperature does not currently influence engine performance. A more stable and accurate combustion model is required before incorporating this feedback mechanism.

#### Exhaust Modeling

Exhaust gas temperature is not currently calculated with high fidelity but could be implemented relatively easily in the future.

## Summary:

Given the scarcity of learning resources on this topic, and acknowledging my limitations as neither a chemist nor a physicist, some aspects of the simulation are approximations and may contain inaccuracies.
