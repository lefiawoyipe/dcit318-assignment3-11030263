
using System;
using System.Collections.Generic;

// Marker Interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Products
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int qty, string brand, int warranty)
    {
        Id = id; Name = name; Quantity = qty; Brand = brand; WarrantyMonths = warranty;
    }

    public override string ToString() => $"[E] {Name} (ID:{Id}) Brand:{Brand} Quantity:{Quantity} Warranty:{WarrantyMonths}months";
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int qty, DateTime expiry)
    {
        Id = id; Name = name; Quantity = qty; ExpiryDate = expiry;
    }

    public override string ToString() => $"[G] {Name} (ID:{Id}) Qty:{Quantity} Exp:{ExpiryDate:d}";
}

// Custom exceptions
public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) {} }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) {} }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) {} }

// Generic repo
public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id)) throw new DuplicateItemException($"Item with ID {item.Id} exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var it)) throw new ItemNotFoundException($"Item {id} not found.");
        return it;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id)) throw new ItemNotFoundException($"Item {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id); 
        item.Quantity = newQuantity;
    }
}

// Manager
public class WareHouseManager
{
    public InventoryRepository<ElectronicItem> _electronics = new();
    public InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));

        _groceries.AddItem(new GroceryItem(101, "Milk", 50, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(102, "Rice", 100, DateTime.Now.AddMonths(6)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach(var item in repo.GetAllItems()) Console.WriteLine(item);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Increased item {id} by {quantity}. New qty: {repo.GetItemById(id).Quantity}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed item {id}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        Console.WriteLine("Groceries:");
        manager.PrintAllItems(manager._groceries);
        Console.WriteLine();
        Console.WriteLine("Electronics:");
        manager.PrintAllItems(manager._electronics);
        Console.WriteLine();

       
        try
        {
            Console.WriteLine("Adding duplicate electronic item...");
            manager._electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Lenovo", 12));
        }
        catch(Exception ex) { Console.WriteLine($"Caught: {ex.Message}"); }

        manager.RemoveItemById(manager._groceries, 999); 
        try { manager._groceries.UpdateQuantity(101, -5); } catch(Exception ex) { Console.WriteLine($"Caught: {ex.Message}"); }
    }
}
