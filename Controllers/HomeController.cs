using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        //datayı hazırla 
        //view gönder
        //viewden model içinden çekeceksin
        return View();
    }

    public async Task<IActionResult> Doctors()
    {
        var doctors = await _context.Doctors.ToListAsync();
        return View(doctors);
    }
    
    public async Task<IActionResult> MakeAppointment(int? doctorId = 0)
    {
        var doctors = await _context.Doctors.ToListAsync();
        return View(doctors);
    }
    
    public async Task<JsonResult> GetDoctor(int doctorId)
    {
        var doctor = await _context.Doctors.FindAsync(doctorId);
        return Json(doctor);
    }

    [HttpPost]
    public async Task<JsonResult> SetAppointment(int doctorId, string appointmentDate, string patientFullname, string patientEmail)
    {
        try
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            // daha önce belirtilen doktor ve saat için randevu yoksa olkuştur eğer var ise kullanıcıya saati veya doktoru değiştirmesi için uyarı verirlir.
            var isExist = await _context.Appointments.AnyAsync(x => x.DoctorId == doctorId && x.StartDate == DateTime.Parse(appointmentDate));
            if (isExist) return Json(new { approved = false, message = "Seçtiğiniz hekim için belirtilen saatte randevu mevcut. Lütfen başka bir saat veya hekim seçiniz." });

            string roomNameGuid = Guid.NewGuid().ToString("N");
            string link = $"{this.Request.Scheme}://{this.Request.Host}/join?roomName={roomNameGuid}";
            var appointment = new Appointment
            {
                DoctorId = doctor.Id,
                StartDate = DateTime.Parse(appointmentDate),
                EndDate = DateTime.Parse(appointmentDate).AddMinutes(30),
                PatientFullname = patientFullname,
                PatientEmail = patientEmail,
                RoomName = roomNameGuid,
                Link = link
            };
            // randevuyu tabloya ekleyip veriyi kaydediyoruz
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // doktora ve hastaya mail gönderimi
            await SenMail(new string[] { doctor.Email, patientEmail },
                "Randevu Oluşturuldu", 
                $"<p>Randevu Oluşturuldu</p><br /><br />" +
                $"Randevu Bilgisi: {doctor.Fullname}({doctor.Speciality}) {appointment.StartDate.Value.ToString("g")} - {appointment.EndDate.Value.ToString("HH:mm")}<br />" +
                $"Danışan: {patientFullname}<br /><br />" +
                $"Canlı randevuya bağlanmak için <a href=\"{link}\" target=\"_blank\">tıklayın</a>");
            
            return Json(new
            {
                approved = true,
                message = "Randevu başarıyla oluşturuldu",
                appointmentInfos = new
                {
                    doctorFullname = doctor.Fullname,
                    doctorSpeciality = doctor.Speciality,
                    appointmentDate = $"{appointment.StartDate.Value.ToString("g")} - {appointment.EndDate.Value.ToString("HH:mm")}",
                    patientFullname = appointment.PatientFullname,
                    patientEmail = appointment.PatientEmail
                }
            });
        }
        catch (Exception e)
        {
            return Json(new { approved = false, message = e.Message });
        }
    }

    public async Task<bool> SenMail(string[] emails, string subject, string body)
    {
        // mail gönderme işlemleri
        try
        {
            var senderMail = "onlinedoktor@kodbilimi.com";
            var senderMailPassword = "tfYdxGK9G7_10=-_";
        
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.To.Add(string.Join(",", emails));
            mail.From = new MailAddress(senderMail, "Online Doktor", Encoding.UTF8);
            mail.Subject = subject;
            mail.Body = body;
            SmtpClient smtpClient = new SmtpClient("mail.kurumsaleposta.com")
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 5000,
                Credentials = new NetworkCredential(senderMail, senderMailPassword),
                Host = "mail.kurumsaleposta.com",
                Port = 587,
                EnableSsl = false,
                UseDefaultCredentials = false
            };
        
            await smtpClient.SendMailAsync(mail);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    [Route("join")]
    public async Task<IActionResult> Join(string roomName)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.RoomName == roomName);
        if (appointment == null) return RedirectToAction("Index");
        return View(appointment);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
