using System;
using System.Collections.Generic;

namespace Tetramorph.Doctor.Models;


public class ClientData
{
    public int Id { get; set; }
    public string CardNumber { get; set; }
    public string Name { get; set; }
    public List<Diagnosis> Diagnosis { get; set; }
    public Sex Sex { get; set; }
    public DateTime BirthDate { get; set; }

    public override string ToString()
    {
        return $"№{CardNumber} {Name}";
    }
}

public enum Sex
{
    Male,
    Female
}