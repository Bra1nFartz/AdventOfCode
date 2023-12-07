// See https://aka.ms/new-console-template for more information

var filename = "../../../input.txt";
var reader = new StreamReader(filename);

var seeds = new List<Tuple<long, long>>();

var l = new Dictionary<string, List<List<long>>>();

var seedsLine = reader.ReadLine().Split(":", StringSplitOptions.RemoveEmptyEntries)[1]
    .Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();

for (int i = 0; i < seedsLine.Count; i += 2)
{
    seeds.Add(new Tuple<long, long>(seedsLine[i], seedsLine[i + 1]));
}

while (!reader.EndOfStream)
{
    var line = reader.ReadLine().Split(":")[0];
    var numbersToAdd = new List<List<long>>();
    if (!string.IsNullOrWhiteSpace(line) && !line.Any(char.IsDigit))
    {
        var lineTosplit = reader.ReadLine();
        if (lineTosplit.Any(char.IsDigit))
        {
            while (!string.IsNullOrWhiteSpace(lineTosplit) && lineTosplit.Any(char.IsDigit))
            {
                numbersToAdd.Add(lineTosplit.Split(" ").Select(long.Parse).ToList());
                lineTosplit = reader.ReadLine();
            }
        }

        l.Add(line, numbersToAdd);
    }
}

var lastLocation = new List<Tuple<long, long>>();
foreach (var location in l)
{
    var newSeeds = new List<Tuple<long, long>>();
    newSeeds.AddRange(seeds);
    var passedSeeds = new List<Tuple<long, long>>();

    foreach (var locationVal in location.Value)
    {
        foreach (var seed in seeds)
        {
            if ((locationVal[1] > seed.Item1 && locationVal[1] > seed.Item1 + seed.Item2) || seed.Item1 > locationVal[1] + locationVal[2])
            {
                continue;
            }

            // fully inside
            if (seed.Item1 >= locationVal[1] && seed.Item1 + seed.Item2 <= locationVal[1] + locationVal[2])
            {
                newSeeds.Remove(seed);
                var diff = locationVal[1] - locationVal[0];
                passedSeeds.Add(new Tuple<long, long>(seed.Item1 - diff, seed.Item2));
                continue;
            }

            // over and under
            if (seed.Item1 < locationVal[1] && seed.Item1 + seed.Item2 > locationVal[1] + locationVal[2])
            {
                newSeeds.Remove(seed);
                newSeeds.Add(new Tuple<long, long>(seed.Item1, locationVal[1] - seed.Item1));
                passedSeeds.Add(new Tuple<long, long>(locationVal[0], locationVal[2]));
                newSeeds.Add(new Tuple<long, long>(locationVal[1] + locationVal[2],
                    seed.Item1 + seed.Item2 - (locationVal[1] + locationVal[2])));
                continue;
            }

            // only over
            if (seed.Item1 + seed.Item2 > locationVal[1] + locationVal[2])
            {
                newSeeds.Remove(seed);
                var diff = locationVal[1] - locationVal[0];
                passedSeeds.Add(new Tuple<long, long>(seed.Item1 - diff, locationVal[1] + locationVal[2] - seed.Item1));
                newSeeds.Add(new Tuple<long, long>(locationVal[1] + locationVal[2],
                    seed.Item1 + seed.Item2 - (locationVal[1] + locationVal[2])));
                continue;
            }

            // only under
            if (seed.Item1 < locationVal[1])
            {
                newSeeds.Remove(seed);
                var diff = locationVal[1] - locationVal[0];
                newSeeds.Add(new Tuple<long, long>(seed.Item1, locationVal[1] - seed.Item1));
                passedSeeds.Add(new Tuple<long, long>(locationVal[1] - diff, seed.Item1 + seed.Item2 - locationVal[1]));
                continue;
            }
        }

        seeds.Clear();
        seeds.AddRange(newSeeds);
    }

    seeds.AddRange(passedSeeds);
    lastLocation = seeds;
}

Console.WriteLine(lastLocation.Select(x => x.Item1).ToArray().Min());