using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;


class Program
{
    
    private static List<Patient> patients = new List<Patient>();
    private static List<DrugInventory> drugInventory = new List<DrugInventory>();
    private static FileHandler patientFileHandler = new FileHandler("patients.txt");
    private static FileHandler drugFileHandler = new FileHandler("drugs.txt");

    private static IPatientManagement patientManager = new Patient();
    private static IDrugManagement drugManager = new DrugInventory();

    public static void Main(string[] args)
    {
        bool isAuthenticated = false;

        while (!isAuthenticated)
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            // Mask password input
            Console.Write("Enter password: ");
            string password = ReadPassword();

            if (Login(username, password))
            {
                Console.WriteLine("\nLogin successful!");
                isAuthenticated = true; // Exit loop
                Console.Clear();
                LoadData();
                MainMenu();
            }
            else
            {
                Console.WriteLine("\nAuthentication failed.");
                Console.Write("Would you like to try again? (Y/N): ");
                string choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice != "Y")
                {
                    Console.Clear();
                    Console.WriteLine("\nExiting program. Goodbye!");
                    return; // Exit the program
                }
            }
            Console.Write('\n');
            Console.Clear();
        }
    }

    // Method to handle password input with masking
    private static string ReadPassword()
    {
        StringBuilder password = new StringBuilder();
        ConsoleKey key;

        do
        {
            key = Console.ReadKey(intercept: true).Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
            {
                // Remove the last character when Backspace is pressed
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b"); // Move the cursor back and erase the last character
            }
            else if (key != ConsoleKey.Enter && key != ConsoleKey.Backspace)
            {
                // Append the key directly to the password StringBuilder
                password.Append((char)key); // Corrected line: append the character directly
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter); 

        Console.WriteLine(); 
        return password.ToString(); 
    }


    private static bool Login(string username, string password)
    {
        // Case-insensitive comparison for both username and password
        return username.Equals("admin", StringComparison.OrdinalIgnoreCase) && password.Equals("password", StringComparison.OrdinalIgnoreCase);
    }


    private static void MainMenu()
    {
        DisplayHeader();
        while (true)
        {
            string[] menuOptions = { "Manage Patients", "Manage Drug Inventory", "View Data Analytics", "[red1]Exit[/]" };
            int selectedIndex = ArrowNavigationMenu("[yellow]Main Menu[/]", menuOptions, true); // Show subHeader only in Main Menu

            switch (selectedIndex)
            {
                case 0: ManagePatients(); break;
                case 1: ManageDrugs(); break;
                case 2: ShowAnalyticsMenu(); break;
                case 3:
                    DisplayHeader();
                    return;
            }
        }
    }

    private static void DisplayHeader()
    {
        AnsiConsole.Clear();
        string text = @"
                                ███████╗    ███████╗    ██╗  ██╗    ██████╗     ███████╗
                                ██╔════╝    ██╔════╝    ██║  ██║    ██╔══██╗    ██╔════╝
                                ███████╗    █████╗      ███████║    ██████╔╝    ███████╗
                                ╚════██║    ██╔══╝      ██╔══██║    ██╔══██╗    ╚════██║
                                ███████║    ███████╗    ██║  ██║    ██║  ██║    ███████║
                                ╚══════╝    ╚══════╝    ╚═╝  ╚═╝    ╚═╝  ╚═╝    ╚══════╝";

        AnsiConsole.MarkupLine($"[skyblue2]{text}[/]");
        AnsiConsole.MarkupLine("                                [indianred1][skyblue2]S[/]MART       [skyblue2]E[/]LECTRONIC  [skyblue2]H[/]EALTH      [skyblue2]R[/]ECORD      [skyblue2]S[/]YSTEM[/]");
        AnsiConsole.MarkupLine("\n                                           [greenyellow]SDG 3: GOOD HEALTH AND WELL-BEING[/]");
        AnsiConsole.MarkupLine("                                    [greenyellow]SDG 9: INDUSTRY, INNOVATION, AND INFRASTRACTURE[/]");
        AnsiConsole.MarkupLine("                                                   Tindogan, Jesnar T.\n");
        AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private static void subHeader()
    {
        AnsiConsole.Clear();
        string text = @"
                                ███████╗    ███████╗    ██╗  ██╗    ██████╗     ███████╗
                                ██╔════╝    ██╔════╝    ██║  ██║    ██╔══██╗    ██╔════╝
                                ███████╗    █████╗      ███████║    ██████╔╝    ███████╗
                                ╚════██║    ██╔══╝      ██╔══██║    ██╔══██╗    ╚════██║
                                ███████║    ███████╗    ██║  ██║    ██║  ██║    ███████║
                                ╚══════╝    ╚══════╝    ╚═╝  ╚═╝    ╚═╝  ╚═╝    ╚══════╝";

        AnsiConsole.MarkupLine($"[seagreen2]{text}[/]");
        AnsiConsole.MarkupLine("                                [paleturquoise1][seagreen2]S[/]MART       [seagreen2]E[/]LECTRONIC  [seagreen2]H[/]EALTH      [seagreen2]R[/]ECORD      [seagreen2]S[/]YSTEM[/]\n");
        
    }

    private static void ManagePatients()
    {
        string[] patientOptions = { "Add Patient", "Update Patient", "Delete Patient", "Search Patient", "Main Menu" };
        bool backToMenu = false;

        while (!backToMenu)
        {
            int selectedIndex = ArrowNavigationMenu("[yellow]Patient Management[/]", patientOptions, false); // Hide subHeader
            switch (selectedIndex)
            {
                case 0: patientManager.AddPatient(patients, patientFileHandler); break;
                case 1: patientManager.UpdatePatient(patients, patientFileHandler); break;
                case 2: patientManager.DeletePatient(patients, patientFileHandler); break;
                case 3: patientManager.SearchPatient(patients); break;
                case 4: backToMenu = true; break;
            }
        }
    }


    private static void ManageDrugs()
    {
        string[] drugOptions = { "Add Drug", "Update Drug Stock", "Show Inventory", "Delete Drug by ID", "Main Menu" };
        bool backToMenu = false;

        while (!backToMenu)
        {
            int selectedIndex = ArrowNavigationMenu("[yellow]Drug Management[/]", drugOptions, false); // Hide subHeader
            switch (selectedIndex)
            {
                case 0: drugManager.AddDrug(drugInventory, drugFileHandler); break;
                case 1: drugManager.UpdateDrug(drugInventory, drugFileHandler); break;
                case 2: drugManager.ShowInventory(drugInventory); break;
                case 3: drugManager.DeleteDrug(drugInventory, drugFileHandler); break;
                case 4: backToMenu = true; break;
            }
        }
    }

    private static void ShowAnalyticsMenu()
    {
        string[] analyticsOptions = { "Overall Patient Distribution", "Filter by Medical Condition", "Main Menu" };
        bool backToMenu = false;

        while (!backToMenu)
        {
            int selectedIndex = ArrowNavigationMenu("[yellow]View Data Analytics[/]", analyticsOptions, false); // Hide subHeader
            switch (selectedIndex)
            {
                case 0: Analytics.DisplayOverallDistribution(patients); break;
                case 1: Analytics.DisplayFilteredDistribution(patients); break;
                case 2: backToMenu = true; break;
            }
        }
    }

    private static int ArrowNavigationMenu(string title, string[] options, bool showSubHeader)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            var table = new Table().Centered();
            table.AddColumn(new TableColumn(title).Centered());

            if (showSubHeader) subHeader(); // Show subHeader only when specified

            for (int i = 0; i < options.Length; i++)
            {
                string optionText = i == selectedIndex
                    ? $"[paleturquoise1]==> [/][paleturquoise1]{options[i]}[/] [paleturquoise1]<==[/]"
                    : $"      [skyblue2]{options[i]}[/]     ";
                table.AddRow(optionText);
            }
                 

            AnsiConsole.Render(table);
            AnsiConsole.MarkupLine("                                     (Use [yellow]Arrow Up/Down[/] and [yellow]Enter[/] key to Navigate)\n");

            key = Console.ReadKey(intercept: true).Key;

            if (key == ConsoleKey.UpArrow)
                selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : options.Length - 1;
            else if (key == ConsoleKey.DownArrow)
                selectedIndex = (selectedIndex < options.Length - 1) ? selectedIndex + 1 : 0;

        } while (key != ConsoleKey.Enter);

        return selectedIndex;
    }

    private static void LoadData()
    {
        patients = patientFileHandler.LoadPatients();
        drugInventory = drugFileHandler.LoadDrugs();
    }
}
