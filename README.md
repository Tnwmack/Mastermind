# Mastermind
This project is a Mastermind game with genetic algorithm and Knuth algorithm solvers.

This program implements the classic board game Mastermind. It is written in C# with .NET Core 3.1. Features include selection of 2-20 colors, selection of 4-10 columns, and selection of the number of rows for the game. A configurable genetic algorithm solver and Knuth solver are provided.

Test code coverage is provided by Coverlet.
To generate the code coverage (test project dir): dotnet test --settings ../coverletArgs.runsettings
To generate the HTML report: dotnet reportgenerator "-reports:TestResults\<GUID>\coverage.cobertura.xml" "-targetdir:TestResults\html" -reporttypes:HTML;

Trevor Mack

12/28/2019
