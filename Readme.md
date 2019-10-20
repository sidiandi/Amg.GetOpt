# Amg.GetOpt

Library for easy implementation of [getopt](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html#Argument-Syntax) compliant command line interfaces.

## Usage

Install package via nuget
````
nuget install-package Amg.GetOpt
````

Decorate your public methods with the [System.ComponentModel.Description](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.descriptionattribute?view=netcore-3.0) attribute to turn them into commands.

Decorate your public properties with the [System.ComponentModel.Description](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.descriptionattribute?view=netcore-3.0) attribute to turn them into options.

```csharp
    class Program
    {
        static void Main(string[] args) => Amg.GetOpt.GetOpt.Run(args, new Program());

        [Description("Add two numbers.")]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [Description("Greet the world.")]
        public void Greet()
        {
            Console.WriteLine("Hello, {Name}.");
        }

        [Description("Name to greet")]
        public string Name { get; set; } = "world";
    }
```

Output:
```
>example.exe
usage: example [options] <command> [<args>]
Run a command.

Commands:

add <a: int32> <b: int32> : Add two numbers.
greet : Greet the world.

Options:

--name <string> : Name to greet
```

See [the full example here](example/Program.cs).
