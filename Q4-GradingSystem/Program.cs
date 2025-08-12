
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string m) : base(m) {} }
public class MissingFieldException : Exception { public MissingFieldException(string m) : base(m) {} }

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score) { Id = id; FullName = fullName; Score = score; }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }

    public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();
        using var sr = new StreamReader(inputFilePath);
        string? line;
        int lineNumber = 0;
        while ((line = sr.ReadLine()) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 3) throw new MissingFieldException($"Line {lineNumber}: missing fields.");
            if (!int.TryParse(parts[0], out int id)) throw new InvalidScoreFormatException($"Line {lineNumber}: invalid ID.");
            string name = parts[1];
            if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int score))
            {
                throw new InvalidScoreFormatException($"Line {lineNumber}: score '{parts[2]}' not an integer.");
            }
            students.Add(new Student(id, name, score));
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var sw = new StreamWriter(outputFilePath, false);
        foreach(var s in students)
        {
            sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
        }
    }
}

class Program
{
    static void Main()
    {
        var processor = new StudentResultProcessor();
        string input = "students.txt";
        string output = "report.txt";
        try
        {
            var students = processor.ReadStudentsFromFile(input);
            processor.WriteReportToFile(students, output);
            Console.WriteLine($"Report written to {output}");
            foreach(var s in students) Console.WriteLine(s);
        }
        catch(FileNotFoundException)
        {
            Console.WriteLine("Input file not found.");
        }
        catch(InvalidScoreFormatException ex)
        {
            Console.WriteLine("Invalid score format: " + ex.Message);
        }
        catch(MissingFieldException ex)
        {
            Console.WriteLine("Missing field: " + ex.Message);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
    }
}
