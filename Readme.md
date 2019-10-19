# Amg.GetOpt

getopt compliant command line parser.

Grammar:

````
CommandLine = Options Commands
Options = Option Options | ""
Commands = Command Commands | ""
Option = LongOption | ShortOption
LongOption = LongOptionToken Arg?
ShortOption = ShortOptionToken Arg?
Command = OptionArg OptionArgs
OptionArgs = OptionArg OptionArgs | ""
OptionArg = Args Options
Args = Arg Args | ""

````