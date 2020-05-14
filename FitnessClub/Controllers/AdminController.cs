using FitnessClub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace FitnessClub.Controllers
{
    public class AdminController : Controller
    {
        FitnessClubEntity db = new FitnessClubEntity();
        //public IQueryable<Client> client;
        public ActionResult Index()
        {
            //var visit = new Visiting()
            //{
            //    idVisiting = 14,
            //    idSeasonTicket = 10,
            //    date = now.Date
            //    };
            //db.Visiting.Add(visit);
            //db.SaveChanges();
            //client = db.Client;
            return View();
        }

        #region Client

        //public ActionResult Client()
        //{
        //    return View(db.Client);
        //}
        public ActionResult Client(string sort, string search)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sort) ? "name_desc" : "";
            ViewBag.DateSortParm = sort == "Date" ? "date_desc" : "Date";

            IQueryable<Client> users = db.Client;
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(p => p.surname.Contains(search));
            }
            switch (sort)
            {
                case "name_desc":
                    users = users.OrderByDescending(c => c.surname);
                    break;
                case "Date":
                    users = users.OrderBy(c => c.birthday);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(c => c.birthday);
                    break;
                default:
                    users = users.OrderBy(c => c.surname);
                    break;
            }

            return View(users);
        }
        public ActionResult ShowClient(int id)
        {
            Client client = db.Client.Where(cl => cl.idClient == id).FirstOrDefault();
            var tickets = (from c in db.Client
                           join st in db.SeasonTicket on c.idClient equals st.idClient
                           join s in db.Service on st.idService equals s.idService
                           where c.idClient == id
                           select s.name).ToList();
            ViewBag.TicketsClient = tickets;
            return View(client);
        }
        public ActionResult CreateClient()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateClient(Client client)
        {
            try
            {
                DateTime thisDay = DateTime.Today;
                if (client.birthday >= thisDay)
                {
                    ModelState.AddModelError("birthday", "Дата рождения не может быть позднее сегодняшнего дня");
                }
                if (ModelState.IsValid)
                {
                    db.Client.Add(client);
                    db.SaveChanges();
                    return RedirectToAction("Client");
                }
                else
                {
                    return View(client);
                }
            }
            catch (Exception)
            {
                return View();
            }

        }

        public ActionResult EditClient(int id)
        {
            return View(db.Client.Where(cl => cl.idClient == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditClient(int id, Client client)
        {
            try
            {
                client.idClient = id;
                db.Entry(client).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Client");
            }
            catch (Exception)
            {
                return View();
            }

        }
        #endregion


        //public ActionResult SeasonTicket()
        //{
        //    return View(db.SeasonTicket);
        //}

        [HttpGet]
        public ActionResult SeasonTicket(string surname, string service, DateTime? startdate, int? costfrom, int? costto, string sort)
        {
            var services = db.Service.ToList();
            ViewBag.Services = services.ToList();
            IQueryable<SeasonTicket> tickets = db.SeasonTicket;

            ViewBag.NameSortParm = sort == "name" ? "name_desc" : "name";
            ViewBag.DateSortParm = sort == "dateEnd" ? "date_desc" : "dateEnd";
            ViewBag.CostSortParm = sort == "cost" ? "cost_desc" : "cost";

            switch (sort)
            {
                case "name_desc":
                    tickets = tickets.OrderByDescending(st => st.Client.surname);
                    break;
                case "dateEnd":
                    tickets = tickets.OrderBy(st => st.dateEnd);
                    break;
                case "date_desc":
                    tickets = tickets.OrderByDescending(st => st.dateEnd);
                    break;
                case "name":
                    tickets = tickets.OrderBy(st => st.Client.surname);
                    break;
                case "cost":
                    tickets = tickets.OrderBy(st => st.totalCost);
                    break;
                case "cost_desc":
                    tickets = tickets.OrderByDescending(st => st.totalCost);
                    break;
            }

            if (!string.IsNullOrEmpty(surname))
            {
                tickets = tickets.Where(p => p.Client.surname.Contains(surname));
            }
            if (!string.IsNullOrEmpty(service))
            {
                tickets = tickets.Where(p => p.Service.name.Contains(service));
            }
            if (startdate != null)
            {
                tickets = tickets.Where(p => p.dateStart >= startdate);
            }
            if (costfrom != null)
            {
                tickets = tickets.Where(p => p.totalCost >= costfrom);
            }
            if (costto != null)
            {
                tickets = tickets.Where(p => p.totalCost <= costto);
            }
            //var output = new
            //{
            //    tickets = tickets,
            //    services = services
            //};
            return View(tickets);
        }

        public ActionResult EditSeasonTicket(int id)
        {
            SelectList services = new SelectList(db.Service, "name", "name");
            ViewBag.Services = services;
            SelectList clients = new SelectList(db.Client, "surname", "surname");
            ViewBag.Clients = clients;
            SelectList coaches = new SelectList(db.Coach, "surname", "surname");
            ViewBag.Coaches = coaches;
            return View(db.SeasonTicket.Where(st => st.idSeasonTicket == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditSeasonTicket(int id, SeasonTicket seasonTicket)
        {
            //try
            {
                seasonTicket.idSeasonTicket = id;

                int idService = db.Service.Where(s => s.name == seasonTicket.Service.name).FirstOrDefault().idService;
                seasonTicket.idService = idService;
                int idClient = db.Client.Where(c => c.surname == seasonTicket.Client.surname).FirstOrDefault().idClient;
                seasonTicket.idClient = idClient;
                int idCoach = db.Coach.Where(c => c.surname == seasonTicket.Coach.surname).FirstOrDefault().idCoach;
                seasonTicket.idCoach = idCoach;

                var st = db.SeasonTicket.Where(c => c.idSeasonTicket == seasonTicket.idSeasonTicket).FirstOrDefault();
                seasonTicket.saleAge = st.saleAge;
                seasonTicket.saleClasses = st.saleClasses;
                seasonTicket.saleСonstantСlient = st.saleСonstantСlient;
                seasonTicket.totalCost = st.totalCost;

                List<Visiting> vis = db.Visiting.Where(v => v.idSeasonTicket == id).ToList();
                for (int i = 0; i < seasonTicket.Visitings.Count(); i++)
                {
                    vis[i].date = seasonTicket.Visitings[i].date;
                }
                db.SaveChanges();
                //db.Entry(seasonTicket).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SeasonTicket");
            }
            //catch (Exception)
            //{
            //    return View();
            //}

        }

        public ActionResult ShowSeasonTicket(int id)
        {
            SeasonTicket seasonTicket = db.SeasonTicket.Where(st => st.idSeasonTicket == id).FirstOrDefault();
            //var visiting = (from st in db.SeasonTicket
            //                join st in db.SeasonTicket on c.idClient equals st.idClient
            //               join s in db.Service on st.idService equals s.idService
            //               where c.idClient == id
            //               select s.name).ToList();
            //ViewBag.TicketsClient = tickets;
            return View(seasonTicket);
        }
        [HttpGet]
        public ActionResult CreateSeasonTicket()
        {
            SelectList services = new SelectList(db.Service, "name", "name");
            ViewBag.Services = services;
            SelectList clients = new SelectList(db.Client, "surname", "surname");
            ViewBag.Clients = clients;
            var coaches = db.Coach.ToList();
            Coach coach = new Coach
            {
                surname = "нет"
            };
            coaches.Insert(0, coach);
            SelectList coachesSel = new SelectList(coaches, "surname", "surname");
            ViewBag.Coaches = coachesSel;
            DateTime thisDay = DateTime.Now;
            ViewBag.Day = thisDay;
            return View();
            return View(db.Service);
        }
        [HttpPost]
        public ActionResult CreateSeasonTicket(SeasonTicket seasonTicket)
        {
            try
            {
                if (seasonTicket.classesTotal == 0)
                {
                    ModelState.AddModelError("classesTotal", "Введите кол-во занятий");
                }
                DateTime thisDay = DateTime.Today;
                if (seasonTicket.dateStart < thisDay)
                {
                    ModelState.AddModelError("dateStart", "Нельзя вводить прошедшую дату");
                }
                if (seasonTicket.dateEnd < thisDay)
                {
                    ModelState.AddModelError("dateEnd", "Нельзя вводить прошедшую дату");
                }
                if (seasonTicket.dateEnd < seasonTicket.dateStart)
                {
                    ModelState.AddModelError("dateEnd", "Дата окончания раньше, чем дата начала");
                }

                //if (seasonTicket.dateEnd == DateTime.Parse("01.01.0001 0:00:00"))
                //{
                //    ModelState.AddModelError("dateEnd", "Введите дату окончания");
                //}

                if (seasonTicket.Coach.surname != "нет")
                    seasonTicket.idCoach = db.Coach.Where(c => c.surname == seasonTicket.Coach.surname).FirstOrDefault().idCoach;
                if (seasonTicket.idCoach != null)
                {
                    List<string> servicesCoach = (from c in db.Coach
                                                  join a in db.Activity on c.idCoach equals a.idCoach
                                                  join s in db.Service on a.idService equals s.idService
                                                  where c.idCoach == seasonTicket.idCoach
                                                  select s.name).ToList();

                    int check = 0;
                    foreach (var item in servicesCoach)
                    {
                        if (seasonTicket.Service.name == item)
                        {
                            check = 1;
                            break;
                        }
                    }
                    if (check == 0)
                    {
                        ModelState.AddModelError("Coach.surname", "Тренер не продоставляет выбранную услугу");
                        ViewBag.ServicesCoach = servicesCoach;
                    }
                }

                ModelState.Remove("Client.name");
                ModelState.Remove("Client.sex");
                ModelState.Remove("Coach.name");
                if (ModelState.IsValid)
                {
                    int idService = db.Service.Where(s => s.name == seasonTicket.Service.name).FirstOrDefault().idService;
                    seasonTicket.idService = idService;
                    int idClient = db.Client.Where(c => c.surname == seasonTicket.Client.surname).FirstOrDefault().idClient;
                    seasonTicket.idClient = idClient;

                    db.SeasonTicket.Add(seasonTicket);
                    db.SaveChanges();
                    return RedirectToAction("SeasonTicket");
                }
                else
                {
                    SelectList services = new SelectList(db.Service, "name", "name");
                    ViewBag.Services = services;
                    SelectList clients = new SelectList(db.Client, "surname", "surname");
                    ViewBag.Clients = clients;
                    SelectList coaches = new SelectList(db.Coach, "surname", "surname");
                    ViewBag.Coaches = coaches;
                    return View(seasonTicket);
                }
            }
            catch (Exception)
            {
                return View();
            }

        }

        public ActionResult AddVisiting(int id)
        {
            var client = (from st in db.SeasonTicket
                          join c in db.Client on st.idClient equals c.idClient
                          where st.idSeasonTicket == id
                          //select c.name)
                          select new
                          {
                              name = c.name,
                              surname = c.surname,
                          });
            foreach (var item in client)
            {
                ViewBag.Name = item.name;
                ViewBag.Surname = item.surname;
            }
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddVisiting(int id, Visiting visiting)
        {
            try
            {
                //var str = "01.01.0001 0:00:00";
                //DateTime date;
                //DateTime.TryParse(str, out date);
                //if (visiting.date == date)
                //{
                //    ModelState.AddModelError("classesTotal", "Введите кол-во занятий");
                //}    

                visiting.idSeasonTicket = id;
                if (ModelState.IsValid)
                {
                    db.Visiting.Add(visiting);
                    db.SaveChanges();
                    return RedirectToAction("EditSeasonTicket", new { id });
                }
                else
                {
                    return View(visiting);
                }
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult Service(string sort)
        {
            ViewBag.CostSortParm = sort == "cost" ? "cost_desc" : "cost";
            IQueryable<Service> services = db.Service;
            switch (sort)
            {
                case "cost":
                    services = services.OrderBy(s => s.cost);
                    break;
                case "cost_desc":
                    services = services.OrderByDescending(s => s.cost);
                    break;
            }

            return View(services.ToList());
        }

        public ActionResult EditService(int id)
        {
            return View(db.Service.Where(s => s.idService == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditService(int id, Service service)
        {
            try
            {
                service.idService = id;
                db.Entry(service).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Service");
            }
            catch (Exception)
            {
                return View();
            }

        }

        public ActionResult CreateService()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateService(Service service)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Service.Add(service);
                    db.SaveChanges();
                    return RedirectToAction("Service");
                }
                else
                {
                    return View(service);
                }
            }
            catch (Exception)
            {
                return View();
            }

        }

        public ActionResult Visiting()
        {
            return View();
        }

        public ActionResult Coach(string sort)
        {
            IQueryable<Coach> coaches = db.Coach;
            ViewBag.ExperienceSortParm = sort == "experience" ? "experience_desc" : "experience";
            ViewBag.NameSortParm = sort == "name" ? "name_desc" : "name";
            switch (sort)
            {
                case "experience":
                    coaches = coaches.OrderBy(c => c.experience);
                    break;
                case "experience_desc":
                    coaches = coaches.OrderByDescending(c => c.experience);
                    break;
                case "name":
                    coaches = coaches.OrderBy(c => c.surname);
                    break;
                case "name_desc":
                    coaches = coaches.OrderByDescending(c => c.surname);
                    break;
            }
            return View(coaches);
        }

        public ActionResult CreateCoach()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCoach(Coach coach)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Coach.Add(coach);
                    db.SaveChanges();
                    return RedirectToAction("Coach");
                }
                else
                {
                    return View(coach);
                }
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult EditCoach(int id)
        {
            return View(db.Coach.Where(cl => cl.idCoach == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditCoach(int id, Coach coach)
        {
            try
            {
                coach.idCoach = id;
                db.Entry(coach).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Coach");
            }
            catch (Exception)
            {
                return View();
            }

        }

        public ActionResult ShowCoach(int id)
        {
            Coach coach = db.Coach.Where(c => c.idCoach == id).FirstOrDefault();
            var list = (from c in db.Coach
                        join a in db.Activity on c.idCoach equals a.idCoach
                        join s in db.Service on a.idService equals s.idService
                        where c.idCoach == id
                        select s).ToList();
            List<string> ser = new List<string>();
            foreach (var item in list)
            {
                ser.Add(item.name);
            }
            coach.listActivities = ser;
            return View(coach);
        }

        public ActionResult SeasonTicketGraphic(string month)
        {
            int numMonth;
            
            if (String.IsNullOrEmpty(month))
            {
                numMonth = DateTime.Today.Month;
            }
            else
            {
                numMonth = GetNumMonth(month);
            }
            
            var list = db.SeasonTicket.ToList();

            var hres =
             from st in list
             where st.dateStart.Month == numMonth
             group st by st.dateStart into s
             select new { tickets = s.ToList(), date = s.Key };
            
            var h = hres.ToList();
            List<int> costs = new List<int>();

            int day;
            day = System.DateTime.DaysInMonth(2020, numMonth);
            List<int> w = new List<int>();
            int check = 0;
            int commonSumm = 0;
            for (int i = 1; i <= day; i++)
            {
                if (check != h.Count())
                {
                    if (h[check].date.Day == i)
                    {
                        int sum = 0;
                        foreach (var item in h[check].tickets)
                        {
                            sum += (int)item.totalCost;
                        }
                        check++;
                        commonSumm += sum;
                        costs.Add(sum);
                    }
                    else
                    {
                        costs.Add(0);
                    }
                }
                else
                {
                    costs.Add(0);
                }
                w.Add(i);
            }
            //var costs = list.Select(x => x.totalCost).Distinct();


            //hRes.Add(h[0]);
            //day = 
            //for (int i = 1; i < h.Count(); i++)
            //{
            //    if(h[i] == h[i-1])
            //}
            ViewBag.NUMMONTH = numMonth - 1;
            ViewBag.COSTS = costs;
            ViewBag.MONTH = month;
            ViewBag.DAYS = w;
            ViewBag.COMMONSUMM = commonSumm;
            return View();
        }

        public ActionResult CoachGraphic(string month)
        {
            int numMonth;

            if (String.IsNullOrEmpty(month))
            {
                numMonth = DateTime.Today.Month;
            }
            else
            {
                numMonth = GetNumMonth(month);
            }

            var list = db.SeasonTicket.ToList().Where(s=>s.idCoach!=null);

            var hres =
             from st in list
             where st.dateStart.Month == numMonth
             group st by st.Coach.surname into s
             select new { costs = s.ToList().Sum(st=>st.totalCost), coach = s.Key };
            
            var Res = hres.ToList();

            List<int> h = new List<int>();
            
            foreach (var item in Res)
            {
                h.Add((int)item.costs);
            }

            List<string> w = new List<string>();

            foreach (var item in Res)
            {
                w.Add(item.coach);
            }
          
            foreach (var item in db.Coach.ToList().Select(c => c.surname))
            {
                if(w.FirstOrDefault(c=>c == item)==null)
                {
                    w.Add(item);
                    h.Add(0);
                }    
            }
            ViewBag.NUMMONTH = numMonth - 1;
            ViewBag.COSTS = h;
            ViewBag.MONTH = month;
            ViewBag.COACHES = w;
            //ViewBag.COMMONSUMM = commonSumm;
            return View();
        }

            private int GetNumMonth(string month)
            {
            int numMonth;
            switch (month)
            {
                case "Январь":
                    numMonth = 1;
                    break;
                case "Февраль":
                    numMonth = 2;
                    break;
                case "Март":
                    numMonth = 3;
                    break;
                case "Апрель":
                    numMonth = 4;
                    break;
                case "Май":
                    numMonth = 5;
                    break;
                case "Июнь":
                    numMonth = 6;
                    break;
                case "Июль":
                    numMonth = 7;
                    break;
                case "Август":
                    numMonth = 8;
                    break;
                case "Сентябрь":
                    numMonth = 9;
                    break;
                case "Октябрь":
                    numMonth = 10;
                    break;
                case "Ноябрь":
                    numMonth = 10;
                    break;
                case "Декабрь":
                    numMonth = 12;
                    break;
                default:
                    numMonth = -1;
                    break;
            }
            return numMonth;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}