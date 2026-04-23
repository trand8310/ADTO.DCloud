namespace ADTOSharp.EntityFramework
{
    public interface IShouldInitializeDcontext
    {
        void Initialize(ADTOSharpEfDbContextInitializationContext initializationContext);
    }
}