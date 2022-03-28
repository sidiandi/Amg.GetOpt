# Amg.GetOpt

Library for easy implementation of [getopt](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html#Argument-Syntax) compliant command line interfaces.

## Usage

Install package via nuget
````
dotnet add package Amg.GetOpt
````

Decorate the public methods and properties you want to expose via the command line with 
the [System.ComponentModel.Description](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.descriptionattribute) attribute.

```csharp
class Program
{
    static int Main(string[] args) => Amg.GetOpt.GetOpt.Main(args);

	[Description("Add two numbers.")]
	public int Add(int a, int b)
	{
		return a + b;
	}

	[Description("Greet the world.")]
	public void Greet()
	{
		Console.WriteLine($"Hello, {Name}.");
	}

	[Short("n"), Description("Name to greet")]
	public string Name { get; set; } = "world";
}
```

Run the *greet* command:
```
>example.exe greet --name Alice
Hello, Alice.
```

Get help:
```
>example.exe
usage: example [options] <command> [<args>]
Run a command.

Commands:

add <a: int32> <b: int32> : Add two numbers.
greet : Greet the world.

Options:

-n|--name <string> : Name to greet
```

See [the full example here](example/Program.cs).

## Use without `[Description]` Attribute.

If you do not use the `[Description]` attribute at all, the library will expose all `public` methods and properties.

```csharp
using System;

namespace example;

class Program
{
    static int Main(string[] args) => Amg.GetOpt.GetOpt.Main(args);

    public int Add(int a, int b)
    {
        return a + b;
    }

    public void Greet()
    {
        Console.WriteLine($"Hello, {Name}.");
    }

    public string Name { get; set; } = "world";
}
```

See [the full example here](example-no-attributes/Program.cs).

Output: 

```
$ example-no-attributes
usage: example-no-attributes [options] <command> [<args>]
Run a command.

Commands:
add <a: int32> <b: int32>
greet

Options:
-h|--help : Print help and exit.
--name <string>
-v|--verbosity <quiet|minimal|normal|detailed> : Logging verbosity
--version : Print version and exit.
```
