using Microsoft.AspNetCore.Mvc;

namespace AuraTest.Controllers
{
    public class ActionsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActionsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            string logDirectoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Logs");
            string logFilePath = Path.Combine(logDirectoryPath, "edit_log.txt");
            if (System.IO.File.Exists(logFilePath))
            {
                string logContent = System.IO.File.ReadAllText(logFilePath);
                return View("Index", logContent);
            }
            else
            {
                // Handle case where log file does not exist
                return View("Index", "Log file not found.");
            }
           
        }
        [HttpPost]
        public IActionResult ClearLog()
        {
            string logDirectoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Logs");
            string logFilePath = Path.Combine(logDirectoryPath, "edit_log.txt");

            if (System.IO.File.Exists(logFilePath))
            {
                System.IO.File.WriteAllText(logFilePath, string.Empty);
            }

            return RedirectToAction("Index");
        }
    }
}
