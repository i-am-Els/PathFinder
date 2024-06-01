using Script.Enums;

namespace Script.Services.Undo
{
    // The usefulness of this item is in doubt but I hope to use it as DTO for Actions committed just like the Error DTO
    public record Action
    {
        public string Name;
        public EActionType Type { get; set; }
    }
}