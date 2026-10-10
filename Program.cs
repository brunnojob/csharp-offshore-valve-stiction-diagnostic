using System.Globalization;
using System.Text.Json;

static class Program
{
    static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    static double Numeric(string value)
    {
        double result = double.Parse(value, Culture);
        Require(double.IsFinite(result), "Finite numeric input required");
        return result;
    }
    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
    static List<string[]> Input(int columns)
    {
        var rows = new List<string[]>();
        string? line;
        while ((line = Console.ReadLine()) is not null)
        {
            Require(rows.Count < 100000, "Input capacity exceeded");
            string[] fields = line.Split(',');
            Require(fields.Length == columns && fields.All(f => !string.IsNullOrWhiteSpace(f)), "Invalid CSV row");
            rows.Add(fields);
        }
        Require(rows.Count > 0, "Missing input");
        return rows;
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.SequenceEqual(new[] { "--self-test" })) { Test(); Console.WriteLine("Tests passed"); }
            else Run();
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error.Message); return 2; }
    }

    record Sample(double Time, double Command, double Position);
    record Finding(double Time, string Type, double Magnitude);
    static List<Finding> Diagnose(List<Sample> samples)
    {
        var output = new List<Finding>();
        double previousTime = -1, previousCommand = 0, previousPosition = 0, anchorCommand = 0, anchorPosition = 0;
        double? mismatchStart = null;
        int direction = 0;
        foreach (var s in samples)
        {
            Require(s.Time >= 0 && s.Time > previousTime && s.Command >= 0 && s.Command <= 100 && s.Position >= 0 && s.Position <= 100, "Invalid trend");
            if (previousTime < 0) { anchorCommand = s.Command; anchorPosition = s.Position; }
            else
            {
                double change = s.Command - previousCommand;
                int nextDirection = Math.Abs(change) > 0.2 ? Math.Sign(change) : direction;
                if (direction != 0 && nextDirection != direction) { anchorCommand = previousCommand; anchorPosition = previousPosition; }
                direction = nextDirection;
                double demandTravel = Math.Abs(s.Command - anchorCommand);
                if (demandTravel >= 5 && Math.Abs(s.Position - anchorPosition) < 0.5)
                {
                    output.Add(new Finding(s.Time, "STUCK_TRAVEL", demandTravel)); anchorCommand = s.Command; anchorPosition = s.Position;
                }
            }
            if (Math.Abs(s.Command - s.Position) > 10)
            {
                mismatchStart ??= s.Time;
                if (s.Time - mismatchStart.Value >= 3) { output.Add(new Finding(s.Time, "PERSISTENT_MISMATCH", Math.Abs(s.Command - s.Position))); mismatchStart = s.Time; }
            }
            else mismatchStart = null;
            previousTime = s.Time; previousCommand = s.Command; previousPosition = s.Position;
        }
        return output;
    }
    static void Test()
    {
        var stuck = Diagnose(new List<Sample> { new(0, 20, 20), new(1, 26, 20), new(2, 40, 20), new(5, 40, 20) });
        Require(stuck.Any(f => f.Type == "STUCK_TRAVEL") && stuck.Any(f => f.Type == "PERSISTENT_MISMATCH"), "Stiction not found");
        Require(Diagnose(new List<Sample> { new(0, 20, 20), new(1, 30, 30), new(2, 20, 20) }).Count == 0, "Healthy tracking rejected");
    }
    static void Run()
    {
        var samples = Input(3).Select(r => new Sample(Numeric(r[0]), Numeric(r[1]), Numeric(r[2]))).ToList();
        foreach (var f in Diagnose(samples)) Console.WriteLine(JsonSerializer.Serialize(f));
    }

}
