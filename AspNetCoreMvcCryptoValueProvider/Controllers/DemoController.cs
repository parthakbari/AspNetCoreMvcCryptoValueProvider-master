using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreMvcCryptoValueProvider.Models;

namespace AspNetCoreMvcCryptoValueProvider.Controllers
{
    public class DemoController : Controller
    {

        public IActionResult Example1(int param1, string param2)
        {
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            return View();
        }
        [CryptoValueProvider]
        public IActionResult Example2(int SECRETPERSONID, string SECRETPARAM2, Person person)
        {
            person.PersonId = SECRETPERSONID;
            //_repository.UpdatePerson(person);

            ViewBag.secretPersonId = SECRETPERSONID;
            ViewBag.secretParam2 = SECRETPARAM2;
            ViewBag.person = person;

            return View();
        }
    }
}