using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRTest.Models;

namespace SignalRTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext db;
        private readonly IHubContext<SignalServer> hubContext;
        public HomeController(DataContext _db, IHubContext<SignalServer> _hubContext)
        {
            db = _db;
            hubContext = _hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var people = await db.People.OrderByDescending(p=>p.Id).ToListAsync();
            return View(people);
        }

        [HttpPost]
        public async Task InsertPerson(Person person)
        {
            db.Add(person);
            await db.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<Person> GetLastPersona()
        {
            var person = await db.People.OrderByDescending(c=>c.Id).FirstOrDefaultAsync();
            return person;
        }
    }
}
