using System.Data;

namespace AdventOfCode._2023._20;

public class Day20 : AdventOfCodeBase<Day20>
{
    public const string Low = "low";
    public const string High = "high";

    public override object? Solution1(string input)
        => ModuleNetwork.Parse(input).Display().PushButton(1000).PulseCount switch
        {
            (var low, var high) => (low * high).Dump()
        };

    public override object? Solution2(string input)
        => ModuleNetwork.Parse(input) switch
        {
            var network => network.GetInputs("dd") switch
            {
                var inputs => inputs.Any()
                    ? (from inputModule in inputs.AsParallel()
                       let machineCopy = ModuleNetwork.Parse(input)
                       let ttf = machineCopy.TimeToFirst(inputModule, High)
                       select new KeyValuePair<string, int>(inputModule, ttf)).ToDictionary() switch
                    {
                        var dic => dic.Values.Aggregate(1L, (a, b) => a * b)
                    }
                    : "Does not work on sample data"
            }
        };

    class ModuleNetwork
    {
        private readonly IDictionary<string, ModuleBase> modules;

        public ModuleNetwork Display()
        {
            $"graph LR".Dump();
            foreach (var module in modules.Values)
                foreach (var output in module.Outputs)
                    $"  {module.Name}({module.GetType().Name switch
                    {
                        nameof(FlipFlop) => $"%{module.Name}",
                        nameof(Conjunction) => $"&{module.Name}",
                        _ => module.Name
                    }}) --> {output}".Dump();

            return this;
        }

        public ModuleNetwork(IDictionary<string, ModuleBase> modules)
        {
            this.modules = modules;
            foreach(var module in modules.Values)
            {
                module.Initialize(modules.Values);
            }
        }

        public IEnumerable<string> GetInputs(string module)
            => modules.Values.Where(m => m.Outputs.Contains(module))
                    .Select(m => m.Name);

        public (long Low, long High) PulseCount { get; private set; }

        public ModuleNetwork PushButton(int times)
        {
            for(var i = 0; i < times; i++)
            {
                $"PUSH BUTTON COUNT {i + 1} * * * * * * * * * * * * * * * * * * * * * *".Dump();
                PushButtonOnce().ToArray();
            }
            return this;
        }

        public int TimeToFirst(string module, string type)
        {
            for(var i = 1; ; i++)
            {
                var pulses = PushButtonOnce().ToArray();
                foreach(var pulse in pulses)
                {
                    if(pulse.Source == module && pulse.Type == type)
                    {
                        return i;
                    }
                }
            }
        }

        IEnumerable<Pulse> PushButtonOnce()
        {
            var queue = new Queue<Pulse>();
            queue.Enqueue(new Pulse("button", "low", "broadcaster"));

            while(queue.TryDequeue(out var pulse))
            {
                $"{pulse.Source} -{pulse.Type}-> {pulse.Destination}".Dump();

                PulseCount = pulse.Type switch
                {
                    Low => (PulseCount.Low + 1, PulseCount.High),
                    High => (PulseCount.Low, PulseCount.High + 1),
                };

                LatestPulse[pulse.Destination] = pulse.Type;
                yield return pulse;

                if(modules.TryGetValue(pulse.Destination, out var module))
                {
                    foreach (var newPulse in module.Process(pulse))
                    {
                        queue.Enqueue(newPulse);
                    }
                }
            }
        }

        public Dictionary<string, string> LatestPulse { get; } = [];

        public static ModuleNetwork Parse(string input)
            => new(
                input.AsLines()
                     .Select(ModuleBase.Parse)
                     .ToDictionary(x => x.Name)
                );
    }

    abstract class ModuleBase(string name, string[] outputs)
    {
        public string Name => name;
        public string[] Outputs => outputs;

        public abstract IEnumerable<Pulse> Process(Pulse pulse);

        protected IEnumerable<Pulse> SendToAll(string type)
            => from output in outputs
               select new Pulse(Name, type, output);

        public static ModuleBase Parse(string line)
            => Regex.Match(line, @"([%&]?)(\w+) -> (.*)") is { Success: true } match
            ? (match.Result("$1"), match.Result("$2"), match.Result("$3").Split(", ")) switch
            {
                (var moduleType, var moduleName, var outputs) => moduleType switch
                {
                    "%" => new FlipFlop(moduleName, outputs),
                    "&" => new Conjunction(moduleName, outputs),
                    "" => new Broadcaster(outputs),
                }
            }
            : throw new Exception($"Invalid module definition {line}");

        public virtual void Initialize(ICollection<ModuleBase> values) { }
    }

    class Broadcaster(string[] outputs) : ModuleBase("broadcaster", outputs)
    {
        public override IEnumerable<Pulse> Process(Pulse pulse)
            => SendToAll(pulse.Type);
    }

    class FlipFlop(string flipFlopName, string[] outputs) : ModuleBase(flipFlopName, outputs)
    {
        bool isOn = false;

        public override IEnumerable<Pulse> Process(Pulse pulse)
        {
            if (pulse.Type == Low)
            {
                isOn = !isOn;
                return SendToAll(isOn ? High : Low);
            }

            return [];
        }
    }

    class Conjunction(string conjunctionName, string[] outputs) : ModuleBase(conjunctionName, outputs)
    {
        // Note to self. We might need to preload these with all inputs
        Dictionary<string, string> memory = [];

        public override void Initialize(ICollection<ModuleBase> values)
        {
            foreach(var module in values)
            {
                if(module.Outputs.Contains(Name))
                {
                    memory[module.Name] = Low;
                }
            }
        }

        public override IEnumerable<Pulse> Process(Pulse pulse)
        {
            memory[pulse.Source] = pulse.Type;
            if(memory.Values.All(x => x == High))
            {
                return SendToAll(Low);
            }
            else
            {
                return SendToAll(High);
            }
        }
    }

    record Pulse(string Source, string Type, string Destination);
}