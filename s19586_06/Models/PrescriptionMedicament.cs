namespace s19586_06.Models
{
    public class PrescriptionMedicament
    {
        public int IdPrescription { get; set; }
        public Prescription Prescription { get; set; }
        public int IdMedicament { get; set; }
        public Medicament Medicament { get; set; }
    }
}
