using APBD_Zadanie_6.Interfaces;
using APBD_Zadanie_6.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_Zadanie_6.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly Context _context;

        public PatientRepository(Context context)
        {
            _context = context;
        }

        public async Task<Patient> GetPatientAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.IdMedicamentNav)
                .FirstOrDefaultAsync(p => p.IdPatient == id);
        }

        public async Task AddPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<Patient> GetPatientDetailsAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.IdMedicamentNav)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(p => p.IdPatient == id);
        }
    }
}
