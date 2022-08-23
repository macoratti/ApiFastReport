using ApiFastReport.Context;
using ApiFastReport.Models;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ApiFastReport.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [Route("list")]
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<Product>>> ListProducts()
    {
        try
        {
            var products = await _context.Products.ToListAsync();

            if (products is null)
                return NotFound();

            return Ok(products);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Route("report")]
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<Product>>> ReportProducts()
    {
        try
        {
            var products = await _context.Products.ToListAsync();

            if (products is null)
                return NotFound();

            var webReport = new WebReport();

            webReport.Report.Load(Path.Combine(_webHostEnvironment.ContentRootPath, 
                "wwwroot/reports", "productsreport.frx"));

            GenerateDataTableReport(products, webReport);

            webReport.Report.Prepare();

            using MemoryStream stream = new MemoryStream();

            webReport.Report.Export(new PDFSimpleExport(), stream);

            stream.Flush();
            byte[] arrayReport = stream.ToArray();

            return File(arrayReport, "application/zip", "ProductsReport.pdf");
        }
        catch (Exception)
        {

            throw;
        }
    }

    private void GenerateDataTableReport(List<Product> products, WebReport webReport)
    {
        var productsDataTable = new DataTable();
        productsDataTable.Columns.Add("ProductName", typeof(string));
        productsDataTable.Columns.Add("UnitsInStock", typeof(int));
        productsDataTable.Columns.Add("UnitPrice", typeof(decimal));

        foreach (var item in products)
        {
            productsDataTable.Rows.Add(item.ProductName,
                           item.UnitsInStock, item.UnitPrice);
        }
        //registra o datatable para usar no relatorio
        webReport.Report.RegisterData(productsDataTable, "Products");
    }
}
