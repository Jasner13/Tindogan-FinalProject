using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Patient : Person, IPatientManagement
{
    private int purok;
    private string medicalHistory;
    private string prescription;
    private string householdSerialNumber;
    private string contactNumber;
    

    // Static counter to generate unique patient IDs
    private static int patientIdCounter = LoadLastPatientId();

    public int Purok
    {
        get { return purok; }
        set { purok = value; }
    }

    public string MedicalHistory
    {
        get { return medicalHistory; }
        set { medicalHistory = value; }
    }

    public string Prescription
    {
        get { return prescription; }
        set { prescription = value; }
    }

    public string HouseholdSerialNumber
    {
        get { return householdSerialNumber; }
        set { householdSerialNumber = value; }
    }

    public string ContactNumber
    {
        get { return contactNumber; }
        set { contactNumber = value; }
    }

    public DateTime DateOfVisit { get; set; }

    // Constructor with parameters
    public Patient(string id, string name, int age, string gender, int purok, string history, string prescription, string householdSerialNumber, string contactNumber, DateTime dateOfVisit)
        : base(id, name, age, gender)
    {
        this.purok = purok;
        this.medicalHistory = history;
        this.prescription = prescription;
        this.householdSerialNumber = householdSerialNumber;
        this.contactNumber = contactNumber;
        this.DateOfVisit = dateOfVisit;  // Set the visit date
    }

    // Parameterless constructor
    public Patient() : base("", "", 0, "")
    {
        this.householdSerialNumber = "";
        this.contactNumber = "";
    }

    // Method to generate unique patient ID
    public static string GeneratePatientId()
    {
        string newId = "PT-" + (patientIdCounter++).ToString("D3"); // Generate drug ID with 3 digits, like DG-001
        SaveLastPatientId(); // Save the updated counter after generating an ID
        return newId;
    }
    private static int LoadLastPatientId()
    {
        if (File.Exists("patientIdCounter.txt"))
        {
            return int.Parse(File.ReadAllText("patientIdCounter.txt"));
        }
        return 1;
    }
    private static void SaveLastPatientId()
    {
        File.WriteAllText("patientIdCounter.txt", patientIdCounter.ToString());
    }
    public static void ResetPatientIdCounter()
    {
        patientIdCounter = 1;
        File.WriteAllText("patientIdCounter.txt", "1");
    }

    public void AddPatient(List<Patient> patients, FileHandler fileHandler)
    {
        
        AnsiConsole.MarkupLine("\n[cyan1]=-=-=[/] [yellow]Add New Patient[/] [cyan1]=-=-=[/]");

        // Ask for the Date of Visit
        DateTime dateOfVisit;
        do
        {
            Console.Write("Date of Visit (DD/MM/YYYY): ");
        } while (!DateTime.TryParse(Console.ReadLine(), out dateOfVisit));

        // Generate a unique patient ID
        string id = GeneratePatientId(); // Get a unique ID
        string name;
        do
        {
            Console.Write("Name: ");
            name = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(name));

        // Validate Age input 
        int age;
        do
        {
            Console.Write("Age: ");
        } while (!int.TryParse(Console.ReadLine(), out age) || age <= 0);

        // Validate Gender input (it shouldn't be empty)
        string gender;
        do
        {
            Console.Write("Sex: ");
            gender = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(gender));

        // Validate Purok input
        int purok;
        do
        {
            Console.Write("Purok: ");
        } while (!int.TryParse(Console.ReadLine(), out purok) || purok <= 0);
        // Retry if input is not a positive number

        // Validate Medical History input 
        string history;
        do
        {
            Console.Write("Medical History: ");
            history = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(history));
        // Repeat until a valid medical history is entered

        string prescription;
        do
        {
            Console.Write("Prescription: ");
            prescription = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(prescription));

        string householdSerialNumber;
        do
        {
            Console.Write("Household Serial Number: ");
            householdSerialNumber = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(householdSerialNumber));

        string contactNumber;
        do
        {
            Console.Write("Contact Number: ");
            contactNumber = Console.ReadLine();
            
        } while (string.IsNullOrWhiteSpace(contactNumber) || !contactNumber.All(char.IsDigit));
       

        var patient = new Patient(id, name, age, gender, purok, history, prescription, householdSerialNumber, contactNumber, dateOfVisit);

        patients.Add(patient);

        // Saving updated patient list to file
        fileHandler.SavePatients(patients);

        Console.WriteLine("\nPatient added successfully!");
        AnsiConsole.MarkupLine($"Patient ID [yellow]==>[/]  [cyan1]{id}[/]  [yellow]<==[/]");
        AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
        
        Console.ReadKey();
    }

    public void UpdatePatient(List<Patient> patients, FileHandler fileHandler)
    {
        AnsiConsole.MarkupLine("\n[cyan1]=-=-=[/] [yellow]Update Patient[/] [cyan1]=-=-=[/]");
        Console.Write("Patient ID or Name to update: ");
        string Input = Console.ReadLine();
        var patientToUpdate = patients.FirstOrDefault(p =>
            p.Id.Equals(Input, StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals(Input, StringComparison.OrdinalIgnoreCase));

        // Use patientToUpdate directly, no need for another find
        if (patientToUpdate != null)
        {
            Console.Write("New Name (leave blank to keep current): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)) patientToUpdate.Name = name;

            Console.Write("New Age (leave blank to keep current): ");
            string ageInput = Console.ReadLine();
            if (int.TryParse(ageInput, out int age)) patientToUpdate.Age = age;

            Console.Write("New Purok (leave blank to keep current): ");
            string purokInput = Console.ReadLine();
            if (int.TryParse(purokInput, out int purok)) patientToUpdate.Purok = purok;

            Console.Write("New Medical History (leave blank to keep current): ");
            string history = Console.ReadLine();
            if (!string.IsNullOrEmpty(history)) patientToUpdate.MedicalHistory = history;

            Console.Write("New Prescription (leave blank to keep current): ");
            string prescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(prescription)) patientToUpdate.Prescription = prescription;

            fileHandler.SavePatients(patients);
            AnsiConsole.MarkupLine("Patient [aqua]updated[/] successfully!");
            AnsiConsole.MarkupLine("\n[green]Press any key to continue...[/]");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Patient not found.");
            AnsiConsole.MarkupLine("\n[green]Press any key to continue...[/]");
            Console.ReadKey();
        }
    }


    public void DeletePatient(List<Patient> patients, FileHandler fileHandler)
    {
        
        AnsiConsole.MarkupLine("\n[cyan1]=-=-=[/] [yellow]Delete Patient[/] [cyan1]=-=-=[/]");
        Console.Write("Enter Patient ID or Name to delete: ");
        string input = Console.ReadLine();

        // Search for the patient by ID or Name (case-insensitive)
        var patientToDelete = patients.FirstOrDefault(p =>
            p.Id.Equals(input, StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals(input, StringComparison.OrdinalIgnoreCase));

        if (patientToDelete != null)
        {
            Console.WriteLine($"Patient found: {patientToDelete.Name} (ID: {patientToDelete.Id}).");
            Console.Write("Are you sure you want to delete this patient? (yes/no): ");
            string confirmation = Console.ReadLine();
            
            if (confirmation?.ToLower() == "yes")
            {
                patients.Remove(patientToDelete);
                fileHandler.SavePatients(patients); // Save changes to the file
                AnsiConsole.MarkupLine("Patient [red]deleted[/] successfully!");
                AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Operation canceled.");
                AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
                Console.ReadKey();
            }
        }
        else
        {
            Console.WriteLine("No patient found with the given ID or Name.");
            AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
            Console.ReadKey();
        }
    }

    public void SearchPatient(List<Patient> patients)
    {
       
        
        AnsiConsole.MarkupLine("\n[cyan1]=-=-=[/] [yellow]Search Patient[/] [cyan1]=-=-=[/]");
        Console.Write("Patient ID or Name to search: ");
        string input = Console.ReadLine()?.Trim();  // Handle potential null and spaces

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Search term cannot be empty.");
            return;
        }

        var matchingPatients = patients.Where(p =>
            p.Id.Equals(input, StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals(input, StringComparison.OrdinalIgnoreCase)).ToList();

        if (matchingPatients.Any())
        {
            
            var table = new Table();
            table.AddColumn("Date of Visit");
            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Age");
            table.AddColumn("Gender");
            table.AddColumn("Medical History");
            table.AddColumn("Prescription");
            table.AddColumn("Contact Number");
            table.AddColumn("Address");
           
            table.Title = new TableTitle("\n[cyan1]=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=[/] [yellow]P A T I E N T'S   I N F O R M A T I O N[/] [cyan1]=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=[/]");
           

            foreach (var patient in matchingPatients)
            {
                string address = $"Purok-{patient.Purok}, {patient.HouseholdSerialNumber}, Brgy. Bayogo, Madrid, Surigao del Sur";
                table.AddRow(
                    patient.DateOfVisit.ToShortDateString(),
                    patient.Id,
                    patient.Name,
                    patient.Age.ToString(),
                    patient.Gender,
                    patient.MedicalHistory,
                    patient.Prescription,
                    patient.ContactNumber,
                    address
                );
            }

            AnsiConsole.Write(table);  // Display the table with Spectre.Console

            // Wait for user input before returning to the menu
            AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
            Console.ReadKey();  // This will keep the table visible until the user presses a key
        }
        else
        {
            Console.WriteLine("No patient found with the given ID or Name.");
            AnsiConsole.Markup("\n[green]Press any key to continue...[/]");
            Console.ReadKey();  // Wait for user input before returning to the menu
        }
    }

}
