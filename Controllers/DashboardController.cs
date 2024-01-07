using DOCUMENTMANAGER.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.IO.Compression;
using System.Text;

namespace DOCUMENTMANAGER.Controllers
{
    public class DashboardController : Controller
    {

        private readonly DOCMANAGERContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        public DashboardController(IFileProvider fileProvider, DOCMANAGERContext context, IWebHostEnvironment hostingEnvironment)
        {
            _fileProvider = fileProvider;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var documents = GetDocuments();
            return View(documents);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile profilePhoto, IFormFile aadharCard, IFormFile panCard, IFormFile voterId, IFormFile marksheet)
        {
           long? UserId = Convert.ToInt64(HttpContext.Session.GetString("User_Id"));
            // Your file upload logic for each document type
            if (profilePhoto != null)
            {
                if (profilePhoto.Length > 0)
                {
                    var fileName = Path.GetFileName(voterId.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads", profilePhoto.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePhoto.CopyToAsync(stream);
                    }


                    SaveDocumentToDatabase(fileName, filePath,UserId);
                }
            }

            if (aadharCard != null)
            {
                // Process Aadhar Card
                if (aadharCard.Length > 0)
                {
                    var fileName = Path.GetFileName(aadharCard.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads", aadharCard.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await aadharCard.CopyToAsync(stream);

                    }

                    SaveDocumentToDatabase(fileName, filePath, UserId);
                }
            }

            if (panCard != null)
            {
                // Process Pan Card
                if (panCard.Length > 0)
                {
                    var fileName = Path.GetFileName(panCard.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads", panCard.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await panCard.CopyToAsync(stream);

                    }

                    SaveDocumentToDatabase(fileName, filePath, UserId);
                }
            }

            if (voterId != null)
            {
                // Process Voter Id
                if (voterId.Length > 0)
                {
                    var fileName = Path.GetFileName(voterId.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads", voterId.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await voterId.CopyToAsync(stream);

                    }

                    SaveDocumentToDatabase(fileName, filePath, UserId);
                }
            }

            if (marksheet != null)
            {
                // Process Marksheet
                 if (marksheet.Length > 0)
                {
                    var fileName = Path.GetFileName(marksheet.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads", marksheet.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await marksheet.CopyToAsync(stream);

                    }

                    SaveDocumentToDatabase(fileName, filePath, UserId);
                }
            }

            // Redirect or return a response as needed
            return RedirectToAction("Index");
        }

        public IActionResult Download(string[] fileNames)
        {
            var memoryStream = new MemoryStream();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileName in fileNames)
                {

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
                    var entry = archive.CreateEntry(Path.GetFileName(filePath));

                    using (var entryStream = entry.Open())
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream, "application/zip", $"Documents_{DateTime.Now:yyyyMMddHHmmss}.zip");
        }


        private List<Document> GetDocuments()
        {
            long? UserId = Convert.ToInt64(HttpContext.Session.GetString("User_Id")); 
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            //var fileNames = Directory.GetFiles(directory).Select(Path.GetFileName).ToList();

            var Document = _context.Document.Where(c=> c.User_Id == UserId).Distinct().ToList();
            //return Document;
            

            return Document;    
        }
        
        private void SaveDocumentToDatabase(string fileName, string filePath,long? UserId)
        {
            // Assuming you're using Entity Framework or another ORM for database operations
            // Replace this with the actual code to save data to your database

            var document = new Document
            {
                FileName = fileName,
                FilePath = filePath,
                UploadDate = DateTime.Now.Date,
                User_Id = UserId
            };
            _context.Add(document);
            _context.SaveChanges();
        }
    }
}
