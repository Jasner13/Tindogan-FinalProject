using System;
using System.Collections.Generic;
using System.IO;

public class FileHandler
{
    private string filePath;

    public FileHandler(string path)
    {
        filePath = path;
    }

    // Method to load patients from file with error handling
    public List<Patient> LoadPatients()
    {
        var patients = new List<Patient>();

        try
        {
            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var data = line.Split(',');

                    // Adjust the number of fields to match the new properties
                    if (data.Length >= 9) // 9 fields now (added DateOfVisit)
                    {
                        try
                        {
                            var patient = new Patient(
                                data[0], // ID
                                data[1], // Name
                                int.TryParse(data[2], out int age) ? age : 0,  // Fallback to 0 if parse fails
                                data[3], // Gender
                                int.TryParse(data[4], out int purok) ? purok : 0, // Fallback to 0 if parse fails
                                data[5], // MedicalHistory
                                data[6], // Prescription
                                data[7], // HouseholdSerialNumber
                                data[8], // ContactNumber
                                DateTime.TryParse(data[9], out DateTime dateOfVisit) ? dateOfVisit : DateTime.MinValue // DateOfVisit, fallback to MinValue if parsing fails
                            );
                            patients.Add(patient);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing patient data: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Patients file not found.");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied to the file: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error while accessing the file: {ex.Message}");
        }

        return patients;
    }

    // Method to save patients to file with error handling
    public void SavePatients(List<Patient> patients)
    {
        try
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var patient in patients)
                {
                    // Patient's data
                    writer.WriteLine($"{patient.Id},{patient.Name},{patient.Age},{patient.Gender},{patient.Purok}," +
                        $"{patient.MedicalHistory},{patient.Prescription},{patient.HouseholdSerialNumber}," +
                        $"{patient.ContactNumber},{patient.DateOfVisit:yyyy-MM-dd}"); // Date format
                }
            }

            if (patients.Count == 0)
            {
                Patient.ResetPatientIdCounter(); // Reset the static counter
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied when trying to save data: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error while saving data: {ex.Message}");
        }
        
    }

    // Method to load drugs from file with error handling
    public List<DrugInventory> LoadDrugs()
    {
        var drugs = new List<DrugInventory>();

        try
        {
            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var data = line.Split(',');
                    if (data.Length >= 4)
                    {
                        try
                        {
                            var drug = new DrugInventory(
                                data[0],
                                data[1],
                                int.TryParse(data[2], out int quantity) ? quantity : 0, // Fallback to 0 if parse fails
                                DateTime.TryParse(data[3], out DateTime expiryDate) ? expiryDate : DateTime.MinValue // Fallback to MinValue if parse fails
                            );
                            drugs.Add(drug);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing drug data: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Drugs file not found.");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied to the file: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error while accessing the file: {ex.Message}");
        }

        return drugs;
    }

    // Method to save drugs to file with error handling
    public void SaveDrugs(List<DrugInventory> drugs)
    {
        try
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var drug in drugs)
                {
                    writer.WriteLine($"{drug.DrugID},{drug.Name},{drug.Quantity},{drug.ExpiryDate}");
                }
            }

            // If the inventory is empty, reset the drug ID counter
            if (drugs.Count == 0)
            {
                DrugInventory.ResetDrugIdCounter(); // Reset the static counter
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied when trying to save data: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error while saving data: {ex.Message}");
        }
    }
}
