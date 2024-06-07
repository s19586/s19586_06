using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s19586_06.Context;
using s19586_06.Models;
using System.Linq;
using System.Threading.Tasks;

namespace s19586_06.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly PrescriptionContext _context;

        public PrescriptionsController(PrescriptionContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription(AddPrescriptionRequest request)
        {
            var patient = await _context.Patients.FindAsync(request.PatientId);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = request.PatientFirstName,
                    LastName = request.PatientLastName,
                    BirthDate = request.PatientBirthDate
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var doctor = await _context.Doctors.FindAsync(request.DoctorId);
            if (doctor == null)
            {
                return BadRequest("Doctor not found.");
            }

            var medicaments = await _context.Medicaments
                .Where(m => request.MedicamentIds.Contains(m.IdMedicament))
                .ToListAsync();

            if (medicaments.Count != request.MedicamentIds.Count)
            {
                return BadRequest("Some medicaments not found.");
            }

            if (request.MedicamentIds.Count > 10)
            {
                return BadRequest("Too many medicaments.");
            }

            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                Patient = patient,
                Doctor = doctor,
                PrescriptionMedicaments = medicaments.Select(m => new PrescriptionMedicament { Medicament = m }).ToList()
            };

            if (prescription.DueDate < prescription.Date)
            {
                return BadRequest("DueDate must be greater than or equal to Date.");
            }

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }
    }

    public class AddPrescriptionRequest
    {
        public int PatientId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public DateTime PatientBirthDate { get; set; } // Dodaj ten atrybut do requestu
        public int DoctorId { get; set; }
        public List<int> MedicamentIds { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
    }
}
