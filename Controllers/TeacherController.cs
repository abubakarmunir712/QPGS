using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QPGS.Models;

namespace QPGS.Controllers;

public class TeacherController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Classes()
    {
        return View();
    }
    public IActionResult Subjects(){
        return View();
    }

}