## Summary

Compare the CPU and memory usage of a barebones C# coroutine (e.g. async method / Task) vs a golang goroutine.


## Methodology

* Close web browsers, etc.

* Run `swapoff -a` so swap isn't hiding memory usage.

* Configure min/max threads via environment variables (optional)

* Run for 30 seconds, e.g: 

    ````
    /usr/bin/time -v dotnet run -c Release -- 1000000 1 10000000 &> csharp_results.txt
    ````

## Results

(Under construction)

### Machine 1

* Ubuntu 18.04, Pentium, 8GB RAM, dotnet core 2.2

* [C#, 1 million, default threads](results/csharp_1m.txt)


## Summary

(Under construction)
