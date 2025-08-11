
using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private readonly List<T> items = new();
    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new List<T>(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item == null) return false;
        items.Remove(item);
        return true;
    }
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }
    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }
    public override string ToString() => $"{Name} (ID:{Id}) Age:{Age} Gender:{Gender}";
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }
    public Prescription(int id, int patientId, string med, DateTime date)
    {
        Id = id; PatientId = patientId; MedicationName = med; DateIssued = date;
    }
    public override string ToString() => $"Prescription {Id}: {MedicationName} on {DateIssued:d}";
}

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Kwame Ntim", 30, "M"));
        _patientRepo.Add(new Patient(2, "John Boakye", 45, "M"));
        _patientRepo.Add(new Patient(3, "Leslie Elikem", 29, "F"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Vitamin D", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Ibuprofen", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Aspirin", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach(var p in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(p.PatientId))
                _prescriptionMap[p.PatientId] = new List<Prescription>();
            _prescriptionMap[p.PatientId].Add(p);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("Patients:");
        foreach(var pat in _patientRepo.GetAll()) Console.WriteLine($" - {pat}");
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        Console.WriteLine($"Prescriptions for patient {patientId}:");
        if (_prescriptionMap.TryGetValue(patientId, out var list))
        {
            foreach(var p in list) Console.WriteLine($" - {p}");
        }
        else Console.WriteLine(" - None found");
    }
}

class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        Console.WriteLine();
        app.PrintPrescriptionsForPatient(1); 
    }
}
    