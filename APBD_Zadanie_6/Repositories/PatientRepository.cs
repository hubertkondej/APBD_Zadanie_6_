using System.Threading.Tasks;
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
                .ThenInclude(p => p.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
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
                        .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(p => p.IdPatient == id);
        }

        Task<Patient> IPatientRepository.GetPatientAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AddPatientAsync(Patient patient)
        {
            throw new NotImplementedException();
        }

        Task<Patient> IPatientRepository.GetPatientDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<Patient> IPatientRepository.GetPatientAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AddPatientAsync(Patient patient)
        {
            throw new NotImplementedException();
        }

        Task<Patient> IPatientRepository.GetPatientDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
