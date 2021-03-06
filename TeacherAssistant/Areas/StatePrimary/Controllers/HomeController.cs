﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TeacherAssistant.Models;
using TeachersAssistant.DataAccess.Interfaces;
using TeachersAssistant.Domain.Model.Models;
using TeachersAssistant.Models;
using TeachersAssistant.Services.Concretes;
namespace TeacherAssistant.Areas.StatePrimary.Controllers
{

    [Authorize(Roles = "Administrator,StatePrimary")]
    public class HomeController : Controller
    {
        private TeachersAssistantRepositoryServices _teacherRepository;
        public HomeController()
        {

        }
        public HomeController(ICalendarBookingRepositoryMarker calendarRepositoryMarker,
            IClassroomRepositoryMarker classroomRepositoryMarker,
            IFreeDocumentRepositoryMarker freeDocumentRepositoryMarker,
            IFreeDocumentStudentRepositoryMarker freeDocumentStudentRepositoryMarker,
            IFreeVideoRepositoryMarker freeVideoRepositoryMarker,
            IFreeVideoStudentRepositoryMarker freeVideoStudentRepositoryMarker,
            IPaidDocuemtStudentRepositoryMarker paidDocuemtStudentRepositoryMarker,
            IPaidDocumentRepositoryMarker paidDocumentRepositoryMarker,
            IPaidVideoRepositoryMarker paidVideoRepositoryMarker,
            IPaidVideoStudentRepositoryMarker paidVideoStudentRepositoryMarker,
            IStudentRepositoryMarker studentRepositoryMarker,
            IStudentTypeRepositoryMarker studentTypeRepositoryMarker,
            ISubjectRepositoryMarker subjectRepositoryMarker,
            ITeacherRepositoryMarker teacherRepositoryMarker,
            IBookingTimeRepositoryMarker bookingTimeRepositoryMarker)
        {
            var unitOfWork = new TeachersAssistantUnitOfWork(calendarRepositoryMarker,
             classroomRepositoryMarker,
             freeDocumentRepositoryMarker,
             freeDocumentStudentRepositoryMarker,
             freeVideoRepositoryMarker,
             freeVideoStudentRepositoryMarker,
             paidDocuemtStudentRepositoryMarker,
             paidDocumentRepositoryMarker,
             paidVideoRepositoryMarker,
             paidVideoStudentRepositoryMarker,
             studentRepositoryMarker,
             studentTypeRepositoryMarker,
             subjectRepositoryMarker,
             teacherRepositoryMarker,
             bookingTimeRepositoryMarker);
            _teacherRepository = new TeachersAssistantRepositoryServices(unitOfWork);
            _teacherRepository.GetSubjectList();
        }
        public enum MediaType { Document, Video }
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }
        public ActionResult AboutUs()
        {
            ViewBag.Message = "About Us.";

            return View();
        }

        public ActionResult ContactUs()
        {
            ViewBag.Message = "Contact Us.";

            return View();
        }
        [HttpGet]
        public ActionResult OnlineClassRoom()
        {
            ViewBag.Message = "Online Classroom.";

            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Administrator,Grammar11Plus,StatePrimary,StateJunior")]
        public ViewResult PrivateClass()
        {
            return View("PrivateClass");
        }
        [HttpGet]
        public ActionResult BookTeacherHelpTime()
        {
            ViewBag.Message = "Book Teacher Time.";
            GetUIDropdownLists();

            var calendars = _teacherRepository.GetTeacherCalendar();
            var calendarBookingViewModels = new List<CalendarBookingViewModel>();
            if (calendars != null)
            {
                var classRooms = _teacherRepository.GetClassrooms();

                var calendarsLeftJoin = from cal in calendars
                                        join cls in classRooms on
                                        cal.CalendarBookingId equals cls.CalendarId into res
                                        from q in res.DefaultIfEmpty()
                                        select new
                                        {
                                            ClassroomId = q == null ? null : q.ClassroomId,
                                            SubjectId = cal.SubjectId,
                                            TeacherId = cal.TeacherId,
                                            StudentId = cal.StudentId,
                                            BookingTimeId = cal.BookingTimeId
                                        };
                foreach (var cal in calendarsLeftJoin)
                {
                    var student = _teacherRepository.GetStudentById(cal.StudentId);
                    var subject = _teacherRepository.GetSubjectById(cal.SubjectId);
                    var teacher = _teacherRepository.GetTeacherById(cal.TeacherId);
                    var bookingTime = _teacherRepository.GetBookingById(cal.BookingTimeId);

                    if (bookingTime == null) continue;
                    calendarBookingViewModels.Add(new CalendarBookingViewModel { Teacher = teacher, Subject = subject, Student = student, BookingTime = bookingTime, ClassroomId = cal.ClassroomId });
                }
            }
            ViewBag.CalendarUiList = calendarBookingViewModels.ToArray();


            return View("BookTeacherHelpTime");
        }

        [HttpPost]
        public ActionResult BookTeacherHelpTime(TeacherCalendarViewModel bookingTimeViewModel)
        {
            ViewBag.Message = "Book Teacher Time.";
            GetUIDropdownLists();

            if (bookingTimeViewModel.CalendarBookingId < 1)
            {
                ModelState.AddModelError("Select", "Calendar BookingId required");
            }

            if (bookingTimeViewModel.Select != null)
            {
                if (ModelState.IsValid)
                {
                    var calendarBookingViewModels = new List<CalendarBookingViewModel>();
                    var calendar =
                        _teacherRepository.GetTeacherCalendarByBookingId(bookingTimeViewModel.CalendarBookingId);
                    Student student = _teacherRepository.GetStudentById(calendar.StudentId);
                    Subject subject = _teacherRepository.GetSubjectById(calendar.SubjectId);
                    bookingTimeViewModel.StudentId = (int)student.StudentId;
                    bookingTimeViewModel.SubjectId = (int)subject.SubjectId;
                    bookingTimeViewModel.CalendarBookingId = calendar.CalendarBookingId;
                    ModelState.Clear();

                    var calendars = _teacherRepository.GetTeacherCalendar();
                    var classRooms = _teacherRepository.GetClassrooms();

                    var calendarsLeftJoin = from cal in calendars
                                            join cls in classRooms on
                                            cal.CalendarBookingId equals cls.CalendarId into res
                                            from q in res.DefaultIfEmpty()
                                            select new
                                            {
                                                ClassroomId = q == null ? null : q.ClassroomId,
                                                SubjectId = cal.SubjectId,
                                                TeacherId = cal.TeacherId,
                                                StudentId = cal.StudentId,
                                                BookingTimeId = cal.BookingTimeId
                                            };
                    if (calendarsLeftJoin != null)
                        foreach (var cal in calendarsLeftJoin)
                        {
                            student = _teacherRepository.GetStudentById(cal.StudentId);
                            subject = _teacherRepository.GetSubjectById(cal.SubjectId);
                            var teacher = _teacherRepository.GetTeacherById(cal.TeacherId);
                            var bookingTime = _teacherRepository.GetBookingById(cal.BookingTimeId);

                            if (bookingTime == null) continue;
                            calendarBookingViewModels.Add(new CalendarBookingViewModel
                            {
                                Teacher = teacher,
                                Subject = subject,
                                Student = student,
                                BookingTime = bookingTime,
                                ClassroomId = cal.ClassroomId
                            });
                        }
                    ViewBag.CalendarUiList = calendarBookingViewModels.ToArray();
                    return View("BookTeacherHelpTime", bookingTimeViewModel);
                }
                return View("BookTeacherHelpTime", bookingTimeViewModel);
            }
            if (bookingTimeViewModel.Delete != null)
            {
                if (ModelState.IsValid)
                {
                    var teacherCalendar =
                        _teacherRepository.GetTeacherCalendarByBookingId(bookingTimeViewModel.CalendarBookingId);
                    _teacherRepository.DeleteTeacherCalendarByBooking(teacherCalendar);
                    return View("SuccessfullCreation");
                }
                return View("BookTeacherHelpTime", bookingTimeViewModel);
            }
            if (bookingTimeViewModel.SubjectId < 1)
            {
                ModelState.AddModelError("Subject", "Subject Id is required");
            }
            if (ModelState.IsValid)
            {
                Teacher teacher = _teacherRepository.GetTeacherById(bookingTimeViewModel.TeacherId);
                Student student = _teacherRepository.GetStudentByName(User.Identity.Name);
                Subject subject = _teacherRepository.GetSubjectById(bookingTimeViewModel.SubjectId);
                foreach (var bookingTime in bookingTimeViewModel.BookingTimes)
                {
                    _teacherRepository.SaveOrUpdateBooking(teacher, student, subject, bookingTime,
                        bookingTimeViewModel.Description);
                }

                return View("SuccessfullCreation");
            }
            return View("BookTeacherHelpTime", bookingTimeViewModel);
        }
        [HttpGet]
        public ActionResult TeachersCalendar()
        {
            GetUIDropdownLists();
            ViewBag.CalendarUiList = null;
            ViewBag.Message = "Teachers Calendar.";
            var calendars = _teacherRepository.GetTeacherCalendar();
            var calendarBookingViewModels = new List<CalendarBookingViewModel>();
            var classRooms = _teacherRepository.GetClassrooms();

            var calendarsLeftJoin = from cal in calendars
                                    join cls in classRooms on
                                    cal.CalendarBookingId equals cls.CalendarId into res
                                    from q in res.DefaultIfEmpty()
                                    select new
                                    {
                                        ClassroomId = q == null ? null : q.ClassroomId,
                                        SubjectId = cal.SubjectId,
                                        TeacherId = cal.TeacherId,
                                        StudentId = cal.StudentId,
                                        BookingTimeId = cal.BookingTimeId
                                    };

            if (calendarsLeftJoin != null)
            {
                foreach (var cal in calendarsLeftJoin)
                {
                    var student = _teacherRepository.GetStudentById(cal.StudentId);
                    var subject = _teacherRepository.GetSubjectById(cal.SubjectId);
                    var teacher = _teacherRepository.GetTeacherById(cal.TeacherId);
                    var bookingTime = _teacherRepository.GetBookingById(cal.BookingTimeId);

                    if (bookingTime == null) continue;
                    calendarBookingViewModels.Add(new CalendarBookingViewModel { Teacher = teacher, Subject = subject, Student = student, BookingTime = bookingTime, ClassroomId = cal.ClassroomId });
                }
            }
            ViewBag.CalendarUiList = calendarBookingViewModels.ToArray();
            return View("TeachersCalendar");

        }
        [HttpPost]
        public ActionResult TeachersCalendar(int teacherId)
        {
            GetUIDropdownLists();
            if (teacherId < 1)
            {
                ModelState.AddModelError("NoTeacher", "You need to pick a teacher");
            }
            ViewBag.Message = "Teachers Calendar.";
            var calendars = _teacherRepository.GetTeacherCalendar(teacherId);
            if (calendars == null)
            {
                ModelState.AddModelError("NoTeacherCalendar", "Teacher hasn't a calendar booking");
            }
            if (ModelState.IsValid)
            {
                var calendarBookingViewModels = new List<CalendarBookingViewModel>();
                var classRooms = _teacherRepository.GetClassrooms();

                var calendarsLeftJoin = from cal in calendars
                                        join cls in classRooms on
                                        cal.CalendarBookingId equals cls.CalendarId into res
                                        from q in res.DefaultIfEmpty()
                                        select new
                                        {
                                            ClassroomId = q == null ? null : q.ClassroomId,
                                            SubjectId = cal.SubjectId,
                                            TeacherId = cal.TeacherId,
                                            StudentId = cal.StudentId,
                                            BookingTimeId = cal.BookingTimeId
                                        };
                if (calendarsLeftJoin != null)
                {
                    foreach (var cal in calendarsLeftJoin)
                    {
                        var student = _teacherRepository.GetStudentByName(User.Identity.Name);
                        if (student == null || student.StudentId != cal.StudentId) continue;
                        var subject = _teacherRepository.GetSubjectById(cal.SubjectId);
                        var teacher = _teacherRepository.GetTeacherById(teacherId);
                        var bookingTime = _teacherRepository.GetBookingById(cal.BookingTimeId);

                        if (bookingTime == null) continue;
                        calendarBookingViewModels.Add(new CalendarBookingViewModel
                        {
                            Teacher = teacher,
                            Subject = subject,
                            Student = student,
                            BookingTime = bookingTime,
                            ClassroomId = cal.ClassroomId
                        });
                    }

                }
                ViewBag.CalendarUiList = calendarBookingViewModels.ToArray();
                return View("TeachersCalendar",
                    calendarBookingViewModels.Count() > 0
                        ? calendarBookingViewModels.ToArray()[calendarBookingViewModels.Count() - 1]
                        : null);
            }

            return View("TeachersCalendar");
        }
        public ActionResult DownloadFreeDocuments()
        {
            ViewBag.Message = "Download Free Documents.";
            var documents = _teacherRepository.GetFreeDocuments("StatePrimary");
            var videos = _teacherRepository.GetFreeVideos("StatePrimary");

            ViewBag.FreeDocumentsList = documents;
            ViewBag.FreeVideosList = videos;
            return View("DownloadFreeDocuments");
        }

        public ActionResult DownloadPaidDocuments()
        {
            ViewBag.Message = "Download Paid Documents."; 
            var documents = _teacherRepository.GetPaidDocuments("StatePrimary");
            var videos = _teacherRepository.GetPaidVideos("StatePrimary");

            ViewBag.PaidDocumentsList = documents;
            ViewBag.PaidVideosList = videos;

            return RedirectToRoute(new { controller = "Home", action = "DownloadPaidDocuments", namespaces = "TeacherAssistant.Controllers", area = "" });
        }
        private List<SelectListItem> GetClassroomList()
        {
            var classRoomList = new List<SelectListItem>();
            var classrooms = _teacherRepository.GetClassrooms();

            classRoomList.Add(new SelectListItem { Text = "Pick a Class", Value = 0.ToString() });
            classRoomList.AddRange(classrooms.Select(p => new SelectListItem { Text = string.Format("[{0}] Class", p.ClassroomId), Value = p.ClassroomId.ToString() }).ToList());
            return classRoomList;
        }
        private List<SelectListItem> GetStudentsList()
        {
            var students = _teacherRepository.GetStudentList();
            var studentList = new List<SelectListItem>();
            studentList.Add(new SelectListItem { Text = "Pick a Student", Value = 0.ToString() });

            foreach (var student in students)
            {
                studentList.Add(new SelectListItem { Text = student.EmailAddress, Value = student.StudentId.ToString() });
            }

            return studentList;
        }

        private List<SelectListItem> GetSubjectList()
        {
            var subjects = _teacherRepository.GetSubjectList();
            var subjectList = new List<SelectListItem>();
            subjectList.Add(new SelectListItem { Text = "Pick a Subject", Value = 0.ToString() });

            foreach (var subject in subjects)
            {
                subjectList.Add(new SelectListItem { Text = subject.SubjectName, Value = subject.SubjectId.ToString() });
            }

            return subjectList;
        }
        private List<SelectListItem> GetTeacherList()
        {
            var teachers = _teacherRepository.GetTeacherList();
            var teacherList = new List<SelectListItem>();
            teacherList.Add(new SelectListItem { Text = "Pick a Teacher", Value = 0.ToString() });

            foreach (var teacher in teachers)
            {
                teacherList.Add(new SelectListItem { Text = teacher.EmailAddress, Value = teacher.TeacherId.ToString() });
            }

            return teacherList;
        }
        private List<SelectListItem> GetCalendarList()
        {
            var calendarList = new List<SelectListItem>();
            var teacherCalendars = _teacherRepository.GetTeacherCalendar();
            calendarList.Add(new SelectListItem { Text = "Pick a Canlendar By Id and Teacher", Value = 0.ToString() });

            foreach (var cal in teacherCalendars)
            {
                calendarList.Add(new SelectListItem { Text = string.Format("[CalendarID:{0}] By ", cal.CalendarBookingId.ToString()) + GetTeacherList().Single(p => Convert.ToInt32(p.Value) == cal.TeacherId).Text, Value = cal.CalendarBookingId.ToString() });
            }
            return calendarList;
        }
        private List<SelectListItem> GetProductList()
        {
            var productList = new List<SelectListItem>();
            productList.Add(new SelectListItem { Text = "Pick Product Item", Value = 0.ToString() });

            productList.AddRange(_teacherRepository.GetProductsList().Select(p => new SelectListItem { Text = p.prodName, Value = p.prodId.ToString() }).ToList());
            return productList;
        }
        private List<SelectListItem> GetRolesSelectList()
        {
            var rolesList = new List<SelectListItem>();

            foreach (var role in Roles.GetAllRoles())
            {
                rolesList.Add(new SelectListItem { Text = role, Value = role });
            }

            return rolesList;
        }
        private void GetUIDropdownLists()
        {
            ViewBag.TeacherList = GetTeacherList();
            ViewBag.RoleList = GetRolesSelectList();
            ViewBag.StudentList = GetStudentsList();
            ViewBag.SubjectList = GetSubjectList();
            ViewBag.CalendarBookingList = GetCalendarList();
            ViewBag.ClassroomList = GetClassroomList();
        }
    }
}