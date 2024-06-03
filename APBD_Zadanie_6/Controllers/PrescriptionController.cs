using APBD_Zadanie_6.Models;
using Microsoft.AspNetCore.Mvc;

using APBD_Zadanie_6.Interfaces;
using APBD_Zadanie_6.Models.DTOs;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly IPatientRepository _patientRepository;
    private readonly Context _context;

    public PrescriptionController(IPatientRepository patientRepository, Context context)
    {
        _patientRepository = patientRepository;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDto prescriptionDto)
    {
        if (prescriptionDto.Medicaments.Count > 10)
        {
            return BadRequest("Prescription can include a maximum of 10 medicaments.");
        }

        if (prescriptionDto.DueDate < prescriptionDto.Date)
        {
            return BadRequest("DueDate must be greater than or equal to Date.");
        }

        var patient = await _patientRepository.GetPatientAsync(prescriptionDto.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                IdPatient = prescriptionDto.IdPatient,
                FirstName = prescriptionDto.FirstName,
                LastName = prescriptionDto.LastName,
                BirthDate = DateTime.Now
            };

            await _patientRepository.AddPatientAsync(patient);
        }

        var doctor = await _context.Doctors.FindAsync(prescriptionDto.IdDoctor);
        if (doctor == null)
        {
            return BadRequest("Doctor not found");
        }

        var medicamentIds = prescriptionDto.Medicaments.Select(md => md.IdMedicament).ToList();
        var medicaments = await _context.Medicaments.Where(m => medicamentIds.Contains(m.IdMedicament)).ToListAsync();

        if (medicaments.Count == prescriptionDto.Medicaments.Count)
        {
            var prescription = new Prescription
            {
                Date = prescriptionDto.Date,
                DueDate = prescriptionDto.DueDate,
                Patient = patient,
                Doctor = doctor,
                PrescriptionMedicaments = prescriptionDto.Medicaments.Select(md => new PrescriptionMedicament
                {
                    IdMedicament = md.IdMedicament,
                    Dose = md.Dose,
                    Details = md.Details
                }).ToList()
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok();
        }
        return BadRequest("One or more medicaments not found");
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetPatientDetails(int id)
    {
        var patient = await _patientRepository.GetPatientDetailsAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        var result = new
        {
            patient.IdPatient,
            patient.FirstName,
            patient.LastName,
            patient.BirthDate,
            Prescriptions = patient.Prescriptions.Select(p => new
            {
                p.IdPrescription,
                p.Date,
                p.DueDate,
                Medicaments = p.PrescriptionMedicaments.Select(pm => new
                {
                    pm.Medicament.IdMedicament,
                    pm.Medicament.Name,
                    pm.Medicament.Description,
                    pm.Dose,
                    pm.Details
                }),
                Doctor = new
                {
                    p.Doctor.IdDoctor,
                    p.Doctor.FirstName,
                    p.Doctor.LastName,
                    p.Doctor.Email
                }
            }).OrderBy(p => p.DueDate)
        };

        return Ok(result);
    }
}
