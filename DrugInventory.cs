using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Spectre.Console;

public class DrugInventory: IDrugManagement
{
    private static int drugIdCounter = LoadLastDrugId(); // Static counter for generating unique IDs
    private string drugID;
    private string name;
    private int quantity;
    private DateTime expiryDate;

    public string DrugID
    {
        get { return drugID; }
        private set { drugID = value; }
    }
    public string GetDrugID()
    {
        return DrugID;
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }

    public DateTime ExpiryDate
    {
        get { return expiryDate; }
        set { expiryDate = value; }
    }

    // Generate a unique Drug ID using the drugIdCounter
    public static string GenerateDrugId()
    {
        string newId = "DG-" + (drugIdCounter++).ToString("D3"); // Generate drug ID with 3 digits, like DG-001
        SaveLastDrugId(); // Save the updated counter after generating an ID
        return newId;
    }

    // Constructor for creating a new drug
    public DrugInventory(string id, string name, int quantity, DateTime expiry)
    {
        DrugID = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiry;
    }

    public DrugInventory() { }

    // Add a new drug to the inventory
    public void AddDrug(List<DrugInventory> inventory, FileHandler fileHandler)
    {
        
        ShowInventory(inventory);
        AnsiConsole.MarkupLine("\n[cyan1]=-=-=[/] [yellow]Add New Drug[/] [cyan1]=-=-=[/]");

        // Generate a unique ID for the drug
        string id = GenerateDrugId();

        // Validate Name input (it shouldn't be empty)
        string name;
        do
        {
            Console.Write("Enter Name: ");
            name = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(name));

        // Validate Quantity input (must be a positive integer)
        int quantity;
        do
        {
            Console.Write("Enter Quantity: ");
        } while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0);

        // Validate Expiry Date input (must be a valid future date)
        DateTime expiryDate;
        do
        {
            Console.Write("Enter Expiry Date (DD/MM/YYYY): ");
        } while (!DateTime.TryParse(Console.ReadLine(), out expiryDate) || expiryDate <= DateTime.Now);

        // Create new drug object
        var drug = new DrugInventory(id, name, quantity, expiryDate);
        inventory.Add(drug);
       
        // Save updated drug list to file
        fileHandler.SaveDrugs(inventory);
        AnsiConsole.MarkupLine($"Drug added successfully!");
        AnsiConsole.MarkupLine($"Drug ID [yellow]==>  [/][cyan1]{id}[/]  [yellow]<==[/]\n");
        ShowInventory(inventory);
        //AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
        //Console.ReadKey();
    }

    // Remove expired drugs from inventory
    public bool RemoveExpiredDrugs(List<DrugInventory> inventory)
    {
        DateTime currentDate = DateTime.Now;
        int initialCount = inventory.Count;
        inventory.RemoveAll(drug => drug.ExpiryDate < currentDate);
        return inventory.Count < initialCount; // Returns true if any drugs were removed
    }

    public void UpdateDrug(List<DrugInventory> inventory, FileHandler fileHandler)
    {
        ShowInventory(inventory);
        AnsiConsole.MarkupLine("\n[cyan1]=-=-=[/] [yellow]Update Drug[/] [cyan1]=-=-=[/]");
        Console.Write("Enter Drug ID to update: ");
        string id = Console.ReadLine();
        var drug = inventory.Find(d => d.DrugID == id);

        if (drug != null)
        {
            
            if (!string.IsNullOrEmpty(name)) drug.Name = name;

            Console.Write("Enter new Quantity (leave blank to keep current): ");
            string quantityInput = Console.ReadLine();
            if (int.TryParse(quantityInput, out int quantity)) drug.Quantity = quantity;

            Console.Write("Enter new Expiry Date (leave blank to keep current): ");
            string expiryInput = Console.ReadLine();
            if (DateTime.TryParse(expiryInput, out DateTime expiry)) drug.ExpiryDate = expiry;

            fileHandler.SaveDrugs(inventory);
            
            AnsiConsole.MarkupLine("[aqua]Drug updated successfully.[/]");
            ShowInventory(inventory);
            
           
        }
        else
        {
            Console.WriteLine("Drug not found.");
            AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
            Console.ReadKey();
        }
    }

    public void DeleteDrug(List<DrugInventory> inventory, FileHandler fileHandler)
    {
        ShowInventory(inventory);

        RemoveExpiredDrugs(inventory);
        AnsiConsole.MarkupLine("\n\n[cyan1]=-=-=[/] [yellow]Delete Drug[/] [cyan1]=-=-=[/]");
        Console.Write("Enter Drug ID to delete: ");
        string drugId = Console.ReadLine();

        // Compare directly with the DrugID property
        var drugToDelete = inventory.FirstOrDefault(d => d.DrugID == drugId);

        if (drugToDelete != null)
        {
            inventory.Remove(drugToDelete);
            AnsiConsole.Markup($"Drug with ID [yellow]{drugId}[/] has been [red]deleted[/].\n\n");
            fileHandler.SaveDrugs(inventory);  // Save changes after deletion
            ShowInventory(inventory);     
        }
        else
        {
            Console.WriteLine("Drug not found.");
            AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
            Console.ReadKey();
        }
    }



    public void ShowInventory(List<DrugInventory> inventory)
    {
        // Check if inventory is empty
        if (inventory.Count == 0)
        {
            
            Console.WriteLine("The inventory is empty. No drugs are available.");
            AnsiConsole.Markup("\n[green]Press any key to continue adding drug...[/]");
            Console.ReadKey(); // Wait for user input before returning to the menu
            return;
        }

        // Remove expired drugs
        bool expiredDrugsRemoved = RemoveExpiredDrugs(inventory);

        // Render the table
       
        var table = new Table();
        table.AddColumn("Drug ID");
        table.AddColumn("Name");
        table.AddColumn("Quantity");
        table.AddColumn("[red]Expiry Date[/]");
        table.Title = new TableTitle("[cyan1] =-=-=-=[/] [yellow]D R U G  I N V E N T O R Y[/] [cyan1]=-=-=-=[/]");

        foreach (var drug in inventory)
        {
            table.AddRow(drug.DrugID, drug.Name, drug.Quantity.ToString(), drug.ExpiryDate.ToShortDateString());
        }

        AnsiConsole.Write(table);

        // Notify the user if expired drugs were removed
        if (expiredDrugsRemoved)
        {
            AnsiConsole.MarkupLine("\n[red]Expired drug(s) have been removed.[/]");
        }

        AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
        Console.ReadKey(); // Wait for user input before returning
    }





    private static void SaveLastDrugId()
    {
        File.WriteAllText("drugIdCounter.txt", drugIdCounter.ToString());
    }

    private static int LoadLastDrugId()
    {
        if (File.Exists("drugIdCounter.txt"))
        {
            return int.Parse(File.ReadAllText("drugIdCounter.txt"));
        }
        return 1;
    }

    public static void ResetDrugIdCounter()
    {
        drugIdCounter = 1;
        File.WriteAllText("drugIdCounter.txt", "1");
    }
}
