# NDesk.Options.Extensions

Ever thought so much verbiage could be said about a command line argument?
Actually, it's not a slam dunk, by any stretch of the imagination, and can
be made a bit easier to contend with. That is the purpose of this library,
to facilitate easier exposure of command line arguments.

## The Basics

This library is exactly what it says it is: an Extension to the
[NDesk.Options](https://github.com/gibbed/NDesk.Options) library.
So if you are unfamiliar with Options, you should get familiar with that
first. You can do that by installing Extensions, or just run with Options,
at your discretion.

Basically, Extensions wraps the Options in a friendlier (at least in my
opinion) fluent-style-variable completion for command line options.

## OptionSet and RequiredValuesOptionSet

Extensions builds upon the (by now) familiar OptionSet in its purest form.
Additionally, it is possible to add required values using the
RequiredValuesOptionSet Extension to OptionSet.

## Adding Variables

There are several ways to add variables to support simple variables. Let's
assume that we have the following OptionSet available:

```
var os = new OptionSet();
```

- Switch is a simple command line class that says whether it is Enabled.
Enabled means that the switch was specified at the command line.

```
var option = os.AddSwitch("Option", "Switch to set");
// Parse the arguments...
if (option.Enabled)
{
    // Do something...
}
```

For convenience, Switch implicitly converts to bool:

```
if (option)
{
    // Do something...
}
```

- Variables can also have a built-in type associated with them.

```
var option = os.AddVariable<int>("Option", "Value to set");
// Parse the arguments...
if (option.Value > 0)
{
    // Do something...
}
```

For convenience, Variable implicitly converts to its underlying Value.

- VariableLists can be specified. A variable list is a name associated with
a list of values, for as many times as the name/value appears, you will have
those values listed in the variable when you work with it.

```
var timeouts = os.AddVariableList<int>("Timeout", "Timeout in milliseconds");
// Parse the arguments...
foreach (var timeout in timeouts.Values)
{
    // Do something with timeout...
}
```

For convenience, VariableList also looks-and-feels like an IEnumerable:

```
foreach (var timeout in timeouts)
{
    // ...
}
```

- Last but not least, VariableMatrix supports cataloging a dictionary of
name/value pairs associated with a command line argument. At the command
line these appear like:

<pre>... -n:Name=Value -n:Name2=Value2 "-n:Name3=Value3 Value4 Value5"</pre>

Or such as this at the language level:

```
new string[]{"-n:Name=Value", "-n:Name2=Value2", "-n:Name3=Value3 Value4 Value5",};
```

Which parses to:

<pre>{{"Name","Value"},{"Name2",Value2"},{"Name3","Value3 Value4 Value5"}}</pre>

Okay, now for the code example:

```
var option = os.AddVariableMatrix<int>("Option", "Matrix option values");
// Parse the arguments...
foreach (var key in option.Matrix.Keys)
{
    // Do something with option[key]...
}
```

For convenience, VariableMatrix also looks-and-feels like an IDictionary whose
key is string and whose value is the specified type.

```
foreach (var key in option)
{
    // ...
}
```

## Parsing Arguments

Arguments are passed through the Program Main method as per usual.
For instance:

```
public static void Main(params string[] args)
{
    // ...
}
```

A convenience controller has been provided, ConsoleManager, which may be used
as follows:

```
var os = new RequiredValuesOptionSet();
// ...
var cm = new ConsoleManager("My Console", os);
// ...
var writer = new TextWriter();
if (cm.TryParseOrShowHelp(writer, args))
{
    // TODO: Do something with the variables...
}
```

Standard Extension parsing functionality writes a usage blurb including the
ConsoleName, passed to the ConsoleManager ctor, as well as option descriptions.
This occurs when parsing is incomplete or in error for any reason.

In production code, any TextWriter will do, but you probably want to interact
with the Console for your output. Straightforward enough:

```
if (cm.TryParseOrShowHelp(Console.Out, args))
{
    // ...
}
```

There you go. I practically handed you your production usage right there.

## Roadmap

I welcome ideas and ways to improve upon the extensibility of Extensions.
At present, here are some plausible areas of extension and/or improvement
include, but are not limited to:

- [ ] Support custom variable conversions, such as for custom-types.
Basically means injecting lambdas to do the conversions as you see fit.
Within reason, with command line limitations: you would not want to completely
deserialize a class or struct, for instance. Indeed, you might want to convert
an enumerated value from a string, however.

- [ ] Required variables could do with a face lift of sorts, a bit richer of
a model. Basically that says not only the underlying Value when it is
discovered, but also whether it IsMissing, when it is not discovered. This
would potentially Obsolete if not remove the Requirement class altogether
I think, and/or the manner in which required-variables are discerned.

- [ ] Support .NET 4.5 ReadOnlyDictionary. I cannot think of a use-case right
off hand for VariableMatrix, in particular, however, in its present form, it
supports a raw IDictionary implementation, which leaves a bit of a gap, open
to end-user (that's you, potentially) tampering. A better implementation would
be to depend upon ReadOnlyDictionary, or roll-your-own implementation of it
for earlier .NET framework versions. That would be straightforward enough
I think, hopefully without confusing anyone's collection frameworks terribly.
At present we simply implement a full IDictionary and throw ReadOnlyException
when mutation requests are encountered.

## Code Organization

In general, the library itself depends on there being an OptionSet instance
available. The library does not enforce ways for you to organize your code.
However, I would strongly encourage you to adopt a [S.O.L.I.D.]
(http://en.wikipedia.org/wiki/SOLID_%28object-oriented_design%29),
[D.R.Y.](http://en.wikipedia.org/wiki/Don%27t_repeat_yourself) approach.
Which means you necessarily separate the concerns of options, parsing, etc,
from the rest of your application. However, this topic is beyond the scope of
this repository.

## NuGet Deployment

I am currently deploying through the [NuGet Gallery](https://www.nuget.org/)
for purposes of distribution. You are welcome to add this dependency through
NuGet, download the source, or whatever.

## Target Environments

At present Extensions is targeting several .NET framework versions for
Any CPU. It may be worth refining that to 32- and 64-bit targets as needs be.

1. .NET 3.5
2. .NET 4.0
3. .NET 4.0 Client Profile

I expect that .NET 4.5 will be interesting to support as well.

## Disclaimer

This software is provided "as-is" and any expressed or implied warranties,
including, but not limited to, the implied warranties of merchantability
and fitness for a particular purpose are disclaimed. In no event shall the
regents or contributors be liable for any direct, indirect, incidental,
special, exemplary, or consequential damages (including, but not limited to,
procurement of substitute goods or services; loss of use, data, or profits;
or business interruption) however caused and on any theory of liability,
whether in contract, strict liability, or tort (including negligence or
otherwise) arising in any way out of the use of this software, even if
advised of the possibility of such damage.
