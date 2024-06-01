namespace GridMaker.Editor.Services.IO
{
    public interface IFileInteraction
    {
        public void Save();

        public void Load();

        public void Delete();
    }
}