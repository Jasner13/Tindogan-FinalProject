using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

public class Analytics
{
    public static void DisplayOverallDistribution(List<Patient> patients)
    {
        if (!patients.Any())
        {
            Console.WriteLine("No patients available for analytics.");
            Console.Write("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var patientDistribution = patients.GroupBy(p => p.Purok)
            .Select(g => new { Purok = g.Key, Count = g.Count() });

        AnsiConsole.MarkupLine("\n[cyan1]=-=-=[/] [yellow]Overall " +
            "Patient Distribution[/] [cyan1]=-=-=[/]");
        DisplayBarGraph(patientDistribution);

        AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
        Console.ReadKey();
    }

    public static void DisplayFilteredDistribution(List<Patient> patients)
    {
        Console.Write("Enter Medical Condition to filter by: ");
        string condition = Console.ReadLine();

        var filteredPatients = patients.Where(p => p.MedicalHistory.IndexOf(condition,
            StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        if (!filteredPatients.Any())
        {
            AnsiConsole.MarkupLine($"No patients found with medical condition '[blue]{condition}[/]'");
            AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var patientDistribution = filteredPatients.GroupBy(p => p.Purok)
            .Select(g => new { Purok = g.Key, Count = g.Count() });

        AnsiConsole.MarkupLine($"\n[cyan1]=-=-=[/] Patient Distribution for '[yellow]{condition}[/]' [cyan1]=-=-=[/]");
        DisplayBarGraph(patientDistribution);

        AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private static void DisplayBarGraph(IEnumerable<dynamic> distribution,
        ConsoleColor barColor = ConsoleColor.Yellow)
    {
        Console.WriteLine("");
        foreach (var item in distribution)
        {
            Console.Write($"Purok {item.Purok}: ");
            for (int i = 0; i < item.Count; i++)
            {
                Console.BackgroundColor = barColor;
                Console.Write(" ");
            }
            Console.ResetColor();
            Console.WriteLine($" [ {item.Count} patient(s) ]");
        }
    }

}
