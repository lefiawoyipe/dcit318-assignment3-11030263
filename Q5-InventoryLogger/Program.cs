
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public interface IInventoryEntity { int Id { get; } }

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath) { _filePath = filePath; }

    public void Add(T item) => _log.Add(item);

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        using var fs = File.Create(_filePath);
        JsonSerializer.Serialize(fs, _log, options);
    }

    public void LoadFromFile()
    {
        if (!File.Exists(_filePath)) { _log = new List<T>(); return; }
        using var fs = File.OpenRead(_filePath);
        _log = JsonSerializer.Deserialize<List<T>>(fs) ?? new List<T>();
    }
}

public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;
    public InventoryApp(string path) { _logger = new InventoryLogger<InventoryItem>(path); }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Chair", 30, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Table", 15, DateTime.Now.AddDays(-1)));
        _logger.Add(new InventoryItem(3, "Lamp", 10, DateTime.Now.AddMonths(-1)));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach(var i in _logger.GetAll()) Console.WriteLine(i);
    }
}

class Program
{
    static void Main()
    {
        string path = "inventory.json";
        var app = new InventoryApp(path);

        app.SeedSampleData();
        app.SaveData();

        
        var app2 = new InventoryApp(path);
        app2.LoadData();
        Console.WriteLine("Loaded items:");
        app2.PrintAllItems();
    }
}
