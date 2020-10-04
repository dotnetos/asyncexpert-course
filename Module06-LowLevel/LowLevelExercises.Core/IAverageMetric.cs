namespace LowLevelExercises.Core
{
    public interface IAverageMetric
    {
        void Report(int value);
        double Average { get; }
    }
}