namespace Script.Entities
{
    public abstract class SelectableEntity : Entity
    {
        public abstract void Select();
        public abstract void Deselect();
    }
}