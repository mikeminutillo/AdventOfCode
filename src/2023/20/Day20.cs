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

    //public override object? Solution2(string input)
    //{
    //    var network = ModuleNetwork.Parse(input);
    //    var i = 1;
    //    while(true)
    //    {
    //        network.PushButtonOnce();
    //        i++;
    //        if (network.LatestPulse["rx"] == Low)
    //        {
    //            break;
    //        }
    //    }
    //    return i.Dump();
    //}



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

        public (long Low, long High) PulseCount { get; private set; }

        public ModuleNetwork PushButton(int times)
        {
            for(var i = 0; i < times; i++)
            {
                $"PUSH BUTTON COUNT {i + 1} * * * * * * * * * * * * * * * * * * * * * *".Dump();
                PushButtonOnce();
            }
            return this;
        }

        public ModuleNetwork PushButtonOnce()
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

                if(modules.TryGetValue(pulse.Destination, out var module))
                {
                    foreach (var newPulse in module.Process(pulse))
                    {
                        queue.Enqueue(newPulse);
                    }
                }
            }
            return this;
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

    class FlipFlop(string name, string[] outputs) : ModuleBase(name, outputs)
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

    class Conjunction(string name, string[] outputs) : ModuleBase(name, outputs)
    {
        // Note to self. We might need to preload these with all inputs
        Dictionary<string, string> memory = [];

        public override void Initialize(ICollection<ModuleBase> values)
        {
            foreach(var module in values)
            {
                if(module.Outputs.Contains(name))
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