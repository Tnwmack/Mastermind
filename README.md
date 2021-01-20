# Mastermind

This project is a Mastermind game with genetic algorithm and Knuth algorithm solvers.

The program implements the classic board game Mastermind. It is written in C# and .NET Framework 4.7.2 with support for .NET Core 3.1. Features include selection of 2 to 20 colors, selection of 4 to 10 columns, and selection of the number of guessing rows. A configurable genetic algorithm solver and Knuth solver are provided. The answer row can be modified by clicking on each color peg, or right-clicking will generate a random answer.

## Build Notes
### Code Coverage

Test code coverage is provided by Coverlet.
Run the following commands from the MastermindTests folder.

To start code coverage analysis:

    dotnet test --settings ../coverletArgs.runsettings

To convert the analysis to an HTML report:

    dotnet reportgenerator "-reports:TestResults/<GUID>/coverage.cobertura.xml" "-targetdir:TestResults\html" -reporttypes:HTML;

## Future Goals

 - Implement a simulated annealing solver
 - Implement a genetic algorithm for the genetic algorithm tuning parameters
 - Replace WinForms with WPF/UWP (or possibly Blazor and convert to a web app)

Trevor Mack
12/28/2019
