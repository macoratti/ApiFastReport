namespace ApiFastReport.Models;

public class Product
{
    public int ProductID { get; set; }
    public string? ProductName { get; set; }
    public short UnitsInStock { get; set; }
    public decimal UnitPrice { get; set; }
}
