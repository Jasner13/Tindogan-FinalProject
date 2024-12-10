using System.Collections.Generic;

public interface IPatientManagement
{
    void AddPatient(List<Patient> patients, FileHandler fileHandler);
    void UpdatePatient(List<Patient> patients, FileHandler fileHandler);
    void DeletePatient(List<Patient> patients, FileHandler fileHandler);
    void SearchPatient(List<Patient> patients);
}

public interface IDrugManagement
{
    void AddDrug(List<DrugInventory> inventory, FileHandler fileHandler);
    void UpdateDrug(List<DrugInventory> inventory, FileHandler fileHandler);
    void DeleteDrug(List<DrugInventory> inventory, FileHandler fileHandler);
    void ShowInventory(List<DrugInventory> inventory);
}
