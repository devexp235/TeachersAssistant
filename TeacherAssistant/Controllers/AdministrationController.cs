﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TeacherAssistant.Models;
using TeachersAssistant.Domain.Model.Models;
using TeachersAssistant.Models;
using TeachersAssistant.Services.Concretes;
using TeachersAssistant.Services.Interfaces;
using TeachersAssistant.DataAccess.Interfaces;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using AutoMapper;

namespace TeacherAssistant.Controllers
{
    [Authorize(Roles = "Admin,Administrator")]
    public class AdministrationController : Controller
    {
        private ApplicationUserManager _userManager;

        TeachersAssistantRepositoryServices _repositoryServices;
        public AdministrationController()
        {

        }

        public AdministrationController(ICalendarBookingRepositoryMarker calendarRepositoryMarker,
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
            IBookingTimeRepositoryMarker bookingRepositoryMarker)
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
             bookingRepositoryMarker);
            _repositoryServices = new TeachersAssistantRepositoryServices(unitOfWork);
        }
        [HttpGet]
        public ActionResult ManageClassRoom()
        {
            GetUIDropdownLists();
            return View("ManageClassRoom");
        }

        [HttpPost]
        public ActionResult ManageClassRoom(ClassroomViewModel classRoomViewModel)
        {
            GetUIDropdownLists();
            var classRoomModel = (Classroom)Mapper.Map(classRoomViewModel, typeof(ClassroomViewModel), typeof(Classroom));
            if (classRoomViewModel.Select != null)
            {
                if (classRoomViewModel.ClassroomId < 1 || classRoomViewModel.CalendarBookingId < 1)
                {
                    ModelState.AddModelError("ClassroomId", "ClassroomId and CalendarId are required");
                }
                if (ModelState.IsValid)
                {
                    var calendar = _repositoryServices.GetTeacherCalendarByBookingId(classRoomViewModel.CalendarBookingId);
                    var classroom = _repositoryServices.GetClassroomById(classRoomViewModel.ClassroomId);
                    classRoomViewModel = (ClassroomViewModel)Mapper.Map(classroom, typeof(Classroom), typeof(ClassroomViewModel));
                    classRoomViewModel.CalendarBookingId = (int)calendar.CalendarBookingId;
                    classRoomViewModel.SubjectId = calendar.SubjectId;
                    ModelState.Clear();
                }
                return View("ManageClassRoom", classRoomViewModel);
            }

            if (ModelState.IsValid)
            {
                if (classRoomViewModel.Delete != null)
                {
                    var classroom = _repositoryServices.GetClassroomById(classRoomViewModel.ClassroomId);
                    _repositoryServices.DeleteClassroom(classroom);
                    return View("SuccessfullCreation");
                }
                else
                {
                    var calendar = _repositoryServices.GetTeacherCalendarByBookingId(classRoomViewModel.CalendarBookingId);
                    _repositoryServices.ManageClassRoom(classRoomModel);
                    calendar.SubjectId = classRoomModel.SubjectId;
                    calendar.ClassId = (int)classRoomModel.ClassroomId;
                    _repositoryServices.SaveOrUpdateCalendar(calendar);
                    return View("SuccessfullCreation");
                }
            }
            return View("ManageClassRoom", classRoomViewModel);
        }
        [HttpGet]
        public ActionResult ManageStudent()
        {
            GetUIDropdownLists();
            return View("ManageStudent");
        }

        [HttpPost]
        public ActionResult ManageStudent(StudentViewModel studentViewModel)
        {
            GetUIDropdownLists();
            if (studentViewModel.Select != null)
            {
                if (studentViewModel.StudentId < 1)
                {
                    ModelState.AddModelError("StudentId", "StudentId is required");
                }
                if (ModelState.IsValid)
                {
                    var student = _repositoryServices.GetStudentById(studentViewModel.StudentId);
                    studentViewModel = (StudentViewModel)Mapper.Map(student, typeof(Student), typeof(StudentViewModel));
                    ModelState.Clear();
                }
                return View("ManageStudent", studentViewModel);
            }
            if (ModelState.IsValid)
            {
                if (studentViewModel.Delete != null)
                {
                    var student = _repositoryServices.GetStudentById(studentViewModel.StudentId);
                    _repositoryServices.DeleteStudent(student);
                    return View("SuccessfullCreation");
                }
                else
                {
                    var studentModel =
                        (Student)Mapper.Map(studentViewModel, typeof(StudentViewModel), typeof(Student));
                    _repositoryServices.ManageStudent(studentModel);
                    return View("SuccessfullCreation");
                }
            }

            return View("ManageStudent", studentViewModel);
        }

        [HttpGet]
        public ActionResult ManageSubject()
        {
            GetUIDropdownLists();
            return View("ManageSubject");
        }

        [HttpPost]
        public ActionResult ManageSubject(SubjectViewModel subjectViewModel)
        {
            GetUIDropdownLists();
            if (subjectViewModel.Select != null)
            {
                ModelState.Clear();
                if (subjectViewModel.SubjectId < 1)
                {
                    ModelState.AddModelError("SubjectId", "SubjectId is required");
                }
                if (ModelState.IsValid)
                {
                    var subject = _repositoryServices.GetSubjectById(subjectViewModel.SubjectId);
                    subjectViewModel = (SubjectViewModel)Mapper.Map(subject, typeof(Subject), typeof(SubjectViewModel));

                    ModelState.Clear();
                }
                return View("ManageSubject", subjectViewModel);
            }
            if (subjectViewModel.Delete != null)
            {
                ModelState.Clear();
                if (subjectViewModel.SubjectId < 1)
                {
                    ModelState.AddModelError("SubjectId", "SubjectId is required");
                }
                if (ModelState.IsValid)
                {
                    var subject = _repositoryServices.GetSubjectById(subjectViewModel.SubjectId);
                    _repositoryServices.DeleteSubject(subject);
                    return View("SuccessfullCreation");
                }
                return View("ManageSubject", subjectViewModel);
            }
            var subjectModel = (Subject)Mapper.Map(subjectViewModel, typeof(SubjectViewModel), typeof(Subject));
            if (subjectViewModel.SubjectName != null)
            {
                _repositoryServices.ManageSubject(subjectModel);
                return View("SuccessfullCreation");

            }
            return View("ManageSubject", subjectViewModel);
        }
        [HttpGet]
        public ActionResult ManageTeacher()
        {
            GetUIDropdownLists();
            return View("ManageTeacher");
        }

        [HttpPost]
        public ActionResult ManageTeacher(TeacherViewModel teacherViewModel)
        {
            GetUIDropdownLists();

            if (teacherViewModel.Select != null)
            {
                if (teacherViewModel.TeacherId < 1)
                {
                    ModelState.AddModelError("TeacherId", "Teacher Id is required");
                }
                if (ModelState.IsValid)
                {
                    var teacher = _repositoryServices.GetTeacherById(teacherViewModel.TeacherId);
                    teacherViewModel = (TeacherViewModel)Mapper.Map(teacher, typeof(Teacher), typeof(TeacherViewModel));
                    ModelState.Clear();
                    return View("ManageTeacher", teacherViewModel);
                }
                return View("ManageTeacher", teacherViewModel);
            }
            var teacherModel = (Teacher)Mapper.Map(teacherViewModel, typeof(TeacherViewModel), typeof(Teacher));
            if (ModelState.IsValid)
            {
                if (teacherViewModel.Delete != null)
                {
                    var teacher = _repositoryServices.GetTeacherByName(teacherViewModel.EmailAddress);
                    _repositoryServices.DeleteTeacher(teacher);
                    return View("SuccessfullCreation");
                }
                else
                {
                    _repositoryServices.ManageTeachers(teacherModel);
                    return View("SuccessfullCreation");
                }
            }
            return View("ManageTeacher", teacherViewModel);
        }

        [HttpGet]
        public ActionResult UploadMedia()
        {
            GetUIDropdownLists();
            return View("UploadMedia");
        }
        [HttpPost]
        public ActionResult UploadMedia(UploadMediaViewModel mediaModel)
        {
            try
            {
                GetUIDropdownLists();

                ViewBag.RoleList = GetRolesSelectList();


                var mediaType = mediaModel.MediaType;
                var virtualPath = string.Empty;

                //Save file to relevant fileSystem:
                switch (mediaModel.RoleName.ToLower())
                {
                    case "grammar11plus":
                        if (mediaType.ToLower().StartsWith("paid"))
                            virtualPath = "~/Documents/Grammar11Plus/PaidDocuments";
                        else virtualPath = "~/Documents/Grammar11Plus/FreeDocuments";
                        break;
                    case "stateprimary":
                        if (mediaType.ToLower().StartsWith("paid"))
                            virtualPath = "~/Documents/StatePrimary/PaidDocuments";
                        else virtualPath = "~/Documents/StatePrimary/FreeDocuments";
                        break;
                    case "statejunior":
                        if (mediaType.ToLower().StartsWith("paid"))
                            virtualPath = "~/Documents/StateJunior/PaidDocuments";
                        else virtualPath = "~/Documents/StateJunior/FreeDocuments";
                        break;
                    default:
                        if (mediaType.ToLower().StartsWith("paid"))
                            virtualPath = "~/Documents/Administration/PaidDouments";
                        else virtualPath = "~/Documents/Administration/FreeDouments";
                        break;
                }


                if (mediaModel.Select != null)
                {
                    ModelState.Clear();
                    if (mediaModel.MediaId == null || string.IsNullOrEmpty(mediaType))
                    {
                        ModelState.AddModelError("MediaId", "Media Id Required");
                        return UploadMedia(mediaModel);
                    }
                    switch (mediaType.ToLower())
                    {
                        case "paiddocument":
                            var paidDoc = _repositoryServices.GetPaidDocumentById(mediaModel.MediaId);
                            return View(new UploadMediaViewModel { MediaId = paidDoc.PaidDocumentId, RoleName = paidDoc.RoleName, Name = paidDoc.FilePath });
                        case "freedocument":
                            var freeDoc = _repositoryServices.GetFreeDocumentById(mediaModel.MediaId);
                            return View(new UploadMediaViewModel { MediaId = freeDoc.FreeDocumentId, RoleName = freeDoc.RoleName, Name = freeDoc.FilePath });

                        case "paidvideo":
                            var paidVideo = _repositoryServices.GetPaidVideoById(mediaModel.MediaId);
                            return View(new UploadMediaViewModel { MediaId = paidVideo.PaidVideoId, RoleName = paidVideo.RoleName, Name = paidVideo.FilePath });
                        case "freevideo":
                            var freeVideo = _repositoryServices.GetFreeVideoById(mediaModel.MediaId);
                            return View(new UploadMediaViewModel { MediaId = freeVideo.FreeVideoId, RoleName = freeVideo.RoleName, Name = freeVideo.FilePath });
                    }
                    return View(mediaModel);
                }
                else if (mediaModel.Delete != null)
                {
                    ModelState.Clear();
                    if (mediaModel.MediaId == null || string.IsNullOrEmpty(mediaType))
                    {
                        ModelState.AddModelError("MediaId", "Media Id Required");
                        return UploadMedia(mediaModel);
                    }


                    switch (mediaType.ToLower())
                    {
                        case "paiddocument":
                            dynamic doc = _repositoryServices.GetPaidDocumentById(mediaModel.MediaId);
                            if (doc != null)
                            {
                                var delFileInfo = new FileInfo(Server.MapPath(doc.FilePath));
                                if (delFileInfo.Exists)
                                {
                                    delFileInfo.Delete();
                                }

                            }
                            _repositoryServices.DeletePaidDocumentById(mediaModel.MediaId);
                            return View("SuccessfullCreation");
                        case "freedocument":
                            doc = _repositoryServices.GetFreeDocumentById(mediaModel.MediaId);
                            if (doc != null)
                            {
                                var delFileInfo = new FileInfo(Server.MapPath(doc.FilePath));
                                if (delFileInfo.Exists)
                                {
                                    delFileInfo.Delete();
                                }

                            }
                            _repositoryServices.DeleteFreeDocumentById(mediaModel.MediaId);
                            return View("SuccessfullCreation");
                        case "paidvideo":
                            doc = _repositoryServices.GetPaidVideoById(mediaModel.MediaId);
                            if (doc != null)
                            {
                                var delFileInfo = new FileInfo(Server.MapPath(doc.FilePath));
                                if (delFileInfo.Exists)
                                {
                                    delFileInfo.Delete();
                                }

                            }
                            _repositoryServices.DeletePaidVideoById(mediaModel.MediaId);
                            return View("SuccessfullCreation");
                        case "freevideo":
                            doc = _repositoryServices.GetFreeVideoById(mediaModel.MediaId);
                            if (doc != null)
                            {
                                var delFileInfo = new FileInfo(Server.MapPath(doc.FilePath));
                                if (delFileInfo.Exists)
                                {
                                    delFileInfo.Delete();
                                }

                            }
                            _repositoryServices.DeleteFreeVideoById(mediaModel.MediaId);
                            return View("SuccessfullCreation");
                    }
                    return View(mediaModel);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        HttpPostedFileBase file = mediaModel.MediaContent;
                        var fileName = file.FileName;
                        var fileBuffer = new byte[file.ContentLength];
                        var fileContent = file.InputStream.Read(fileBuffer, 0, fileBuffer.Length);
                        file.InputStream.Flush();
                        file.InputStream.Close();

                        var physicalPath = Server.MapPath(virtualPath);
                        var dirInfo = new DirectoryInfo(physicalPath);
                        if (!dirInfo.Exists) dirInfo.Create();

                        FileInfo fileInfo = new FileInfo(physicalPath + "\\" + file.FileName);
                        if (fileInfo.Exists) fileInfo.Delete();
                        using (var fileStream = fileInfo.Create())
                        {
                            fileStream.Write(fileBuffer, 0, fileBuffer.Length);
                            fileStream.Flush();
                            fileStream.Close();
                        }

                        var paidDocument = new PaidDocument
                        {
                            FilePath = "~" + Url.Content(virtualPath) + "/" + file.FileName,
                            SubjectId = mediaModel.SubjectId,
                            RoleName = mediaModel.RoleName
                        };
                        //Save file Path To DB: 
                        switch (mediaModel.MediaType.ToLower())
                        {
                            case "paiddocument":
                                _repositoryServices.SaveOrUpdatePaidDocument(paidDocument);
                                break;
                            case "freedocument":
                                var freeDocument = AutoMapper.Mapper.Map(paidDocument, typeof(PaidDocument),
                                    typeof(FreeDocument));
                                _repositoryServices.SaveOrUpdateFeeDocument(freeDocument as FreeDocument);
                                break;
                            case "paidvideo":
                                var paidVideo = AutoMapper.Mapper.Map(paidDocument, typeof(PaidDocument),
                                    typeof(PaidVideo));
                                _repositoryServices.SaveOrUpdatePaidVideo(paidVideo as PaidVideo);
                                break;
                            case "freevideo":
                                var freeVideo = AutoMapper.Mapper.Map(paidDocument, typeof(PaidDocument),
                                    typeof(FreeVideo));
                                _repositoryServices.SaveOrUpdateFreeVideo(freeVideo as FreeVideo);
                                break;
                        }
                        return View("SuccessfullCreation");
                    }

                    return View("UploadMedia", mediaModel);
                }

            }
            catch (Exception e)
            {
                ModelState.Clear();
                ModelState.AddModelError("FileAccess", string.Format("{0}", e.Message));
                return View("UploadMedia");
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [HttpGet]
        public ActionResult TeachersCalendar()
        {
            GetUIDropdownLists();
            ViewBag.Message = "Teachers Calendar.";
            var calendars = _repositoryServices.GetTeacherCalendar();
            if (calendars == null)
            {
                ModelState.AddModelError("NoTeacherCalendar", "Teacher hasn't a calendar booking");
            }
            if (!ModelState.IsValid)
                return View();
            var calendarBookingViewModels = new List<CalendarBookingViewModel>();
            var classRooms = _repositoryServices.GetClassrooms();
            if (calendars != null)
            {
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
                    var student = _repositoryServices.GetStudentById(cal.StudentId);
                    var subject = _repositoryServices.GetSubjectById(cal.SubjectId);
                    var teacher = _repositoryServices.GetTeacherById(cal.TeacherId);
                    var bookingTime = _repositoryServices.GetBookingById(cal.BookingTimeId);

                    if (bookingTime == null) continue;
                    calendarBookingViewModels.Add(new CalendarBookingViewModel { ClassroomId = cal.ClassroomId, Teacher = teacher, Subject = subject, Student = student, BookingTime = bookingTime });
                }

            }
            ViewBag.CalendarUiList = calendarBookingViewModels.ToArray();

            return PartialView("TeachersCalendar", calendarBookingViewModels.Count() > 0 ? calendarBookingViewModels.ToArray()[calendarBookingViewModels.Count() - 1] : null);
        }
        [HttpGet]
        public ActionResult ManageTeachersCalendar()
        {
            GetUIDropdownLists();
            ViewBag.Message = "Manage Teachers Calendar";
            var calendars = _repositoryServices.GetTeacherCalendar();
            var calendarBookingViewModels = new List<CalendarBookingViewModel>();
            var classRooms = _repositoryServices.GetClassrooms();
            if (calendars != null)
            {
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
                    var student = _repositoryServices.GetStudentById(cal.StudentId);
                    var subject = _repositoryServices.GetSubjectById(cal.SubjectId);
                    var teacher = _repositoryServices.GetTeacherById(cal.TeacherId);
                    var bookingTime = _repositoryServices.GetBookingById(cal.BookingTimeId);

                    if (bookingTime == null) continue;
                    calendarBookingViewModels.Add(new CalendarBookingViewModel { Teacher = teacher, Subject = subject, Student = student, BookingTime = bookingTime, ClassroomId = cal.ClassroomId });
                }
            }
            ViewBag.CalendarUiList = calendarBookingViewModels.ToArray();
            return View("ManageTeacherCalendar");
        }
        private void GetTeacherBookingTimes(TeacherCalendarViewModel bookTeacherTime)
        {
            var teacherCalendar = _repositoryServices.GetTeacherCalendarByBookingId(bookTeacherTime.CalendarBookingId);
            Student student = _repositoryServices.GetStudentById(teacherCalendar.StudentId);
            Subject subject = _repositoryServices.GetSubjectById(teacherCalendar.SubjectId);
            bookTeacherTime.StudentId = (int)student.StudentId;
            bookTeacherTime.SubjectId = (int)subject.SubjectId;
            bookTeacherTime.CalendarBookingId = teacherCalendar.CalendarBookingId;


            var calendarBookingViewModels = new List<CalendarBookingViewModel>();
            student = _repositoryServices.GetStudentById(teacherCalendar.StudentId);
            subject = _repositoryServices.GetSubjectById(teacherCalendar.SubjectId);
            bookTeacherTime.StudentId = (int)student.StudentId;
            bookTeacherTime.SubjectId = (int)subject.SubjectId;
            bookTeacherTime.CalendarBookingId = teacherCalendar.CalendarBookingId;

            var classRooms = _repositoryServices.GetClassrooms();
            var calendars = _repositoryServices.GetTeacherCalendar();
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
                student = _repositoryServices.GetStudentById(cal.StudentId);
                subject = _repositoryServices.GetSubjectById(cal.SubjectId);
                var teacher = _repositoryServices.GetTeacherById(cal.TeacherId);
                var bookingTime = _repositoryServices.GetBookingById(cal.BookingTimeId);

                if (bookingTime == null || teacher.TeacherId != bookTeacherTime.TeacherId) continue;
                calendarBookingViewModels.Add(new CalendarBookingViewModel { Teacher = teacher, Subject = subject, Student = student, BookingTime = bookingTime, ClassroomId = cal.ClassroomId });
            }
            ViewBag.CalendarUiList = calendarBookingViewModels.ToArray();
            ModelState.Clear();
        }
        [HttpPost]
        public ActionResult ManageTeachersCalendar(TeacherCalendarViewModel bookingTimeViewModel)
        {
            ViewBag.Message = "Book Teacher Time.";
            GetUIDropdownLists();

            if (bookingTimeViewModel.Select != null)
            {
                ModelState.Clear();
                if (bookingTimeViewModel.CalendarBookingId < 1)
                {
                    ModelState.AddModelError("CalendarBookingId", "Calendar BookingId required");
                }
                if (ModelState.IsValid)
                {
                    var calendarBookingViewModels = new List<CalendarBookingViewModel>();
                    var calendar = _repositoryServices.GetTeacherCalendarByBookingId(bookingTimeViewModel.CalendarBookingId);
                    Student student = _repositoryServices.GetStudentById(calendar.StudentId);
                    Subject subject = _repositoryServices.GetSubjectById(calendar.SubjectId);
                    bookingTimeViewModel.StudentId = (int)student.StudentId;
                    bookingTimeViewModel.SubjectId = (int)subject.SubjectId;
                    bookingTimeViewModel.CalendarBookingId = calendar.CalendarBookingId;
                    ModelState.Clear();


                    var calendars = _repositoryServices.GetTeacherCalendar();
                    var classRooms = _repositoryServices.GetClassrooms();
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
                        student = _repositoryServices.GetStudentById(cal.StudentId);
                        subject = _repositoryServices.GetSubjectById(cal.SubjectId);
                        var teacher = _repositoryServices.GetTeacherByName(User.Identity.Name);
                        var bookingTime = _repositoryServices.GetBookingById(cal.BookingTimeId);

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
                    return View("ManageTeacherCalendar", bookingTimeViewModel);
                }
                return View("ManageTeacherCalendar", bookingTimeViewModel);
            }
            if (bookingTimeViewModel.Delete != null)
            {
                ModelState.Clear();
                if (bookingTimeViewModel.CalendarBookingId < 1)
                {
                    ModelState.AddModelError("CalendarBookingId", "Calendar BookingId required");
                }
                if (ModelState.IsValid)
                {
                    var teacherCalendar =
                        _repositoryServices.GetTeacherCalendarByBookingId(bookingTimeViewModel.CalendarBookingId);
                    _repositoryServices.DeleteTeacherCalendarByBooking(teacherCalendar);
                    return View("SuccessfullCreation");
                }
                return View("ManageTeacherCalendar", bookingTimeViewModel);
            }
            if (bookingTimeViewModel.SubjectId < 1)
            {
                ModelState.AddModelError("SubjectId", "Subject Id is required");
            }
            if (ModelState.IsValid)
            {
                Teacher teacher = _repositoryServices.GetTeacherById(bookingTimeViewModel.TeacherId);
                Student student = _repositoryServices.GetStudentById(bookingTimeViewModel.StudentId);
                Subject subject = _repositoryServices.GetSubjectById(bookingTimeViewModel.SubjectId);
                foreach (var bookingTime in bookingTimeViewModel.BookingTimes)
                {
                    _repositoryServices.SaveOrUpdateBooking(teacher, student, subject, bookingTime,
                        bookingTimeViewModel.Description);
                }

                return View("SuccessfullCreation");
            }
            return View("ManageTeacherCalendar", bookingTimeViewModel);
        }
        [HttpGet]
        public ActionResult ManageProducts()
        {
            var products = _repositoryServices.GetProductsList();
            ViewBag.ProductIdsList = GetProductList();
            return View("ManageProducts");
        }
        [HttpPost]
        public ViewResult ManageProducts(ProductViewModel productModel)
        {
            var products = _repositoryServices.GetProductsList();
            ViewBag.ProductIdsList = GetProductList();

            if (productModel.Select != null)
            {
                if (productModel.ProductId < 1)
                {
                    ModelState.AddModelError("ProductId", "Product Id Required");
                }
                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    var prod = _repositoryServices.GetProductById(productModel.ProductId);
                    productModel = Mapper.Map(prod, typeof(SHOP_PRODS), typeof(ProductViewModel)) as ProductViewModel;
                    productModel.DocumentType = prod.IsPaidDocument ? 0 : prod.IsPaidVideo ? 1 : -1;
                    return View("ManageProducts", productModel);
                }

                return View("ManageProducts", productModel);
            }

            if (ModelState.IsValid)
            {
                var shopProd = new SHOP_PRODS
                {
                    prodName = productModel.ProductName,
                    prodDesc = productModel.ProductDescription,
                    prodPrice = productModel.ProductPrice,
                    prodId = productModel.ProductId,
                    DocumentId = productModel.DocumentId
                };

                if (productModel.Delete != null)
                {
                    _repositoryServices.DeleteProduct(shopProd);
                    return View("SuccessfullCreation");
                }
                if (shopProd != null && productModel.DocumentType != -1)
                {
                    shopProd.IsPaidDocument = productModel.DocumentType == 0;
                    shopProd.IsPaidVideo = productModel.DocumentType == 1;
                }
                _repositoryServices.SaveOrUpdate(shopProd);
                return View("SuccessfullCreation");
            }
            return View("ManageProducts", productModel);
        }
        [HttpGet]
        public JsonResult GetPaidDocuments(int paidDocument)
        {
            switch (paidDocument)
            {
                case 0:
                    //PaidDocuemnts
                    var paidDocs = _repositoryServices.GetPaidDocuments();

                    return Json(paidDocs.Select<PaidDocument, dynamic>(p => new { DocumentId = p.PaidDocumentId, DocumentName = p.FilePath.Substring(p.FilePath.LastIndexOf("/")) }).ToArray(), JsonRequestBehavior.AllowGet);
                case 1:
                    //PaidVideos
                    var paidVids = _repositoryServices.GetPaidVideos();
                    return Json(paidVids.Select<PaidVideo, dynamic>(p => new { DocumentId = p.PaidVideoId, DocumentName = p.FilePath.Substring(p.FilePath.LastIndexOf("/")) }).ToArray(), JsonRequestBehavior.AllowGet);
                default:
                    return null;
            }
        }
        [HttpGet]
        public ActionResult CreateRoles()
        {
            return View("CreateRoles");
        }

        [HttpPost]
        public ActionResult CreateRoles(UserRoleViewModel role)
        {
            try
            {
                if (ModelState.IsValid && !Roles.RoleExists(role.RoleName))
                {
                    Roles.CreateRole(role.RoleName);
                    return View("SuccessfullCreation");
                }
                else
                {
                    return View("CreateRoles");
                }
            }
            catch (Exception ex)
            {
                return View("CreateRoles");
            }
        }

        private bool AssignRoles(string user, string role)
        {
            try
            {
                Roles.AddUserToRole(user, role);
                return true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public bool RemoveRole(string role)
        {
            try
            {
                Roles.DeleteRole(role);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult AssignUserRole()
        {
            ViewBag.RoleName = GetRolesSelectList();
            return View("AssignUserRole");
        }



        [HttpPost]
        public ActionResult AssignUserRole(Models.UserRoleViewModel userInRole)
        {

            ViewBag.RoleName = GetRolesSelectList();

            if (ModelState.IsValid && AssignRoles(userInRole.Username, userInRole.RoleName))
            {
                return View("SuccessfullCreation");
            }
            return View("AssignUserRole", userInRole);
        }
        [HttpGet]

        public ActionResult RemoveUserFromRole()
        {
            ViewBag.RoleName = GetRolesSelectList();
            return View("RemoveUserFromRole");
        }

        [HttpPost]
        public ActionResult RemoveUserFromRole(Models.UserRoleViewModel userInRole)
        {
            var rolesList = new List<string>();

            foreach (var role in Roles.GetAllRoles())
            {
                rolesList.Add(role);
            }
            ViewBag.RoleName = new SelectList(rolesList);

            if (ModelState.IsValid && RemoveUserFromRole(userInRole.Username, userInRole.RoleName))
            {
                return View("SuccessfullCreation");
            }
            return View("RemoveUserFromRole", userInRole);
        }

        private bool RemoveUserFromRole(string username, string roleName)
        {
            try
            {
                Roles.RemoveUserFromRole(username, roleName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private List<SelectListItem> GetClassroomList()
        {
            var classRoomList = new List<SelectListItem>();
            var classrooms = _repositoryServices.GetClassrooms();

            classRoomList.Add(new SelectListItem { Text = "Pick a Class", Value = 0.ToString() });
            classRoomList.AddRange(classrooms.Select(p => new SelectListItem { Text = string.Format("[{0}] Class", p.ClassroomId), Value = p.ClassroomId.ToString() }).ToList());
            return classRoomList;
        }
        private List<SelectListItem> GetStudentsList()
        {
            var students = _repositoryServices.GetStudentList();
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
            var subjects = _repositoryServices.GetSubjectList();
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
            var teachers = _repositoryServices.GetTeacherList();
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
            var teacherCalendars = _repositoryServices.GetTeacherCalendar();
            calendarList.Add(new SelectListItem { Text = "Pick a Canlendar By Id and Teacher", Value = 0.ToString() });

            foreach (var cal in teacherCalendars)
            {
                calendarList.Add(new SelectListItem { Text = string.Format("[CalendarID:{0}] By ", cal.CalendarBookingId.ToString()) + GetTeacherList().Single(p => Convert.ToInt32(p.Value) == cal.TeacherId).Text, Value = cal.CalendarBookingId.ToString() });
            }
            return calendarList;
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
        private List<SelectListItem> GetProductList()
        {
            var productList = new List<SelectListItem>();
            productList.Add(new SelectListItem { Text = "Pick Product Item", Value = 0.ToString() });

            productList.AddRange(_repositoryServices.GetProductsList().Select(p => new SelectListItem { Text = p.prodName, Value = p.prodId.ToString() }).ToList());
            return productList;
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
        [HttpGet]
        public JsonResult GetMediaDocumentIdsFor(string mediaType, string role)
        {
            switch (mediaType.ToLower())
            {
                case "paiddocument":
                    return Json(_repositoryServices.GetPaidDocuments(role)
                    .Select(p => new SelectListItem { Text = p.PaidDocumentId.ToString() + " " + p.FilePath.Substring(p.FilePath.LastIndexOf("/") + 1), Value = p.PaidDocumentId.ToString() }).ToList(), JsonRequestBehavior.AllowGet);

                case "freedocument":
                    return Json(_repositoryServices.GetFreeDocuments(role)
                         .Select(p => new SelectListItem { Text = p.FreeDocumentId.ToString() + " " + p.FilePath.Substring(p.FilePath.LastIndexOf("/") + 1), Value = p.FreeDocumentId.ToString() }).ToList(), JsonRequestBehavior.AllowGet);

                case "paidvideo":
                    return Json(_repositoryServices.GetPaidVideos(role)
                          .Select(p => new SelectListItem { Text = p.PaidVideoId.ToString() + " " + p.FilePath.Substring(p.FilePath.LastIndexOf("/") + 1), Value = p.PaidVideoId.ToString() }).ToList(), JsonRequestBehavior.AllowGet);

                case "freevideo":
                    return Json(_repositoryServices.GetFreeVideos(role)
                          .Select(p => new SelectListItem { Text = p.FreeVideoId.ToString() + " " + p.FilePath.Substring(p.FilePath.LastIndexOf("/") + 1), Value = p.FreeVideoId.ToString() }).ToList(), JsonRequestBehavior.AllowGet);

                default:
                    return null;
            }
        }
    }
}