using APBD_Zadanie_6.Models;
using System.Threading.Tasks;

namespace APBD_Zadanie_6.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient> GetPatientAsync(int id);
        Task AddPatientAsync(Patient patient);
        Task<Patient> GetPatientDetailsAsync(int id);
    }
}
