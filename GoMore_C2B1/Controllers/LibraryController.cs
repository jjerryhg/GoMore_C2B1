using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoMore_C2B1.Controllers
{
    public class LibraryController : Controller
    {
        // GET: Library
        public ActionResult ModelLibrary()
        {
            return View();
        }
        public ActionResult UploadedModel()
        {
            return View();
        }
    }
}