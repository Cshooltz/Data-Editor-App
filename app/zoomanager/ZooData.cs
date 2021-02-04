using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using Newtonsoft.Json; // Full featured JSON serializer
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

public class ZooData
{
    private int _ZooID = 0;
    private int _AnimalID = 0;
    private int _ZooAnimalID = 0;
    private int ZooID
    {
        get
        {
            _ZooID++;
            return _ZooID - 1;
        }
    }
    private int AnimalID
    {
        get
        {
            _AnimalID++;
            return _AnimalID - 1;
        }
    }
    private int ZooAnimalID
    {
        get
        {
            _ZooAnimalID++;
            return _ZooAnimalID - 1;
        }
    }

    protected SparkLib.DataObject<Zoo> _Zoos = new SparkLib.DataObject<Zoo>();
    protected SparkLib.DataObject<Animal> _Animals = new SparkLib.DataObject<Animal>();
    protected SparkLib.DataObject<ZooAnimal> _ZooAnimals = new SparkLib.DataObject<ZooAnimal>();

    public SparkLib.DataObject<Zoo> Zoos { get => _Zoos; set { } }
    public SparkLib.DataObject<Animal> Animals { get => _Animals; set { } }
    public SparkLib.DataObject<ZooAnimal> ZooAnimals { get => _ZooAnimals; set { } }

    public ZooData()
    {

    }

    public void AddZoo(string name)
    {
        if (_Zoos.Exists(zoo => zoo.Name == name))
        {
            GD.Print($"Zoo {name} already present.");
            return;
        }
        else
        {
            _Zoos.Add(new Zoo() { ID = ZooID, Name = name });
        }
    }

    public void RemoveZoo(int id)
    {
        int i = _Zoos.FindIndex(zoo => zoo.ID == id);
        if (i != -1)
        {
            _Zoos.RemoveAt(i);
        }
        return;
    }

    public Zoo GetZoo(string name)
    {
        return _Zoos.Find(zoo => zoo.Name.Equals(name));
    }

    public void AddAnimal(string name)
    {
        if (_Animals.Exists(animal => animal.Name.Equals(name)))
        {
            GD.Print($"Animal {name} already present.");
            return;
        }
        else
        {
            _Animals.Add(new Animal() { ID = AnimalID, Name = name });
        }
    }

    public void RemoveAnimal(int id)
    {
        int i = _Animals.FindIndex(animal => animal.ID == id);
        if (i != -1)
        {
            _Animals.RemoveAt(i);
        }
        return;
    }
    public Animal GetAnimal(string name)
    {
        return _Animals.Find(animal => animal.Name.Equals(name));
    }

    public void AddZooAnimal(Zoo zoo, Animal animal)
    {
        var AnimalsInZoo = (from zooanimal in _ZooAnimals
                            where zooanimal.ZooID == zoo.ID
                            select zooanimal).ToList();

        if (AnimalsInZoo.Exists(za => za.AnimalID == animal.ID))
        {
            GD.Print($"Animal {animal.Name} already present in {zoo.Name} Zoo.");
            return;
        }
        else
        {
            _ZooAnimals.Add(new ZooAnimal() { ID = ZooAnimalID, ZooID = this.ZooID, AnimalID = this.AnimalID });
        }
        return;
    }

    public void RemoveZooAnimal(int id)
    {
        int i = _ZooAnimals.FindIndex(za => za.ID == id);
        if (i != -1)
        {
            _ZooAnimals.RemoveAt(i);
        }
        return;
    }


    /*
    public ZooAnimal GetZooAnimal(string name)
    {
        return (from zooanimal in _ZooAnimals
                where zooanimal.Name.Equals(name)
                select zooanimal).First();
    }
    */

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public async Task<string> ToJsonAsync()
    {
        return await Task.Run(() => JsonConvert.SerializeObject(this));
    }

    public override string ToString()
    {
        StringBuilder OutputString = new StringBuilder("");
        OutputString.AppendLine("Zoo list:");
        foreach (Zoo zoo in Zoos)
        {
            OutputString.AppendLine($"\t{zoo.ID}: {zoo.Name}");
        }
        OutputString.AppendLine("Animal list:");
        foreach (Animal animal in Animals)
        {
            OutputString.AppendLine($"\t{animal.ID}: {animal.Name}");
        }
        OutputString.AppendLine("Zoo Animal list:");
        foreach (ZooAnimal za in ZooAnimals)
        {
            OutputString.AppendLine($"\tID: {za.ID}, ZooID: {za.ZooID}, AnimalID: {za.AnimalID}");
        }
        return OutputString.ToString();
    }
}
public struct Zoo
{
    public int ID;
    public string Name;

    public object[] ToObjectArray()
    {
        return new object[] { ID, Name };
    }

    public string[] ToStringArray()
    {
        return new string[] { ID.ToString(), Name };
    }
}

public struct Animal
{
    public int ID;
    public String Name;

    public object[] ToObjectArray()
    {
        return new object[] { ID, Name };
    }

    public string[] ToStringArray()
    {
        return new string[] { ID.ToString(), Name };
    }
}

public struct ZooAnimal
{
    public int ID;
    public int ZooID;
    public int AnimalID;

    public object[] ToObjectArray()
    {
        return new object[] { ID, ZooID, AnimalID };
    }

    public int[] ToIntArray()
    {
        return new int[] { ID, ZooID, AnimalID };
    }

    public string[] ToStringArray()
    {
        return new string[] { ID.ToString(), ZooID.ToString(), AnimalID.ToString() };
    }
}