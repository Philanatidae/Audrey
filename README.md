# Audrey

A simple ECS for C#. This was created for my personal projects, however I decided to put the library up for others to use if they wish. Audrey is loosely based on [Ashley](https://github.com/libgdx/ashley).

Audrey initially used maps of components in entities. With many entities, this turned out to have poor performance. I rewrote Audrey to use sparse sets, similar to EnTT, which greatly improved performance in my own projects. While the library as a whole is not as fast as EnTT, it remains a good choice for a simple yet performant ECS.

# Installing

Download the repository as a zip (or add as a submodule to your project's Git repository), add the 'Audrey' project to your solution, and add Audrey as a project reference to the project you wish to use it in. Audrey does not reference other projects.

# How To Use

The heart of Audrey is the entity Engine. The entity Engine manages entities within the ECS:

    Engine engine = new Engine();

Entities are containers for components, and are created through the Engine:

    Entity entity = engine.CreateEntity();
    
Components are PODs ("Plain Old Data"). For example, a sprite's position and texture could be different components. All components inherit the IComponent interface:

    public class PositionComponent : IComponent {
        public float X;
        public float Y;
    }
    
Components can be attached to Entities via the ```AddComponent``` method. There are also other methods concerning Components that Entities can perform:

    entity.AddComponent(new PositionComponent()); // Adds a component instance to the entity
    entity.HasComponent<PositionComponent>(); // Returns true if an entity has a component
    entity.GetComponent<PositionComponent>(); // Returns the instance of the component specified. If not found, it returns null.
    entity.RemoveComponent<PositionComponent>(); // Removes the instance of the component specified.
    
Entities can only have one Component per type at a time, however can have unlimited Components (i.e. cannot have two PositionComponent's, but can have a PositionComponent and a TextureComponent).

Families are essentially filters for the Engine:

    // Entities that match this family must have a PositionComponent and a Speed component, either a LineComponent or a CircleComponent (must have at least one), and must not have a FooComponent.
    Family family = Family.All(typeof(PositionComponent), typeof(SpeedComponent)).One(typeof(LineComponent), typeof(CircleComponent)).Exclude(FooComponent).Get();
    
    // Returns true if an entity matches the family, false otherwise
    family.Matches(entity);
    
The Engine can return lists of entities, and can return lists of entities that match a certain family. The lists are cached to reduce the iteration counts:

    ImmutableList<Entity> allEntities = engine.GetEntities(); // All entities in the engine
    ImmutableList<Entity> famEntities = engine.GetEntitiesFor(family); // All entities in the engine that match a given family
    
```ImmutableList<T>``` is a wrapper around ```List<T>``` that prevents additions/removals, and is necessary to cache Entity Families. Because the lists are cached, you do not need to call ```engine.GetEntitiesFor(family)``` every tick; the ```ImmutableList<T>``` always references a list within the Engine:

    ImmutableList<Entity> famEntities = engine.GetEntitiesFor(family);
    ...
    public void Update(float dt) {
        foreach(Entity entity in famEntities) {
            // This list will always be up to date,
            // no matter when entities and/or components are added/removed
        }
    }

# Contributing

Create an issus to identify bugs/improvements. If you would like to contribute, feel free to create a pull request. Pull requests must pass the unit tests, and if new features are added unit tests must be written for them.

# License

MIT
