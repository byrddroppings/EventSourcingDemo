using System;
using System.Web.Mvc;
using MediatorLib;
using EventSourcingDemo.Features;
using EventSourcingDemo.Models;

namespace EventSourcingDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index(Guid? id)
        {
            var viewModel = _mediator.Query(new ViewModelQuery(id));
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Person());
        }

        [HttpPost]
        public ActionResult Create(Person person, bool pending)
        {
            if (!ModelState.IsValid)
                return View(person);

            var status = pending ? "Pending" : "Approved";
            _mediator.Execute(new CreatePersonCommand { Person = person, Status = status });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var person = _mediator.Query(new FindPersonQuery { PersonId = id });
            return View(person);
        }

        [HttpPost]
        public ActionResult Edit(Person updated, bool pending)
        {
            if (!ModelState.IsValid)
                return View(updated);

            var status = pending ? "Pending" : "Approved";
            var before = _mediator.Query(new FindPersonQuery { PersonId = updated.PersonId });

            _mediator.Execute(new UpdatePersonCommand { Rollback = new Rollback<Person>(before, updated), Status = status });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Approve(int id)
        {
            _mediator.Execute(new ApproveLogCommand { CommandLogId = id });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Reject(int id)
        {
            _mediator.Execute(new RejectLogCommand { CommandLogId = id });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ClearAll()
        {
            _mediator.Execute(new ClearDataCommand { ClearLogs = true, ClearPeople = true });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ClearPeople()
        {
            _mediator.Execute(new ClearDataCommand { ClearLogs = false, ClearPeople = true });
            return RedirectToAction("Index");
        }
    }
}