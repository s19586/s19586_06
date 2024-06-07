using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s19586_06.Context;
using s19586_06.Models;
using System.Threading.Tasks;

namespace s19586_06.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PrescriptionContext _context;

        public PatientsController(PrescriptionContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(p => p.Doctor)
                .Include(p => p.Prescriptions)
                    .ThenInclude(p => p.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.Medicament)
                .FirstOrDefaultAsync(p => p.IdPatient == id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }
    }
}
