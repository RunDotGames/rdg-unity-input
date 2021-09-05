# rdg-unity-input
> An Event Driven Input Framework For Unity

## Concepts

### Key Actions

Abstraction for key and mouse button events

* *Move (Left/Right/Up/Down)*: Commonly bound to `WASD` or `arrow` keys
* *Interact*: Primary interaction button. Commonly bound to `E`
* *Modify*: Secondary interaction. Commonly bound to `F`

### Gesture

Abstraction for touch and mouse movement events

* *Tap*: Single left click or touch
* *Twist*: Right click and drag or pinch twist
* *Zoom*: Scroll or pinch zoom
* *Drag*: Left click and drag or touch drag

## Usage

Scriptable Objects provide a common reference and configuration for event generation. Additionally, they decouple event production from consumption across scenes for a given project. Add new scriptable objects into your asset folder from `Assets -> Create -> RDG -> Input`.

A single event producer, added to each scene, triggers events on its referenced Scriptable Object. Add a producer to your scene from `Add Component -> RDG -> Input`

Reference input scriptable objects to respond to input events in your project scripts. As a general lifecycle rule, your scripts should subscribe on `Start` and unsubscribe `OnDestroy` to avoid reference leaks and event loop performance issues.


## Demo

See the provided demo scenes under the `Demo` folder for example usages.
## Helper Classes

Scripts subscribing to and *owning* references to helper classes can easily unsubscribe from those classes using the `Release` on clean up.

* `KeyActionStack`: A stack representation of a series of held key events. Can help simulate an axis or mutually exclusive set of key press events.
* `KeyActionFilter`: A filter for key events. Can help  avoid control logic when responding to specific key actions. 
