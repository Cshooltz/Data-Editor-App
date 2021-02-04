using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Reflection;
using SparkLib;

public class DataTable : Panel, IDataParser
{
    /*
        Done:
            - Table size manipulation
            - Initial data loading method
            - Enabling/disabling user data editing
        Todo:
            - Let user select data source
            - Let user load data
            - Save data method
            - Add system for typing data
                - Enum for possible types?
                - overloaded methods for specifying types?
                - ???
    */

    /*
        Data spec:
            The table doesn't care how the data is stored.
            It cares about getting the data in the right format to parse.
            Namely, a series of objects that have named fields.
            Basically something like this dict:
            {
                {Row1: {Column1: data, Column2: data, Column3: data}}
                {Row2: {Column1: data2, Column2: data2, Column3: data2}}
                {Row3: {Column1: data3, Column2: data3, Column3: data3}}
            }
            Each column should have the same data type, though I could make a table
            that supports arbitrary objects.
            The biggest things are the data needs to be arranged in objects (rows),
            each field needs a string name (column header) and a type, and each
            field needs a valid ToString() function. 
    */
    protected const int BASECOLUMNIDX = 2; // Start iterating on columns from here

    protected int _Rows = 1; // Min 1, max whatever
    protected int _Columns = 4; // Min 1, max whatever
    protected bool _Edit = false;
    protected VBoxContainer? ColumnTemplate;
    protected Label? RowsLabel;
    protected Label? ColumnsLabel;
    protected HBoxContainer? ColumnsNode;
    protected VBoxContainer? IDCol;


    protected int Rows
    {
        get { return _Rows; }
        set
        {
            _Rows = value;
            RowsLabel!.Text = "Rows: " + _Rows;
        }
    }

    protected int Columns
    {
        get { return _Columns; }
        set
        {
            _Columns = value;
            ColumnsLabel!.Text = "Cols: " + _Columns;
        }
    }

    public bool Edit
    {
        get { return _Edit; }
        set
        {
            SetEditMode(value);
            _Edit = value;
        }
    }

    protected void SetEditMode(bool Editable)
    {
        // Foreach column, add a new LineEdit.
        Godot.Collections.Array ColList = ColumnsNode!.GetChildren();
        // Columns 0-3 map to children 2-5
        for (int i = BASECOLUMNIDX; i < (BASECOLUMNIDX + Columns); i++)
        {
            Node Col = (ColList[i] as Node)!;
            Godot.Collections.Array ColChildren = Col.GetChildren();
            int count = Col.GetChildCount();
            for (int j = 0; j < count; j++)
            {
                (ColChildren[j] as LineEdit)!.Editable = Editable;
            }
        }
        return;
    }


    public DataTable()
    {

    }
    public override void _Ready()
    {
        ColumnTemplate = GetNode<VBoxContainer>("ScrollContainer/Table/Columns/ColumnTemplate");
        RowsLabel = GetNode<Label>("ScrollContainer/Table/TableControls/RowsLabel");
        ColumnsLabel = GetNode<Label>("ScrollContainer/Table/TableControls/ColumnsLabel");
        ColumnsNode = GetNode<HBoxContainer>("ScrollContainer/Table/Columns");
        IDCol = ColumnsNode.GetNode<VBoxContainer>("IDCol");
    }

    public void LoadData(string[] columnNames, object[][] data)
    {
        ParseDoubleArray<object>(columnNames, data);
    }

    public void LoadData<T>(SparkLib.DataObject<T> data)
    {
        ParseDataObject<T>(data);
    }

    // Takes a title and column index starting at 0. 
    public void SetColumnTitle(string title, int column)
    {
        if (column < 0 || column >= Columns)
        {
            GD.PrintErr("DataTable.SetColumnTitle: Attempted to set column title on out of range column.");
            return;
        }
        // Get Column
        Node Col = ColumnsNode!.GetChild(BASECOLUMNIDX + column)!;
        // Set text in first LineEdit
        Col.GetChild<LineEdit>(0).Text = title;
    }

    public void SetColumns(int num)
    {
        if (num < 1)
        {
            GD.PrintErr("DataTable.SetColumns: Cannot set columns to value less than 1.");
            return;
        }
        else if (num == Columns)
        {
            return;
        }
        else if (Columns > num) // Want to shrink Columns until it equals num
        {
            while (Columns > num)
            {
                RemoveColumn();
            }
        }
        else if (Columns < num) // Want to grow Columns until it equals num
        {
            while (Columns < num)
            {
                AddColumn();
            }
        }
        return;
    }

    public void SetRows(int num)
    {
        if (num < 1)
        {
            GD.PrintErr("DataTable.SetRows: Cannot set rows to value less than 1.");
            return;
        }
        else if (num == Rows)
        {
            return;
        }
        else if (Rows > num) // Want to shrink Rows until it equals num
        {
            while (Rows > num)
            {
                RemoveRow();
            }
        }
        else if (Rows < num) // Want to grow Rows until it equals num
        {
            while (Rows < num)
            {
                AddRow();
            }
        }
        return;
    }

    // Iterates through all of the columns in the table
    // and duplicates that last Label Node in each and clears the text.
    public void AddRow()
    {
        // Add a new ID row.
        Label NewIDLabel = (Label)IDCol!.GetChild(IDCol.GetChildCount() - 1)!.Duplicate();
        NewIDLabel.Text = (Rows).ToString();
        IDCol.AddChild(NewIDLabel);

        // Foreach column, add a new LineEdit.
        Godot.Collections.Array ColList = ColumnsNode!.GetChildren();
        // Columns 0-3 map to children 2-5
        for (int i = BASECOLUMNIDX; i < (BASECOLUMNIDX + Columns); i++)
        {
            Node Col = (ColList[i] as Node)!;
            LineEdit NewRow = (LineEdit)Col.GetChild(Col.GetChildCount() - 1)!.Duplicate()!;
            NewRow!.Text = "";
            Col.AddChild(NewRow);
        }
        Rows++;
        return;
    }

    // Iterates through all of the columns in the table
    // and removes the last Lable Node from each.
    public void RemoveRow()
    {
        if (Rows - 1 < 1)
        {
            GD.PrintErr("DataTable.RemoveRow: Could not remove row, rows cannot be less than 1.");
            return;
        }

        // Remove the last ID row.
        Label TrashIDLabel = (Label)IDCol!.GetChild(IDCol.GetChildCount() - 1)!;
        IDCol.RemoveChild(TrashIDLabel);
        TrashIDLabel.QueueFree();

        // Foreach column, add a new LineEdit.
        Godot.Collections.Array ColList = ColumnsNode!.GetChildren();
        // Columns 0-3 map to children 2-5
        for (int i = BASECOLUMNIDX; i < (BASECOLUMNIDX + Columns); i++)
        {
            Node Col = (ColList[i] as Node)!;
            LineEdit TrashRow = (LineEdit)Col.GetChild(Col.GetChildCount() - 1)!;
            Col.RemoveChild(TrashRow);
        }
        Rows--;
        return;
    }

    // Adds a new column using an existing 
    // column as a template.
    public void AddColumn()
    {
        VBoxContainer NewColumn = (VBoxContainer)ColumnsNode!.GetChild(ColumnsNode.GetChildCount() - 1)!.Duplicate();
        int count = NewColumn.GetChildCount();
        Godot.Collections.Array NewColumnChildren = NewColumn.GetChildren();
        for (int i = 0; i < count; i++)
        {
            (NewColumnChildren[i] as LineEdit)!.Text = "";
        }
        ColumnsNode!.AddChild(NewColumn);
        Columns++;
        return;
    }

    public void AddColumn(string columnName)
    {
        VBoxContainer NewColumn = (VBoxContainer)ColumnsNode!.GetChild(ColumnsNode.GetChildCount() - 1)!.Duplicate();
        int count = NewColumn.GetChildCount();
        Godot.Collections.Array NewColumnChildren = NewColumn.GetChildren();
        (NewColumnChildren[0] as LineEdit)!.Text = columnName;
        for (int i = 1; i < count; i++)
        {
            (NewColumnChildren[i] as LineEdit)!.Text = "";
        }
        ColumnsNode!.AddChild(NewColumn);
        Columns++;
        return;
    }

    public void RemoveColumn()
    {
        if (Columns - 1 < 1)
        {
            GD.PrintErr("DataTable.RemoveColumn: Could not remove column, columns cannot be less than 1.");
            return;
        }
        VBoxContainer TrashColumn = (VBoxContainer)ColumnsNode!.GetChild(ColumnsNode.GetChildCount() - 1)!;
        ColumnsNode.RemoveChild(TrashColumn);
        TrashColumn.QueueFree();
        Columns--;
        return;
    }

    // How to reorder rows? How to drag and drop?

    // SIGNALS
    protected void _OnAddRowPressed()
    {
        AddRow();
    }

    protected void _OnRemoveRowPressed()
    {
        RemoveRow();
    }

    protected void _OnAddColumnPressed()
    {
        AddColumn();
    }

    protected void _OnRemoveColumnPressed()
    {
        RemoveColumn();
    }

    protected void _OnEditToggleToggled(bool buttonPressed)
    {
        Edit = buttonPressed;
    }

    // IDataParser
    public void ParseDataObject<T>(SparkLib.DataObject<T> data)
    {
        SetColumns(data.fieldNames.Length);
        SetRows(data.Count);
        int col = 0;
        foreach (string name in data.fieldNames)
        {
            SetColumnTitle(name, col);
            col++;
        }
        int row = 1;
        foreach (T obj in data)
        {
            col = 0;
            object[] valueList = data.GetEntryData(row - 1);
            foreach (object value in valueList)
            {
                Node ColNode = ColumnsNode!.GetChild(BASECOLUMNIDX + col);
                ColNode.GetChild<LineEdit>(row).Text = value.ToString();
                col++;
            }
            row++;
        }
    }
    public void ParseDictOfLists<T, U>(System.Collections.Generic.Dictionary<T, U[]> data)
    {
        SetColumns(data.Count);
        SetRows(1);
        int col = 0;
        foreach (KeyValuePair<T, U[]> item in data)
        {
            SetColumnTitle(item!.Key!.ToString(), col);
            Node ColNode = ColumnsNode!.GetChild(BASECOLUMNIDX + col);
            int row = 1;
            foreach (object? obj in item.Value)
            {
                ColNode.GetChild<LineEdit>(row).Text = obj!.ToString();
                row++;
            }
            col++;
        }
        return;
    }
    public void ParseListOfDicts<T, U>(System.Collections.Generic.Dictionary<T, U>[] data)
    {
        GD.Print("ParseListOfDicts: Not Implemented.");
        return;
    }
    public void ParseSingleArray<T>(string name, T[] data)
    {
        SetColumns(1);
        SetRows(data.Length);
        SetColumnTitle(name, 0);
        int row = 1;
        Node ColNode = ColumnsNode!.GetChild(BASECOLUMNIDX);
        foreach (T obj in data)
        {
            ColNode.GetChild<LineEdit>(row).Text = obj!.ToString();
            row++;
        }
        return;
    }
    public void ParseDoubleArray<T>(string[] names, T[][] data)
    {
        // data[i][j] - each entry i should be an array containing
        // the data of an object, the data of which can be accessed via j.
        // The length of array j must match length of array columnNames.
        // Or does it? You could have unnamed columns. Or truncate data. 
        SetColumns(names.Length);
        SetRows(data.Length);
        int col = 0;
        foreach (string name in names)
        {
            SetColumnTitle(name, col);
            col++;
        }
        int row = 1;
        foreach (T[] objArray in data)
        {
            // Iterates each row
            //GD.Print("In row iteration: ");
            //GD.Print(row);
            //GD.Print(col);
            col = 0;
            foreach (T obj in objArray)
            {
                // Iterates over columns
                //GD.Print("In column iteration: " + col);
                Node ColNode = ColumnsNode!.GetChild(BASECOLUMNIDX + col);
                ColNode.GetChild<LineEdit>(row).Text = obj!.ToString();
                col++;
            }
            row++;
        }
    }
}
